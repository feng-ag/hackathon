using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Newtonsoft.Json;


namespace MapEditor
{
    [Serializable]
    public class ItemData
    {
        [JsonProperty("id")]
        public string id;

        [JsonProperty("type")]
        public int type;

        [JsonProperty("rot")]
        public float itemRot;

        [JsonProperty("pos")]
        /// <summary>Object Pos</summary>
        public Vector3 itemPos;


        [JsonIgnore]
        public Item item;


        [JsonIgnore]
        public ItemTypeData TypeData => MapEditorManager.Instance.itemTypeDataGroup.GetTypeData(type);

        public string GetShortId()
        {
            return id.Substring(0, 8);
        }

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
            Vector3 grid = cursorPos - itemTypeData.CursorOffsetV3;  //先不算旋轉

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
            Vector3 objectPos = gridPos - itemTypeData.GridOffsetV3;        //先不算旋轉

            return objectPos;
        }




        public static Vector3 ObjectPosToGridPos(Vector3 objectPos, int type, float rot)
        {
            ItemTypeData itemTypeData = MapEditorManager.Instance.itemTypeDataGroup.GetTypeData(type);

            Quaternion q = Quaternion.AngleAxis(rot, Vector3.up);

            //Vector3 gridPos = objectPos + q * itemTypeData.GridOffsetV3;
            Vector3 gridPos = objectPos + itemTypeData.GridOffsetV3;        //先不算旋轉

            return gridPos;

        }


        public static Vector3 ObjectPosToCursorPos(Vector3 objectPos, int type, float rot)
        {
            ItemTypeData itemTypeData = MapEditorManager.Instance.itemTypeDataGroup.GetTypeData(type);

            Quaternion q = Quaternion.AngleAxis(rot, Vector3.up);

            //Vector3 cursorPos = objectPos + q * itemTypeData.GridOffsetV3;
            Vector3 cursorPos = objectPos + itemTypeData.GridOffsetV3;        //先不算旋轉

            return cursorPos;

        }



        //--


        public static Item EmbedAtCursorPos(Vector3 cursorPos, int type, float rot, Transform root)
        {

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


            Item result = Embed(objectPos, type, rot, root);
            return result;

        }

        public static Item Embed(Vector3 objectPos, int type, float rot, Transform root, string id=null)
        {
            ItemTypeData itemTypeData = MapEditorManager.Instance.itemTypeDataGroup.GetTypeData(type);

            bool isValid = VaildEmbed(objectPos, type, rot);

            if (!isValid)
            {
                if (id != null)
                {
                    Debug.Log($"{id} embed failed at ObjectPos(${objectPos.x}, ${objectPos.z})/Rot(${rot})");
                }
                return null;
            }

            Item item = GameObject.Instantiate(itemTypeData.item.gameObject, objectPos, Quaternion.identity, root).GetComponent<Item>();

            ItemData itemData = new ItemData
            {
                id = id ?? Guid.NewGuid().ToString(),
                type = type,
                itemRot = rot,
                itemPos = objectPos,
                item = item,
            };

            item.data = itemData;

            item.SyncData();

            MapDataManager.Instance.AddItem(itemData);

            return item;
        }

        static bool VaildEmbed(Vector3 objectPos, int type, float rot)
        {
            ItemTypeData itemTypeData = MapEditorManager.Instance.itemTypeDataGroup.GetTypeData(type);

            Vector3 gridPos = ObjectPosToGridPos(objectPos, type, rot);

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
                    Vector3.one * 0.45F,    //比0.5略小
                    Vector3.forward,
                    Quaternion.identity,
                    0,
                    MapEditorManager.Instance.itemLayer.value);

                if (hits.Length > 0)
                {
                    ////Grid
                    //GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //g.transform.localPosition = vaildRotPos;
                    //g.transform.localScale = new Vector3(0.1F, 5F, 0.1F);
                    //g.name = $"WRONG";

                    
                    foreach(var hit in hits)
                    {
                        hit.collider.gameObject.name = "AAA";
                    }


                    return false;
                }

            }


            return true;
        }


        public static void UnEmbed(ItemData itemData)
        {
            MapDataManager.Instance.RemoveItem(itemData);

            if (itemData.item != null)
            {
                GameObject.Destroy(itemData.item.gameObject);
            }
        }

        //--



        static Vector3[] neighborItemPosList = new Vector3[]
        {
                new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 0, -1),
        };


        public static Dictionary<Vector3, ItemData> GetNeighborItems(ItemData itemData)
        {
            // NOTICE:
            // 只適用 1x1 物件


            Dictionary<Vector3, ItemData> result = new Dictionary<Vector3, ItemData>();

            foreach (Vector3 pos in neighborItemPosList)
            {
                RaycastHit[] hits = Physics.BoxCastAll(
                    itemData.itemPos + pos,
                    Vector3.one * 0.45F,    //比0.5略小
                    Vector3.forward,
                    Quaternion.identity,
                    0,
                    MapEditorManager.Instance.itemLayer.value);

                if(hits.Length == 0)
                {
                    continue;
                }

                Item item = hits
                    .Select(hit => hit.collider.GetComponent<ItemColLinker>())
                    .Where(linker => linker != null)
                    .Select(linker => linker.item)
                    .FirstOrDefault(item => item.Data.type == 3 || item.Data.type == 4);     //篩選直路彎路

                if (item != null)
                {
                    result.Add(pos * 0.5F, item.Data);
                }
            }


            return result;

        }


        public static Dictionary<Vector3, ItemData> GetConnectItems(ItemData itemData)
        {
            var neighbors = GetNeighborItems(itemData);
            var ports = GetConnectPorts(itemData);

            var result = neighbors.Where(n => ports.Any(p => p == n.Key));

            return result.ToDictionary(d => d.Key, d => d.Value);

        }


        public static List<Vector3> GetConnectPorts(ItemData itemData)
        {
            List<Vector3> result = new List<Vector3>();

            //這裡先寫死Rot=0時，ConnectPorts的定義
            if (itemData.type == 3)         //直路
            {
                result.Add(new Vector3(0, 0, 0.5F));
                result.Add(new Vector3(0, 0, -0.5F));
            }
            else if (itemData.type == 4)    //彎路
            {
                result.Add(new Vector3(0, 0, 0.5F));
                result.Add(new Vector3(-0.5F, 0, 0));
            }

            //

            //根據該 Item 角度，轉換 Port 的對應位置
            float itemRot = itemData.itemRot;
            Quaternion q = Quaternion.AngleAxis(itemRot, Vector3.up);

            result = result.Select(port => q * port).ToList();

            return result;

        }


        //--

        public static ItemData ImportFromJson(string json)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {

            };

            ItemData itemData = JsonConvert.DeserializeObject<ItemData>(json, settings);

            return itemData;
        }

        public static string ExportToJson(ItemData itemData)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings(){
             
            };

            string json = JsonConvert.SerializeObject(itemData, settings);
            
            return json;
        }



    }

}
