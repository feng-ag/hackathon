using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using UnityEditorInternal;
namespace ArcadeGalaxyKit
{
    public class BuildTab : GeneratorTab
    {
        List<GeneratedCubData> generatedCubDatas = new List<GeneratedCubData>();

        public BuildTab()
        {
            tabName = "生成頁面";
        }
        Vector2 _scrollPos = new Vector2(0, 0);
        private ReorderableList list;
        Editor _tmpEditor;
        public override void DrawTab()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            if (GUILayout.Button("Gen"))
            {
                SetUpBuildFolder();
                Gen();
            }
            GUILayout.Button("UpdateMeta");
            if (GUILayout.Button("Try Launch Scene"))
            {
                if (!EditorApplication.isPlaying)
                {
                    Scene activeScene = SceneManager.GetActiveScene();
                    var scenePath = GeneratorDefaultPath.ToolScenePath;
                    var tokens = scenePath.Split('/');
                    if (!activeScene.name.Equals(tokens[tokens.Length - 1].Replace(".unity", "")))
                    {
                        EditorSceneManager.OpenScene(GeneratorDefaultPath.ToolScenePath);
                    }
                    EditorSceneManager.MarkSceneDirty(activeScene);
                    EditorApplication.EnterPlaymode();
                    GameObject testAdd = new GameObject();
                    Undo.RegisterCreatedObjectUndo(testAdd, "Created testAdd ");
                    testAdd.name = "testAdd";
                    var runTimeData = testAdd.AddComponent<RunTimeGeneratorDataContainer>();
                    runTimeData.generatedCubDatas = generatedCubDatas;
                    runTimeData.config = ConfigTab.Instance.GetConfig();
                    //Generate photo
                    //UnityEngine.Object.DestroyImmediate(testAdd);
                    //EditorApplication.ExitPlaymode();
                }
                else
                {
                    EditorUtility.DisplayDialog("NFTGenerator Info", "先離開 play mode 吧，老兄。" + EditorSceneManager.loadedSceneCount.ToString(), "Ok");
                }

            }
            if (GUILayout.Button("End Gen"))
            {
                EditorApplication.ExitPlaymode();
                EditorApplication.playModeStateChanged += ((ExitingPlayMode) =>
                {
                    EditorSceneManager.OpenScene(GeneratorDefaultPath.ToolScenePath);
                });

            };
            if (GUILayout.Button("Show Build Folder"))
            {
                var buildPathString = ConfigTab.Instance.GetConfig().BuildFolderPath;
                buildPathString = Path.GetFullPath(buildPathString);
                System.Diagnostics.Process.Start(buildPathString);
            };
            //if (list != null)
            //{
            //    EditorGUILayout.LabelField("TempData");
            //    list.DoLayoutList();
            //}
            //else
            //{
            //    list = new ReorderableList(generatedCubDatas, typeof(GeneratedCubData), false, false, false, false);
            //}
            EditorGUILayout.EndScrollView();
        }

