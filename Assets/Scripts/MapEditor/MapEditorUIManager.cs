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

    [SerializeField]
    RectTransform targetInfoContainer;


    List<UI_PlaceItemElement> uiItemList = new List<UI_PlaceItemElement>();
    List<UI_PlaceItemElement> uiEnvList = new List<UI_PlaceItemElement>();




    [SerializeField]
    Toggle[] tabs;

    [SerializeField]
    GameObject[] tabPages;


    public static MapEditorUIManager Instance;


    public int CurrentItemPainterIndex { get; set; } = -1;


    private void Awake()
    {
        Instance = this;

    }

    void Start()
    {
        MapEditorManager gm = MapEditorManager.Instance;



        // Item
        UserItemPainterDataGroup userItemPainterDataGroup = gm.userItemPainterDataGroup;        
        foreach (var (item, index) in userItemPainterDataGroup.Select((item, index) => (item, index)))
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





        editItemRotateLeft.onClick.AddListener(() =>
        {
            currentEditItem.Rotate(90);
            MapEditorManager.Instance.targetCursor.Rotation = currentEditItem.Rotation;
        });
        editItemRotateRight.onClick.AddListener(() =>
        {
            currentEditItem.Rotate(-90);
            MapEditorManager.Instance.targetCursor.Rotation = currentEditItem.Rotation;
        });
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
        SetSelectedItemPainter(gm.CurrentItemType);
        SetSelectedEnv(gm.CurrentEnvIndex);

    }

    private void Update()
    {

        if (MapEditorManager.Instance.CurrentControlState == MapEditorManager.ControlState.Peek)
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
        }


        if (Input.GetKeyDown(KeyCode.X))
        {
            DeleteAllItem();
        }

    }


    void DeleteItem()
    {
        if (currentEditItem != null)
        {
            if (currentEditItem.TypeData.isUnique == true)
            {
                return;
            }

            MapEditorManager.Instance.DeleteItem(currentEditItem.Data);
        }
    }

    void RotateItem()
    {
        if (currentEditItem != null)
        {
            currentEditItem.Rotate(90);
            MapEditorManager.Instance.targetCursor.Rotation = currentEditItem.Rotation;
        }
    }


    void DeleteAllItem()
    {
        foreach(var itemData in MapDataManager.Instance.GetAllItems().ToArray())
        {
            if(itemData.TypeData.isUnique == true)
            {
                continue;
            }

            ItemData.UnEmbed(itemData);
        }
        
        MapEditorManager.Instance.targetCursor.Hide();
    }


    public void SetSelectedItemPainter(int selectItemPainterIndex)
    {
        CurrentItemPainterIndex = selectItemPainterIndex;

        if (selectItemPainterIndex > 0)
        {
            MapEditorManager.Instance.CurrentControlState = MapEditorManager.ControlState.Place;
            MapEditorManager.Instance.cursor.Show();
        }
        else
        {
            MapEditorManager.Instance.CurrentControlState = MapEditorManager.ControlState.Peek;
            MapEditorManager.Instance.cursor.Hide();
        }

        foreach (var (el, index) in uiItemList.Select((item, index) => (item, index)))
        {
            el.SetSelect(selectItemPainterIndex == index);
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

        editItemContainer.gameObject.SetActive(item != null);

        editItemName.text = item == null ? "None" : item.TypeData.name;


        SetTargetInfo();
    }

    public void SetTargetInfo()
    {
        if (currentEditItem == null)
        {
            return;
        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(currentEditItem.transform.position);

        targetInfoContainer.position = screenPos;
        targetInfoContainer.localScale = Vector3.one * (1F / MapEditorManager.Instance.ZoomRatio);
    }



}
