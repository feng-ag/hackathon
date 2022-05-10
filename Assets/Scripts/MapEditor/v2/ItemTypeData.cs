using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


namespace MapEditor
{

    [Serializable]
    public class ItemTypeData: ScriptableObject, ITitleAndIconReadable
    {

        public int type;


        //public Pos center;

        public Pos[] grids = new Pos[0];


        public string name;
        public Sprite icon;

        public GameObject prefab;

        [Obsolete("可能直接預設以Item座標中心旋轉，不再偏移")]
        public Vector2 placeOffset;

        public Vector2 cursorOffset;





        public string Title => name;

        public Sprite Icon => icon;



        public override string ToString()
        {
            return $"{{ type: {type}, name:{name}, prefabs:{ prefab} }}";
        }


    }

}