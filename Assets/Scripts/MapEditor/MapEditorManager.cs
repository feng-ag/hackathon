using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.IO;
using System.Linq;
using MapEditor;

#if(UNITY_EDITOR)
using UnityEditor;
#endif

public class MapEditorManager : MonoBehaviour
{
    [SerializeField]
    public GameObject editorRoot;

    [SerializeField]
    Camera cam;

    [SerializeField]
    Collider ground;

    [SerializeField]
    public Transform mapRoot;

    [SerializeField]
    public Transform itemRoot;

    [SerializeField]
    Transform envRoot;

    [SerializeField]
    LayerMask groundLayer;

    [SerializeField]
    public LayerMask itemLayer;

    public const int itemLayerIndex = 11;

    [SerializeField]
    public ItemTypeDataGroup itemTypeDataGroup;

    [SerializeField]
    public UserItemPainterDataGroup userItemPainterDataGroup;

    [SerializeField]
    public EnvironmentData environmentData;

    [SerializeField]
    public MapEditor.Cursor cursor;

    [SerializeField]
    public MapEditor.Cursor targetCursor;

    [SerializeField]
    Transform camRoot;

    [SerializeField]
    TextAsset defaultMapJson;



    public enum ControlState
    {
        None = 0,
        Place = 1,
        Peek = 2,
        Move = 3,
    }

    ControlState LastControlState = ControlState.Peek;
    public ControlState CurrentControlState = ControlState.Peek;



    public Action<int> onClickPlaceItem { get; private set; }
    public Action<int> onClickEnvItem { get; private set; }


    public static MapEditorManager Instance;

    public int CurrentItemType { get; set; } = -1;
    public int CurrentEnvIndex { get; set; } = 0;

    GameObject currentEnvenmentObject;

    void Awake()
    {
        Instance = this;

        ConstructUIEvents();

        //MapEditorController.Instance.HideMapEditor();

        void ConstructUIEvents()
        {
            onClickPlaceItem += (int itemPainterIndex) =>
            {

                if(MapEditorUIManager.Instance.CurrentItemPainterIndex == itemPainterIndex)
                {
                    ChangeToPeek(null);
                }
                else
                {
                    HidePeek();
                    ChangeItemPainter(itemPainterIndex);
                }


            };

            onClickEnvItem += (int index) =>
            {
                CurrentEnvIndex = index;
                MapEditorUIManager.Instance.SetSelectedEnv(index);
                ChangeEnvTo(index);
                //Debug.Log($"Set ENV Index = {index}");
            };
        }
    }


    void ChangeToPeek(Item itemBase)
    {
        CurrentControlState = ControlState.Peek;
        MapEditorUIManager.Instance.SetSelectedItemPainter(-1);
        CurrentItemType = -1;

        if (itemBase != null)
        {
            ShowPeek(itemBase);
        }
        else
        {
            HidePeek();
        }


    }

    void ShowPeek(Item item)
    {
        targetCursor.ResetCursor();
        targetCursor.BuildCursor(item.TypeData);
        targetCursor.SetCursor(item.data);
        targetCursor.Show();
        cursor.Hide();
        MapEditorUIManager.Instance.SetTarget(item);
    }

    void HidePeek()
    {
        targetCursor.Hide();
        MapEditorUIManager.Instance.SetTarget(null);
    }


    void ChangeItemPainter(int itemPainterIndex)
    {
        UserItemPainterData userItemPainterData = userItemPainterDataGroup.GetUserItemPainterData(itemPainterIndex);

        ItemTypeData typeData = userItemPainterData.GetTypeData();

        CurrentControlState = ControlState.Place;
        MapEditorUIManager.Instance.SetSelectedItemPainter(itemPainterIndex);

        CurrentItemType = typeData.type;

        if(MapEditorUIManager.Instance.CurrentItemPainterIndex != itemPainterIndex)
        {
            cursor.ResetCursor();
        }
        else
        {
            // OnAfterPaint
            if (userItemPainterData.isRandomRot == true)
            {
                cursor.Rotation = UnityEngine.Random.Range(0, 4) * 90F;
            }
        }

        cursor.BuildCursor(typeData);
        cursor.Show();
    }


    private void Start()
    {
        StartCoroutine(LoadDefaultMap());
    }


    public IEnumerator LoadDefaultMap() 
    {
        return MapDataManager.Instance.LoadMap(defaultMapJson.text);
    }


    public void ChangeEnvTo(int index)
    {
        if (currentEnvenmentObject != null)
        {
            Destroy(currentEnvenmentObject);
        }

        currentEnvenmentObject = Instantiate(environmentData.GetEnvironmentAt(index).RandomPickPrefab(), envRoot);
    }


    public void DeleteItem(ItemData itemData)
    {
        ItemData.UnEmbed(itemData);

        MapEditorUIManager.Instance.SetTarget(null);
        targetCursor.Hide();
    }




    Vector2 moveBeginMousePos;
    Vector3 camBeginPos;

