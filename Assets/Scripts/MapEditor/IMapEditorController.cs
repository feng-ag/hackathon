using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapEditorController 
{

    public IEnumerator CreateMapObject(Transform root);


    public Transform GetStartingPoint();


    public void ShowMapEditor();

    public void HideMapEditor();

}
