using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ArcadeGalaxyKit
{
    public class ConfigTab : GeneratorTab
    {
        public static ConfigTab Instance;
        GeneratorConfig GeneratorConfig;
        public ConfigTab()
        {
            tabName = "生成器設定";
            SetInstance();
        }
        public override void init()
        {
            SetUpDefaultConfig();
            SetInstance();
            base.init();
        }
        public GeneratorConfig GetConfig()
        {
            Debug.Log("GetConfig " + isInit);
            if (isInit)
            {
                if (!GeneratorConfig) SetUpDefaultConfig();
                return GeneratorConfig;
            }
            else { return null; }
        }

        Vector2 _scrollPos = new Vector2(0, 0);
        Editor _tmpEditor;
        public override void DrawTab()
        {

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            var loadedConfig = (GeneratorConfig)EditorGUILayout.ObjectField("設定檔", GeneratorConfig, typeof(GeneratorConfig), false);
            if (loadedConfig != GeneratorConfig)
            {
                _tmpEditor = Editor.CreateEditor(loadedConfig);
                GeneratorConfig = loadedConfig;
                PreferenceTab.Instance.GeneratorPreference.LastReadConfigPath = AssetDatabase.GetAssetPath(GeneratorConfig);
            }
            if (GeneratorConfig)
            {
                if (!_tmpEditor) _tmpEditor = Editor.CreateEditor(GeneratorConfig);
                _tmpEditor.DrawDefaultInspector();
                if (GUILayout.Button("恢復預設值"))
                {
                    ResetConfig(GeneratorConfig);
                };
            }
            if (GUILayout.Button("讀取最後讀取設定檔"))
            {
                SetUpDefaultConfig();
            };
            EditorGUILayout.EndScrollView();

        }
        void ResetConfig(GeneratorConfig config)
        {
            if (config)
            {
                GUI.FocusControl(null);
                config.BuildFolderPath = Path.Combine(Application.persistentDataPath, "NFT Generator", "Build");
                config.namePrefix = "Arcade Galaxy Cub";
                config.NFTDescription = "Default Description";
                AssignDefaultAssetInFolder<AnimalBodyTypeSetting>(config,
                    Application.dataPath + "/GameData/CubStyleCenterProfile/AnimalType/",
                    "animalBodyTypeSettings");
                AssignDefaultAssetInFolder<SkinSetting>(config,
                    Application.dataPath + "/GameData/CubStyleCenterProfile/Skin/",
                    "skinSettings");
                AssignDefaultAssetInFolder<EyesSetting>(config,
                    Application.dataPath + "/GameData/CubStyleCenterProfile/Eyes/",
                    "eyesSettings");
                AssignDefaultAssetInFolder<GlassesSetting>(config,
                    Application.dataPath + "/GameData/CubStyleCenterProfile/Glasses/",
                    "glassesSettings");
                AssignDefaultAssetInFolder<TireSetting>(config,
                    Application.dataPath + "/GameData/CubStyleCenterProfile/Tire/",
                    "tireSettings");
                EditorUtility.SetDirty(config);

                Debug.Log("Reset Done");
            }
        }

        void AssignDefaultAssetInFolder<T>(GeneratorConfig config, string dirName, string assignFieldName)
        {
            //cubComponentsPool
            var filesStrings = Directory.GetFiles(dirName, "*.asset");
            List<T> objects = new List<T>();
            foreach (string fileString in filesStrings)
            {
                string assetPath = "Assets" + fileString.Replace(Application.dataPath, "").Replace('\\', '/');
                var loadedObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
                var castedObj = (T)Convert.ChangeType(loadedObj, typeof(T));
                objects.Add(castedObj);
            }
            var field = typeof(CubComponentsPool).GetField(assignFieldName);
            field.SetValue(config.cubComponentsPool, objects);
        }
        void SetInstance()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void SetUpDefaultConfig()
        {
            if (!GeneratorConfig)
            {
                if (PreferenceTab.Instance != null)
                {
                    var lastReadConfigPath = PreferenceTab.Instance.GeneratorPreference.LastReadConfigPath;
                    if (lastReadConfigPath != null)
                    {
                        if (!lastReadConfigPath.Equals(""))
                        {
                            GeneratorConfig = AssetDatabase.LoadAssetAtPath(lastReadConfigPath, typeof(GeneratorConfig)) as GeneratorConfig;
                            isInit = true;
                        }
                    }
                }

                if (!GeneratorConfig)
                {
                    Debug.Log("找不到最後讀取設定檔");
                    Debug.Log("建立預設設定檔..");
                    var configObj = ScriptableObject.CreateInstance(typeof(GeneratorConfig));
                    AssetDatabase.CreateAsset(configObj, GeneratorDefaultPath.DefaultConfigPath);
                    PreferenceTab.Instance.GeneratorPreference.LastReadConfigPath = GeneratorDefaultPath.DefaultConfigPath;
                    GeneratorConfig = configObj as GeneratorConfig;
                    ResetConfig(GeneratorConfig);
                    isInit = true;
                    SetUpDefaultConfig();
                }
                else
                {
                    isInit = false;
                }
            }
        }
    }
}