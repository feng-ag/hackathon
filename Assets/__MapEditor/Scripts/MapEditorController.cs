using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MapEditor
{
    public class MapEditorController : IMapEditorController
    {

        static MapEditorController _Instance;
        public static MapEditorController Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new MapEditorController();
                }
                return _Instance;

            }
        }

        string mapJsonCache = null;



        public IEnumerator LoadMap(string mapJson = null)
        {
            if (mapJson == null)
            {
                mapJson = mapJsonCache ?? MapEditorManager.Instance.defaultMapJson.text;
            }

            Debug.Log(mapJson);

            yield return MapDataManager.Instance.LoadMap(mapJson, MapEditorManager.Instance.itemRoot);
        }

        public void ShowMapEditor()
        {
            MapEditorManager.Instance.StartCoroutine(LoadAndShowMap());

            IEnumerator LoadAndShowMap()
            {
                yield return LoadMap();
                MapEditorManager.Instance.editorRoot.SetActive(true);
            }
        }

        public void HideMapEditor()
        {
            MapEditorManager.Instance.editorRoot.SetActive(false);
        }





        public IEnumerator CreateMapObject(Transform root)
        {
            string mapJson = MapDataManager.Instance.ExportMap();
            mapJsonCache = mapJson;

            yield return MapDataManager.Instance.LoadMap(mapJson, root);
        }

        public Transform GetStartingPoint()
        {
            const int TERMINAL_TRAIL_TYPE = 18;

            ItemData startingPointItemData = MapDataManager.Instance.GetAllItems().First(item => item.type == TERMINAL_TRAIL_TYPE);

            return startingPointItemData.item.transform;
        }


    }
}