    void Update()
    {
        if (EventSystem.current == null)
        {
            Debug.LogError("EventSystem.Current is null");
            return;
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            LastControlState = CurrentControlState;
            CurrentControlState = ControlState.Move;
            //Debug.Log(CurrentControlState);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            CurrentControlState = LastControlState;
        }

        //

        if (CurrentControlState == ControlState.Peek || CurrentControlState == ControlState.Place)
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Item item = null;

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000, itemLayer.value))
                {
                    // Peek

                    if (hitInfo.collider.TryGetComponent(out ItemColLinker itemColLinker))
                    {
                        item = itemColLinker.item;

                        if (MapEditorUIManager.Instance.currentEditItem != item)
                        {
                            ChangeToPeek(item);
                        }
                        else
                        {
                            ChangeToPeek(null);
                            MapEditorUIManager.Instance.currentEditItem = null;
                            cursor.Hide();
                        }
                    }

                }
                else if (Physics.Raycast(ray, out RaycastHit hitInfo2, 1000, groundLayer.value))
                {
                    // Place

                    if (CurrentControlState != ControlState.Place)
                    {
                        ChangeToPeek(null);
                    }
                    else
                    {
                        ItemTypeData itemTypeData = itemTypeDataGroup.GetTypeData(CurrentItemType);

                        if (itemTypeData != null)
                        {

                            item = ItemData.EmbedAtCursorPos(hitInfo2.point, CurrentItemType, cursor.Rotation, itemRoot);

                            int itemPainterIndex = MapEditorUIManager.Instance.CurrentItemPainterIndex;
                            UserItemPainterData itemPainter = userItemPainterDataGroup.GetUserItemPainterData(itemPainterIndex);


                            if (item != null)
                            {
                                //IsUnique 功能
                                if(itemTypeData.isUnique == true)
                                {
                                    ItemData[] sameTypeItemData = MapDataManager.Instance.GetAllItems()
                                        .Where(itemData => itemData.type == itemTypeData.type)  //篩選同類型的 Item
                                        .Where(itemData => itemData.item != item)               //略過剛剛才新增的的 Item
                                        .ToArray();

                                    foreach(var itemData in sameTypeItemData)
                                    {
                                        ItemData.UnEmbed(itemData);
                                    }
                                }

                                if (itemPainter.isRandomRot == true)
                                {
                                    //重新設定一次，讓N取1可以再取一次
                                    ChangeItemPainter(itemPainterIndex);
                                }
                            }
                        }
                    }
                }
                else
                {
                    targetCursor.Hide();
                    MapEditorUIManager.Instance.SetTarget(null);
                }
            }
        }
        
        if (CurrentControlState == ControlState.Move)
        {
            //

            if (Input.GetMouseButtonDown(0))
            {
                moveBeginMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                camBeginPos = camRoot.localPosition;

                //Debug.Log(moveBeginMousePos);

            }
            if (Input.GetMouseButton(0))
            {
                Vector2 v = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - moveBeginMousePos;

                //Debug.Log($"Move {v}");

                const float moveSpeed = 0.015F;

                Vector3 v3 = new Vector3(v.x, 0, v.y);
                Vector3 moveValue3 = Quaternion.Euler(0, -45, 0) * (v3 * moveSpeed * -1);

                //Debug.Log($"{(v3 * moveSpeed * -1)} -> {moveValue3}");

                camRoot.localPosition = camBeginPos + moveValue3;
            }

        }


        //



        if (Input.GetKey(KeyCode.Minus))
        {
            Zoom(zoomRatio + 1F * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.Equals))
        {
            Zoom(zoomRatio - 1F * Time.deltaTime);
        }
        else if (!EventSystem.current.IsPointerOverGameObject() &&
            (Input.mouseScrollDelta.y > 0F || Input.mouseScrollDelta.y < 0F))
        {
            Zoom(zoomRatio - 0.1F * Input.mouseScrollDelta.y);
        }


        if (CurrentControlState == ControlState.Place)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                cursor.Rotate(90F);
            }
        }


#if(UNITY_EDITOR)

        if (Input.GetKeyDown(KeyCode.M))
        {
            string mapJson = MapDataManager.Instance.ExportMap();

            Debug.Log(mapJson);


            string assetPath = AssetDatabase.GetAssetPath(defaultMapJson);

            string filePath = $"{Application.dataPath}{assetPath.Substring(6)}";

            File.WriteAllText(filePath, mapJson, System.Text.Encoding.UTF8);

            EditorUtility.SetDirty(defaultMapJson);

            AssetDatabase.Refresh();

        }

#endif


    }


    float zoomRatio = 1F;
    void Zoom(float ratio)
    {
        ratio = Mathf.Clamp(ratio, 0.5F, 1.5F);
        zoomRatio = ratio;

        float z = -25F * ratio;
        cam.transform.localPosition = new Vector3(0, 0, z);
    }


    void FixedUpdate()
    {
        if (EventSystem.current == null)
        {
            return;
        }


        //Cursor
        if (CurrentControlState != ControlState.Move && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            //if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000, itemLayer.value))
            //{
            //    if (hitInfo.collider.TryGetComponent(out ItemColLinker itemColLinker))
            //    {
            //        ItemTypeData itemTypeData = itemTypeDataGroup.GetTypeData(CurrentItemType);
            //        Item item = itemColLinker.item;

            //        cursor.Position = item.transform.position + itemTypeData.cursorOffsetV3;
            //        cursor.Hide();
            //    }
            //}
            //else
            if (Physics.Raycast(ray, out RaycastHit hitInfo2, 1000, groundLayer.value))
            {
                if (CurrentItemType >= 0)
                {
                    ItemTypeData itemTypeData = itemTypeDataGroup.GetTypeData(CurrentItemType);


                    Vector3 hitPos = hitInfo2.point;

                    

                    float x = Mathf.Round(hitPos.x);
                    float z = Mathf.Round(hitPos.z);
                    Vector3 pos = ItemData.CursorPosToGridPos(hitPos, itemTypeData.type, cursor.Rotation); //new Vector3(x, 0, z);

                    cursor.Position = pos;
                    cursor.Show();
                }
            }

        }
    }




}
