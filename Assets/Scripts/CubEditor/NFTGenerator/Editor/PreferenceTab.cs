using UnityEditor;
using UnityEngine;
namespace ArcadeGalaxyKit
{
    public class PreferenceTab : GeneratorTab
    {
        public static PreferenceTab Instance;
        public GeneratorPreference GeneratorPreference;
        public PreferenceTab()
        {
            tabName = "偏好設定";
            init();
        }
        public GeneratorPreference GetGeneratorPreference()
        {
            if (isInit)
            {
                if (!GeneratorPreference) LoadPreference();
                return GeneratorPreference;
            }
            else
            {
                return null;
            }
        }
        void SetInstance()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void LoadPreference(bool fourceReload = false)
        {
            if (!tmpEditor || fourceReload)
            {
                if (!GeneratorPreference || fourceReload)
                {
                    var tryLoad = AssetDatabase.LoadAssetAtPath(GeneratorDefaultPath.PreferencePath, typeof(GeneratorPreference));
                    Object preferencceObj;
                    if (tryLoad == null)
                    {
                        preferencceObj = ScriptableObject.CreateInstance(typeof(GeneratorPreference));
                        AssetDatabase.CreateAsset(preferencceObj, GeneratorDefaultPath.PreferencePath);
                    }
                    else
                    {
                        preferencceObj = tryLoad;
                    }

                    GeneratorPreference = preferencceObj as GeneratorPreference;
                    isInit = true;
                }
                tmpEditor = Editor.CreateEditor((Object)GeneratorPreference);
            }
        }
        public override void init()
        {
            LoadPreference();
            SetInstance();
            base.init();
        }
        Editor tmpEditor;
        public override void DrawTab()
        {
            EditorGUILayout.BeginToggleGroup("限制編輯", false);
            var loadedGeneratorPreference = (GeneratorPreference)EditorGUILayout.ObjectField("偏好設定", GeneratorPreference, typeof(GeneratorPreference), false);
            EditorGUILayout.HelpBox(tabName + " 只會有一份且在固定位置，不能更改位置\n一些編輯器內部的索引位置會寫在偏好設定裡 。", MessageType.Info);
            EditorGUILayout.EndToggleGroup();
            if (GeneratorPreference)
            {
                tmpEditor.DrawDefaultInspector();
            }

            if (GUILayout.Button("Reload 偏好設定"))
            {
                LoadPreference(true);
            }
        }
    }
}