using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapEditorController 
{

    public GameObject CreacteMapObject();


    public Transform GetStartingPoint(GameObject map);


    public void ShowMapEditor();

    public void HideMapEditor();

}
