using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ArcadeGalaxyKit
{
    public class CubLoader : MonoBehaviour
    {
        public static CubLoader instance { get { return _instance; } }
        private static CubLoader _instance;
        public bool AutoLoadCub = true;
        CarTemplate carTemplate;
        public GameObject kartContainer;
        string artCarBodyPath = "Assets/Art/CarComponent/CarBody/MainBody";
        bool isCanInitWheel = false;
        GameObject container = null;
        void Awake()
        {
            if (!instance)
            {
                _instance = this;
            }
            if (AutoLoadCub) LoadCub();
        }
        private void Update()
        {
            //if (isCanInitWheel) {
            //    isCanInitWheel = false;
            //    //Setup Tire Colider
            //    var kart = container.GetComponent<KartGame.KartSystems.ArcadeKart>();
            //    WheelCollider[] wheels = container.GetComponentsInChildren<WheelCollider>();
            //    kart.FrontLeftWheel = wheels[0];
            //    kart.FrontRightWheel = wheels[1];
            //    kart.RearRightWheel = wheels[2];
            //    kart.RearLeftWheel = wheels[3];
            //    kart.CenterOfMass = container.transform.Find("TireType/TireA_InGame(Clone)/WheelColliders").transform;
            //    container.GetComponent<KartGame.KartSystems.KartAnimation>().WheelReset();
                

            //    container.SetActive(true);
            //}
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
                    container = Instantiate(kartContainer) ;
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
                    toLoad.Add(PrefabUtility.LoadPrefabContents(artCarBodyPath + "/" + "CarBody" + ".prefab"));
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
                GameObject bodyObj = modelRoot.transform.GetChild(0).gameObject;
                var bodyMRD = bodyObj.GetComponent<MeshRenderer>();
                ChangeEyesType(carTemplate, bodyMRD);

                ////Change Tire
                //var tireRoot = container.transform.Find("TireType");
                //foreach (Transform child in tireRoot)
                //{
                //    Destroy(child.gameObject);
                //}
                //if (carTemplate.tireSetting.meshPrefab)
                //{
                //    GameObject q = Instantiate(carTemplate.tireSetting.meshPrefab) as GameObject;
                //    q.transform.parent = tireRoot.transform;
                //}
                //isCanInitWheel = true;


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
    }
}
