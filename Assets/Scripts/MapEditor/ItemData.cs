using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ItemData : ScriptableObject, IEnumerable, IEnumerable<ItemData.Item>
{
    [System.Serializable]
    public struct Item
    {
        public string name;
        public Sprite icon;
        public GameObject[] prefabs;

        public GameObject RandomPickPrefab()
        {
            return prefabs[Random.Range(0, prefabs.Length)];
        }
    }

    [SerializeField]
    List<Item> data = new List<Item>();


    public Item Current => throw new System.NotImplementedException();


    public Item GetItem(string name)
    {
        return data.SingleOrDefault(item => item.name == name);
    }

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
