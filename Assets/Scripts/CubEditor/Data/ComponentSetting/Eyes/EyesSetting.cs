using System;
using UnityEngine;

namespace ArcadeGalaxyKit
{
    /// <summary>
    /// Store tire setting
    /// </summary>
    [CreateAssetMenu(
    menuName = "ArcadeGalaxyKit/CubEditor/ComponentSetting/Eyes/BaseEyes",
    fileName = "BaseEyesSetting")]
    [Serializable]
    public class EyesSetting : CubComponentSetting
    {
        public Texture2D eyesTex;
        public Texture2D eyesMask;
    }
}