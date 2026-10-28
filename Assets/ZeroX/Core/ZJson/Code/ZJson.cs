using System;
using System.Text;

namespace ZeroX.JsonSystem
{
    public static class ZJson
    {
        /// <summary>
        /// Nếu json null hoặc empty thì sẽ trả về null
        /// </summary>
        public static ZJsonNode JsonToNode(string jsonString)
        {
            return ZJson_StringToNode.Parse(jsonString);
        }


        
        #region Json To Object

        /// <summary>
        /// Nếu json null, "", "null" thì sẽ trả về null
        /// </summary>
        public static object JsonToObject(string jsonString, Type objType)
        {
            return ZJson_StringToObject.ParseStringOverwriteObject(jsonString, objType, null);
        }

        /// <summary>
        /// Nếu json null, "", "null" thì sẽ trả về null:
        /// <para>Nếu TObject nullable thì sẽ trả về null</para>
        /// <para>Nếu TObject không nullable thì sẽ throw Exception</para>
        /// </summary>
        public static TObject JsonToObject<TObject>(string jsonString)
        {
            return (TObject)ZJson_StringToObject.ParseStringOverwriteObject(jsonString, typeof(TObject), null);
        }
        
        /// <summary>
        /// Nếu json null, "", "null" thì sẽ trả về null
        /// </summary>
        public static object JsonToObjectOverwrite(string jsonString, Type objType, object objToOverwrite)
        {
            return ZJson_StringToObject.ParseStringOverwriteObject(jsonString, objType, objToOverwrite);
        }
        
        /// <summary>
        /// Nếu json null, "", "null" thì sẽ trả về null
        /// </summary>
        public static TObject JsonToObjectOverwrite<TObject>(string jsonString, TObject objToOverwrite)
        {
            return (TObject)ZJson_StringToObject.ParseStringOverwriteObject(jsonString, typeof(TObject), objToOverwrite);
        }

        #endregion



        #region Object To Node and String

        /// <summary>
        /// Nếu obj null thì trả về null
        /// <para>Nếu obj là scalar hoặc enum thì throw, top-level chỉ nhận object, dictionary, list, array, built-in</para>
        /// </summary>
        public static ZJsonNode ObjectToNode(object obj)
        {
            if (obj == null)
                return null;
            
            Type objType = obj.GetType();
            if(ZJsonReflection.IsScalarType(objType) || objType.IsEnum)
                throw new FormatException(ZJsonConst.EM_IncorrectJsonStringFormat);
            
            return ZJson_ObjectToNode.ParseMaster(objType, obj);
        }
        
        [ThreadStatic] private static StringBuilder cachedBuilder;

        /// <summary>
        /// Nếu obj null thì trả về ""
        /// <para>Nếu obj là scalar hoặc enum thì throw, top-level chỉ nhận object, dictionary, list, array, built-in</para>
        /// </summary>
        public static string ObjectToJson(object obj)
        {
            if (obj == null)
                return "";


            Type objType = obj.GetType();
            if(ZJsonReflection.IsScalarType(objType) || objType.IsEnum)
                throw new FormatException(ZJsonConst.EM_IncorrectJsonStringFormat);


            //Tái sử dụng builder theo thread. Mượn - trả để an toàn nếu bị gọi re-entrant (custom serializable gọi ngược ObjectToJson)
            StringBuilder builder = cachedBuilder;
            if (builder == null)
            {
                builder = new StringBuilder(256);
            }
            else
            {
                cachedBuilder = null;
                builder.Clear();
            }

            ZJson_ObjectToString.ParseMaster(builder, objType, obj);
            string result = builder.ToString();

            cachedBuilder = builder;
            return result;
        }

        #endregion
    }
}