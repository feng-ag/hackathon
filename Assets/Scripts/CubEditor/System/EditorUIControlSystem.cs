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
        private CarTemplate currentEditingCarTemplate;
        [Header("需自動調整大小的UI")]
        public GridLayoutGroup attributeContentEdit;
        float lastRecordScreenWidth = 0;
        /// <summary>
        /// Reload UI infomation with selected car template
        /// </summary>
        void SetupSelectedCardTemplateUI(CarTemplate carTemplate)
        {
            var fields = typeof(CarTemplate).GetFields();
            int i = fields.Length - 1;
            for (; i >= 0; i--)
            {
                var field = fields[i];
                if (field.FieldType.IsEnum)
                {
                    var newRow = Instantiate(textDropdownRowPrefab);
                    newRow.transform.SetParent(AttributeEditUIContent.transform);
                    newRow.transform.SetAsFirstSibling();
                    newRow.SetActive(true);
                    newRow.GetComponentInChildren<Text>().text = ParseFieldString(field.Name);
                    int c = 0;
                    var optionStrings = System.Enum.GetValues(field.FieldType);
                    var options = newRow.GetComponentInChildren<Dropdown>();
                    for (; c < optionStrings.Length; c++)
                    {
                        Dropdown.OptionData option = new Dropdown.OptionData();
                        option.text = optionStrings.GetValue(c).ToString(); ;
                        options.options.Add(option);
                        options.value = (int)field.GetValue(carTemplate);
                    }
                    options.onValueChanged.AddListener((c) => { field.SetValue(carTemplate, c); });
                    options.onValueChanged.AddListener((c) => { OutFitChangingSysten.instance.OnChange();});
                }
            }

        }

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
            return result;
        }
        void Start()
        {
            currentEditingCarTemplate = DataManager.instance.CurrentEditingCarTemplate;
            if (currentEditingCarTemplate)
            {
                SetupSelectedCardTemplateUI(currentEditingCarTemplate);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
        string statusMessage = "";

        void OnGUI()
        {
            if (attributeContentEdit) {
                var rectTrans = attributeContentEdit.transform as RectTransform;
                if (lastRecordScreenWidth != Screen.width) {
                    Vector2 tmp = attributeContentEdit.cellSize;
                    tmp.x = rectTrans.rect.width-10f;
                    attributeContentEdit.cellSize = tmp;
                    lastRecordScreenWidth = Screen.width;
                }
            }

            float debugPanelWidth = 500;
            float debugPanelheight = 90;

            float messagePaddingLeft = 10;
            float messageHeight = 60;
            float messageWidth = 480;
            // Make a background box
            GUI.Box(new Rect(Screen.width - debugPanelWidth, Screen.height - debugPanelheight, debugPanelWidth, debugPanelheight), "Message Panel");
            GUI.TextArea(new Rect(Screen.width - debugPanelWidth + messagePaddingLeft, Screen.height - debugPanelheight + 20, messageWidth, messageHeight), statusMessage);
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