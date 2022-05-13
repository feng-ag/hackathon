using System;
using UnityEngine;

namespace ArcadeGalaxyKit
{
    [CreateAssetMenu(
     menuName = "ArcadeGalaxyKit/CubEditor/ComponentSetting/ComponentSeries",
     fileName = "ComponentSeries")]
    [Serializable]
    public class ComponentSeries : ScriptableObject
    {
        public string seriesName;
    }
}