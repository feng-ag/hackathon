using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MapEditor
{
    public class Item : MonoBehaviour
    {
        [SerializeField]
        Transform root;

        public ItemData Data { get; set; }

        public ItemTypeData TypeData => Data.TypeData;


        public void Rotate(float angle)
        {
            root.Rotate(0, angle, 0);

            // TODO
        }

        public void UnEmbed()
        {
            Data.UnEmbed();
        }

    }
}