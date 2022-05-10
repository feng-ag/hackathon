using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace MapEditor
{
    public class Cursor : MonoBehaviour
    {
        [SerializeField]
        GameObject temp;

        [SerializeField]
        Transform root;


        float rotation = 0F;


        Vector3 _Position;
        public Vector3 Position
        {
            set
            {
                _Position = value;
                transform.position = _Position;
            }
            get => _Position;
        }


        List<GameObject> cursorGrids = new List<GameObject>();

        public void BuildCursor(ItemTypeData itemTypeData)
        {
            Vector3 placeOffsetV3 = new Vector3(itemTypeData.placeOffset.x, 0, itemTypeData.placeOffset.y);
            root.localPosition = placeOffsetV3;


            foreach(var g in cursorGrids.ToArray())
            {
                Destroy(g);
            }

            foreach(var pos in itemTypeData.grids)
            {
                GameObject g = Instantiate(temp, transform.localPosition + pos.GetVector3(), Quaternion.identity, root);
                cursorGrids.Add(g);
                g.SetActive(true);
            }
        }

        public void Rotate(float angle)
        {
            root.Rotate(0, angle, 0);

            rotation = angle;
        }


        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }


    }
}