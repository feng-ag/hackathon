using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeGalaxyKit
{
    [CreateAssetMenu(menuName = "ArcadeGalaxyKit/CubEditor/ComponentSetting/Preset/AnimalPreset",
    fileName = "AnimalPresetSetting")]
    public class AnimalPreset : CubComponentSetting
    {
        public AnimalBodyTypeSetting animalBodyTypeSetting;
        public SkinSetting skinSetting;
        public EyesSetting eyesSetting;
        public GlassesSetting glassesSetting;
        public TireSetting tireSetting;
    }
}
