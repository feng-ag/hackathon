using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    public interface IMapEditorController
    {
        public IEnumerator LoadMap(string mapJson);

        public void ShowMapEditor();
        public void HideMapEditor();


        public IEnumerator CreateMapObject(Transform root);

        public Transform GetStartingPoint();


    }
}