        void Gen()
        {
            generatedCubDatas.Clear();
            var config = ConfigTab.Instance.GetConfig();
            var amountToGen = config.GenerateNFTAmount;
            var componentPool = config.cubComponentsPool;
            var prefix = config.namePrefix;
            var description = config.NFTDescription;
            var url = config.BaseURL;
            for (int i = 0; i < amountToGen; i++)
            {
                var indexNotForPrograme = i + 1;
                GeneratedCubData cubData = ScriptableObject.CreateInstance("GeneratedCubData") as GeneratedCubData;
                cubData.date = DateTime.Now.ToString();
                cubData.edition = indexNotForPrograme.ToString();
                cubData.cubName = prefix + " " + indexNotForPrograme.ToString();
                cubData.description = description;
                cubData.image = "ipfs://" + url + "/" + indexNotForPrograme.ToString() + ".png";
                var index = Random.Range((int)0, (int)componentPool.animalBodyTypeSettings.Count);
                cubData.animalBodyTypeSetting = componentPool.animalBodyTypeSettings[index];

                index = Random.Range((int)0, (int)componentPool.skinSettings.Count);
                cubData.skinSetting = componentPool.skinSettings[index];

                index = Random.Range((int)0, (int)componentPool.eyesSettings.Count);
                cubData.eyesSetting = componentPool.eyesSettings[index];

                index = Random.Range((int)0, (int)componentPool.glassesSettings.Count);
                cubData.glassesSetting = componentPool.glassesSettings[index];

                index = Random.Range((int)0, (int)componentPool.tireSettings.Count);
                cubData.tireSetting = componentPool.tireSettings[index];
                generatedCubDatas.Add(cubData);
            }
            GenTempData();
            GenMeta();
        }
        void GenMeta()
        {
            foreach (var data in generatedCubDatas)
            {
                ExportGeneratedDataToJson(data);
            }
        }
        void GenTempData()
        {
            if (PreferenceTab.Instance.isInit)
            {
                var preference = PreferenceTab.Instance.GeneratorPreference;
                var tmpPath = GeneratorDefaultPath.TempDataPath;
                if (Directory.Exists(tmpPath))
                {
                    var dir = new DirectoryInfo(@tmpPath);
                    dir.Delete(true);
                }
                if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);
                if (generatedCubDatas.Count > 0)
                {
                    foreach (var data in generatedCubDatas)
                    {
                        var dataPath = tmpPath + "/" + data.cubName + ".asset";
                        AssetDatabase.CreateAsset(data, dataPath);
                    }
                }
                AssetDatabase.Refresh();
            }
        }
        public override void OnEnable()
        {
            CheckTempData();
            base.OnEnable();
        }
        void CheckTempData()
        {
            generatedCubDatas.Clear();
            var tempDataPath = GeneratorDefaultPath.TempDataPath;
            if (!Directory.Exists(tempDataPath)) Directory.CreateDirectory(tempDataPath);
            var filesStrings = Directory.GetFiles(tempDataPath, "*.asset");
            List<GeneratedCubData> objects = new List<GeneratedCubData>();
            foreach (string fileString in filesStrings)
            {
                string assetPath = fileString.Replace(Application.dataPath, "").Replace('\\', '/');
                var loadedObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GeneratedCubData)) as GeneratedCubData;
                generatedCubDatas.Add(loadedObj);
            }
        }
        void SetUpBuildFolder()
        {
            //Check Folder Existed
            var buildPathString = ConfigTab.Instance.GetConfig().BuildFolderPath;
            buildPathString = Path.GetFullPath(buildPathString);
            if (!Directory.Exists(buildPathString)) Directory.CreateDirectory(buildPathString);
            var imgFolderPath = Path.Combine(buildPathString, "image");
            if (!Directory.Exists(imgFolderPath)) Directory.CreateDirectory(imgFolderPath);
            var metaFolderPath = Path.Combine(buildPathString, "meta");
            if (!Directory.Exists(metaFolderPath)) Directory.CreateDirectory(metaFolderPath);
        }
        public void ExportGeneratedDataToJson(GeneratedCubData data)
        {
            var buildPathString = ConfigTab.Instance.GetConfig().BuildFolderPath;
            buildPathString = Path.GetFullPath(buildPathString);
            var metaFolderPath = Path.Combine(buildPathString, "meta");
            if (data)
            {
                string json_text = "";
                json_text += "{\n";
                var fields = typeof(GeneratedCubData).GetFields();
                foreach (var field in fields)
                {
                    json_text += "\"" + field.Name + "\"" + ":";
                    switch (field.FieldType.Name)
                    {
                        case "String":
                            if (field.GetValue(data) != null)
                                json_text += "\"" + field.GetValue(data).ToString() + "\"," + "\n";
                            else json_text += "\"\"," + "\n";
                            break;
                        default:
                            if (field.FieldType.BaseType.Name.Equals("CubComponentSetting"))
                            {
                                var styleNameField = typeof(CubComponentSetting).GetField("styleName");
                                if (field.Name != "tireSetting") json_text += "\"" + styleNameField.GetValue(field.GetValue(data)).ToString() + "\"," + "\n";
                                else json_text += "\"" + styleNameField.GetValue(field.GetValue(data)).ToString() + "\"" + "\n";
                            }
                            else
                            {
                                Debug.Log(field.FieldType.BaseType + ":" + field.FieldType);
                                json_text += "\"\"," + "\n";
                            }
                            break;
                    }
                }

                json_text += "}";
                string json = JsonUtility.ToJson(data, true);

                string fileName = data.edition + ".json";
                string path = Path.Combine(metaFolderPath, fileName);
                File.WriteAllText(path, json_text);
            }
        }
    }
}