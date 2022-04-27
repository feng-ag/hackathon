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
        public CarTemplate carTemplate;
        string artCarBodyPath = "Assets/Art/CarComponent/CarBody/MainBody";

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
            if (carTemplate)
            {
                GameObject cubLoaded = new GameObject();
                cubLoaded.name = "CubRoot";
                {
                    //Load Prefab
                    List<GameObject> toLoad = new List<GameObject>();
                    toLoad.Add(PrefabUtility.LoadPrefabContents(artCarBodyPath + "/" + "CarBody" + ".prefab"));
                    if (carTemplate.animalBodyTypeSetting) toLoad.Add(carTemplate.animalBodyTypeSetting.meshPrefab);
                    if (carTemplate.tireSetting.meshPrefab) toLoad.Add(carTemplate.tireSetting.meshPrefab);
                    if (carTemplate.glassesSetting.meshPrefab) toLoad.Add(carTemplate.glassesSetting.meshPrefab);

                    if (toLoad.Count > 0)
                    {
                        foreach (GameObject toGen in toLoad)
                        {
                            GameObject q = Instantiate(toGen) as GameObject;
                            q.transform.parent = cubLoaded.transform;
                        }
                    }
                }
                //Change Material
                GameObject bodyObj = cubLoaded.transform.GetChild(0).gameObject;
                var bodyMRD = bodyObj.GetComponent<MeshRenderer>();
                ChangeEyesType(carTemplate, bodyMRD);

                return cubLoaded;
            }
            else
            {
                LoadCub();
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
