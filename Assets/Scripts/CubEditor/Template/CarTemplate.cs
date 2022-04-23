using System;
using UnityEngine;

namespace ArcadeGalaxyKit
{
    [CreateAssetMenu(
    menuName = "ArcadeGalaxyKit/Templates/CarTemplate",
    fileName = "CarTemplate")]
    [Serializable]
    public class CarTemplate : ScriptableObject
    {
        public CarSkinType carSkinType;
        public CarEyesType carEyesType;
        public CarTireType carTireType;
        public CarGlassesType carGlassesType;

    }
}