using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace MapEditor
{
    public class Item : MonoBehaviour
    {
        [SerializeField]
        Transform root;

        [SerializeField]
        Transform gridRoot;

        [SerializeField]
        Transform colRoot;

        [SerializeField]
        Transform mainGrid;

        [SerializeField]
        Transform cursorCenter;



        [SerializeField]
        public ItemData data;

        public ItemData Data => data;

        public ItemTypeData TypeData => Data.TypeData;

        public float Rotation => root.eulerAngles.y;

        public List<Vector3> GetGrids()
        {
            List<Vector3> grids = new List<Vector3>();
            Vector3 o = TypeData.GridOffsetV3;
            foreach (Transform col in gridRoot)
            {
                grids.Add(col.localPosition - o);
            }
            return grids;
        }

        public Vector2 GetRootGridPos()
        {
            return new Vector2(mainGrid.localPosition.x, mainGrid.localPosition.z);
        }

        public Vector2 GetCursorCenterPos()
        {
            return new Vector2(cursorCenter.localPosition.x, cursorCenter.localPosition.z);
        }

        public void SyncData()
        {
            transform.position = data.itemPos;

            Quaternion rot = Quaternion.Euler(0, data.itemRot, 0);
            root.rotation = rot;
            gridRoot.rotation = rot;
        }


        public void Rotate(float angle)
        {
            bool isValid = ValidRotate(angle);

            if (isValid)
            {
                root.Rotate(0, angle, 0);
                gridRoot.rotation = root.rotation;
                data.itemRot = root.eulerAngles.y;
            }
            else
            {
                Debug.LogError("Invaild rotate");
            }
        }
        public void SetRotation(float angle)
        {
            float deltaRot = angle - Rotation;

            if (deltaRot == 0F)
            {
                return;
            }

            bool isValid = ValidRotate(deltaRot);

            if (isValid)
            {
                root.Rotate(0, deltaRot, 0);
                gridRoot.rotation = root.rotation;
                data.itemRot = root.eulerAngles.y;
            }
            else
            {
                Debug.LogError("Invaild rotate");
            }
        }



        public bool ValidRotate(float angle)
        {
            Quaternion originRotation = root.localRotation;

            gridRoot.Rotate(0, angle, 0);

            HashSet<Transform> cols = new HashSet<Transform>();

            foreach(Transform child in gridRoot)
            {
                cols.Add(child);
            }


            foreach(Transform child in cols)
            {

                Vector3 pos = child.position;

                RaycastHit[] hits = Physics.BoxCastAll(
                    pos,
                    Vector3.one * 0.45F,    //比0.5略小，避免誤碰到其他格
                    Vector3.forward,
                    Quaternion.identity,
                    0,
                    MapEditorManager.Instance.itemLayer.value);


                if (hits.Length > 0 && !cols.Intersect(hits.Select(h => h.transform)).Any())
                {
                    foreach (var hit in hits)
                    {
                        hit.collider.name += Random.Range(0, 999);
                        Debug.Log(hit.collider.name);
                    }

                    gridRoot.localRotation = originRotation;
                    return false;
                }

            }

            gridRoot.localRotation = originRotation;
            return true;
        }


        public void UnEmbed()
        {
            ItemData.UnEmbed(Data);
        }


        public void HideColRoot()
        {
            gridRoot.gameObject.SetActive(false);
            colRoot.gameObject.SetActive(false);
        }


#if(UNITY_EDITOR)


        [ContextMenu("Create Col")]
        public void CreateCol()
        {
            GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/MapEditor/Items/@ColBase.prefab");

            GameObject ins = UnityEditor.PrefabUtility.InstantiatePrefab(prefab, gridRoot) as GameObject; ;

            ins.GetComponent<ItemColLinker>().item = this;

            UnityEditor.EditorUtility.SetDirty(gameObject);

            UnityEditor.EditorGUIUtility.PingObject(ins);

            Debug.Log("OK");


        }


#endif

    }
}