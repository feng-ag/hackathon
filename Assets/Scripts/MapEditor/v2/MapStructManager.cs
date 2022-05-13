using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace MapEditor
{

    public class MapStructManager 
    {


        static MapStructManager _Instance;
        public static MapStructManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new MapStructManager();
                }
                return _Instance;
            }
        }



        [JsonProperty("items")]
        public List<ItemData> items = new List<ItemData>();


        public void AddItem(ItemData itemData)
        {
            items.Add(itemData);
        }


        public void RemoveItem(ItemData itemData)
        {
            items.Remove(itemData);
        }



        public void ImportMap(string mapJson)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {

            };

            MapStructManager map = JsonConvert.DeserializeObject<MapStructManager>(mapJson, settings);

            foreach(var item in map.items)
            {
                ItemData.Embed(item.itemPos, item.type, item.itemRot, MapEditorManager.Instance.itemRoot);
            }


        }

        public string ExportMap()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {

            };

            string json = JsonConvert.SerializeObject(this, settings);

            return json;
        }

    }

}