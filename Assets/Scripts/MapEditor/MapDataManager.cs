using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace MapEditor
{

    public class MapDataManager 
    {


        static MapDataManager _Instance;
        public static MapDataManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new MapDataManager();
                }
                return _Instance;
            }
        }


        MapData currentMap = new MapData();


        public List<ItemData> GetAllItems()
        {
            return currentMap.items;
        }


        public void AddItem(ItemData itemData)
        {
            currentMap.items.Add(itemData);
        }


        public void RemoveItem(ItemData itemData)
        {
            currentMap.items.Remove(itemData);
        }






        public IEnumerator LoadMap(string mapJson)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {

            };

            MapData loadMap = JsonConvert.DeserializeObject<MapData>(mapJson, settings);
            MapData newMap = new MapData();
            currentMap = newMap;

            Debug.Log($"Load Map: {loadMap.title}");

            // Title
            newMap.title = loadMap.title;

            // Env
            MapEditorManager.Instance.ChangeEnvTo(loadMap.env);
            newMap.env = loadMap.env;


            MapEditorManager.Instance.itemRoot.gameObject.SetActive(false);

            // Items
            foreach (var item in loadMap.items)
            {
                yield return new WaitForFixedUpdate();  // �]��EmbedValid�|�Ψ�Physical.Cast�A�����n��FixedUpdate��s�~�వ�U�ӧP�_

                ItemData.Embed(item.itemPos, item.type, item.itemRot, MapEditorManager.Instance.itemRoot, item.id);
            }

            MapEditorManager.Instance.itemRoot.gameObject.SetActive(true);


            //�ˬd
            string reExportMapJson = ExportMap();
            if(mapJson != reExportMapJson)
            {
                Debug.LogError($"load/export json is different.\n\n{mapJson}\n\n{reExportMapJson}");
            }


        }

        public string ExportMap()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {

            };

            string json = JsonConvert.SerializeObject(currentMap, settings);

            return json;
        }

    }

}