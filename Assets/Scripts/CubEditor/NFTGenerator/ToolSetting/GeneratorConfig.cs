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
        public string BuildFolderPath;
        public string namePrefix;
        public string NFTDescription;
        public string BaseURL;
        public int GenerateNFTAmount;
        public CubComponentsPool cubComponentsPool;
    }
    public static class GeneratorDefaultPath
    {
        static public string DefaultDataFolder = Application.dataPath + "/GameData/NFTGenerator";
        static public string DefaultSystemFolder = Application.dataPath + "/GameData/NFTGenerator/System";
        public const string PreferencePath = "Assets/GameData/NFTGenerator/System/Preference.asset";
        public const string TempDataPath = "Assets/GameData/NFTGenerator/TempData/";
        public const string ToolScenePath = "Assets/Scenes/CarNFTGenrerateorScene.unity";
        public const string DefaultConfigPath = "Assets/GameData/NFTGenerator/System/NFT Generator Config.asset";
    }
}