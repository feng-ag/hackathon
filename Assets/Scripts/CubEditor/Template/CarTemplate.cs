using System;
using UnityEngine;

namespace ArcadeGalaxyKit
{
    [CreateAssetMenu(
    menuName = "ArcadeGalaxyKit/Templates/CarTemplate",
    fileName = "CarTemplate")]
    [Serializable]
    public class CarTemplate : ScriptableObject
    {
        public AnimalPreset animalPreset;
        public AnimalBodyTypeSetting animalBodyTypeSetting;
        public SkinSetting skinSetting;
        public EyesSetting eyesSetting;
        public GlassesSetting glassesSetting;
        public TireSetting tireSetting;
    }
}