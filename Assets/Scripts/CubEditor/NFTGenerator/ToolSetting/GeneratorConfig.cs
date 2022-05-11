using System;
using UnityEngine;


namespace ArcadeGalaxyKit
{
    [CreateAssetMenu(
    menuName = "ArcadeGalaxyKit/NFT Generator/Config",
    fileName = "NFT Generator Config")]
    [Serializable]
    public class GeneratorConfig : ScriptableObject
    {
        public const string PreferencePath = "Assets/Scripts/CubEditor/NFTGenerator/Preference.asset";
        public const string TempDataPath = "Assets/Scripts/CubEditor/NFTGenerator/TempData/";
        public const string ToolScenePath = "Assets/Scenes/CarNFTGenrerateorScene.unity";
        public string BuildFolderPath;
        public string namePrefix;
        public string NFTDescription;
        public string BaseURL;
        public int GenerateNFTAmount;
        public CubComponentsPool cubComponentsPool;
    }
}