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

        public List<Vector3> Grids => item.GetGrids();


        public string name;
        public Sprite icon;

        public Item item;

        public Vector2 placeOffset => item.GetRootGridPos();
        public Vector3 placeOffsetV3 => new Vector3(placeOffset.x, 0, placeOffset.y);

        public Vector2 cursorOffset => item.GetCursorCenterPos();
        public Vector3 cursorOffsetV3 => new Vector3(cursorOffset.x, 0, cursorOffset.y);





        public string Title => name;

        public Sprite Icon => icon;



        public override string ToString()
        {
            return $"{{ type: {type}, name:{name}, item:{item} }}";
        }


    }

}