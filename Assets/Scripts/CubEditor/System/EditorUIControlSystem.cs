using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UI;
using UnityEngine;
namespace ArcadeGalaxyKit
{
    public class EditorUIControlSystem : MonoBehaviour
    {
        [Header("鏡頭")]
        public Camera carEdittingCamera;
        public Camera carPickUpCamera; //for snapshot
        [Header("需求 UI 組件")]
        public GameObject AttributeEditUIContent;
        public GameObject textDropdownRowPrefab;
        public GameObject buttonGroupRowPrefab;
        public GameObject buttonGroupUnitButtonPrefab;
        private CarTemplate currentEditingCarTemplate;
        private CubStyleCenter cubStyleCenter;
        [Header("每頁 UI 最大欄數")]
        public int maxRow = 1;

        /// <summary>
        /// Referenced by ui button
        /// </summary>
        public void GenerateSnapShot()
        {
            saveCapture();
        }

        /// <summary>
        /// Export cartemplate date to json file svae at Application.persistentDataPath
        /// </summary>
        public void ExportCarTemplateToJson()
        {
            if (currentEditingCarTemplate)
            {
                string json = JsonUtility.ToJson(currentEditingCarTemplate);
                string fileName = currentEditingCarTemplate.name + "_cartemplate.json";
                //string m_Path = Application.dataPath + "/GameData/CarTemplateJson/" + fileName;
                string k_Path = Application.persistentDataPath + "/" + fileName;

                System.IO.File.WriteAllText(k_Path, json);
                statusMessage = "Save json to " + k_Path;

            }
            else
            {
                statusMessage = "未拖入任何 car template 至 EditorUIControlSystem.";
            }

        }
        string ParseFieldString(string fieldText)
        {
            string result = "";
            int i = 0;
            foreach (char c in fieldText)
            {
                if (i == 0)
                {
                    result += char.ToUpper(c);
                }
                else if (char.IsUpper(c))
                {
                    result = result + " " + char.ToLower(c);
                }
                else
                {
                    result += c;
                }
                i++;
            }
            result = result.Replace(" settings", "");
            result = char.ToUpper(result[0]) + result.Substring(1);
            return result;
        }
        void Start()
        {
            cubStyleCenter = DataManager.instance.cubStyleCenter;
            currentEditingCarTemplate = DataManager.instance.currentEditingCarTemplate;
            if (currentEditingCarTemplate)
            {
                SetupCubStyleCenterSettingUI(cubStyleCenter, currentEditingCarTemplate);
            }
        }

