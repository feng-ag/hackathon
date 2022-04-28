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




    private void Start()
    {
        
    }

    public void SetSelect(bool isSelect)
    {
        selected.SetActive(isSelect);
    }

    public void SetItem(ITitleAndIconReadable item) {

        nameText.text = item.Title;
        iconImg.sprite = item.Icon;

    }

    public void SetOnClick(Action action)
    {
        button.onClick.AddListener(() => action());
    }

}
