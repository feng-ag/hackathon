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
        [Header("車部件GameObject")]
        public GameObject tiresGroup;//tire objs should arrange as enum CarTireType's order in scene
        public GameObject glassesGroup;
        public GameObject carBodyOther;
        [Header("Skin貼圖")]
        public Texture2D[] skins;//skin texture should arrange as enum CarSkinType's order in scene
        public Texture2D[] eyeTextures;//eye skin texture should arrange as enum CarEyesType's order in scene
        public Texture2D eyeMask;
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
                ChangeTireType(currentEditingCarTemplate);
                if (lastEditingCarTemplate.carEyesType != currentEditingCarTemplate.carEyesType
                    || lastEditingCarTemplate.carSkinType != currentEditingCarTemplate.carSkinType)
                {
                    ChangeSkinType(currentEditingCarTemplate);
                    ChangeEyesType(currentEditingCarTemplate);
                }
                ChangeGlassesType(currentEditingCarTemplate);
            }
        }
        void ChangeSkinType(CarTemplate carTemplate)
        {
            carBodyMRD.material.SetTexture("_MainTex", skins[(int)carTemplate.carSkinType]);
            lastEditingCarTemplate.carSkinType = carTemplate.carSkinType;
        }
        void ChangeEyesType(CarTemplate carTemplate)
        {
            if (eyeMask)
            {
                Texture2D oriTexture = skins[(int)carTemplate.carSkinType];
                Texture2D eyeTexture = eyeTextures[(int)carTemplate.carEyesType];
                Texture2D eyeAddedTexture = new Texture2D(oriTexture.width, oriTexture.height);
                eyeAddedTexture.SetPixels(oriTexture.GetPixels());

                for (int r = 0; r < oriTexture.height; r++)
                {
                    for (int c = 0; c < oriTexture.width; c++)
                    {
                        Color color = oriTexture.GetPixel(r, c, 0);
                        if (eyeMask.GetPixel(r, c, 0) != Color.black)
                        {
                            color = eyeTexture.GetPixel(r, c, 0);
                            eyeAddedTexture.SetPixel(r, c, color);
                        }

                    }
                }

                eyeAddedTexture.Apply();
                carBodyMRD.material.SetTexture("_MainTex", eyeAddedTexture);
            }
            lastEditingCarTemplate.carEyesType = carTemplate.carEyesType;
        }
        void ChangeGlassesType(CarTemplate carTemplate)
        {

            foreach (Transform tire in glassesGroup.transform)
            {
                tire.gameObject.SetActive(false);
            }
            if (carTemplate.carGlassesType != CarGlassesType.None)
            {
                glassesGroup.transform.GetChild((int)carTemplate.carGlassesType - 1).gameObject.SetActive(true);
            }
            lastEditingCarTemplate.carGlassesType = carTemplate.carGlassesType;
        }

        void ChangeTireType(CarTemplate carTemplate)
        {
            foreach (Transform tire in tiresGroup.transform)
            {
                tire.gameObject.SetActive(false);
            }
            var tireObj = tiresGroup.transform.GetChild((int)carTemplate.carTireType).gameObject;
            tireObj.SetActive(true);
            //var MRD=tireObj.GetComponentInChildren<MeshRenderer>();
            //MRD.sharedMaterial.color = carTemplate.CarTireColor1;
            //lastEditingCarTemplate.CarTireColor1 = carTemplate.CarTireColor1;
            lastEditingCarTemplate.carTireType = carTemplate.carTireType;
        }
        #endregion
    }
}