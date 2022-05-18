using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace MapEditor
{
    public class MapData
    {

        public string title = "Untitle Map";


        public int env = 0;


        [JsonProperty("items")]
        public List<ItemData> items = new List<ItemData>();






    }
}