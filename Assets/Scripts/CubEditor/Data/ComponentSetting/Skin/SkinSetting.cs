using System;
using UnityEngine;

namespace ArcadeGalaxyKit
{
    /// <summary>
    /// Store base skin setting
    /// </summary>
    [CreateAssetMenu(
    menuName = "ArcadeGalaxyKit/Cub/ComponentSetting/Skin/BaseSkin",
    fileName = "BaseSkinSetting")]
    [Serializable]
    public class SkinSetting : CubComponentSetting
    {
        public Texture2D skinTex;
    }
}