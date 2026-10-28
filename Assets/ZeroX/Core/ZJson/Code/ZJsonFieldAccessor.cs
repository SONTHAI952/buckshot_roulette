using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ZeroX.JsonSystem
{
    /// <summary>
    /// Bọc FieldInfo kèm getter/setter compile sẵn bằng Expression tree (nhanh hơn GetValue/SetValue nhiều lần)
    /// </summary>
    public sealed class ZJsonFieldAccessor
    {
        public readonly FieldInfo fieldInfo;
        public readonly string name;
        public readonly Type fieldType;
        public readonly bool fieldIsValueType;
        public readonly Func<object, object> getter;
        public readonly Action<object, object> setter;

        public ZJsonFieldAccessor(FieldInfo fieldInfo)
        {
            this.fieldInfo = fieldInfo;
            this.name = fieldInfo.Name;
            this.fieldType = fieldInfo.FieldType;
            this.fieldIsValueType = fieldType.IsValueType;
            this.getter = BuildGetter(fieldInfo);
            this.setter = BuildSetter(fieldInfo);
        }

        private static Func<object, object> BuildGetter(FieldInfo fieldInfo)
        {
            var pObj = Expression.Parameter(typeof(object), "o");

            //Unbox với struct để đọc trực tiếp trên box, Convert với class
            var cast = fieldInfo.DeclaringType.IsValueType
                ? Expression.Unbox(pObj, fieldInfo.DeclaringType)
                : Expression.Convert(pObj, fieldInfo.DeclaringType);

            var body = Expression.Convert(Expression.Field(cast, fieldInfo), typeof(object));
            return Expression.Lambda<Func<object, object>>(body, pObj).Compile();
        }

        private static Action<object, object> BuildSetter(FieldInfo fieldInfo)
        {
            if (fieldInfo.IsInitOnly) //Field readonly: Expression.Assign không cho phép -> fallback reflection
                return fieldInfo.SetValue;

            var pObj = Expression.Parameter(typeof(object), "o");
            var pVal = Expression.Parameter(typeof(object), "v");

            //Unbox với struct để mutate trực tiếp trên box (Convert sẽ tạo copy và mất giá trị gán)
            var cast = fieldInfo.DeclaringType.IsValueType
                ? Expression.Unbox(pObj, fieldInfo.DeclaringType)
                : Expression.Convert(pObj, fieldInfo.DeclaringType);

            //Field kiểu value-type nhận null thì về default (đồng bộ hành vi SetValue(obj, null) cũ)
            Expression valueExpression;
            if (fieldInfo.FieldType.IsValueType)
            {
                valueExpression = Expression.Condition(
                    Expression.Equal(pVal, Expression.Constant(null)),
                    Expression.Default(fieldInfo.FieldType),
                    Expression.Convert(pVal, fieldInfo.FieldType));
            }
            else
            {
                valueExpression = Expression.Convert(pVal, fieldInfo.FieldType);
            }

            var body = Expression.Assign(Expression.Field(cast, fieldInfo), valueExpression);
            return Expression.Lambda<Action<object, object>>(body, pObj, pVal).Compile();
        }
    }
}