        /// <summary>
        /// Load UI infomation with cubStyleCenter
        /// </summary>
        void SetupCubStyleCenterSettingUI(CubStyleCenter cubStyleCenter, CarTemplate carTemplate)
        {
            var fields = typeof(CubStyleCenter).GetFields();
            int i = fields.Length - 1;
            for (; i >= 0; i--)
            {
                var field = fields[i];
                if (field.Name == "tireSettings" || field.Name == "animalBodyTypeSettings") { continue; }
                {
                    //Instantiate Prefab
                    var newRow = Instantiate(buttonGroupRowPrefab);
                    newRow.transform.SetParent(AttributeEditUIContent.transform);
                    var viewPortRectTrans = AttributeEditUIContent.transform.parent.transform as RectTransform;
                    var rectTrans = newRow.transform as RectTransform;
                    newRow.transform.SetAsFirstSibling();
                    newRow.SetActive(true);
                    newRow.GetComponentInChildren<Text>().text = ParseFieldString(field.Name);

                    //Setting Value
                    object options = field.GetValue(cubStyleCenter);
                    object presetFieldValue = null;
                    var btnGroupRoot = newRow.transform.GetChild(1).transform;
                    if (carTemplate.animalPreset)
                    {
                        var PresetField = typeof(AnimalPreset).GetField(field.Name.Substring(0, field.Name.Length - 1));
                        if (PresetField != null)
                        {
                            presetFieldValue = PresetField.GetValue(carTemplate.animalPreset);
                            GenerateButtonByFieldObj(field, presetFieldValue, carTemplate, btnGroupRoot);
                        }
                    }
                    IEnumerable enumerable = options as IEnumerable;
                    if (enumerable != null)
                    {
                        
                        //Genereate UI setting for each field in enumerable
                        foreach (var obj in enumerable)
                        {
                            GenerateButtonByFieldObj(field, obj, carTemplate, btnGroupRoot);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generate UI button accroding to CubStyleCenter's Field
        /// </summary>
        void GenerateButtonByFieldObj(FieldInfo field, object obj, CarTemplate carTemplate, Transform btnGroupRoot)
        {
            var newBtnObj = Instantiate(buttonGroupUnitButtonPrefab);
            newBtnObj.transform.SetParent(btnGroupRoot);
            var newBtn = newBtnObj.GetComponent<Button>();
            object setValue = obj;
            if (setValue is CubComponentSetting)
            {
                var img = newBtnObj.GetComponent<Image>();
                var componentSetting = setValue as CubComponentSetting;
                if (componentSetting.UIIcon)
                {
                    img.sprite = componentSetting.UIIcon;
                }
            }
            if (field.Name == "animalPresets")
            {
                // Special Case:
                // Cause CarTemplate.cs didn't have this field. Replace CarTemplate's setting with setting under AnimalPreset
                newBtn.onClick.AddListener(() =>
                {
                    AnimalPreset animalPreset = setValue as AnimalPreset;
                    carTemplate.animalPreset = animalPreset;
                    carTemplate.animalBodyTypeSetting = animalPreset.animalBodyTypeSetting;
                    carTemplate.eyesSetting = animalPreset.eyesSetting;
                    carTemplate.skinSetting = animalPreset.skinSetting;
                    carTemplate.glassesSetting = animalPreset.glassesSetting;
                    carTemplate.tireSetting = animalPreset.tireSetting;
                });
            }
            else
            {
                newBtn.onClick.AddListener(() =>
                {
                    try
                    {
                        var getField = typeof(CarTemplate).GetField(field.Name.Substring(0, field.Name.Length - 1));
                        getField.SetValue(carTemplate, setValue);
                    }
                    catch
                    {
                        Debug.LogError("CarTemplate didn't have field : " + field.Name + " Check definition of both class."); ;
                    }
                }
                );
            }
            newBtn.onClick.AddListener(() => { OutFitChangingSysten.instance.OnChange(); });
        }

        string statusMessage = "";
        [Header("腳本Debug工具")]
        public bool isShowEditorUISystemMessagePanel = false;
        void OnGUI()
        {
            if (isShowEditorUISystemMessagePanel)
            {
                float debugPanelWidth = 500;
                float debugPanelheight = 90;

                float messagePaddingLeft = 10;
                float messageHeight = 60;
                float messageWidth = 480;
                // Make a background box
                GUI.Box(new Rect(Screen.width - debugPanelWidth, Screen.height - debugPanelheight, debugPanelWidth, debugPanelheight), "Message Panel");
                GUI.TextArea(new Rect(Screen.width - debugPanelWidth + messagePaddingLeft, Screen.height - debugPanelheight + 20, messageWidth, messageHeight), statusMessage);
            }
        }

        public enum CaptureSize
        {
            CameraSize,
            ScreenResolution,
            FixedSize
        }
        //Screenshot size
        [Header("截圖設定")]
        public CaptureSize captureSize = CaptureSize.CameraSize;

        ///< summary > Save screenshot < / summary >
        ///< param name = "camera" > target camera < / param >
        public void saveCapture()
        {
            Vector2 pixelSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            Vector2 size = pixelSize;
            if (captureSize == CaptureSize.CameraSize)
            {
                size = new Vector2(carPickUpCamera.pixelWidth, carPickUpCamera.pixelHeight);
            }
            else if (captureSize == CaptureSize.ScreenResolution)
            {
                size = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            }
            string fileName = "cameraCapture.png";
            string k_Path = Application.persistentDataPath + "/" + fileName;
            saveTexture(k_Path, capture(carPickUpCamera, (int)size.x, (int)size.y));
        }

        ///< summary > camera screenshot < / summary >
        ///< param name = "camera" > target camera < / param >
        ///< param name = "width" > width < / param >
        ///< param name = "height" > height < / param >
        public Texture2D capture(Camera camera, int width, int height)
        {
            RenderTexture rt = new RenderTexture(width, height, 0);
            rt.depth = 24;
            rt.antiAliasing = 8;
            camera.targetTexture = rt;
            camera.RenderDontRestore();
            RenderTexture.active = rt;
            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false, true);
            Rect rect = new Rect(0, 0, width, height);
            texture.ReadPixels(rect, 0, 0);
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            camera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
            return texture;
        }

        ///< summary > Save map < / summary >
        ///< param name = "path" > save path < / param >
        /// <param name="texture">Texture2D</param>
        public void saveTexture(string path, Texture2D texture)
        {
            System.IO.File.WriteAllBytes(path, texture.EncodeToPNG());
            statusMessage = "saved screenshot to:" + path;
        }
    }
}