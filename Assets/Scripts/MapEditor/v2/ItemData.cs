using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



namespace MapEditor
{
    [Serializable]
    public class ItemData
    {
        public string id;

        public int type;

        public float itemRot;

        /// <summary>
        /// Place Pos
        /// </summary>
        public Vector3 itemPos;

        public Item item;


        public ItemTypeData TypeData => MapEditorManager.Instance.itemTypeDataGroup.GetTypeData(type);


        /*
         
        ObjectPos
        CursorPos
        GridPos

         */


        public static Vector3 CursorPosToGridPos(Vector3 cursorPos, int type, float rot)
        {
            ItemTypeData itemTypeData = MapEditorManager.Instance.itemTypeDataGroup.GetTypeData(type);

            Quaternion q = Quaternion.AngleAxis(rot, Vector3.up);

            //Vector3 grid = cursorPos - q * itemTypeData.CursorOffsetV3;
            Vector3 grid = cursorPos - itemTypeData.CursorOffsetV3;  //這邊不處理旋轉

            float x = Mathf.Round(grid.x);
            float z = Mathf.Round(grid.z);
            Vector3 gridPos = new Vector3(x, 0, z);

            return gridPos;

        }

        public static Vector3 GridPosToObjectPos(Vector3 gridPos, int type, float rot)
        {
            ItemTypeData itemTypeData = MapEditorManager.Instance.itemTypeDataGroup.GetTypeData(type);

            Quaternion q = Quaternion.AngleAxis(rot, Vector3.up);

            //Vector3 objectPos = gridPos - q * itemTypeData.GridOffsetV3;
            Vector3 objectPos = gridPos - itemTypeData.GridOffsetV3;        //這邊不處理旋轉

            return objectPos;
        }




        public static Vector3 ObjectPosToGridPos(Vector3 objectPos, int type, float rot)
        {
            ItemTypeData itemTypeData = MapEditorManager.Instance.itemTypeDataGroup.GetTypeData(type);

            Quaternion q = Quaternion.AngleAxis(rot, Vector3.up);

            Vector3 gridPos = objectPos + q * itemTypeData.GridOffsetV3;

            return gridPos;

        }


        public static Vector3 ObjectPosToCursorPos(Vector3 objectPos, int type, float rot)
        {
            ItemTypeData itemTypeData = MapEditorManager.Instance.itemTypeDataGroup.GetTypeData(type);

            Quaternion q = Quaternion.AngleAxis(rot, Vector3.up);

            Vector3 cursorPos = objectPos + q * itemTypeData.GridOffsetV3;

            return cursorPos;

        }



        //--


        static Vector3 CursorToOffsetCursorPos(Vector3 cursorPos, int type)
        {
            ItemTypeData itemTypeData = MapEditorManager.Instance.itemTypeDataGroup.GetTypeData(type);

            Vector3 cursorOffsetV3 = itemTypeData.CursorOffsetV3;
            Vector3 hitPos = cursorPos + cursorOffsetV3;

            float x = Mathf.Round(hitPos.x);
            float z = Mathf.Round(hitPos.z);
            Vector3 pos3 = new Vector3(x, 0, z);

            return pos3;
        }

        static Vector3 OffsetCursorPosToPlacePos(Vector3 offsetCursorPos, int type)
        {
            ItemTypeData itemTypeData = MapEditorManager.Instance.itemTypeDataGroup.GetTypeData(type);

            return offsetCursorPos + itemTypeData.GridOffsetV3;
        }




        public static Item Embed(Vector3 cursorPos, int type, float rot, Transform root)
        {
            ItemTypeData itemTypeData = MapEditorManager.Instance.itemTypeDataGroup.GetTypeData(type);

            bool isValid = VaildEmbed(cursorPos, type, rot);

            if (!isValid) 
            {
                return null;
            }

            ////
            //GameObject g3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //g3.transform.position = cursorPos;
            //g3.transform.localScale = new Vector3(0.1F, 12F, 0.1F);
            //g3.name = "cursor";
            ////

            Vector3 gridPos = CursorPosToGridPos(cursorPos, type, rot);

            ////
            //GameObject g2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //g2.transform.position = gridPos;
            //g2.transform.localScale = new Vector3(0.1F, 12F, 0.1F);
            //g2.name = "grid";
            ////

            Vector3 objectPos = GridPosToObjectPos(gridPos, type, rot); 

            ////
            //GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //g.transform.position = objectPos;
            //g.transform.localScale = new Vector3(0.1F, 12F, 0.1F);
            //g.name = "obj";
            ////

            Item item = GameObject.Instantiate(itemTypeData.item.gameObject, objectPos, Quaternion.identity, root).GetComponent<Item>();

            ItemData itemData = new ItemData
            {
                id = Guid.NewGuid().ToString(),
                type = type,
                itemRot = rot,
                itemPos = objectPos,
                item = item,
            };

            item.data = itemData;

            item.SyncData();

            MapStructManager.Instance.AddItem(itemData);

            return item;
        }

        static bool VaildEmbed(Vector3 cursorPos, int type, float rot)
        {
            ItemTypeData itemTypeData = MapEditorManager.Instance.itemTypeDataGroup.GetTypeData(type);

            Vector3 gridPos = CursorPosToGridPos(cursorPos, type, rot);
            Vector3 objectPos = GridPosToObjectPos(gridPos, type, rot);

            //Vector3 cursorPos = CursorToOffsetCursorPos(pos, type);
            //Vector3 placePos = OffsetCursorPosToPlacePos(cursorPos, type);

            ////Center
            //GameObject gd = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //gd.transform.localPosition = placePos;
            //gd.transform.localScale = new Vector3(0.1F, 10F, 0.1F);

            foreach (var grid in itemTypeData.Grids)
            {
                Vector3 pos3 = gridPos + grid - itemTypeData.GridOffsetV3;

                Vector3 vaildPos = pos3 - objectPos;
                Quaternion q = Quaternion.AngleAxis(rot, Vector3.up);
                Vector3 vaildRotPos = objectPos + q * vaildPos;


                RaycastHit[] hits = Physics.BoxCastAll(
                    vaildRotPos,
                    Vector3.one * 0.45F,    //比0.5略小，避免誤碰到其他格
                    Vector3.forward,
                    Quaternion.identity,
                    0,
                    MapEditorManager.Instance.itemLayer.value);

                ////Grid
                //GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //g.transform.localPosition = vaildRotPos;
                //g.transform.localScale = new Vector3(0.1F, 5F, 0.1F);

                if (hits.Length > 0)
                {
                    return false;
                }

            }

            return true;
        }


        public static void UnEmbed(ItemData itemData)
        {
            MapStructManager.Instance.RemoveItem(itemData);

            GameObject.Destroy(itemData.item.gameObject);
        }

    }

}
