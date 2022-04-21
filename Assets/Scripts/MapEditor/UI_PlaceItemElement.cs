using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UI_PlaceItemElement : MonoBehaviour
{

    [SerializeField]
    Button button;

    [SerializeField]
    Text nameText;

    [SerializeField]
    Image iconImg;

    [SerializeField]
    GameObject selected;


    ItemData.Item item;


    private void Start()
    {
        
    }

    public void SetSelect(bool isSelect)
    {
        selected.SetActive(isSelect);
    }

    public void SetItem(ItemData.Item _item) {
        item = _item;

        nameText.text = item.name;
        iconImg.sprite = item.icon;
    }

    public void SetOnClick(Action action)
    {
        button.onClick.AddListener(() => action());
    }

}
