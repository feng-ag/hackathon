using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ArcadeGalaxyKit
{
    /// <summary>
    /// Change car's texture, mesh once current editing CarTemplate's data is changed
    /// </summary>
    public class OutFitChangingSysten : MonoBehaviour
    {
        [Header("車部件 Renderer")]
        public MeshRenderer carBodyMRD;
        public GameObject CubPosAnchor;
        [Header("車部件PlaceHolder")]
        public GameObject tirePlaceHolder;
        public GameObject glassesPlaceHolder;
        public GameObject animalTypePlaceHolder;
        [Header("縮放比例(測試用)")]//Umimplement
        public float scaleXYZ = 0.01067533f;
        public float posYOffset = 9.66f;
        public static OutFitChangingSysten instance { get { return _instance; } }
        private static OutFitChangingSysten _instance;
        private CarTemplate currentEditingCarTemplate;
        private CarTemplate lastEditingCarTemplate;

        private void Awake()
        {
            if (instance == null)
            {
                _instance = this;
            }
        }
        private void Start()
        {
            currentEditingCarTemplate = DataManager.instance.currentEditingCarTemplate;
            lastEditingCarTemplate = ScriptableObject.CreateInstance("CarTemplate") as CarTemplate;

            if (currentEditingCarTemplate)
            {
                OnChange();
            }
        }

        #region changing outfit API
        /// <summary>
        /// Reference by UI button & OutFitChangingSysten.Start()
        /// </summary>
        public void OnChange()
        {
            if (!lastEditingCarTemplate.Equals(currentEditingCarTemplate))
            {
                if (currentEditingCarTemplate.tireSetting != lastEditingCarTemplate.tireSetting)
                {
                    ChangeTireType(currentEditingCarTemplate);
                }
                if (lastEditingCarTemplate.eyesSetting != currentEditingCarTemplate.eyesSetting
                    || lastEditingCarTemplate.skinSetting != currentEditingCarTemplate.skinSetting)
                {
                    ChangeSkinType(currentEditingCarTemplate);
                    ChangeEyesType(currentEditingCarTemplate);
                }
                if (currentEditingCarTemplate.glassesSetting != lastEditingCarTemplate.glassesSetting)
                {
                    ChangeGlassesType(currentEditingCarTemplate);
                }
            }
        }
        void ChangeSkinType(CarTemplate carTemplate)
        {
            carBodyMRD.material.SetTexture("_MainTex", carTemplate.skinSetting.skinTex);
            lastEditingCarTemplate.skinSetting = carTemplate.skinSetting;
        }
        void ChangeEyesType(CarTemplate carTemplate)
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
                carBodyMRD.material.SetTexture("_MainTex", eyeAddedTexture);
            }
            lastEditingCarTemplate.eyesSetting = carTemplate.eyesSetting;
        }
        void ChangeGlassesType(CarTemplate carTemplate)
        {
            if (glassesPlaceHolder)
            {
                foreach (Transform obj in glassesPlaceHolder.transform)
                {
                    Destroy(obj.gameObject);
                }
                if (carTemplate.glassesSetting.meshPrefab)
                {
                    var glassedObj = Instantiate(carTemplate.glassesSetting.meshPrefab);
                    var localR = glassedObj.transform.localRotation;
                    var CubPosAnchorLocalRY = CubPosAnchor.transform.localRotation.eulerAngles.y;
                    glassedObj.transform.RotateAround(CubPosAnchor.transform.position, Vector3.up, CubPosAnchorLocalRY);
                    glassedObj.transform.SetParent(glassesPlaceHolder.transform);
                }
            }
            else
            {
                Debug.LogError("Can't find glassesPlaceHolder.");
            }

            lastEditingCarTemplate.glassesSetting = carTemplate.glassesSetting;
        }

        void ChangeTireType(CarTemplate carTemplate)
        {
            if (tirePlaceHolder)
            {
                foreach (Transform obj in tirePlaceHolder.transform)
                {
                    Destroy(obj.gameObject);
                }
                if (carTemplate.tireSetting.meshPrefab)
                {
                    var tireGameObj = Instantiate(carTemplate.tireSetting.meshPrefab);
                    var localR = tireGameObj.transform.localRotation;
                    var CubPosAnchorLocalRY = CubPosAnchor.transform.localRotation.eulerAngles.y;
                    tireGameObj.transform.RotateAround(CubPosAnchor.transform.position, Vector3.up, CubPosAnchorLocalRY);
                    tireGameObj.transform.SetParent(tirePlaceHolder.transform);
                }
            }
            else
            {
                Debug.LogError("Can't find tirePlaceHolder.");
            }

            lastEditingCarTemplate.tireSetting = carTemplate.tireSetting;
            //TODO check if tire setting has color attribute
            //var MRD=tireObj.GetComponentInChildren<MeshRenderer>();
            //MRD.sharedMaterial.color = carTemplate.CarTireColor1;
            //lastEditingCarTemplate.CarTireColor1 = carTemplate.CarTireColor1;
        }
        #endregion
    }
}