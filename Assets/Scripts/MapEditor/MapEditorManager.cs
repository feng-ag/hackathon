using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class MapEditorManager : MonoBehaviour
{
    [SerializeField]
    public GameObject editorRoot;

    [SerializeField]
    Camera cam;

    [SerializeField]
    Collider ground;

    [SerializeField]
    public Transform spawnRoot;

    [SerializeField]
    LayerMask groundLayer;

    [SerializeField]
    LayerMask itemLayer;

    [SerializeField]
    public ItemData itemData;

    [SerializeField]
    public Transform cursor;

    [SerializeField]
    public Transform targetCursor;

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

    public ControlState CurrentControlState = ControlState.Place;


    public struct MapEditorItemData
    {
        public Vector2 pos;
        public int type;
        public float rotate;

        public ItemBase item;
    }

    public Dictionary<Vector2, MapEditorItemData> mapEditorItemDataQuery = new Dictionary<Vector2, MapEditorItemData>();




    public Action<int> onClickPlaceItem { get; private set; }

    public static MapEditorManager Instance;


    void Awake()
    {
        Instance = this;

        ConstructUIEvents();

        MapEditorController.Instance.HideMapEditor();

        void ConstructUIEvents()
        {
            onClickPlaceItem += (int index) =>
            {
                CurrentPlaceItemIndex = index;
                MapEditorUIManager.Instance.SetSelectedItem(index);
                Debug.Log($"Set Index = {index}");
            };
        }
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
        
        GameObject mapIns = Instantiate(map, spawnRoot);


        ItemBase[] itemBaseList = mapIns.GetComponentsInChildren<ItemBase>();


        foreach (var itemBase in itemBaseList)
        {


            MapEditorItemData mapEditorItemData = new MapEditorItemData
            {
                item = itemBase,
                pos = itemBase.pos,
                rotate = itemBase.Rotate,
                type = itemBase.itemType,
            };

            mapEditorItemDataQuery[itemBase.pos] = mapEditorItemData;

        }

    }



    public int CurrentPlaceItemIndex { get; set; } = 0;


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
                ItemBase itemBase = null;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000, itemLayer.value) &&
                    hitInfo.collider.TryGetComponent(out itemBase))
                {
                    CurrentControlState = ControlState.Place;
                    targetCursor.position = itemBase.transform.position;
                    targetCursor.gameObject.SetActive(true);
                    MapEditorUIManager.Instance.SetTarget(itemBase);
                }
                else if (Physics.Raycast(ray, out RaycastHit hitInfo2, 1000, groundLayer.value))
                {
                    CurrentControlState = ControlState.Peek;
                    float x = Mathf.Round(hitInfo2.point.x);
                    float z = Mathf.Round(hitInfo2.point.z);
                    Vector3 pos3 = new Vector3(x, 0, z);

                    ItemData.Item item = itemData.GetItemAt(CurrentPlaceItemIndex);


                    itemBase = Instantiate(item.RandomPickPrefab(), pos3, Quaternion.identity, spawnRoot).GetComponent<ItemBase>();

                    Vector2 pos2 = new Vector2(x, z);
                    itemBase.itemType = CurrentPlaceItemIndex;
                    itemBase.pos = pos2;

                    MapEditorItemData mapEditorItemData = new MapEditorItemData
                    {
                        pos = pos2,
                        rotate = 0,
                        type = CurrentPlaceItemIndex,
                        item = itemBase,
                    };

                    mapEditorItemDataQuery[pos2] = mapEditorItemData;

                    targetCursor.gameObject.SetActive(true);
                    targetCursor.transform.position = itemBase.transform.position;
                    MapEditorUIManager.Instance.SetTarget(itemBase);
                }
                else
                {
                    targetCursor.gameObject.SetActive(false);
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

        if (CurrentControlState != ControlState.Move && !EventSystem.current.IsPointerOverGameObject())
        {
            ItemBase itemBase = null;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000, itemLayer.value) &&
                hitInfo.collider.TryGetComponent(out itemBase))
            {
                CurrentControlState = ControlState.Place;
                cursor.position = itemBase.transform.position;
                cursor.gameObject.SetActive(true);
            }
            else if (Physics.Raycast(ray, out RaycastHit hitInfo2, 1000, groundLayer.value))
            {
                CurrentControlState = ControlState.Peek;
                float x = Mathf.Round(hitInfo2.point.x);
                float z = Mathf.Round(hitInfo2.point.z);
                Vector3 pos = new Vector3(x, 0, z);


                cursor.gameObject.SetActive(true);
                cursor.transform.position = pos;
            }
            //else
            //{
            //    cursor.gameObject.SetActive(false);
            //    UIManager.Instance.SetTarget(null);
            //}

        }
    }




}
