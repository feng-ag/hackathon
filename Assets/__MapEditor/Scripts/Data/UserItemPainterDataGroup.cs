using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnionCollections.DataEditor;
using System.Linq;

namespace MapEditor
{

    public class UserItemPainterDataGroup : ScriptableObject, IEnumerable, IEnumerable<UserItemPainterData>
    {


        [NodeElement]
        [SerializeField]
        UserItemPainterData[] data = new UserItemPainterData[0];


        public UserItemPainterData GetUserItemPainterData(int index)
        {
            if (index < 0 || index > data.Length)
            {
                return null;
            }

            return data[index];
        }

        public IEnumerator GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator<UserItemPainterData> IEnumerable<UserItemPainterData>.GetEnumerator()
        {
            return ((IEnumerable<UserItemPainterData>)data).GetEnumerator();
        }


#if (UNITY_EDITOR)

        //[NodeAction]
        //void CreateFromTypeData()
        //{

        //    string itemTypeDataGroupGUID = UnityEditor.AssetDatabase.FindAssets("t:ItemTypeDataGroup")[0];
        //    string itemTypeDataGroupPath = UnityEditor.AssetDatabase.GUIDToAssetPath(itemTypeDataGroupGUID);
        //    ItemTypeDataGroup itemTypeDataGroup = UnityEditor.AssetDatabase.LoadAssetAtPath<ItemTypeDataGroup>(itemTypeDataGroupPath);

        //    string dataPath = $"Assets/Scriptable Objects/MapEditor/UserItemPainterData";


        //    var d = data.ToList();
        //    foreach (ItemTypeData itemTypeData in itemTypeDataGroup)
        //    {
        //        UserItemPainterData asset = CreateInstance<UserItemPainterData>();

        //        asset.typeDataList = new ItemTypeData[] { itemTypeData };
        //        asset.name = $"{itemTypeData.name}";


        //        UnityEditor.AssetDatabase.CreateAsset(asset, $"{dataPath}/UserItemPainter_{itemTypeData.name}.asset");

        //        d.Add(asset);
        //    }

        //    data = d.ToArray();

        //    UnityEditor.EditorUtility.SetDirty(this);

        //    UnityEditor.AssetDatabase.SaveAssets();


        //}


#endif
    }
}