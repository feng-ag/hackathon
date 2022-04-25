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

        public AnimalTypeSetting animalTypeSettings;
        public SkinSetting skinSettings;
        public EyesSetting eyesSettings;
        public GlassesSetting glassesSettings;
        public TireSetting tireSettings;

    }
}