using System;
using UnityEngine;

namespace ArcadeGalaxyKit
{
    /// <summary>
    /// ComponentSetting Base
    /// </summary>
    [Serializable]
    public class ComponentSetting : ScriptableObject
    {
        public string styleName;
        public Sprite UIIcon;
    }
}