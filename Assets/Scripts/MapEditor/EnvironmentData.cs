using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class EnvironmentData : ScriptableObject, IEnumerable, IEnumerable<EnvironmentData.Environment>
{
    [System.Serializable]
    public struct Environment: ITitleAndIconReadable
    {
        public string name;
        public Sprite icon;

        public GameObject[] prefabs;

        public string Title => name;

        public Sprite Icon => icon;


        public GameObject RandomPickPrefab()
        {
            return prefabs[Random.Range(0, prefabs.Length)];
        }

        public override string ToString()
        {
            return $"{{ name:{name}, prefabs:{string.Join(", ", prefabs.Select(p => p.name))} }}";
        }
    }

    [SerializeField]
    List<Environment> data = new List<Environment>();


    //public Environment GetEnvironment(string name)
    //{
    //    return data.SingleOrDefault(item => item.name == name);
    //}

    public Environment GetEnvironmentAt(int index)
    {
        return data[index];
    }

    public IEnumerator<Environment> GetEnumerator()
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
