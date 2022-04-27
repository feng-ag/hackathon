using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ArcadeGalaxyKit
{
    /// <summary>
    /// Store data
    /// </summary>
    public class DataManager : MonoBehaviour
    {
        [Header("≥]©w¿…")]
        public CarTemplate currentEditingCarTemplate;
        public CubStyleCenter cubStyleCenter;
        public static  DataManager instance { get { return _instamce; }}
        private static DataManager _instamce;
        
        private void Awake()
        {
            init();
        }
        void init()
        {
            if (instance == null)
            {
                _instamce = this;
            }
        }
        
    }
}