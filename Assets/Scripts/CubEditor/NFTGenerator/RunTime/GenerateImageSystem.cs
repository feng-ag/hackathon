using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ArcadeGalaxyKit
{
    public class GenerateImageSystem : MonoBehaviour
    {
        public RunTimeGeneratorDataContainer RunTimeGeneratorDataContainer;
        public OutFitChangingSysten outFitChangingSysten;
        List<GeneratedCubData> datas = new List<GeneratedCubData>();
        GeneratorConfig config;
        bool isProcessing = false;
        void Start()
        {
            outFitChangingSysten = OutFitChangingSysten.instance;
            var testAdd = GameObject.Find("testAdd");
            RunTimeGeneratorDataContainer = testAdd.GetComponent<RunTimeGeneratorDataContainer>();
            if (RunTimeGeneratorDataContainer)
            {
                datas = RunTimeGeneratorDataContainer.generatedCubDatas;
                config = RunTimeGeneratorDataContainer.config;
            }
        }

        int dataIndex = -1;
        private void Update()
        {
            if (dataIndex >= datas.Count) return;
            if (isProcessing)
            {
                Process(datas[dataIndex]);
            }
            else
            {
                dataIndex++;
                isProcessing = true;
            }
        }


        void Process(GeneratedCubData data)
        {
            CarTemplate template = ScriptableObject.CreateInstance("CarTemplate") as CarTemplate;
            template.animalBodyTypeSetting = data.animalBodyTypeSetting;
            template.eyesSetting = data.eyesSetting;
            template.glassesSetting = data.glassesSetting;
            template.tireSetting = data.tireSetting;
            template.skinSetting = data.skinSetting;
            outFitChangingSysten.ChangeCarTemplate(template);

            //Gen
            string fileName = data.edition + ".png";
            var pathName = Path.Combine(config.BuildFolderPath, "image", fileName);
            ScreenCapture.CaptureScreenshot(pathName);

            isProcessing = false;
        }
    }
}