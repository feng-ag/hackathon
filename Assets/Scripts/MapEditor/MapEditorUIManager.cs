using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MapEditorUIManager : MonoBehaviour
{
    [SerializeField]
    Canvas canvas;

    [SerializeField]
    RectTransform placeItemContainer;

    [SerializeField]
    GameObject placeItemElement;

    [SerializeField]
    ItemBase currentEditItem;

    //Edit

    [SerializeField]
    RectTransform editItemContainer;

    [SerializeField]
    Text editItemName;

    [SerializeField]
    Button editItemRotateLeft;

    [SerializeField]
    Button editItemRotateRight;

    [SerializeField]
    Button editItemDelete;

    [SerializeField]
    Button editItemClear;


    //State


    [SerializeField]
    Button placeMode;

    [SerializeField]
    Button selectMode;


    List<UI_PlaceItemElement> uiItemList = new List<UI_PlaceItemElement>();


    public static MapEditorUIManager Instance;

    private void Awake()
    {
        Instance = this;

    }

    void Start()
    {
        MapEditorManager gm = MapEditorManager.Instance;

        ItemData itemData = gm.itemData;

        
        foreach (var (item, index) in itemData.Select((item, index) => (item, index)))
        {
            UI_PlaceItemElement itemEl = Instantiate(placeItemElement, placeItemContainer).GetComponent<UI_PlaceItemElement>();
            itemEl.SetItem(item);
            itemEl.SetOnClick(() => gm.onClickPlaceItem(index));
            uiItemList.Add(itemEl);
        }


        editItemRotateLeft.onClick.AddListener(() => currentEditItem.RotateLeft());
        editItemRotateRight.onClick.AddListener(() => currentEditItem.RotateRight());
        editItemDelete.onClick.AddListener(() => DeleteItem());
        editItemClear.onClick.AddListener(() => DeleteAllItem());


        placeMode.onClick.AddListener(() => gm.CurrentControlState = MapEditorManager.ControlState.Place);
        selectMode.onClick.AddListener(() => gm.CurrentControlState = MapEditorManager.ControlState.Peek);


        SetTarget(null);
        SetSelectedItem(gm.CurrentPlaceItemIndex);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateItem();
        }
        else if (Input.GetKeyDown(KeyCode.Delete))
        {
            DeleteItem();
        }
    }


    void DeleteItem()
    {
        if (currentEditItem != null)
        {
            Vector2 pos = currentEditItem.pos;
            MapEditorManager.Instance.mapEditorItemDataQuery.Remove(pos);

            Destroy(currentEditItem.gameObject);
            SetTarget(null);
            MapEditorManager.Instance.targetCursor.gameObject.SetActive(false);
        }
    }

    void RotateItem()
    {
        if (currentEditItem != null)
        {
            currentEditItem.RotateLeft();
        }
    }


    void DeleteAllItem()
    {
        foreach(var item in MapEditorManager.Instance.mapEditorItemDataQuery)
        {
            Destroy(item.Value.item.gameObject);
        }

        MapEditorManager.Instance.mapEditorItemDataQuery.Clear();
        SetTarget(null);
        MapEditorManager.Instance.targetCursor.gameObject.SetActive(false);

    }


    public void SetSelectedItem(int selectIndex)
    {
        foreach (var (el, index) in uiItemList.Select((item, index) => (item, index)))
        {
            el.SetSelect(selectIndex == index);
        }
    }

    public void SetTarget(ItemBase itemBase)
    {
        currentEditItem = itemBase;
        editItemName.text = itemBase == null ? "None" : itemBase.item.name;

        editItemContainer.gameObject.SetActive(itemBase != null);
    }



}