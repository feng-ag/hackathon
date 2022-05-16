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

        ItemTypeData itemTypeData;


        Vector3 _Position;
        public Vector3 Position
        {
            set
            {
                _Position = value - itemTypeData.GridOffsetV3;
                transform.position = _Position;
            }
            get => _Position;
        }



        float _Rotation = 0F;
        public float Rotation
        {
            set
            {
                root.eulerAngles = new Vector3(0, value, 0);
                _Rotation = value;
            }
            get => _Rotation;
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


        public void BuildCursor(ItemTypeData _itemTypeData)
        {
            itemTypeData = _itemTypeData;
            Rotation = 0F;

            foreach (var g in cursorGrids.ToArray())
            {
                Destroy(g);
            }

            foreach (var grid in itemTypeData.Grids)
            {
                Vector3 gridPos = root.transform.position + grid;

                GameObject g = Instantiate(temp, gridPos, Quaternion.identity, root);
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

        public void SetCursor(ItemData _itemData)
        {
            Position = _itemData.item.transform.position + _itemData.TypeData.GridOffsetV3;
            Rotation = _itemData.itemRot;
        }

        public void Rotate(float angle)
        {
            Rotation += angle;
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