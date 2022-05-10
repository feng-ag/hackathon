using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace MapEditor
{
 
    [Serializable]
    public struct Pos
    {
        public int x;
        public int y;

        public Pos(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public Vector2 GetVector2()
        {
            return new Vector2(x, y);
        }
        public Vector3 GetVector3(float vy = 0)
        {
            return new Vector3(x, vy, y);
        }

        public static Pos operator +(Pos a) => a;
        public static Pos operator -(Pos a) => new Pos
        {
            x = -a.x,
            y = -a.y,
        };


        public static Pos operator +(Pos a, Pos b) => new Pos
        {
            x = a.x + b.x,
            y = a.y + b.y,
        };

        public static Pos operator -(Pos a, Pos b) => a + (-b);

    }
}