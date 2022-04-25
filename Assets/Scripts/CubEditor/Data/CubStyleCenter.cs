using System;
using UnityEngine;
namespace ArcadeGalaxyKit
{
    /// <summary>
    /// Store all component setting
    /// </summary>
    [CreateAssetMenu(
    menuName = "ArcadeGalaxyKit/Profile/CubStyleCenter",
    fileName = "CubStyleCenter")]
    [Serializable]
    public class CubStyleCenter : ScriptableObject
    {
        public AnimalTypeSetting[] animalTypeSettings;
        public SkinSetting[] skinSettings;
        public EyesSetting[] eyesSettings;
        public GlassesSetting[] glassesSettings;
        public TireSetting[] tireSettings;
    }
}

