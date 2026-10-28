using System;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace ZeroX.JsonSystem
{
    public sealed class ZJsonScalar : ZJsonNode
    {
        public override ZJsonNodeType NodeType => ZJsonNodeType.Scalar;
        public override int ChildCount => 0;
        public ZJsonScalarType ScalarType { get; private set; } = ZJsonScalarType.String;
        
        
        /// <summary>
        /// Là chuỗi scalar gốc của node
        /// </summary>
        private string raw = ""; //Không bao giờ được null

        public string Raw => raw;


        
        #region Access

        public override ZJsonNode this[string key]
        {
            get => throw new Exception(ZJsonConst.EM_JsonNodeIsNotObject);
            set => throw new Exception(ZJsonConst.EM_JsonNodeIsNotObject);
        }
        
        public override ZJsonNode this[int index]
        {
            get => throw new Exception(ZJsonConst.EM_JsonNodeIsNotList);
            set => throw new Exception(ZJsonConst.EM_JsonNodeIsNotList);
        }

        #endregion
        

        
        #region Constructor

        public ZJsonScalar()
        {
        }
        
        public ZJsonScalar(ZJsonScalarType scalarType, string raw)
        {
            this.ScalarType = scalarType;
            this.raw = raw ?? "";
        }
        
        #endregion

        
        
        #region Primitive
        
        public override bool AsBool
        {
            get => raw.Length == 0 ? false : bool.Parse(raw);
            set
            {
                raw = value ? "true" : "false"; //Nếu dùng ToString() thì sẽ bị viết hoa chữ đầu: True hoặc False
                ScalarType = ZJsonScalarType.Bool;
            }
        }
        
        public override byte AsByte
        {
            get => raw.Length == 0 ? default : byte.Parse(raw, CultureInfo.InvariantCulture);
            set
            {
                raw = value.ToString(CultureInfo.InvariantCulture);
                ScalarType = ZJsonScalarType.Number;
            }
        }
        
        public override sbyte AsSByte
        {
            get => raw.Length == 0 ? default : sbyte.Parse(raw, CultureInfo.InvariantCulture);
            set
            {
                raw = value.ToString(CultureInfo.InvariantCulture);
                ScalarType = ZJsonScalarType.Number;
            }
        }
        
        public override char AsChar
        {
            get => raw.Length == 0 ? default : char.Parse(raw);
            set
            {
                raw = value.ToString(CultureInfo.InvariantCulture);
                ScalarType = ZJsonScalarType.String;
            }
        }
        
        public override double AsDouble
        {
            get => raw.Length == 0 ? default : double.Parse(raw, CultureInfo.InvariantCulture);
            set
            {
                raw = value.ToString(CultureInfo.InvariantCulture);
                ScalarType = ZJsonScalarType.Number;
            }
        }
        
        public override float AsFloat
        {
            get => raw.Length == 0 ? default : float.Parse(raw, CultureInfo.InvariantCulture);
            set
            {
                raw = value.ToString(CultureInfo.InvariantCulture);
                ScalarType = ZJsonScalarType.Number;
            }
        }

        public override int AsInt
        {
            get => raw.Length == 0 ? default : int.Parse(raw, CultureInfo.InvariantCulture);
            set
            {
                raw = value.ToString(CultureInfo.InvariantCulture);
                ScalarType = ZJsonScalarType.Number;
            }
        }
        
        public override uint AsUInt
        {
            get => raw.Length == 0 ? default : uint.Parse(raw, CultureInfo.InvariantCulture);
            set
            {
                raw = value.ToString(CultureInfo.InvariantCulture);
                ScalarType = ZJsonScalarType.Number;
            }
        }
        
        public override nint AsNInt
        {
            get
            {
                if (raw.Length == 0)
                    return default;

                if (IntPtr.Size == 4)
                    return int.Parse(raw, CultureInfo.InvariantCulture);

                return (nint)long.Parse(raw, CultureInfo.InvariantCulture);
            }
            set
            {
                raw = value.ToString();
                ScalarType = ZJsonScalarType.Number;
            }
        }
        
        public override nuint AsNUInt
        {
            get
            {
                if (raw.Length == 0)
                    return default;

                if (IntPtr.Size == 4)
                    return uint.Parse(raw, CultureInfo.InvariantCulture);

                return (nuint)ulong.Parse(raw, CultureInfo.InvariantCulture);
            }
            set
            {
                raw = value.ToString();
                ScalarType = ZJsonScalarType.Number;
            }
        }
        
        public override long AsLong
        {
            get => raw.Length == 0 ? default : long.Parse(raw, CultureInfo.InvariantCulture);
            set
            {
                raw = value.ToString(CultureInfo.InvariantCulture);
                ScalarType = ZJsonScalarType.Number;
            }
        }
        
        public override ulong AsULong
        {
            get => raw.Length == 0 ? default : ulong.Parse(raw, CultureInfo.InvariantCulture);
            set
            {
                raw = value.ToString(CultureInfo.InvariantCulture);
                ScalarType = ZJsonScalarType.Number;
            }
        }
        
        public override short AsShort
        {
            get => raw.Length == 0 ? default : short.Parse(raw, CultureInfo.InvariantCulture);
            set
            {
                raw = value.ToString(CultureInfo.InvariantCulture);
                ScalarType = ZJsonScalarType.Number;
            }
        }
        
        public override ushort AsUShort
        {
            get => raw.Length == 0 ? default : ushort.Parse(raw, CultureInfo.InvariantCulture);
            set
            {
                raw = value.ToString(CultureInfo.InvariantCulture);
                ScalarType = ZJsonScalarType.Number;
            }
        }

        #endregion

        

        #region Like-Primitive

        public override string AsString
        {
            get => raw;
            set
            {
                raw = value == null ? "" : value;
                ScalarType = ZJsonScalarType.String;
            }
        }
        
        public override decimal AsDecimal
        {
            get => raw.Length == 0 ? default : decimal.Parse(raw, CultureInfo.InvariantCulture);
            set
            {
                raw = value.ToString(CultureInfo.InvariantCulture);
                ScalarType = ZJsonScalarType.Number;
            }
        }

        #endregion
        
        
        
        #region To Json String

        public override void ToJson(StringBuilder builder)
        {
            if (ScalarType == ZJsonScalarType.String)
            {
                builder.Append(ZJsonHelper.FixStringForJson(raw));
                return;
            }

            if (ScalarType == ZJsonScalarType.Number)
            {
                builder.Append(raw);
                return;
            }

            if (ScalarType == ZJsonScalarType.Bool)
            {
                builder.Append(raw);
                return;
            }
            
            throw new Exception("Not code for scalar type: " + ScalarType);
        }

        #endregion



        #region To Object

        public override object OverwriteObject(Type objType, object objToOverwrite)
        {
            //Đồng bộ với đường string: chuỗi rỗng cho type không phải string/char thì coi như null (field sẽ về default)
            if (raw.Length == 0 && objType != ZJsonReflection.TYPE_STRING && objType != ZJsonReflection.TYPE_CHAR)
                return null;

            if (objType.IsEnum)
            {
                var underlyingType = Enum.GetUnderlyingType(objType);
                var underlyingValue = Convert.ChangeType(raw, underlyingType, CultureInfo.InvariantCulture);
                return Enum.ToObject(objType, underlyingValue);
            }


            //Convert.ChangeType không hỗ trợ nint/nuint (IntPtr không phải IConvertible)
            if (objType == ZJsonReflection.TYPE_NINT)
                return AsNInt;

            if (objType == ZJsonReflection.TYPE_NUINT)
                return AsNUInt;


            //Shape mismatch: value dạng scalar nhưng type đích không phải scalar -> trả null (field sẽ về null/default)
            if (ZJsonReflection.IsScalarType(objType) == false)
                return null;

            return Convert.ChangeType(raw, objType, CultureInfo.InvariantCulture);
        }

        #endregion
    }
}