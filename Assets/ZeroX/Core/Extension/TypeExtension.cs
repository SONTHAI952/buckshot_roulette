using System.Reflection;
using System;

namespace ZeroX.Extensions
{
    public static class TypeExtension
    {
        static BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public |
                                           BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy;



        #region Get Field Deep
        
        public static FieldInfo GetFieldDeep(this Type type, string fieldName)
        {
            do
            {
                var fieldInfo = type.GetField(fieldName, bindingFlags);
                if (fieldInfo != null)
                    return fieldInfo;

                type = type.BaseType;
            } while (type != null);

            return null;
        }

        public static object GetFieldValueDeep(this Type type, string fieldName, object obj)
        {
            FieldInfo fieldInfo = GetFieldDeep(type, fieldName);
            if (fieldInfo == null)
                throw new Exception("Cannot GetFieldValueDeep because field not exist: " + fieldName);
            
            return fieldInfo.GetValue(obj);
        }
        
        public static T GetFieldValueDeep<T>(this Type type, string fieldName, object obj)
        {
            FieldInfo fieldInfo = GetFieldDeep(type, fieldName);
            if (fieldInfo == null)
                throw new Exception("Cannot GetFieldValueDeep because field not exist: " + fieldName);
            
            return (T)fieldInfo.GetValue(obj);
        }
        
        #endregion

        

        #region Get Property Deep
        
        /// <summary>
        /// Đối với các private property thì cần phải tìm tới từng base type thì mới thấy
        /// </summary>
        public static PropertyInfo GetPropertyDeep(this Type type, string propertyName)
        {
            do
            {
                var propertyInfo = type.GetProperty(propertyName, bindingFlags);
                if (propertyInfo != null)
                    return propertyInfo;

                type = type.BaseType;
            } while (type != null);

            return null;
        }
        
        public static object GetPropertyValueDeep(this Type type, string fieldName, object obj)
        {
            PropertyInfo propertyInfo = GetPropertyDeep(type, fieldName);
            if (propertyInfo == null)
                throw new Exception("Cannot GetPropertyValueDeep because field not exist: " + fieldName);
            
            return propertyInfo.GetValue(obj);
        }
        
        public static T GetPropertyValueDeep<T>(this Type type, string fieldName, object obj)
        {
            PropertyInfo propertyInfo = GetPropertyDeep(type, fieldName);
            if (propertyInfo == null)
                throw new Exception("Cannot GetPropertyValueDeep because field not exist: " + fieldName);
            
            return (T)propertyInfo.GetValue(obj);
        }
        
        #endregion
    }
}