using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


namespace MapEditor
{
    public class Item : MonoBehaviour
    {
        [SerializeField]
        Transform root;

        [SerializeField]
        Transform colRoot;


        [SerializeField]
        public ItemData data;

        public ItemData Data => data;

        public ItemTypeData TypeData => Data.TypeData;


        public void Rotate(float angle)
        {
            bool isValid = ValidRotate(angle);

            if (isValid)
            {
                root.Rotate(0, angle, 0);
                colRoot.rotation = root.rotation;
            }

            // TODO
        }



        public bool ValidRotate(float angle)
        {
            Quaternion originRotation = root.localRotation;

            colRoot.Rotate(0, angle, 0);

            HashSet<Transform> cols = new HashSet<Transform>();

            foreach(Transform child in colRoot)
            {
                cols.Add(child);
            }


            foreach(Transform child in cols)
            {

                Vector3 pos = child.position;
                //Debug.Log(pos);

                RaycastHit[] hits = Physics.BoxCastAll(
                    pos,
                    Vector3.one * 0.45F,    //比0.5略小，避免誤碰到其他格
                    Vector3.forward,
                    Quaternion.identity,
                    0,
                    MapEditorManager.Instance.itemLayer.value);

                //GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //box.transform.position = pos;
                //box.transform.localScale = Vector3.one * 0.9F;

                if (hits.Length > 0 && !cols.Intersect(hits.Select(h => h.transform)).Any())
                {
                    //EditorGUIUtility.PingObject(hit.collider);
                    foreach (var hit in hits)
                    {
                        hit.collider.name += Random.Range(0, 999);
                        Debug.Log(hit.collider.name);
                    }

                    colRoot.localRotation = originRotation;
                    return false;
                }

            }

            colRoot.localRotation = originRotation;
            return true;
        }


        public void UnEmbed()
        {
            Data.UnEmbed();
        }

    }
}