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
    public TextAsset defaultMapJson;



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

    public GameObject CurrentEnvenmentObject { get; private set; }

    void Awake()
    {
        Instance = this;

        ConstructUIEvents();

        MapEditorController.Instance.HideMapEditor();

#if (UNITY_EDITOR)
        if (UnityEngine.SceneManagement.SceneManager.sceneCount == 1)
        {
            MapEditorController.Instance.ShowMapEditor();
        }
#endif

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
                EmbedEnv(index, itemRoot);
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

    }



    public void EmbedEnv(int envIndex, Transform root)
    {
        CurrentEnvenmentObject = Instantiate(environmentData.GetEnvironmentAt(envIndex).RandomPickPrefab(), root);
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
            return;
        }

        //

        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            if (CurrentControlState == ControlState.Peek && Input.GetMouseButtonDown(0))
            {
                TriggerPeekAt(Input.mousePosition);
            }
            else if (CurrentControlState == ControlState.Place && Input.GetMouseButtonDown(0))
            {
                TriggerPlaceAt(Input.mousePosition);
            }
        }

        //

        // �}��/�������ʪ��A
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(2))
        {
            if (CurrentControlState != ControlState.Move)
            {
                LastControlState = CurrentControlState;
            }
            CurrentControlState = ControlState.Move;
        }
        else if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(2))
        {
            CurrentControlState = LastControlState;
        }
        
        // ����
        if (CurrentControlState == ControlState.Move)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(2))
            {
                TriggerMoveStart(Input.mousePosition);

            }
            if (Input.GetMouseButton(0) || Input.GetMouseButton(2))
            {
                TriggerMoving(Input.mousePosition);
            }
        }


        //


        // Zoom
        if (Input.GetKey(KeyCode.Minus))
        {
            Zoom(ZoomRatio + 1F * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.Equals))
        {
            Zoom(ZoomRatio - 1F * Time.deltaTime);
        }
        else if (!EventSystem.current.IsPointerOverGameObject() &&
            (Input.mouseScrollDelta.y > 0F || Input.mouseScrollDelta.y < 0F))
        {
            Zoom(ZoomRatio - 0.1F * Input.mouseScrollDelta.y);
        }

        //

        // Rotate Cursor
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


    void TriggerPeekAt(Vector3 mousePosition)
    {
        Ray ray = cam.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000, itemLayer.value))
        {
            // Peek

            if (hitInfo.collider.TryGetComponent(out ItemColLinker itemColLinker))
            {
                Item item = itemColLinker.item;

                if (MapEditorUIManager.Instance.currentEditItem != item)
                {
                    ChangeToPeek(item);
                }
                else
                {
                    HidePeek();
                    MapEditorUIManager.Instance.currentEditItem = null;
                    cursor.Hide();
                }
            }
        }
        else
        {
            HidePeek();
        }
    }

    void TriggerPlaceAt(Vector3 mousePosition)
    {
        Ray ray = cam.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo2, 1000, groundLayer.value))
        {
            // Place

            if (CurrentControlState != ControlState.Place)
            {
                HidePeek();
            }
            else
            {
                ItemTypeData itemTypeData = itemTypeDataGroup.GetTypeData(CurrentItemType);

                if (itemTypeData != null)
                {
                    Item item = ItemData.EmbedAtCursorPos(hitInfo2.point, CurrentItemType, cursor.Rotation, itemRoot);

                    int itemPainterIndex = MapEditorUIManager.Instance.CurrentItemPainterIndex;
                    UserItemPainterData itemPainter = userItemPainterDataGroup.GetUserItemPainterData(itemPainterIndex);


                    if (item != null)
                    {
                        // IsConnectable �\��
                        if (itemPainter.isConnectable == true)
                        {
                            TriggerConnect(itemPainter, item.Data);
                        }

                        //IsUnique �\��
                        if (itemTypeData.isUnique == true)
                        {
                            ItemData[] sameTypeItemData = MapDataManager.Instance.GetAllItems()
                                .Where(itemData => itemData.type == itemTypeData.type)  //�z��P������ Item
                                .Where(itemData => itemData.item != item)               //���L���~�s�W���� Item
                                .ToArray();

                            foreach (var itemData in sameTypeItemData)
                            {
                                ItemData.UnEmbed(itemData);
                            }
                        }

                        if (itemPainter.isRandomRot == true)
                        {
                            //���s�]�w�@���A��N��1�i�H�A���@��
                            ChangeItemPainter(itemPainterIndex);
                        }
                    }
                }
            }
        }



        void TriggerConnect(UserItemPainterData itemPainter, ItemData itemData)
        {
            var connects = ItemData.GetConnectItems(itemData);

            if (connects.Count == 0)
            {
                //�P��S������F��
                return;
            }
            else if (connects.Count == 1)
            {

                var v1 = connects.First();

                // O���V�F�䪺�V�q
                Vector3 vec1 = v1.Key;

                float rot = ((itemData.itemRot % 360) + 360) % 360;

                Vector3[] dirs = new Vector3[] {
                    new Vector3(0, 0, 1),
                    new Vector3(0, 0, -1),
                };

                // O���V�s����V���V�q
                var rotDirs = dirs.Select(dir => Quaternion.AngleAxis(rot, Vector3.up) * dir);





                //
                //????????????
                bool needFix = rotDirs.Any(rotDir => rotDir != vec1);

                if (needFix)
                {
                    Debug.Log($"FIX ({rot}) {rotDirs} / {vec1}");
                }
                //





            }
            else if (connects.Count == 2)
            {
                Vector3 v1 = connects.Keys.First();
                Vector3 v2 = connects.Keys.Skip(1).First();

                Debug.Log($"{v1} - {v2}");
            }
            else if (connects.Count == 3)
            {
                Vector3 v1 = connects.Keys.First();
                Vector3 v2 = connects.Keys.Skip(1).First();
                Vector3 v3 = connects.Keys.Skip(2).First();

                Debug.Log($"{v1} - {v2} - {v3}");
            }


        }
    }

    void TriggerMoveStart(Vector3 mousePosition)
    {
        moveBeginMousePos = new Vector2(mousePosition.x, mousePosition.y);
        camBeginPos = camRoot.localPosition;

        //Debug.Log(moveBeginMousePos);
    }

    void TriggerMoving(Vector3 mousePosition)
    {
        Vector2 v = new Vector2(mousePosition.x, mousePosition.y) - moveBeginMousePos;

        //Debug.Log($"Move {v}");

        const float moveSpeed = 0.015F;

        Vector3 v3 = new Vector3(v.x, 0, v.y);
        Vector3 moveValue3 = Quaternion.Euler(0, -45, 0) * (v3 * moveSpeed * -1);

        //Debug.Log($"{(v3 * moveSpeed * -1)} -> {moveValue3}");

        camRoot.localPosition = camBeginPos + moveValue3;


        MapEditorUIManager.Instance.SetTargetInfo();
    }




    public float ZoomRatio { get; private set; } = 1F;
    void Zoom(float ratio)
    {
        ratio = Mathf.Clamp(ratio, 0.5F, 1.5F);
        ZoomRatio = ratio;

        float z = -25F * ratio;
        cam.transform.localPosition = new Vector3(0, 0, z);


        MapEditorUIManager.Instance.SetTargetInfo();
    }


    void FixedUpdate()
    {
        if (EventSystem.current == null)
        {
            return;
        }



        //�즲�\��
        if (CurrentControlState == ControlState.Place && !EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButton(0))
            {
                // TODOOOOOOOOOOOOOOOOOOO
                // �קK�P�@�歫�ƹB��
                TriggerPlaceAt(Input.mousePosition);
            }
        }



        //Cursor
        if (CurrentControlState == ControlState.Place &&
            CurrentItemType >= 0 &&
            !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo2, 1000, groundLayer.value))
            {
                ItemTypeData itemTypeData = itemTypeDataGroup.GetTypeData(CurrentItemType);

                Vector3 hitPos = hitInfo2.point;
                Vector3 pos = ItemData.CursorPosToGridPos(hitPos, itemTypeData.type, cursor.Rotation);

                cursor.Position = pos;
            }
        }


    }



}
