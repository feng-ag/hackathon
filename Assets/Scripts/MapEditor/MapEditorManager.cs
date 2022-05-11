using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using MapEditor;

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
    public EnvironmentData environmentData;

    [SerializeField]
    public MapEditor.Cursor cursor;

    [SerializeField]
    public MapEditor.Cursor targetCursor;

    [SerializeField]
    Transform camRoot;

    [SerializeField]
    GameObject defaultMap;



    public enum ControlState
    {
        None = 0,
        Place = 1,
        Peek = 2,
        Move = 3,
    }

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
            onClickPlaceItem += (int index) =>
            {

                if(CurrentItemType == index)
                {
                    ChangeToPeek(null);
                }
                else
                {
                    HidePeek();
                    ChangeToPlace(index);
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
        MapEditorUIManager.Instance.SetSelectedItem(-1);
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
        targetCursor.BuildCursor(item.TypeData);
        targetCursor.Position = item.transform.position - item.TypeData.placeOffsetV3;
        targetCursor.Show();
        cursor.Hide();
        MapEditorUIManager.Instance.SetTarget(item);
    }

    void HidePeek()
    {
        targetCursor.Hide();
        cursor.Show();
        MapEditorUIManager.Instance.SetTarget(null);
    }


    void ChangeToPlace(int index)
    {
        CurrentControlState = ControlState.Place;
        MapEditorUIManager.Instance.SetSelectedItem(index);
        CurrentItemType = index;

        cursor.BuildCursor(itemTypeDataGroup.GetTypeData(index));
    }


    private void Start()
    {
        LoadMap(defaultMap);


    }

    void LoadMap(GameObject map)
    {
        if (map == null)
        {
            return;
        }

        GameObject mapIns = Instantiate(map, mapRoot);


        Item[] itemBaseList = mapIns.GetComponentsInChildren<Item>();


        //foreach (var itemBase in itemBaseList)
        //{


        //    MapEditorItemData mapEditorItemData = new MapEditorItemData
        //    {
        //        item = itemBase,
        //        pos = itemBase.pos,
        //        rotate = itemBase.Rotate,
        //        type = itemBase.itemType,
        //    };

        //    mapEditorItemDataQuery[itemBase.pos] = mapEditorItemData;

        //}


        currentEnvenmentObject = mapIns.transform.Find("EnvRoot").GetChild(0).gameObject;

    }

    void ChangeEnvTo(int index)
    {
        if (currentEnvenmentObject != null)
        {
            Destroy(currentEnvenmentObject);
        }

        currentEnvenmentObject = Instantiate(environmentData.GetEnvironmentAt(index).RandomPickPrefab(), envRoot);
    }


    public bool DeleteItemByUser(ItemData itemData)
    {

        DeleteItem(itemData);

        return true;
    }

    void DeleteItem(ItemData itemData)
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
            return;
        }


        if (CurrentControlState == ControlState.Peek || CurrentControlState == ControlState.Place)
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {

                Item item = null;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000, itemLayer.value))
                {
                    if (hitInfo.collider.TryGetComponent(out ItemColLinker itemColLinker))
                    {
                        item = itemColLinker.item;

                        //初次選擇就顯示info，重複選擇就關閉
                        if (MapEditorUIManager.Instance.currentEditItem != item)
                        {
                            ChangeToPeek(item);
                        }
                        else
                        {
                            ChangeToPeek(null);
                            MapEditorUIManager.Instance.currentEditItem = null;
                        }
                    }

                }
                else
                if (Physics.Raycast(ray, out RaycastHit hitInfo2, 1000, groundLayer.value))
                {
                    if (CurrentControlState != ControlState.Place)
                    {
                        ChangeToPeek(null);
                        return;
                    }

                    ItemTypeData itemTypeData = itemTypeDataGroup.GetTypeData(CurrentItemType);

                    if (itemTypeData != null)
                    {

                        item = ItemData.Embed(hitInfo2.point, CurrentItemType, cursor.Rotation, itemRoot);

                        if (item != null)
                        {
                            targetCursor.BuildCursor(item.TypeData);
                            targetCursor.Position = item.transform.position - item.TypeData.placeOffsetV3;
                            targetCursor.Rotation = cursor.Rotation;
                            targetCursor.Show();
                            MapEditorUIManager.Instance.SetTarget(item);
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
        else if (CurrentControlState == ControlState.Move)
        {
            //

            if (Input.GetMouseButtonDown(0))
            {
                moveBeginMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                camBeginPos = camRoot.localPosition;
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


        if (Input.GetKeyDown(KeyCode.Space))
        {
            CurrentControlState = ControlState.Move;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            CurrentControlState = ControlState.Place;
        }


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

                    Vector3 hitPos = hitInfo2.point + itemTypeData.cursorOffsetV3;

                    float x = Mathf.Round(hitPos.x);
                    float z = Mathf.Round(hitPos.z);
                    Vector3 pos = new Vector3(x, 0, z);

                    cursor.Position = pos;
                    cursor.Show();
                }
            }

        }
    }




}
