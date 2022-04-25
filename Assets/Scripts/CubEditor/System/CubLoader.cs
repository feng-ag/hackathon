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
        public Texture2D[] skins;
        public Texture2D[] eyes;
        public Texture2D eyeMask;
        string artCarBodyPath = "Assets/Art/CarComponent/CarBody";
        string artCarTirePath = "Assets/Art/CarComponent/Tire";
        string artCarGlassesPath = "Assets/Art/CarComponent/Glasses";

        void Awake()
        {
            if (!instance) {
                _instance = this;
            }
            if (AutoLoadCub) LoadCub(carTemplate);
        }

        /// <summary>
        /// Return gameobj of loaded cubtemplate setting and generate cub in scene position (0,0,0)
        /// </summary>
        public GameObject LoadCub(CarTemplate carTemplate)
        {
            if (carTemplate)
            {
                GameObject cubLoaded = new GameObject();
                cubLoaded.name = "CubRoot";
                {  
                    //Load Prefab
                    List<GameObject> toLoad = new List<GameObject>();
                    toLoad.Add(
                        PrefabUtility.LoadPrefabContents(artCarBodyPath + "/" + "CarBody" + ".prefab")
                    );
                    toLoad.Add(
                        PrefabUtility.LoadPrefabContents(
                            artCarBodyPath + "/" + "CarBodyOther_A" + ".prefab"
                        )
                    );
                    string optionChar = GetEnumLastWord(
                        typeof(CarTireType),
                        (int)carTemplate.carTireType
                    );
                    toLoad.Add(
                        PrefabUtility.LoadPrefabContents(
                            artCarTirePath
                                + "/"
                                + "Type"
                                + optionChar
                                + "/Tire_"
                                + optionChar
                                + ".prefab"
                        )
                    );
                    if (carTemplate.carGlassesType != CarGlassesType.None)
                    {
                        optionChar = GetEnumLastWord(
                            typeof(CarGlassesType),
                            (int)carTemplate.carGlassesType
                        );
                        toLoad.Add(
                            PrefabUtility.LoadPrefabContents(
                                artCarGlassesPath
                                    + "/"
                                    + "Type"
                                    + optionChar
                                    + "/glasses_"
                                    + optionChar
                                    + ".prefab"
                            )
                        );
                    }
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
            return null;
        }

        void ChangeEyesType(CarTemplate carTemplate, MeshRenderer meshRenderer)
        {
            if (eyeMask)
            {
                Texture2D oriTexture = skins[(int)carTemplate.carSkinType];
                Texture2D eyeTexture = eyes[(int)carTemplate.carEyesType];
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
                meshRenderer.material.SetTexture("_MainTex", eyeAddedTexture);
            }
        }

        /// <summary>
        /// Return last char in index's enum option
        /// </summary>
        string GetEnumLastWord(System.Type typeEnum, int index)
        {
            string lastWord = "";
            var optionStrings = System.Enum.GetValues(typeEnum);
            string optionChar = optionStrings.GetValue(index).ToString();
            lastWord = optionChar.Substring(optionChar.Length - 1, 1);
            return lastWord;
        }
    }
}
