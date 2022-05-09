using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using MapEditor;


public class MapEditorUIManager : MonoBehaviour
{
    [SerializeField]
    Canvas canvas;

    [SerializeField]
    RectTransform placeItemContainer;

    [SerializeField]
    RectTransform envContainer;


    [SerializeField]
    GameObject placeItemElement;

    [SerializeField]
    public Item currentEditItem;

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


    List<UI_PlaceItemElement> uiItemList = new List<UI_PlaceItemElement>();
    List<UI_PlaceItemElement> uiEnvList = new List<UI_PlaceItemElement>();




    [SerializeField]
    Toggle[] tabs;

    [SerializeField]
    GameObject[] tabPages;


    public static MapEditorUIManager Instance;

    private void Awake()
    {
        Instance = this;

    }

    void Start()
    {
        MapEditorManager gm = MapEditorManager.Instance;



        // Item
        ItemTypeDataGroup itemData = gm.itemDataGroup;        
        foreach (var (item, index) in itemData.Select((item, index) => (item, index)))
        {
            UI_PlaceItemElement itemEl = Instantiate(placeItemElement, placeItemContainer).GetComponent<UI_PlaceItemElement>();
            itemEl.SetItem(item);
            itemEl.SetOnClick(() => gm.onClickPlaceItem(index));
            uiItemList.Add(itemEl);
        }


        // Env
        EnvironmentData environmentData = gm.environmentData;
        foreach (var (item, index) in environmentData.Select((item, index) => (item, index)))
        {
            UI_PlaceItemElement itemEl = Instantiate(placeItemElement, envContainer).GetComponent<UI_PlaceItemElement>();
            itemEl.SetItem(item);
            itemEl.SetOnClick(() => gm.onClickEnvItem(index));
            uiEnvList.Add(itemEl);
        }





        editItemRotateLeft.onClick.AddListener(() => currentEditItem.Rotate(90));
        editItemRotateRight.onClick.AddListener(() => currentEditItem.Rotate(-90));
        editItemDelete.onClick.AddListener(() => DeleteItem());
        editItemClear.onClick.AddListener(() => DeleteAllItem());


        for (int i = 0; i < tabs.Length; i++)
        {
            int iCache = i;
            tabs[i].onValueChanged.AddListener(isOn =>
            {
                for (int p = 0; p < tabPages.Length; p++)
                {
                    tabPages[p].SetActive(p == iCache);
                    
                }
            });
        }


        SetTarget(null);
        SetSelectedItem(gm.CurrentItemType);
        SetSelectedEnv(gm.CurrentEnvIndex);

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
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            DeleteItem();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            DeleteAllItem();
        }
    }


    void DeleteItem()
    {
        //if (currentEditItem != null)
        //{
        //    Vector2 pos = currentEditItem.pos;
        //    if (MapEditorManager.Instance.DeleteItemByUser(item))
        //    {
        //        SetTarget(null);
        //        MapEditorManager.Instance.targetCursor.gameObject.SetActive(false);
        //    }
        //}

        throw new System.NotImplementedException();
    }

    void RotateItem()
    {
        if (currentEditItem != null)
        {
            currentEditItem.Rotate(90);
        }
    }


    void DeleteAllItem()
    {
        //foreach(var item in MapEditorManager.Instance.mapEditorItemDataQuery.Values.ToArray())
        //{
        //    MapEditorManager.Instance.DeleteItemByUser(item);
        //}

        //SetTarget(null);
        //MapEditorManager.Instance.targetCursor.gameObject.SetActive(false);

        throw new System.NotImplementedException();
    }


    public void SetSelectedItem(int selectIndex)
    {
        foreach (var (el, index) in uiItemList.Select((item, index) => (item, index)))
        {
            el.SetSelect(selectIndex == index);
        }
    }

    public void SetSelectedEnv(int selectIndex)
    {
        foreach (var (el, index) in uiEnvList.Select((item, index) => (item, index)))
        {
            el.SetSelect(selectIndex == index);
        }
    }

    public void SetTarget(Item item)
    {
        currentEditItem = item;
        editItemName.text = item == null ? "None" : item.TypeData.name;

        editItemContainer.gameObject.SetActive(item != null);
    }



}
