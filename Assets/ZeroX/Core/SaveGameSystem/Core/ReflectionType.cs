using System;
using System.Collections.Generic;
using UnityEngine;


namespace ZeroX.SaveGameSystem
{
    public static class ReflectionType
    {
        //Special
        
        
        //Primitive Type
        public static readonly Type INT = typeof(int);
        public static readonly Type FLOAT = typeof(float);
        public static readonly Type LONG = typeof(long);
        public static readonly Type DOUBLE = typeof(double);
        public static readonly Type BOOL = typeof(bool);
        
        
        //Effectively Primitive Type
        public static readonly Type STRING = typeof(string);
        public static readonly Type DECIMAL = typeof(decimal);
        public static readonly Type BIG_INTEGER = typeof(System.Numerics.BigInteger);
        
        
        //Generic Type
        public static readonly Type DICTIONARY = typeof(Dictionary<,>);
        public static readonly Type LIST = typeof(List<>);
        
        
        //Bonus Type
        public static readonly Type DateTime = typeof(DateTime);
        public static readonly Type Color = typeof(Color);
        
        public static readonly Type VECTOR2 = typeof(Vector2);
        public static readonly Type VECTOR2_INT = typeof(Vector2Int);
        
        public static readonly Type VECTOR3 = typeof(Vector3);
        public static readonly Type VECTOR3_INT = typeof(Vector3Int);
        
        
        
    }
}