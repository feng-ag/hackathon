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
        // TODO: �ק�Ȯɹ�@
        return new GameObject("Map");
    }

    public Transform GetStartingPoint(GameObject map)
    {
        // TODO: �ק�Ȯɹ�@
        return map.transform;
    }

    public void HideMapEditor()
    {
        // TODO: ��@
    }




}
