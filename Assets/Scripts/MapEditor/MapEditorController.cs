using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


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



    public GameObject CreacteMapObject()
    {
        GameObject map = GameObject.Instantiate(MapEditorManager.Instance.mapRoot.gameObject);

        return map;
    }

    public Transform GetStartingPoint(GameObject map)
    {
        const int TERMINAL_TRAIL_TYPE = 0;
        //MapEditorManager.MapEditorItemData terminal = MapEditorManager.Instance.mapEditorItemDataQuery.Values.First(i => i.type == TERMINAL_TRAIL_TYPE);

        //return terminal.item.inner;

        return new GameObject("¼È¥N°_ÂI").transform;
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
