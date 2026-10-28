using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace ZeroX.JsonSystem
{
    public static class ZJsonReflection
    {
        public const BindingFlags bindingFlags_ForGetAllField = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        public static readonly Type TYPE_OBJECT = typeof(object);
        public static readonly Type TYPE_UNITY_OBJECT = typeof(UnityEngine.Object);
        public static readonly Type TYPE_VALUE_TYPE = typeof(ValueType);

        private static readonly Type TYPE_SERIALIZABLE = typeof(SerializableAttribute);

        private static readonly Type TYPE_SERIALIZE_FIELD = typeof(SerializeField);
        private static readonly Type TYPE_NON_SERIALIZED = typeof(NonSerializedAttribute);

        //Generic Type
        public static readonly Type TYPE_DICTIONARY = typeof(Dictionary<,>);
        public static readonly Type TYPE_LIST = typeof(List<>);

        //Scalar Type
        public static readonly Type TYPE_STRING = typeof(string);
        public static readonly Type TYPE_CHAR = typeof(char);
        public static readonly Type TYPE_BOOL = typeof(bool);
        public static readonly Type TYPE_FLOAT = typeof(float);
        public static readonly Type TYPE_DOUBLE = typeof(double);
        public static readonly Type TYPE_NINT = typeof(nint);
        public static readonly Type TYPE_NUINT = typeof(nuint);

        //Built-in Type có field private, cần handler tay (xem ZJsonBuiltInHandler)
        public static readonly Type TYPE_VECTOR2_INT = typeof(Vector2Int);
        public static readonly Type TYPE_VECTOR3_INT = typeof(Vector3Int);
        public static readonly Type TYPE_RECT = typeof(Rect);
        public static readonly Type TYPE_RECT_INT = typeof(RectInt);
        public static readonly Type TYPE_BOUNDS = typeof(Bounds);
        public static readonly Type TYPE_BOUNDS_INT = typeof(BoundsInt);
        public static readonly Type TYPE_LAYER_MASK = typeof(LayerMask);
        public static readonly Type TYPE_HASH128 = typeof(Hash128);

        //Attribute
        public static readonly Type TYPE_ZJSON_UNITY_OBJECT_TO_JSON = typeof(ZJson_UnityObjectToJsonAttribute);



        private static readonly HashSet<Type> HashSetBuiltInType = new HashSet<Type>()
        {
            // Vector
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Vector2Int),
            typeof(Vector3Int),

            // Rotation
            typeof(Quaternion),
            typeof(Matrix4x4),

            // Color
            typeof(Color),
            typeof(Color32),

            // Math / Geometry
            typeof(Rect),
            typeof(RectInt),
            typeof(Bounds),
            typeof(BoundsInt),

            // Other structs
            typeof(LayerMask),
            typeof(AnimationCurve),
            typeof(Gradient),
            typeof(Hash128),
        };

        private static readonly HashSet<Type> HashSetScalarType = new HashSet<Type>()
        {
            // Boolean
            typeof(bool),

            // Integer
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),

            // Native integer
            typeof(nint),
            typeof(nuint),

            // Floating point
            typeof(float),
            typeof(double),
            typeof(decimal),

            // Character
            typeof(char),

            // Other common scalar-like value types
            typeof(string)
        };



        //Cache: dùng ConcurrentDictionary để đường đọc không cần lock.
        //Các value (array, dict con) đều bất biến sau khi build nên đọc không lock là an toàn.
        private static readonly ConcurrentDictionary<Type, bool> dictTypeSerializable = new ConcurrentDictionary<Type, bool>();
        private static readonly ConcurrentDictionary<Type, ZJsonFieldAccessor[]> dictTypeFieldAccessors = new ConcurrentDictionary<Type, ZJsonFieldAccessor[]>();
        private static readonly ConcurrentDictionary<Type, Dictionary<string, ZJsonFieldAccessor>> dictTypeFieldAccessorByName = new ConcurrentDictionary<Type, Dictionary<string, ZJsonFieldAccessor>>();
        private static readonly ConcurrentDictionary<Type, Func<object>> dictTypeConstructor = new ConcurrentDictionary<Type, Func<object>>();


        public static bool IsBuiltInObjectType(Type type)
        {
            return HashSetBuiltInType.Contains(type);
        }

        public static bool IsScalarType(Type type)
        {
            return HashSetScalarType.Contains(type);
        }


        public static bool CanTypeSerializable(Type type)
        {
            if (dictTypeSerializable.TryGetValue(type, out bool result))
                return result;

            return dictTypeSerializable.GetOrAdd(type, CalculateTypeSerializable);
        }

        private static bool CalculateTypeSerializable(Type type)
        {
            if (HashSetBuiltInType.Contains(type)) //Unity Built-In type trong danh sách mặc định là có thể serializable
                return true;

            if (type.IsArray)
                return true;

            if (type.IsGenericType)
            {
                var fieldTypeGeneric = type.GetGenericTypeDefinition();
                return fieldTypeGeneric == TYPE_LIST || fieldTypeGeneric == TYPE_DICTIONARY;
            }

            if (type.IsEnum)
                return true;


            if (type.IsSubclassOf(TYPE_UNITY_OBJECT))
            {
#if UNITY_EDITOR
                return true;
#else
                return Attribute.IsDefined(type, TYPE_ZJSON_UNITY_OBJECT_TO_JSON);
#endif
            }

            return Attribute.IsDefined(type, TYPE_SERIALIZABLE);
        }



        private static bool CalculateFieldSerializable(FieldInfo fieldInfo)
        {
            if (CanTypeSerializable(fieldInfo.FieldType) == false)
                return false;


            if (fieldInfo.IsPublic)
            {
                //Nếu public thì không được có attribute NonSerialized thì mới được serialize
                return Attribute.IsDefined(fieldInfo, TYPE_NON_SERIALIZED) == false;
            }
            else
            {
                //Nếu Non-Public thì phải có SerializeField thì mới được serialize
                return Attribute.IsDefined(fieldInfo, TYPE_SERIALIZE_FIELD);
            }
        }



        #region Field Accessor

        public static ZJsonFieldAccessor[] GetAllFieldAccessor(Type type)
        {
            if (dictTypeFieldAccessors.TryGetValue(type, out var accessors))
                return accessors;

            return dictTypeFieldAccessors.GetOrAdd(type, BuildFieldAccessors);
        }

        private static ZJsonFieldAccessor[] BuildFieldAccessors(Type type)
        {
            List<ZJsonFieldAccessor> listAccessor = new List<ZJsonFieldAccessor>();

            Type curType = type;
            while (curType != null && curType != TYPE_OBJECT && curType != TYPE_VALUE_TYPE)
            {
                var fieldInfos = curType.GetFields(bindingFlags_ForGetAllField);
                foreach (var fieldInfo in fieldInfos)
                {
                    if (CalculateFieldSerializable(fieldInfo) == false)
                        continue;

                    listAccessor.Add(new ZJsonFieldAccessor(fieldInfo));
                }

                curType = curType.BaseType;
            }

            return listAccessor.ToArray();
        }

        public static ZJsonFieldAccessor GetFieldAccessor(Type type, string fieldName)
        {
            if (dictTypeFieldAccessorByName.TryGetValue(type, out var dictByName) == false)
                dictByName = dictTypeFieldAccessorByName.GetOrAdd(type, BuildFieldAccessorByName);

            dictByName.TryGetValue(fieldName, out ZJsonFieldAccessor accessor);
            return accessor;
        }

        private static Dictionary<string, ZJsonFieldAccessor> BuildFieldAccessorByName(Type type)
        {
            var accessors = GetAllFieldAccessor(type);

            var dictByName = new Dictionary<string, ZJsonFieldAccessor>(accessors.Length);
            foreach (var accessor in accessors)
            {
                //Field trùng tên (class con shadow class cha): giữ field của class con (đứng trước trong mảng)
                if (dictByName.ContainsKey(accessor.name) == false)
                    dictByName.Add(accessor.name, accessor);
            }

            return dictByName;
        }

        /// <summary>
        /// Tìm accessor bằng cách so tên field trực tiếp trên chuỗi json (vùng key raw), không cắt chuỗi
        /// </summary>
        public static ZJsonFieldAccessor MatchFieldAccessor(Type type, string json, int keyStart, int keyLength)
        {
            var accessors = GetAllFieldAccessor(type);

            for (int i = 0; i < accessors.Length; i++)
            {
                var accessor = accessors[i];
                string name = accessor.name;

                if (name.Length != keyLength)
                    continue;

                bool match = true;
                for (int c = 0; c < keyLength; c++)
                {
                    if (json[keyStart + c] != name[c])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                    return accessor;
            }

            return null;
        }

        #endregion



        #region Create Instance

        /// <summary>
        /// Tạo instance qua constructor compile sẵn (nhanh hơn Activator.CreateInstance nhiều lần)
        /// </summary>
        public static object CreateInstance(Type type)
        {
            if (dictTypeConstructor.TryGetValue(type, out var constructor))
                return constructor();

            return dictTypeConstructor.GetOrAdd(type, BuildConstructor)();
        }

        private static Func<object> BuildConstructor(Type type)
        {
            return Expression.Lambda<Func<object>>(Expression.Convert(Expression.New(type), TYPE_OBJECT)).Compile();
        }

        #endregion



#if UNITY_EDITOR
        public static bool IsParseToGUID(Type objType)
        {
            return objType.IsSubclassOf(TYPE_UNITY_OBJECT) && Attribute.IsDefined(objType, TYPE_ZJSON_UNITY_OBJECT_TO_JSON) == false;
        }
#endif
    }
}
