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

        // ?}??/???????????A
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
        
        // ????
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

        if (embedRequestList.Count > 0)
        {
            //Debug.Log("Wait for embed queue clear.");
            return;
        }


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
                        // IsConnectable ?\??
                        if (itemPainter.isConnectable == true)
                        {
                            TriggerConnect(itemPainter, item.Data);
                        }

                        //IsUnique ?\??
                        if (itemTypeData.isUnique == true)
                        {
                            ItemData[] sameTypeItemData = MapDataManager.Instance.GetAllItems()
                                .Where(itemData => itemData.type == itemTypeData.type)  //?z???P?????? Item
                                .Where(itemData => itemData.item != item)               //???L?????~?s?W???? Item
                                .ToArray();

                            foreach (var itemData in sameTypeItemData)
                            {
                                ItemData.UnEmbed(itemData);
                            }
                        }

                        if (itemPainter.isRandomRot == true)
                        {
                            //???s?]?w?@???A??N??1?i?H?A???@??
                            ChangeItemPainter(itemPainterIndex);
                        }
                    }
                }
            }
        }


        void TriggerConnect(UserItemPainterData itemPainter, ItemData itemData)
        {
            int straightType = 3;   // (0, 0, 1) - (0, 0, -1)
            int bendType = 4;       // (0, 0, 1) - (-1, 0, 0)


            var connects = ItemData.GetNeighborItems(itemData);
            var connecteds = connects.Where(i => IsConnectFull(i.Value) == false);

            int connectedCount = connecteds.Count();

            //Debug.Log($"${itemData.GetShortId()}({connectedCount}) : {string.Join(" | ", connects.Select(n => n.Value.GetShortId()))}");


            if (connectedCount == 0)
            {
                //?P???S???????F??

                return;
            }
            else if (connectedCount == 1)
            {
                //?g???@?????F??
                var v1 = connecteds.First();

                FixItem(itemData, v1.Value);
                FixItem(v1.Value, itemData);

            }
            else if (connectedCount == 2)
            {
                var v1 = connecteds.First();
                var v2 = connecteds.Skip(1).First();

                FixItem(itemData, v1.Value);
                FixItem(itemData, v2.Value);

                ConnectItemNextFixedUpdate(itemData, new[] { v1.Key, v2.Key });
            }
            else if (connectedCount == 3)
            {
                var v1 = connecteds.First();
                var v2 = connecteds.Skip(1).First();
                var v3 = connecteds.Skip(2).First();

                FixItem(itemData, v1.Value);
                FixItem(itemData, v2.Value);
                FixItem(itemData, v3.Value);
            }


            bool IsConnectFull(ItemData itemData)
            {
                var ports = ItemData.GetConnectPorts(itemData);
                var connects = ItemData.GetNeighborItems(itemData);


                //Debug.Log($"Is {itemData.id} connect full: ({string.Join(",", ports)} / {string.Join(",", connects.Keys)})");
                int count = connects
                    .Where(c =>
                    {
                        var r = ports.Any(p => p == c.Key);
                        return r;
                    })      // ???o?P itemData port ?s?????F??
                    .Where(o =>
                    {
                        var dirPort = o.Key;
                        var neighbor = o.Value;

                        var neighborPorts = ItemData.GetConnectPorts(neighbor);

                        return neighborPorts.Any(v =>
                        {
                            return v * -1 == dirPort;
                        });
                    })                           // ?C?X?P?F?? ports ?P dir ???? (?Y????)
                    .Count();


                //foreach (var port in ports)
                //{
                //    GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //    g.transform.position = itemData.itemPos + port;
                //    g.transform.localScale = Vector3.one * 0.1F;
                //}


                bool result = count == ports.Count;


                //if (result)
                //{
                //    GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //    g.transform.position = itemData.itemPos;
                //    g.transform.localScale = Vector3.one * 0.5F;
                //}
                //else
                //{
                //    GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //    g.transform.position = itemData.itemPos;
                //    g.transform.localScale = Vector3.one * 0.25F;
                //}


                return result;
            }

            void ChangeItem(ItemData itemData, int newType, float rot)
            {
                if(itemData.type != newType)
                {
                    // UnEmbed
                    embedRequestList.Enqueue(new ItemData.EmbedRequest
                    {
                        requestType = ItemData.EmbedRequest.RequestType.UnEmbed,
                        itemData = itemData,
                    });

                    // Embed
                    embedRequestList.Enqueue(new ItemData.EmbedRequest
                    {
                        requestType = ItemData.EmbedRequest.RequestType.Embed,
                        objectPos = itemData.itemPos,
                        type = newType,
                        rot = rot,
                        root = itemRoot,
                        id = itemData.id,
                    });
                }
                else if(itemData.itemRot != rot)
                {
                    itemData.item.SetRotation(rot);
                }

            }

            void FixItem(ItemData rootItemData, ItemData itemData)
            {

                if (IsConnectFull(itemData))
                {
                    //Debug.Log($"{itemData.GetShortId()} is connect full!");
                    return;
                }

                //Debug.Log($"Fix {itemData.GetShortId()} (Root: {rootItemData.GetShortId()} ) ----------");

                var v1ConnectPorts = ItemData.GetConnectPorts(itemData).Select(port => port + itemData.itemPos);
                var v1ConnectItems = ItemData.GetNeighborItems(itemData);


                var v1OtherConnectSet = v1ConnectItems.Where(m => m.Value != rootItemData).Select(m => m);
                if (v1OtherConnectSet.Any())    //???s?????N??
                {
                    var v1AbsConnect = v1ConnectItems.First(m => m.Value == rootItemData).Key;

                    //Debug.Log($"{itemData.GetShortId()} ==> {string.Join(",", v1OtherConnectSet.Select(v => v.Key))}");

                    //?u???????w???V itemData ?????H???s??
                    Vector3 v1OtherConnect = v1OtherConnectSet.First().Key;
                    var o = v1OtherConnectSet
                        .Where(item =>
                        {
                            var c = ItemData.GetConnectItems(item.Value);
                            bool otherIsConnect = c.Any(oo => oo.Value == itemData);
                            return otherIsConnect;
                        });
                    if (o.Any())
                    {
                        v1OtherConnect = o.First().Key;
                    }

                    if (itemData.type == straightType || itemData.type == bendType)
                    {
                        //foreach (Vector3 connectPort in new[] { v1AbsConnect, v1OtherConnect })
                        //{
                        //    var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //    g.transform.position = itemData.itemPos + connectPort;
                        //    g.transform.localScale = new Vector3(0.1F, 3F, 0.1F);
                        //}

                        ConnectItem(itemData, new[] { v1AbsConnect, v1OtherConnect });
                    }
                }
                else    //?????L?s??
                {
                    var rootDir = v1ConnectItems.First(m => m.Value == rootItemData).Key;


                    ConnectItem(itemData, new[] { rootDir, rootDir * -1 });
                }
            }

            void ConnectItem(ItemData itemData, Vector3[] dirs)
            {
                Vector3 zeroVec = new Vector3(0, 0, 1);
                float absAngle = (Vector3.SignedAngle(zeroVec, dirs[0], Vector3.up) + 360) % 360;       //0 ~ 360
                float otherAngle = (Vector3.SignedAngle(zeroVec, dirs[1], Vector3.up) + 360) % 360;     //0 ~ 360
                float deltaAngle = Mathf.Abs(absAngle - otherAngle);

                //Debug.Log($"Connect {itemData.GetShortId()} | abs:{absAngle}, other:{otherAngle}, delta:{deltaAngle}");

                if (deltaAngle % 180 == 0F)     //??
                {
                    ChangeItem(itemData, straightType, absAngle);
                }
                else if (deltaAngle % 180 == 90) //?s
                {
                    if (deltaAngle == 270)
                    {
                        if (absAngle < otherAngle)
                        {
                            absAngle += 360;
                        }
                        else
                        {
                            otherAngle += 360;
                        }
                    }

                    // 360 / 270 = 0
                    // 90 / 0 = 90
                    // 180 / 90  = 180
                    // 270 / 180 = 270
                    // 180 / 270 = 270
                    // 360 / 270 = 0

                    float resultAngle = (absAngle > otherAngle ? absAngle : otherAngle);
                    //Debug.Log($"rAng:{resultAngle}");

                    ChangeItem(itemData, bendType, resultAngle);
                }
            }

            void ConnectItemNextFixedUpdate(ItemData itemData, Vector3[] dirs)
            {
                StartCoroutine(Task());

                IEnumerator Task()
                {
                    while (embedRequestList.Count > 0)
                    {
                        yield return null;
                    }

                    yield return new WaitForFixedUpdate();

                    ConnectItem(itemData, dirs);
                }
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


    readonly Queue<ItemData.EmbedRequest> embedRequestList = new Queue<ItemData.EmbedRequest>();
    void ResolveEmbedRequest()
    {
        if (embedRequestList.Count > 0)
        {
            ItemData.EmbedRequest request = embedRequestList.Dequeue();
            if (request.requestType == ItemData.EmbedRequest.RequestType.Embed)
            {
                ItemData.Embed(request.objectPos, request.type, request.rot, request.root, request.id);
            }
            else if (request.requestType == ItemData.EmbedRequest.RequestType.UnEmbed)
            {
                ItemData.UnEmbed(request.itemData);
            }
        }
    }


    void FixedUpdate()
    {

        ResolveEmbedRequest();


        if (EventSystem.current == null)
        {
            return;
        }



        //?????\??
        if (CurrentControlState == ControlState.Place && !EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButton(0))
            {
                // TODOOOOOOOOOOOOOOOOOOO
                // ???K?P?@???????B??
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
