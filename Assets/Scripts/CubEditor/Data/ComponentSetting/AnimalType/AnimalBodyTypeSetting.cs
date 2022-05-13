using System;
using UnityEngine;

namespace ArcadeGalaxyKit
{
    /// <summary>
    /// Store base glasses setting
    /// </summary>
    [CreateAssetMenu(
    menuName = "ArcadeGalaxyKit/CubEditor/ComponentSetting/AnimalType/BaseAnimalType",
    fileName = "BaseAnimalTypeSetting")]
    [Serializable]
    public class AnimalBodyTypeSetting : CubComponentSetting
    {
        public GameObject meshPrefab;
    }
}