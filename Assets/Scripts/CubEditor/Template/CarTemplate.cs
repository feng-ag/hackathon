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
        public CarSkinType carSkinType;
        public CarEyesType carEyesType;
        public CarTireType carTireType;
        public Color CarTireColor1;
        public CarGlassesType carGlassesType;

        public AnimalTypeSetting animalTypeSetting;
        public SkinSetting skinSetting;
        public EyesSetting eyesSetting;
        public GlassesSetting glassesSetting;
        public TireSetting tireSetting;

    }
}