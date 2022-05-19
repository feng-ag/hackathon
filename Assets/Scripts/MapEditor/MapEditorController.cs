using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MapEditor;

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



    public IEnumerator CreateMapObject(Transform root)
    {
        string mapJson = MapDataManager.Instance.ExportMap();

        yield return MapDataManager.Instance.LoadMap(mapJson, root);
    }

    public Transform GetStartingPoint()
    {
        const int TERMINAL_TRAIL_TYPE = 18;

        ItemData startingPointItemData = MapDataManager.Instance.GetAllItems().First(item => item.type == TERMINAL_TRAIL_TYPE);

        return startingPointItemData.item.transform;
    }


    public void ShowMapEditor()
    {
        MapEditorManager.Instance.editorRoot.SetActive(true);
    }


    public void HideMapEditor()
    {
        MapEditorManager.Instance.editorRoot.SetActive(false);
    }




}
