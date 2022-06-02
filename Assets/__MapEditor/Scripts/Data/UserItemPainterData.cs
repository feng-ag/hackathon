using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnionCollections.DataEditor;


namespace MapEditor
{
    public class UserItemPainterData : ScriptableObject, ITitleAndIconReadable, IQueryableData
    {
        public string GetID() => Title;

        [SerializeField]
        string name;

        [SerializeField]
        Sprite icon;

        [SerializeField]
        public bool isRandomRot;

        [SerializeField]
        public bool isConnectable;


        [NodeElement]
        [SerializeField]
        public ItemTypeData[] typeDataList = new ItemTypeData[0];


        public string Title => name;

        public Sprite Icon => icon;





        public ItemTypeData GetTypeData()
        {
            if(isConnectable == true)
            {
                return typeDataList[0];     //可連接物件預設先使用[0]
            }
            else
            {
                return typeDataList[Random.Range(0, typeDataList.Length)];
            }
        }



    }
}