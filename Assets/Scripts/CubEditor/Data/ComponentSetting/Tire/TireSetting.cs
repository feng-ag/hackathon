using System;
using UnityEngine;

namespace ArcadeGalaxyKit
{
    /// <summary>
    /// Store base tire setting
    /// </summary>
    [CreateAssetMenu(
    menuName = "ArcadeGalaxyKit/Cub/ComponentSetting/Tire/BaseTire",
    fileName = "BaseTireSetting")]
    [Serializable]
    public class TireSetting : ComponentSetting
    {
        public GameObject meshPrefab;
    }
}