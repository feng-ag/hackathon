using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using OnionCollections.DataEditor;


namespace MapEditor {

    public class ItemTypeDataGroup : ScriptableObject, IEnumerable, IEnumerable<ItemTypeData>
    {
        [NodeElement]
        [SerializeField]
        ItemTypeData[] data = new ItemTypeData[0];


        public ItemTypeData GetTypeData(int type)
        {
            if (type < 0 || type > data.Length)
            {
                return null;
            }

            return data[type];
        }

        public IEnumerator GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator<ItemTypeData> IEnumerable<ItemTypeData>.GetEnumerator()
        {
            return ((IEnumerable<ItemTypeData>)data).GetEnumerator();
        }


#if (UNITY_EDITOR)

        [NodeAction]
        void CreateSelectionsAsNewTypeData()
        {

            Item[] items = UnityEditor.Selection.objects
                .Where(o => o is GameObject)
                .Where(o => (o as GameObject).GetComponent<Item>() != null)
                .Select(o => (o as GameObject).GetComponent<Item>())
                .ToArray();


            int maxType = data.Select(d => d.type).Max();

            string dataPath = $"Assets/Scriptable Objects/MapEditor/ItemTypeData";


            var d = data.ToList();
            foreach (Item item in items) {

                ItemTypeData asset = CreateInstance<ItemTypeData>();

                maxType++;

                asset.type = maxType;
                asset.item = item;
                asset.name = $"{item.name}";

                UnityEditor.AssetDatabase.CreateAsset(asset, $"{dataPath}/ItemType_{item.name}.asset");

                d.Add(asset);
            }

            data = d.ToArray();

            UnityEditor.EditorUtility.SetDirty(this);

            UnityEditor.AssetDatabase.SaveAssets();


        }


#endif


    }
}