using System;
using System.Globalization;
using System.Text;

namespace ZeroX.JsonSystem
{
    public abstract class ZJsonNode
    {
        public abstract ZJsonNodeType NodeType { get; }
        
        public abstract int ChildCount { get; }
        
        
        
        #region Access

        public abstract ZJsonNode this[string key] { get; set; }
        public abstract ZJsonNode this[int index] { get; set; }

        #endregion
        
        
        
        #region To Json String

        public abstract void ToJson(StringBuilder builder);

        public string ToJson()
        {
            StringBuilder builder = new StringBuilder();
            ToJson(builder);
            return builder.ToString();
        }

        /// <summary>
        /// Tương đương với ToJsonString
        /// </summary>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            ToJson(builder);
            return builder.ToString();
        }

        #endregion

        

        #region To Object
        
        /// <summary>
        /// Nếu objToOverwrite null, array hoặc kiểu giá trị thì sẽ trả về object mới
        /// </summary>
        public abstract object OverwriteObject(Type objType, object objToOverwrite);

        public TObject OverwriteObject<TObject>(TObject objToOverwrite)
        {
            return (TObject)OverwriteObject(typeof(TObject), objToOverwrite);
        }
        
        public object ToObject(Type objType)
        {
            return OverwriteObject(objType, null);
        }
        
        public TObject ToObject<TObject>()
        {
            return (TObject)OverwriteObject(typeof(TObject), null);
        }
        
        #endregion
        
        

        #region As
        
        public ZJsonObject AsObject => (ZJsonObject)this;
        public ZJsonList AsList => (ZJsonList)this;
        public ZJsonScalar AsScalar => (ZJsonScalar)this;

        #endregion

        

        #region As Scalar - Primitive

        public virtual bool AsBool
        {
            get => throw ZJsonConst.EX_JsonNodeIsNotScalar;
            set => throw ZJsonConst.EX_JsonNodeIsNotScalar;
        }
        
        public virtual byte AsByte
        {
            get => throw ZJsonConst.EX_JsonNodeIsNotScalar;
            set => throw ZJsonConst.EX_JsonNodeIsNotScalar;
        }
        
        public virtual sbyte AsSByte
        {
            get => throw ZJsonConst.EX_JsonNodeIsNotScalar;
            set => throw ZJsonConst.EX_JsonNodeIsNotScalar;
        }
        
        public virtual char AsChar
        {
            get => throw ZJsonConst.EX_JsonNodeIsNotScalar;
            set => throw ZJsonConst.EX_JsonNodeIsNotScalar;
        }
        
        public virtual double AsDouble
        {
            get => throw ZJsonConst.EX_JsonNodeIsNotScalar;
            set => throw ZJsonConst.EX_JsonNodeIsNotScalar;
        }
        
        public virtual float AsFloat
        {
            get => throw ZJsonConst.EX_JsonNodeIsNotScalar;
            set => throw ZJsonConst.EX_JsonNodeIsNotScalar;
        }
        
        public virtual int AsInt
        {
            get => throw ZJsonConst.EX_JsonNodeIsNotScalar;
            set => throw ZJsonConst.EX_JsonNodeIsNotScalar;
        }
        
        public virtual uint AsUInt
        {
            get => throw ZJsonConst.EX_JsonNodeIsNotScalar;
            set => throw ZJsonConst.EX_JsonNodeIsNotScalar;
        }
        
        public virtual nint AsNInt
        {
            get => throw ZJsonConst.EX_JsonNodeIsNotScalar;
            set => throw ZJsonConst.EX_JsonNodeIsNotScalar;
        }
        
        public virtual nuint AsNUInt
        {
            get => throw ZJsonConst.EX_JsonNodeIsNotScalar;
            set => throw ZJsonConst.EX_JsonNodeIsNotScalar;
        }
        
        public virtual long AsLong
        {
            get => throw ZJsonConst.EX_JsonNodeIsNotScalar;
            set => throw ZJsonConst.EX_JsonNodeIsNotScalar;
        }
        
        public virtual ulong AsULong
        {
            get => throw ZJsonConst.EX_JsonNodeIsNotScalar;
            set => throw ZJsonConst.EX_JsonNodeIsNotScalar;
        }
        
        public virtual short AsShort
        {
            get => throw ZJsonConst.EX_JsonNodeIsNotScalar;
            set => throw ZJsonConst.EX_JsonNodeIsNotScalar;
        }
        
        public virtual ushort AsUShort
        {
            get => throw ZJsonConst.EX_JsonNodeIsNotScalar;
            set => throw ZJsonConst.EX_JsonNodeIsNotScalar;
        }
        
        #endregion
        
        
        
        #region As Scalar - Like-Primitive

        public virtual string AsString
        {
            get => throw ZJsonConst.EX_JsonNodeIsNotScalar;
            set => throw ZJsonConst.EX_JsonNodeIsNotScalar;
        }
        
        public virtual decimal AsDecimal
        {
            get => throw ZJsonConst.EX_JsonNodeIsNotScalar;
            set => throw ZJsonConst.EX_JsonNodeIsNotScalar;
        }

        #endregion



        #region Implicit Operators
        //Cho phép viết node["key"] = 2 thay vì node["key"] = new ZJsonScalar(ZJsonScalarType.Number, "2")

        public static implicit operator ZJsonNode(bool value)
        {
            return new ZJsonScalar(ZJsonScalarType.Bool, value ? "true" : "false");
        }

        public static implicit operator ZJsonNode(string value)
        {
            return value == null ? null : new ZJsonScalar(ZJsonScalarType.String, value);
        }

        public static implicit operator ZJsonNode(char value)
        {
            return new ZJsonScalar(ZJsonScalarType.String, value.ToString());
        }

        public static implicit operator ZJsonNode(int value)
        {
            return new ZJsonScalar(ZJsonScalarType.Number, value.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator ZJsonNode(uint value)
        {
            return new ZJsonScalar(ZJsonScalarType.Number, value.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator ZJsonNode(long value)
        {
            return new ZJsonScalar(ZJsonScalarType.Number, value.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator ZJsonNode(ulong value)
        {
            return new ZJsonScalar(ZJsonScalarType.Number, value.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator ZJsonNode(short value)
        {
            return new ZJsonScalar(ZJsonScalarType.Number, value.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator ZJsonNode(ushort value)
        {
            return new ZJsonScalar(ZJsonScalarType.Number, value.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator ZJsonNode(byte value)
        {
            return new ZJsonScalar(ZJsonScalarType.Number, value.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator ZJsonNode(sbyte value)
        {
            return new ZJsonScalar(ZJsonScalarType.Number, value.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator ZJsonNode(float value)
        {
            return new ZJsonScalar(ZJsonScalarType.Number, value.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator ZJsonNode(double value)
        {
            return new ZJsonScalar(ZJsonScalarType.Number, value.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator ZJsonNode(decimal value)
        {
            return new ZJsonScalar(ZJsonScalarType.Number, value.ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator ZJsonNode(nint value)
        {
            return new ZJsonScalar(ZJsonScalarType.Number, ((long)value).ToString(CultureInfo.InvariantCulture));
        }

        public static implicit operator ZJsonNode(nuint value)
        {
            return new ZJsonScalar(ZJsonScalarType.Number, ((ulong)value).ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}