using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



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

        public float Rotation
        {
            set
            {
                root.eulerAngles = new Vector3(0, value, 0);
            }
            get => root.eulerAngles.y;
        }


        List<GameObject> cursorGrids = new List<GameObject>();


        Material[] cursorGridMats;

        [SerializeField]
        Color normalColor;

        [SerializeField]
        Color triggerColor;



        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == MapEditorManager.itemLayerIndex)
            {
                foreach(var mat in cursorGridMats)
                {
                    mat.color = triggerColor;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == MapEditorManager.itemLayerIndex)
            {
                foreach (var mat in cursorGridMats)
                {
                    mat.color = normalColor;
                }
            }
        }


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

            cursorGridMats = root.GetComponentsInChildren<Renderer>()
                .Select(r => r.material)
                .ToArray();


            foreach (var mat in cursorGridMats)
            {
                mat.color = normalColor;
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