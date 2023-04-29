using System;
using UnityEngine;

namespace LudumDare53.Dialogue
{
    [Serializable]
    public class NameAssociation<T>
    {
        public string Name;
        public T Sprite;
    }
}
