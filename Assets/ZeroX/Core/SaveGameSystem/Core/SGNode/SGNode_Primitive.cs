using System;
using System.Globalization;
using System.Numerics;
using System.Text;
using UnityEngine;
using ZeroX.SaveGameSystem.Security;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace ZeroX.SaveGameSystem
{
    public partial class SGNode
    { 
        //If node is primitive
        private EString e_stringValue;
        private object e_numberValue;


        public static readonly DateTime DefaultDateTime = DateTime.MinValue;
        public static readonly DateTime DefaultDate = DateTime.MinValue.Date;
        public static readonly DateTime DefaultTime = DateTime.MinValue.Date;


        #region Utility

        private void LogWarningNodeValueIsNotNumber()
        {
            Debug.LogWarningFormat("Node value '{0}' is not number", Key);
        }
        
        private int ParseInt(string s)
        {
            try
            {
                return int.Parse(s, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                return (int)float.Parse(s, CultureInfo.InvariantCulture);
            }
        }
        
        private long ParseLong(string s)
        {
            try
            {
                return long.Parse(s, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                return (long)double.Parse(s, CultureInfo.InvariantCulture);
            }
        }
        
        private BigInteger ParseBigInteger(string s)
        {
            try
            {
                return BigInteger.Parse(s, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                int dotIndex = s.IndexOf('.');
                if (dotIndex == -1)
                    throw;
                
                return BigInteger.Parse(s.Substring(0, dotIndex), CultureInfo.InvariantCulture);
            }
        }

        #endregion

        #region String
        
        public string GetString(string defaultValue = "")
        {
            if(NodeType == NodeType.String)
                return e_stringValue == null ? "" : e_stringValue.Value;
            
            if (NodeType == NodeType.Undefined)
                return defaultValue;
            
            
            
            if(IsPrimitive == false)
                throw new Exception("Node value is not primitive");
            
            Debug.LogWarningFormat("Node value '{0}' is not string", Key);
            return e_numberValue == null ? defaultValue : Convert.ToString(e_numberValue, CultureInfo.InvariantCulture);
        }

        public void SetString(string value)
        {
            lock (Database)
            {
                NodeType = NodeType.String;

                if (e_stringValue == null)
                    e_stringValue = new EString(value == null ? "" : value);
                else
                    e_stringValue.Value = value == null ? "" : value;
            
                if(Parent == null)
                    Database.StorageService.SetDatabaseChanged();
                else
                    Database.StorageService.SetNodeChanged(Parent);
            }
        }

        public string StringValue
        {
            get => GetString("");
            set => SetString(value);
        }
        
        #endregion
        
        #region Int

        public int GetInt(int defaultValue = 0)
        {
            if (NodeType == NodeType.Int)
                return ((EInt)e_numberValue).Value;
            
            if (NodeType == NodeType.Undefined)
                return defaultValue;
            
            if(IsPrimitive == false)
                throw new Exception("Node value is not primitive");
            
            

            if (NodeType == NodeType.String)
            {
                LogWarningNodeValueIsNotNumber();
                
                if (e_stringValue == null || string.IsNullOrEmpty(e_stringValue.Value))
                    return defaultValue;
                
                return ParseInt(e_stringValue.Value);
            }
            
            
            
            if (NodeType == NodeType.Float)
                return (int)((EFloat)e_numberValue).Value;
            
            if (NodeType == NodeType.Long)
                return (int)((ELong)e_numberValue).Value;
            
            if (NodeType == NodeType.Decimal)
                return (int)((EDecimal)e_numberValue).Value;
            
            if (NodeType == NodeType.Double)
                return (int)((EDouble)e_numberValue).Value;
            
            if(NodeType == NodeType.Bool)
                return ((EInt)e_numberValue).Value;
            
            if(NodeType == NodeType.BigInteger)
                return (int)((EBigInteger)e_numberValue).Value;
            
            throw new Exception("Not programmed for this value type yet: " + NodeType);
        }

        public void SetInt(int value)
        {
            lock (Database)
            {
                NodeType = NodeType.Int;
                e_numberValue = new EInt(value);
            
                if(Parent == null)
                    Database.StorageService.SetDatabaseChanged();
                else
                    Database.StorageService.SetNodeChanged(Parent);
            }
        }
        
        public int IntValue
        {
            get => GetInt(0);
            set => SetInt(value);
        }
        
        #endregion
        
        #region Float

        public float GetFloat(float defaultValue = 0f)
        {
            if (NodeType == NodeType.Float)
                return ((EFloat)e_numberValue).Value;
            
            if (NodeType == NodeType.Undefined)
                return defaultValue;
            
            if(IsPrimitive == false)
                throw new Exception("Node value is not primitive");
            
            

            if (NodeType == NodeType.String)
            {
                LogWarningNodeValueIsNotNumber();
                
                if (e_stringValue == null || string.IsNullOrEmpty(e_stringValue.Value))
                    return defaultValue;
                
                return float.Parse(e_stringValue.Value, CultureInfo.InvariantCulture);
            }



            if (NodeType == NodeType.Int)
                return ((EInt)e_numberValue).Value;
            
            if (NodeType == NodeType.Long)
                return ((ELong)e_numberValue).Value;
            
            if (NodeType == NodeType.Decimal)
                return (float)((EDecimal)e_numberValue).Value;
            
            if (NodeType == NodeType.Double)
                return (float)((EDouble)e_numberValue).Value;
            
            if(NodeType == NodeType.Bool)
                return ((EInt)e_numberValue).Value;
            
            if(NodeType == NodeType.BigInteger)
                return (float)((EBigInteger)e_numberValue).Value;
            
            throw new Exception("Not programmed for this value type yet: " + NodeType);
        }

        public void SetFloat(float value)
        {
            lock (Database)
            {
                NodeType = NodeType.Float;
                e_numberValue = new EFloat(value);
            
                if(Parent == null)
                    Database.StorageService.SetDatabaseChanged();
                else
                    Database.StorageService.SetNodeChanged(Parent);
            }
        }
        
        public float FloatValue
        {
            get => GetFloat(0);
            set => SetFloat(value);
        }
        
        #endregion
        
        #region Long
        
        public long GetLong(long defaultValue = 0L)
        {
            if (NodeType == NodeType.Long)
                return ((ELong)e_numberValue).Value;
            
            if (NodeType == NodeType.Undefined)
                return defaultValue;
            
            if(IsPrimitive == false)
                throw new Exception("Node value is not primitive");
            
            

            if (NodeType == NodeType.String)
            {
                LogWarningNodeValueIsNotNumber();
                
                if (e_stringValue == null || string.IsNullOrEmpty(e_stringValue.Value))
                    return defaultValue;

                return ParseLong(e_stringValue.Value);
            }



            if (NodeType == NodeType.Int)
                return ((EInt)e_numberValue).Value;
            
            if (NodeType == NodeType.Float)
                return (long)((EFloat)e_numberValue).Value;
            
            if (NodeType == NodeType.Decimal)
                return (long)((EDecimal)e_numberValue).Value;
            
            if (NodeType == NodeType.Double)
                return (long)((EDouble)e_numberValue).Value;
            
            if(NodeType == NodeType.Bool)
                return ((EInt)e_numberValue).Value;
            
            if(NodeType == NodeType.BigInteger)
                return (long)((EBigInteger)e_numberValue).Value;
            
            throw new Exception("Not programmed for this value type yet: " + NodeType);
        }

        public void SetLong(long value)
        {
            lock (Database)
            {
                NodeType = NodeType.Long;
                e_numberValue = new ELong(value);
            
                if(Parent == null)
                    Database.StorageService.SetDatabaseChanged();
                else
                    Database.StorageService.SetNodeChanged(Parent);
            }
        }
        
        public long LongValue
        {
            get => GetLong(0);
            set => SetLong(value);
        }
        
        #endregion
        
        #region Decimal

        public decimal GetDecimal(decimal defaultValue = 0m)
        {
            if (NodeType == NodeType.Decimal)
                return ((EDecimal)e_numberValue).Value;
            
            if (NodeType == NodeType.Undefined)
                return defaultValue;
            
            if(IsPrimitive == false)
                throw new Exception("Node value is not primitive");
            
            

            if (NodeType == NodeType.String)
            {
                LogWarningNodeValueIsNotNumber();
                
                if (e_stringValue == null || string.IsNullOrEmpty(e_stringValue.Value))
                    return defaultValue;
                
                return decimal.Parse(e_stringValue.Value, CultureInfo.InvariantCulture);
            }



            if (NodeType == NodeType.Int)
                return ((EInt)e_numberValue).Value;
            
            if (NodeType == NodeType.Float)
                return (decimal)((EFloat)e_numberValue).Value;
            
            if (NodeType == NodeType.Long)
                return ((ELong)e_numberValue).Value;
            
            if (NodeType == NodeType.Double)
                return (decimal)((EDouble)e_numberValue).Value;
            
            if(NodeType == NodeType.Bool)
                return ((EInt)e_numberValue).Value;
            
            if(NodeType == NodeType.BigInteger)
                return (decimal)((EBigInteger)e_numberValue).Value;
            
            throw new Exception("Not programmed for this value type yet: " + NodeType);
        }

        public void SetDecimal(decimal value)
        {
            lock (Database)
            {
                NodeType = NodeType.Decimal;
                e_numberValue = new EDecimal(value);
            
                if(Parent == null)
                    Database.StorageService.SetDatabaseChanged();
                else
                    Database.StorageService.SetNodeChanged(Parent);
            }
        }
        
        public decimal DecimalValue
        {
            get => GetDecimal(0m);
            set => SetDecimal(value);
        }
        
        #endregion
        
        #region Double

        public double GetDouble(double defaultValue = 0d)
        {
            if (NodeType == NodeType.Double)
                return ((EDouble)e_numberValue).Value;
            
            if (NodeType == NodeType.Undefined)
                return defaultValue;
            
            if(IsPrimitive == false)
                throw new Exception("Node value is not primitive");
            
            

            if (NodeType == NodeType.String)
            {
                LogWarningNodeValueIsNotNumber();
                
                if (e_stringValue == null || string.IsNullOrEmpty(e_stringValue.Value))
                    return defaultValue;
                
                return double.Parse(e_stringValue.Value, CultureInfo.InvariantCulture);
            }



            if (NodeType == NodeType.Int)
                return ((EInt)e_numberValue).Value;
            
            if (NodeType == NodeType.Float)
                return ((EFloat)e_numberValue).Value;
            
            if (NodeType == NodeType.Long)
                return ((ELong)e_numberValue).Value;
            
            if (NodeType == NodeType.Decimal)
                return (double)((EDecimal)e_numberValue).Value;

            if(NodeType == NodeType.Bool)
                return ((EInt)e_numberValue).Value;
            
            if(NodeType == NodeType.BigInteger)
                return (double)((EBigInteger)e_numberValue).Value;
            
            throw new Exception("Not programmed for this value type yet: " + NodeType);
        }

        public void SetDouble(double value)
        {
            lock (Database)
            {
                NodeType = NodeType.Double;
                e_numberValue = new EDouble(value);
            
                if(Parent == null)
                    Database.StorageService.SetDatabaseChanged();
                else
                    Database.StorageService.SetNodeChanged(Parent);
            }
        }
        
        public double DoubleValue
        {
            get => GetDouble(0d);
            set => SetDouble(value);
        }
        
        #endregion

        #region Bool

        public bool GetBool(bool defaultValue = false)
        {
            if(NodeType == NodeType.Bool)
                return ((EInt)e_numberValue).Value != 0;
            
            if (NodeType == NodeType.Undefined)
                return defaultValue;
            
            
            
            if(IsPrimitive == false)
                throw new Exception("Node value is not primitive");

            if (NodeType == NodeType.String)
            {
                LogWarningNodeValueIsNotNumber();
                
                if (e_stringValue == null)
                    return defaultValue;
                
                if (bool.TryParse(e_stringValue.Value, out var parsedValue))
                    return parsedValue;
                else
                    return defaultValue;
            }
            
            
            
            if (NodeType == NodeType.Int)
                return ((EInt)e_numberValue).Value != 0;

            if (NodeType == NodeType.Float)
                return (int)((EFloat)e_numberValue).Value != 0;
            
            if (NodeType == NodeType.Long)
                return ((ELong)e_numberValue).Value != 0;
            
            if (NodeType == NodeType.Decimal)
                return (int)((EDecimal)e_numberValue).Value != 0;
            
            if (NodeType == NodeType.Double)
                return (int)((EDouble)e_numberValue).Value != 0;
            
            if(NodeType == NodeType.BigInteger)
                return ((EBigInteger)e_numberValue).Value != 0;
            
            
            
            throw new Exception("Not programmed for this value type yet: " + NodeType);
        }

        public void SetBool(bool value)
        {
            lock (Database)
            {
                NodeType = NodeType.Bool;
                e_numberValue = new EInt(value ? 1 : 0);
            
                if(Parent == null)
                    Database.StorageService.SetDatabaseChanged();
                else
                    Database.StorageService.SetNodeChanged(Parent);
            }
        }

        public bool BoolValue
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        #endregion
        
        #region BigInt

        public BigInteger GetBigInt(BigInteger defaultValue = default)
        {
            if(NodeType == NodeType.BigInteger)
                return ((EBigInteger)e_numberValue).Value;
            
            if (NodeType == NodeType.Undefined)
                return defaultValue;
            
            if(IsPrimitive == false)
                throw new Exception("Node value is not primitive");
            
            

            if (NodeType == NodeType.String)
            {
                LogWarningNodeValueIsNotNumber();
                
                if (e_stringValue == null || string.IsNullOrEmpty(e_stringValue.Value))
                    return defaultValue;
                
                return ParseBigInteger(e_stringValue.Value);
            }
            
            
            
            if (NodeType == NodeType.Int)
                return ((EInt)e_numberValue).Value;

            if (NodeType == NodeType.Float)
                return (BigInteger)((EFloat)e_numberValue).Value;
            
            if (NodeType == NodeType.Long)
                return ((ELong)e_numberValue).Value;
            
            if (NodeType == NodeType.Decimal)
                return (BigInteger)((EDecimal)e_numberValue).Value;
            
            if (NodeType == NodeType.Double)
                return (BigInteger)((EDouble)e_numberValue).Value;
            
            if(NodeType == NodeType.Bool)
                return ((EInt)e_numberValue).Value;
            
            throw new Exception("Not programmed for this value type yet: " + NodeType);
        }

        public void SetBigInt(BigInteger value)
        {
            lock (Database)
            {
                NodeType = NodeType.BigInteger;
                e_numberValue = new EBigInteger(value);
            
                if(Parent == null)
                    Database.StorageService.SetDatabaseChanged();
                else
                    Database.StorageService.SetNodeChanged(Parent);
            }
        }
        
        public BigInteger BigIntValue
        {
            get => GetBigInt(0);
            set => SetBigInt(value);
        }
        
        #endregion

        #region DateTime

        /// <summary>
        /// If empty, will return DateTime.MinValue
        /// </summary>
        public DateTime GetDateTime(DateTime defaultValue = default)
        {
            string s = GetString("");
            if (string.IsNullOrEmpty(s))
                return defaultValue;

            if (DateTime.TryParseExact(s, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result1))
                return result1;
            
            if (DateTime.TryParseExact(s, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result2))
                return result2;
            
            if (DateTime.TryParseExact(s, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result3))
                return result3;
            
            throw new FormatException("String was not recognized as a valid DateTime.");
        }

        public void SetDateTime(DateTime value)
        {
            string s = value.ToString("dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            SetString(s);
        }

        public DateTime DateTimeValue
        {
            get => GetDateTime(default);
            set => SetDateTime(value);
        }
        
        /// <summary>
        /// Compare date and time dd:MM:yyyy HH:mm:ss. The smallest unit for comparison is seconds.
        /// </summary>
        public bool EqualsDateTime(DateTime otherDateTimeValue)
        {
            var timeValue = GetTime();
            return timeValue.Day == otherDateTimeValue.Day &&
                   timeValue.Month == otherDateTimeValue.Month &&
                   timeValue.Year == otherDateTimeValue.Year &&
                   
                   timeValue.Hour == otherDateTimeValue.Hour &&
                   timeValue.Minute == otherDateTimeValue.Minute &&
                   timeValue.Second == otherDateTimeValue.Second;
        }
        
        #endregion

        #region Date

        public DateTime GetDate(DateTime defaultValue = default)
        {
            string s = GetString("");
            if (string.IsNullOrEmpty(s))
                return defaultValue;
            
            if (DateTime.TryParseExact(s, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result1))
                return result1;
            
            if (DateTime.TryParseExact(s, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result2))
                return result2.Date;

            if (DateTime.TryParseExact(s, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result3))
                return result3.Date;
            
            throw new FormatException("String was not recognized as a valid DateTime.");
        }

        public void SetDate(DateTime value)
        {
            string s = value.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            SetString(s);
        }

        public DateTime DateValue
        {
            get => GetDate(default);
            set => SetDate(value);
        }
        
        /// <summary>
        /// Compare only date dd:MM:yyyy
        /// </summary>
        public bool EqualsDate(DateTime otherDateValue)
        {
            var timeValue = GetTime();
            return timeValue.Day == otherDateValue.Day &&
                   timeValue.Month == otherDateValue.Month &&
                   timeValue.Year == otherDateValue.Year;
        }

        #endregion

        #region Time

        /// <summary>
        /// If empty, will return DateTime.MinValue
        /// </summary>
        public DateTime GetTime(DateTime defaultValue = default)
        {
            string s = GetString("");
            if (string.IsNullOrEmpty(s))
                return defaultValue;
            
            if (DateTime.TryParseExact(s, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result1))
                return result1;
            
            if (DateTime.TryParseExact(s, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result2))
                return result2;
            
            if (DateTime.TryParseExact(s, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result3))
                return result3;

            throw new FormatException("String was not recognized as a valid DateTime.");
        }

        public void SetTime(DateTime value)
        {
            string s = value.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            SetString(s);
        }

        public DateTime TimeValue
        {
            get => GetTime(default);
            set => SetTime(value);
        }

        /// <summary>
        /// Compare only time HH:mm:ss. The smallest unit for comparison is seconds.
        /// </summary>
        public bool EqualsTime(DateTime otherTimeValue)
        {
            var timeValue = GetTime();
            return timeValue.Hour == otherTimeValue.Hour &&
                   timeValue.Minute == otherTimeValue.Minute &&
                   timeValue.Second == otherTimeValue.Second;
        }
        
        #endregion
        
        #region Color
        
        public Color GetColor(Color defaultValue)
        {
            string s = GetString("");
            if (string.IsNullOrEmpty(s))
                return defaultValue;

            if (ColorUtility.TryParseHtmlString("#" + s, out Color color))
                return color;
            
            throw new FormatException("String was not recognized as a valid Color");
        }
        
        /// <summary>
        /// Default color is white
        /// </summary>
        public Color GetColor()
        {
            return GetColor(Color.white);
        }

        public void SetColor(Color value)
        {
            SetString(ColorUtility.ToHtmlStringRGBA(value));
        }

        public Color ColorValue
        {
            get => GetColor(Color.white);
            set => SetColor(value);
        }

        #endregion
        
        #region Vector2
        
        public Vector2 GetVector2(Vector2 defaultValue = default)
        {
            string s = GetString();
            if (string.IsNullOrEmpty(s))
                return defaultValue;

            
            var array = s.Split('_');
            Vector2 value = new Vector2();
            
            
            if(array.Length > 0)
                value.x = float.Parse(array[0], CultureInfo.InvariantCulture);
            
            if(array.Length > 1)
                value.y = float.Parse(array[1], CultureInfo.InvariantCulture);
            
            
            return value;
        }

        public void SetVector2(Vector2 value)
        {
            SetString($"{value.x.ToString(CultureInfo.InvariantCulture)}_{value.y.ToString(CultureInfo.InvariantCulture)}");
        }
        
        public Vector2 Vector2Value
        {
            get => GetVector2();
            set => SetVector2(value);
        }

        #endregion
        
        #region Vector2Int
        
        public Vector2Int GetVector2Int(Vector2Int defaultValue = default)
        {
            string s = GetString();
            if (string.IsNullOrEmpty(s))
                return defaultValue;

            
            var array = s.Split('_');
            Vector2Int value = new Vector2Int();
            
            
            if(array.Length > 0)
                value.x = ParseInt(array[0]);
            
            if(array.Length > 1)
                value.y = ParseInt(array[1]);
            
            
            return value;
        }

        public void SetVector2Int(Vector2Int value)
        {
            SetString($"{value.x.ToString(CultureInfo.InvariantCulture)}_{value.y.ToString(CultureInfo.InvariantCulture)}");
        }
        
        public Vector2Int Vector2IntValue
        {
            get => GetVector2Int();
            set => SetVector2Int(value);
        }

        #endregion
        
        #region Vector3
        
        public Vector3 GetVector3(Vector3 defaultValue = default)
        {
            string s = GetString();
            if (string.IsNullOrEmpty(s))
                return defaultValue;

            
            var array = s.Split('_');
            Vector3 value = new Vector3();
            
            
            if(array.Length > 0)
                value.x = float.Parse(array[0], CultureInfo.InvariantCulture);
            
            if(array.Length > 1)
                value.y = float.Parse(array[1], CultureInfo.InvariantCulture);
            
            if(array.Length > 2)
                value.z = float.Parse(array[2], CultureInfo.InvariantCulture);
            
            
            return value;
        }

        public void SetVector3(Vector3 value)
        {
            SetString($"{value.x.ToString(CultureInfo.InvariantCulture)}_{value.y.ToString(CultureInfo.InvariantCulture)}_{value.z.ToString(CultureInfo.InvariantCulture)}");
        }
        
        public Vector3 Vector3Value
        {
            get => GetVector3();
            set => SetVector3(value);
        }

        #endregion
        
        #region Vector3Int
        
        public Vector3Int GetVector3Int(Vector3Int defaultValue = default)
        {
            string s = GetString();
            if (string.IsNullOrEmpty(s))
                return defaultValue;

            
            var array = s.Split('_');
            Vector3Int value = new Vector3Int();
            
            
            if(array.Length > 0)
                value.x = ParseInt(array[0]);
            
            if(array.Length > 1)
                value.y = ParseInt(array[1]);
            
            if(array.Length > 2)
                value.z = ParseInt(array[2]);
            
            
            return value;
        }

        public void SetVector3Int(Vector3Int value)
        {
            SetString($"{value.x.ToString(CultureInfo.InvariantCulture)}_{value.y.ToString(CultureInfo.InvariantCulture)}_{value.z.ToString(CultureInfo.InvariantCulture)}");
        }
        
        public Vector3Int Vector3IntValue
        {
            get => GetVector3Int();
            set => SetVector3Int(value);
        }

        #endregion
    }
}