using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace MapEditor
{
    public class ItemData
    {
        public int type;

        public float rot;

        public Pos pos;

        public Item item;

        public ItemTypeData TypeData => MapEditorManager.Instance.itemDataGroup.GetTypeData(type);


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
