using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[System.Obsolete]
public class ItemTypeData_v1 : ScriptableObject, IEnumerable, IEnumerable<ItemTypeData_v1.Item>
{
    [System.Serializable]
    public struct Item : ITitleAndIconReadable
    {
        public string name;
        public Sprite icon;
        public bool isUnique;
        public GameObject[] prefabs;

        public string Title => name;

        public Sprite Icon => icon;

        public GameObject RandomPickPrefab()
        {
            return prefabs[Random.Range(0, prefabs.Length)];
        }


        public override string ToString()
        {
            return $"{{ name:{name}, isUnique:{isUnique}, prefabs:{string.Join(", ", prefabs.Select(p => p.name))} }}";
        }
    }

    [SerializeField]
    List<Item> data = new List<Item>();


    public Item GetItemAt(int index)
    {
        return data[index];
    }

    public IEnumerator<Item> GetEnumerator()
    {
        foreach (var item in data)
            yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        foreach (var item in data)
            yield return item;
    }
}
