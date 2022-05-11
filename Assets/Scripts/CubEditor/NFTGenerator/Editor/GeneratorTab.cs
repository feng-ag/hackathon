using UnityEngine;
using UnityEditor;

namespace ArcadeGalaxyKit
{
    public class GeneratorTab
    {
        public GeneratorTab()
        {
        }

        public string tabName = "GeneratorTab";
        public virtual void DrawTab()
        {
            EditorGUILayout.TextField(tabName + " Content");
        }
        public virtual string GetTabName()
        {
            return tabName;
        }
        public virtual void OnEnable() { init(); }
        public bool isInit = false;
        public virtual void init()
        {
            isInit = true;
        }

    }
}