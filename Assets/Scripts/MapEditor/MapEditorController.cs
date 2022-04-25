using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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
        GameObject map = GameObject.Instantiate(MapEditorManager.Instance.spawnRoot.gameObject);

        return map;
    }

    public Transform GetStartingPoint(GameObject map)
    {
        // TODO: 修改暫時實作
        return map.transform;
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
