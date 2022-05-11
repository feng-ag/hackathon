using System.Collections.Generic;
using System;
using UnityEngine;
namespace ArcadeGalaxyKit
{
    [Serializable]
    public class CubComponentsPool
    {
        public int PoolGenerateNFTAmount;
        public List<AnimalBodyTypeSetting> animalBodyTypeSettings;
        public List<SkinSetting> skinSettings;
        public List<EyesSetting> eyesSettings;
        public List<GlassesSetting> glassesSettings;
        public List<TireSetting> tireSettings;
    }
}