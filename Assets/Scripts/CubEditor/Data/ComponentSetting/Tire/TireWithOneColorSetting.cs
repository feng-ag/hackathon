using System;
using UnityEngine;

namespace ArcadeGalaxyKit
{
    /// <summary>
    /// Store tire with one color setting
    /// </summary>
    [CreateAssetMenu(
    menuName = "ArcadeGalaxyKit/Cub/ComponentSetting/Tire/TireWithOneColorSetting",
    fileName = "TireWithOneColorSetting")]
    [Serializable]
    public class TireWithOneColorSetting : TireSetting
    {
        public Color colorOne;
    }
}
