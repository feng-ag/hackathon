using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    public interface ITitleAndIconReadable
    {

        public string Title { get; }
        public Sprite Icon { get; }


    }
}