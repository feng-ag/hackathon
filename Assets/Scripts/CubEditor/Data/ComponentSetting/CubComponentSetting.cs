using System;
using UnityEngine;

namespace ArcadeGalaxyKit
{
    /// <summary>
    /// ComponentSetting Base
    /// </summary>
    [Serializable]
    public class CubComponentSetting : ScriptableObject
    {
        public string styleName;
        public Sprite UIIcon;
        public int rate;
    }
}