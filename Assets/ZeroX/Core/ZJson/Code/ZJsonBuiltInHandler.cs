using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace ZeroX.JsonSystem
{
    /// <summary>
    /// Handler tay cho các built-in type mà JsonUtility không serialize được khi đứng root
    /// (field private không có SerializeField nên JsonUtility.ToJson trả về "{}")
    /// <para>Đường string dùng AppendToJson/FromReader (đọc ghi trực tiếp, không qua node trung gian)</para>
    /// <para>Đường node dùng ToNode/FromNode</para>
    /// </summary>
    public static class ZJsonBuiltInHandler
    {
        private static readonly HashSet<Type> HashSetHandledType = new HashSet<Type>()
        {
            typeof(Vector2Int),
            typeof(Vector3Int),
            typeof(Rect),
            typeof(RectInt),
            typeof(Bounds),
            typeof(BoundsInt),
            typeof(LayerMask),
            typeof(Hash128),
        };

        public static bool IsHandledType(Type type)
        {
            return HashSetHandledType.Contains(type);
        }



        #region To Json String

        public static void AppendToJson(StringBuilder builder, Type type, object obj)
        {
            if (type == ZJsonReflection.TYPE_VECTOR2_INT)
            {
                var v = (Vector2Int)obj;
                builder.Append("{\"x\":");
                builder.Append(v.x);
                builder.Append(",\"y\":");
                builder.Append(v.y);
                builder.Append('}');
                return;
            }

            if (type == ZJsonReflection.TYPE_VECTOR3_INT)
            {
                var v = (Vector3Int)obj;
                builder.Append("{\"x\":");
                builder.Append(v.x);
                builder.Append(",\"y\":");
                builder.Append(v.y);
                builder.Append(",\"z\":");
                builder.Append(v.z);
                builder.Append('}');
                return;
            }

            if (type == ZJsonReflection.TYPE_RECT)
            {
                var v = (Rect)obj;
                builder.Append("{\"x\":");
                builder.Append(v.x.ToString(CultureInfo.InvariantCulture));
                builder.Append(",\"y\":");
                builder.Append(v.y.ToString(CultureInfo.InvariantCulture));
                builder.Append(",\"width\":");
                builder.Append(v.width.ToString(CultureInfo.InvariantCulture));
                builder.Append(",\"height\":");
                builder.Append(v.height.ToString(CultureInfo.InvariantCulture));
                builder.Append('}');
                return;
            }

            if (type == ZJsonReflection.TYPE_RECT_INT)
            {
                var v = (RectInt)obj;
                builder.Append("{\"x\":");
                builder.Append(v.x);
                builder.Append(",\"y\":");
                builder.Append(v.y);
                builder.Append(",\"width\":");
                builder.Append(v.width);
                builder.Append(",\"height\":");
                builder.Append(v.height);
                builder.Append('}');
                return;
            }

            if (type == ZJsonReflection.TYPE_BOUNDS)
            {
                var v = (Bounds)obj;
                builder.Append("{\"center\":");
                AppendVector3(builder, v.center);
                builder.Append(",\"size\":");
                AppendVector3(builder, v.size);
                builder.Append('}');
                return;
            }

            if (type == ZJsonReflection.TYPE_BOUNDS_INT)
            {
                var v = (BoundsInt)obj;
                builder.Append("{\"position\":");
                AppendVector3Int(builder, v.position);
                builder.Append(",\"size\":");
                AppendVector3Int(builder, v.size);
                builder.Append('}');
                return;
            }

            if (type == ZJsonReflection.TYPE_LAYER_MASK)
            {
                var v = (LayerMask)obj;
                builder.Append("{\"value\":");
                builder.Append(v.value);
                builder.Append('}');
                return;
            }

            if (type == ZJsonReflection.TYPE_HASH128)
            {
                var v = (Hash128)obj;
                builder.Append("{\"hash\":\"");
                builder.Append(v.ToString()); //Chuỗi hex nên không cần escape
                builder.Append("\"}");
                return;
            }

            throw new Exception("Not code for built-in type: " + type.Name);
        }

        private static void AppendVector3(StringBuilder builder, Vector3 v)
        {
            builder.Append("{\"x\":");
            builder.Append(v.x.ToString(CultureInfo.InvariantCulture));
            builder.Append(",\"y\":");
            builder.Append(v.y.ToString(CultureInfo.InvariantCulture));
            builder.Append(",\"z\":");
            builder.Append(v.z.ToString(CultureInfo.InvariantCulture));
            builder.Append('}');
        }

        private static void AppendVector3Int(StringBuilder builder, Vector3Int v)
        {
            builder.Append("{\"x\":");
            builder.Append(v.x);
            builder.Append(",\"y\":");
            builder.Append(v.y);
            builder.Append(",\"z\":");
            builder.Append(v.z);
            builder.Append('}');
        }

        #endregion



        #region From Reader

        /// <summary>
        /// Đọc trực tiếp từ reader, không qua node trung gian. Index phải đang ở vị trí của {
        /// <para>Khi kết thúc hàm thì index đang ở vị trí của }</para>
        /// <para>Key nào thiếu thì thành phần đó về default (tương thích data cũ chứa "{}")</para>
        /// </summary>
        public static object FromReader(Type type, ZReader reader)
        {
            if (type == ZJsonReflection.TYPE_VECTOR2_INT)
                return FromReader_Vector2Int(reader);

            if (type == ZJsonReflection.TYPE_VECTOR3_INT)
                return ReadVector3Int(reader);

            if (type == ZJsonReflection.TYPE_RECT)
                return FromReader_Rect(reader);

            if (type == ZJsonReflection.TYPE_RECT_INT)
                return FromReader_RectInt(reader);

            if (type == ZJsonReflection.TYPE_BOUNDS)
                return FromReader_Bounds(reader);

            if (type == ZJsonReflection.TYPE_BOUNDS_INT)
                return FromReader_BoundsInt(reader);

            if (type == ZJsonReflection.TYPE_LAYER_MASK)
                return FromReader_LayerMask(reader);

            if (type == ZJsonReflection.TYPE_HASH128)
                return FromReader_Hash128(reader);

            throw new Exception("Not code for built-in type: " + type.Name);
        }

        private static object FromReader_Vector2Int(ZReader reader)
        {
            int x = 0, y = 0;

            reader.index++; //Move qua {
            while (true)
            {
                string key = reader.ReadNextKey();
                if (key == null)
                    break;

                reader.index++; //Move qua "
                reader.MoveToAfterTwoDot();
                reader.MoveToNonSpace();

                switch (key)
                {
                    case "x": x = ReadInt(reader); break;
                    case "y": y = ReadInt(reader); break;
                    default: reader.SkipValue(); break;
                }

                reader.index++; //Move qua ký tự cuối của value
            }

            return new Vector2Int(x, y);
        }

        private static object FromReader_Rect(ZReader reader)
        {
            float x = 0f, y = 0f, width = 0f, height = 0f;

            reader.index++; //Move qua {
            while (true)
            {
                string key = reader.ReadNextKey();
                if (key == null)
                    break;

                reader.index++; //Move qua "
                reader.MoveToAfterTwoDot();
                reader.MoveToNonSpace();

                switch (key)
                {
                    case "x": x = ReadFloat(reader); break;
                    case "y": y = ReadFloat(reader); break;
                    case "width": width = ReadFloat(reader); break;
                    case "height": height = ReadFloat(reader); break;
                    default: reader.SkipValue(); break;
                }

                reader.index++; //Move qua ký tự cuối của value
            }

            return new Rect(x, y, width, height);
        }

        private static object FromReader_RectInt(ZReader reader)
        {
            int x = 0, y = 0, width = 0, height = 0;

            reader.index++; //Move qua {
            while (true)
            {
                string key = reader.ReadNextKey();
                if (key == null)
                    break;

                reader.index++; //Move qua "
                reader.MoveToAfterTwoDot();
                reader.MoveToNonSpace();

                switch (key)
                {
                    case "x": x = ReadInt(reader); break;
                    case "y": y = ReadInt(reader); break;
                    case "width": width = ReadInt(reader); break;
                    case "height": height = ReadInt(reader); break;
                    default: reader.SkipValue(); break;
                }

                reader.index++; //Move qua ký tự cuối của value
            }

            return new RectInt(x, y, width, height);
        }

        private static object FromReader_Bounds(ZReader reader)
        {
            Vector3 center = default, size = default;

            reader.index++; //Move qua {
            while (true)
            {
                string key = reader.ReadNextKey();
                if (key == null)
                    break;

                reader.index++; //Move qua "
                reader.MoveToAfterTwoDot();
                reader.MoveToNonSpace();

                switch (key)
                {
                    case "center":
                        if (reader.json[reader.index] == '{')
                            center = ReadVector3(reader);
                        else
                            reader.SkipValue();
                        break;

                    case "size":
                        if (reader.json[reader.index] == '{')
                            size = ReadVector3(reader);
                        else
                            reader.SkipValue();
                        break;

                    default: reader.SkipValue(); break;
                }

                reader.index++; //Move qua ký tự cuối của value
            }

            return new Bounds(center, size);
        }

        private static object FromReader_BoundsInt(ZReader reader)
        {
            Vector3Int position = default, size = default;

            reader.index++; //Move qua {
            while (true)
            {
                string key = reader.ReadNextKey();
                if (key == null)
                    break;

                reader.index++; //Move qua "
                reader.MoveToAfterTwoDot();
                reader.MoveToNonSpace();

                switch (key)
                {
                    case "position":
                        if (reader.json[reader.index] == '{')
                            position = ReadVector3Int(reader);
                        else
                            reader.SkipValue();
                        break;

                    case "size":
                        if (reader.json[reader.index] == '{')
                            size = ReadVector3Int(reader);
                        else
                            reader.SkipValue();
                        break;

                    default: reader.SkipValue(); break;
                }

                reader.index++; //Move qua ký tự cuối của value
            }

            return new BoundsInt(position, size);
        }

        private static object FromReader_LayerMask(ZReader reader)
        {
            int value = 0;

            reader.index++; //Move qua {
            while (true)
            {
                string key = reader.ReadNextKey();
                if (key == null)
                    break;

                reader.index++; //Move qua "
                reader.MoveToAfterTwoDot();
                reader.MoveToNonSpace();

                switch (key)
                {
                    case "value": value = ReadInt(reader); break;
                    default: reader.SkipValue(); break;
                }

                reader.index++; //Move qua ký tự cuối của value
            }

            return (LayerMask)value;
        }

        private static object FromReader_Hash128(ZReader reader)
        {
            string hash = "";

            reader.index++; //Move qua {
            while (true)
            {
                string key = reader.ReadNextKey();
                if (key == null)
                    break;

                reader.index++; //Move qua "
                reader.MoveToAfterTwoDot();
                reader.MoveToNonSpace();

                switch (key)
                {
                    case "hash": hash = ReadString(reader); break;
                    default: reader.SkipValue(); break;
                }

                reader.index++; //Move qua ký tự cuối của value
            }

            return string.IsNullOrEmpty(hash) ? default(Hash128) : Hash128.Parse(hash);
        }

        private static Vector3 ReadVector3(ZReader reader)
        {
            float x = 0f, y = 0f, z = 0f;

            reader.index++; //Move qua {
            while (true)
            {
                string key = reader.ReadNextKey();
                if (key == null)
                    break;

                reader.index++; //Move qua "
                reader.MoveToAfterTwoDot();
                reader.MoveToNonSpace();

                switch (key)
                {
                    case "x": x = ReadFloat(reader); break;
                    case "y": y = ReadFloat(reader); break;
                    case "z": z = ReadFloat(reader); break;
                    default: reader.SkipValue(); break;
                }

                reader.index++; //Move qua ký tự cuối của value
            }

            return new Vector3(x, y, z);
        }

        private static Vector3Int ReadVector3Int(ZReader reader)
        {
            int x = 0, y = 0, z = 0;

            reader.index++; //Move qua {
            while (true)
            {
                string key = reader.ReadNextKey();
                if (key == null)
                    break;

                reader.index++; //Move qua "
                reader.MoveToAfterTwoDot();
                reader.MoveToNonSpace();

                switch (key)
                {
                    case "x": x = ReadInt(reader); break;
                    case "y": y = ReadInt(reader); break;
                    case "z": z = ReadInt(reader); break;
                    default: reader.SkipValue(); break;
                }

                reader.index++; //Move qua ký tự cuối của value
            }

            return new Vector3Int(x, y, z);
        }

        private static int ReadInt(ZReader reader)
        {
            if (reader.IsCurrentAtNull())
            {
                reader.index += 3; //null: giữ default
                return 0;
            }

            reader.ReadNextScalarValueBounds(out _, out string value, out int start, out int length);
            if (value == null)
                return int.Parse(reader.json.AsSpan(start, length), System.Globalization.NumberStyles.Integer, CultureInfo.InvariantCulture);

            return int.Parse(value, CultureInfo.InvariantCulture);
        }

        private static float ReadFloat(ZReader reader)
        {
            if (reader.IsCurrentAtNull())
            {
                reader.index += 3; //null: giữ default
                return 0f;
            }

            reader.ReadNextScalarValueBounds(out _, out string value, out int start, out int length);
            if (value == null)
                return float.Parse(reader.json.AsSpan(start, length), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);

            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        private static string ReadString(ZReader reader)
        {
            if (reader.IsCurrentAtNull())
            {
                reader.index += 3; //null: giữ default
                return "";
            }

            reader.ReadNextScalarValue(out _, out string value);
            return value;
        }

        #endregion



        #region To Node

        public static ZJsonObject ToNode(Type type, object obj)
        {
            ZJsonObject node = new ZJsonObject();

            if (type == ZJsonReflection.TYPE_VECTOR2_INT)
            {
                var v = (Vector2Int)obj;
                node.Add("x", v.x);
                node.Add("y", v.y);
                return node;
            }

            if (type == ZJsonReflection.TYPE_VECTOR3_INT)
            {
                var v = (Vector3Int)obj;
                node.Add("x", v.x);
                node.Add("y", v.y);
                node.Add("z", v.z);
                return node;
            }

            if (type == ZJsonReflection.TYPE_RECT)
            {
                var v = (Rect)obj;
                node.Add("x", v.x);
                node.Add("y", v.y);
                node.Add("width", v.width);
                node.Add("height", v.height);
                return node;
            }

            if (type == ZJsonReflection.TYPE_RECT_INT)
            {
                var v = (RectInt)obj;
                node.Add("x", v.x);
                node.Add("y", v.y);
                node.Add("width", v.width);
                node.Add("height", v.height);
                return node;
            }

            if (type == ZJsonReflection.TYPE_BOUNDS)
            {
                var v = (Bounds)obj;
                node.Add("center", Vector3ToNode(v.center));
                node.Add("size", Vector3ToNode(v.size));
                return node;
            }

            if (type == ZJsonReflection.TYPE_BOUNDS_INT)
            {
                var v = (BoundsInt)obj;
                node.Add("position", Vector3IntToNode(v.position));
                node.Add("size", Vector3IntToNode(v.size));
                return node;
            }

            if (type == ZJsonReflection.TYPE_LAYER_MASK)
            {
                var v = (LayerMask)obj;
                node.Add("value", v.value);
                return node;
            }

            if (type == ZJsonReflection.TYPE_HASH128)
            {
                var v = (Hash128)obj;
                node.Add("hash", v.ToString());
                return node;
            }

            throw new Exception("Not code for built-in type: " + type.Name);
        }

        private static ZJsonObject Vector3ToNode(Vector3 v)
        {
            ZJsonObject node = new ZJsonObject();
            node.Add("x", v.x);
            node.Add("y", v.y);
            node.Add("z", v.z);
            return node;
        }

        private static ZJsonObject Vector3IntToNode(Vector3Int v)
        {
            ZJsonObject node = new ZJsonObject();
            node.Add("x", v.x);
            node.Add("y", v.y);
            node.Add("z", v.z);
            return node;
        }

        #endregion



        #region From Node

        /// <summary>
        /// Key nào thiếu trong node thì thành phần đó về default (tương thích data cũ chứa "{}")
        /// </summary>
        public static object FromNode(Type type, ZJsonObject node)
        {
            if (type == ZJsonReflection.TYPE_VECTOR2_INT)
                return new Vector2Int(GetInt(node, "x"), GetInt(node, "y"));

            if (type == ZJsonReflection.TYPE_VECTOR3_INT)
                return new Vector3Int(GetInt(node, "x"), GetInt(node, "y"), GetInt(node, "z"));

            if (type == ZJsonReflection.TYPE_RECT)
                return new Rect(GetFloat(node, "x"), GetFloat(node, "y"), GetFloat(node, "width"), GetFloat(node, "height"));

            if (type == ZJsonReflection.TYPE_RECT_INT)
                return new RectInt(GetInt(node, "x"), GetInt(node, "y"), GetInt(node, "width"), GetInt(node, "height"));

            if (type == ZJsonReflection.TYPE_BOUNDS)
                return new Bounds(GetVector3(node, "center"), GetVector3(node, "size"));

            if (type == ZJsonReflection.TYPE_BOUNDS_INT)
                return new BoundsInt(GetVector3Int(node, "position"), GetVector3Int(node, "size"));

            if (type == ZJsonReflection.TYPE_LAYER_MASK)
                return (LayerMask)GetInt(node, "value");

            if (type == ZJsonReflection.TYPE_HASH128)
            {
                string hash = GetString(node, "hash");
                return string.IsNullOrEmpty(hash) ? default(Hash128) : Hash128.Parse(hash);
            }

            throw new Exception("Not code for built-in type: " + type.Name);
        }

        private static int GetInt(ZJsonObject node, string key)
        {
            if (node.TryGetChild(key, out ZJsonNode child) && child != null)
                return child.AsInt;

            return 0;
        }

        private static float GetFloat(ZJsonObject node, string key)
        {
            if (node.TryGetChild(key, out ZJsonNode child) && child != null)
                return child.AsFloat;

            return 0f;
        }

        private static string GetString(ZJsonObject node, string key)
        {
            if (node.TryGetChild(key, out ZJsonNode child) && child != null)
                return child.AsString;

            return "";
        }

        private static Vector3 GetVector3(ZJsonObject node, string key)
        {
            if (node.TryGetChild(key, out ZJsonNode child) && child is ZJsonObject childObject)
                return new Vector3(GetFloat(childObject, "x"), GetFloat(childObject, "y"), GetFloat(childObject, "z"));

            return default;
        }

        private static Vector3Int GetVector3Int(ZJsonObject node, string key)
        {
            if (node.TryGetChild(key, out ZJsonNode child) && child is ZJsonObject childObject)
                return new Vector3Int(GetInt(childObject, "x"), GetInt(childObject, "y"), GetInt(childObject, "z"));

            return default;
        }

        #endregion
    }
}
