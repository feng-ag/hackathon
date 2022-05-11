using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeGalaxyKit
{
    public class CubLoader : MonoBehaviour
    {

        public Camera carPickUpCamera; //for snapshot
        public static CubLoader instance { get { return _instance; } }
        private static CubLoader _instance;
        public bool AutoLoadCub = true;
        CarTemplate carTemplate;
        public GameObject kartContainer;
        public GameObject kartBody;
        GameObject container = null;
        [Header("Debug視窗")]
        public bool isOpenMessagePanel = false;
        float debugPanelWidth = 500;
        float debugPanelheight = 90;
        float messagePaddingLeft = 10;
        float messageHeight = 60;
        float messageWidth = 480;
        string statusMessage = "";

        void OnGUI()
        {
            if (isOpenMessagePanel)
            {
                string test_path = "Assets/Scripts/CubEditor/NFTGenerator/test.png";
                Rect button = new Rect(0, 0, 50, 50);
                if (GUI.Button(button, "snapshot"))
                {
                    ScreenCapture.CaptureScreenshot(test_path);
                }
                // Make a background box
                GUI.Box(new Rect(Screen.width - debugPanelWidth, Screen.height - debugPanelheight, debugPanelWidth, debugPanelheight), "Message Panel");
                GUI.TextArea(new Rect(Screen.width - debugPanelWidth + messagePaddingLeft, Screen.height - debugPanelheight + 20, messageWidth, messageHeight), statusMessage);
            }
        }

        void Awake()
        {
            if (!instance)
            {
                _instance = this;
            }
            if (AutoLoadCub) LoadCub();
        }

        /// <summary>
        /// Return gameobj of loaded cubtemplate setting and generate cub in scene position (0,0,0)
        /// </summary>
        public GameObject LoadCub()
        {
            carTemplate = Resources.Load<CarTemplate>("GameData/CarTemplate");
            if (carTemplate)
            {
            }
            else
            {
                Debug.LogError("carTemplate load failed in CubLoader");
            }
            if (carTemplate)
            {
                if (kartContainer)
                {
                    container = Instantiate(kartContainer);
                    container.name = carTemplate.name + "_Kart";
                }
                else { return null; }
                //Load Component without tire
                var modelRoot = container.transform.Find("KartVisual/KartModel");
                foreach (Transform child in modelRoot)
                {
                    Destroy(child.gameObject);
                }
                {
                    //Load Prefab
                    List<GameObject> toLoad = new List<GameObject>();
                    toLoad.Add(kartBody);
                    if (carTemplate.animalBodyTypeSetting) toLoad.Add(carTemplate.animalBodyTypeSetting.meshPrefab);
                    if (carTemplate.glassesSetting.meshPrefab) toLoad.Add(carTemplate.glassesSetting.meshPrefab);
                    if (toLoad.Count > 0)
                    {
                        foreach (GameObject toGen in toLoad)
                        {
                            GameObject q = Instantiate(toGen) as GameObject;
                            q.transform.parent = modelRoot.transform;
                        }
                    }
                }
                //Change Material
                GameObject bodyObj = modelRoot.transform.Find("CarBody(Clone)").gameObject;
                var bodyMRD = bodyObj.GetComponent<MeshRenderer>();
                ChangeEyesType(carTemplate, bodyMRD);

                return container;
            }
            return null;
        }
        void ChangeEyesType(CarTemplate carTemplate, MeshRenderer meshRenderer)
        {
            if (carTemplate.eyesSetting.eyesMask)
            {
                Texture2D oriTexture = carTemplate.skinSetting.skinTex;
                Texture2D eyeTexture = carTemplate.eyesSetting.eyesTex;
                Texture2D eyeAddedTexture = new Texture2D(oriTexture.width, oriTexture.height);
                eyeAddedTexture.SetPixels(oriTexture.GetPixels());

                for (int r = 0; r < oriTexture.height; r++)
                {
                    for (int c = 0; c < oriTexture.width; c++)
                    {
                        Color color = oriTexture.GetPixel(r, c, 0);
                        if (carTemplate.eyesSetting.eyesMask.GetPixel(r, c, 0) != Color.black)
                        {
                            color = eyeTexture.GetPixel(r, c, 0);
                            eyeAddedTexture.SetPixel(r, c, color);
                        }

                    }
                }
                eyeAddedTexture.Apply();
                meshRenderer.material.SetTexture("_MainTex", eyeAddedTexture);
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
        }

    }
}
