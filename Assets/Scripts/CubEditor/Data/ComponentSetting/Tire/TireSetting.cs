using System;
using UnityEngine;

namespace ArcadeGalaxyKit
{
    /// <summary>
    /// Store base tire setting
    /// </summary>
    [CreateAssetMenu(
    menuName = "ArcadeGalaxyKit/CubEditor/ComponentSetting/Tire/BaseTire",
    fileName = "BaseTireSetting")]
    [Serializable]
    public class TireSetting : CubComponentSetting
    {
        public GameObject meshPrefab;
    }
}