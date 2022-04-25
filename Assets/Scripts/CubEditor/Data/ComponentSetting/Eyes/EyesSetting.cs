using System;
using UnityEngine;

namespace ArcadeGalaxyKit
{
    /// <summary>
    /// Store tire setting
    /// </summary>
    [CreateAssetMenu(
    menuName = "ArcadeGalaxyKit/Cub/ComponentSetting/Eyes/BaseEyes",
    fileName = "BaseEyesSetting")]
    [Serializable]
    public class EyesSetting : ComponentSetting
    {
        public Texture2D eyesTex;
    }
}