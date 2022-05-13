using System;
using UnityEngine;

namespace ArcadeGalaxyKit
{
    /// <summary>
    /// Store base glasses setting
    /// </summary>
    [CreateAssetMenu(
    menuName = "ArcadeGalaxyKit/CubEditor/ComponentSetting/Glasses/BaseGlasses",
    fileName = "BaseGlassesSetting")]
    [Serializable]
    public class GlassesSetting : CubComponentSetting
    {
        public GameObject meshPrefab;
    }
}