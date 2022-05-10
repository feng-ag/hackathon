using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



namespace MapEditor
{
    [Serializable]
    public class ItemData
    {
        public int type;

        public float rot;

        public Pos pos;

        public Item item;

        public ItemTypeData TypeData => MapEditorManager.Instance.itemTypeDataGroup.GetTypeData(type);


        public void Embed(Pos pos)
        {
            // TODO
        }

        public void UnEmbed()
        {
            // TODO
        }

    }

}
