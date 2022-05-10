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

        [SerializeField]
        Item item;

        public ItemTypeData TypeData => MapEditorManager.Instance.itemTypeDataGroup.GetTypeData(type);


        Vector3 CursorToOffsetCursorPos(Vector3 cursorPos, int type)
        {
            ItemTypeData itemTypeData = MapEditorManager.Instance.itemTypeDataGroup.GetTypeData(type);

            Vector3 cursorOffsetV3 = itemTypeData.cursorOffsetV3;
            Vector3 hitPos = cursorPos + cursorOffsetV3;

            float x = Mathf.Round(hitPos.x);
            float z = Mathf.Round(hitPos.z);
            Vector3 pos3 = new Vector3(x, 0, z);

            return pos3;
        }

        Vector3 OffsetCursorPosToPlacePos(Vector3 offsetCursorPos, int type)
        {
            ItemTypeData itemTypeData = MapEditorManager.Instance.itemTypeDataGroup.GetTypeData(type);

            return offsetCursorPos + itemTypeData.placeOffsetV3;
        }


        public Item Embed(Vector3 pos, int type, Transform root)
        {
            ItemTypeData itemTypeData = MapEditorManager.Instance.itemTypeDataGroup.GetTypeData(type);

            bool isValid = VaildEmbed(pos, type);

            if (!isValid) 
            {
                return null;
            }

            Vector3 offsetCursroPos = CursorToOffsetCursorPos(pos, type);
            Vector3 placePos = OffsetCursorPosToPlacePos(offsetCursroPos, type);

            item = GameObject.Instantiate(itemTypeData.prefab, placePos, Quaternion.identity, root).GetComponent<Item>();

            //Vector2 pos2 = new Vector2(x, z);

            ItemData itemData = new ItemData
            {
                type = type,
                rot = 0,
                //pos = pos2,
                item = item,
            };

            item.data = itemData;

            return item;
        }


        bool VaildEmbed(Vector3 pos, int type)
        {
            ItemTypeData itemTypeData = MapEditorManager.Instance.itemTypeDataGroup.GetTypeData(type);

            Vector3 placePos = CursorToOffsetCursorPos(pos, type);

            Vector3 placeOffsetV3 = itemTypeData.placeOffsetV3;

            foreach (var grid in itemTypeData.grids)
            {
                Vector3 pos3 = grid.GetVector3() + itemTypeData.placeOffsetV3;

                Vector3 vaildPos = placePos + pos3 - placeOffsetV3;

                RaycastHit[] hits = Physics.BoxCastAll(
                    vaildPos,
                    Vector3.one * 0.45F,    //比0.5略小，避免誤碰到其他格
                    Vector3.forward,
                    Quaternion.identity,
                    0,
                    MapEditorManager.Instance.itemLayer.value);

                if (hits.Length > 0)
                {
                    return false;
                }

            }

            return true;
        }



        public void UnEmbed()
        {
            // TODO
        }

    }

}
