using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


namespace MapEditor
{

    [Serializable]
    public class ItemTypeData: ScriptableObject, IQueryableData
    {

        public int type;

        public string GetID() => type.ToString();


        //public Pos center;

        public List<Vector3> Grids => item.GetGrids();


        [SerializeField]
        public string name;


        public Item item;

        public Vector2 GridOffset => item.GetRootGridPos();
        public Vector3 GridOffsetV3 => new Vector3(GridOffset.x, 0, GridOffset.y);

        public Vector2 CursorOffset => item.GetCursorCenterPos();
        public Vector3 CursorOffsetV3 => new Vector3(CursorOffset.x, 0, CursorOffset.y);







        public override string ToString()
        {
            return $"{{ type: {type}, name:{name}, item:{item} }}";
        }


    }

}