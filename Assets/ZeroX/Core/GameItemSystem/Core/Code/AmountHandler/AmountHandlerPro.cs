using System;
using System.Numerics;
using ZeroX.Extensions;
using ZeroX.JsonSystem;
using ZeroX.SaveGameSystem;

namespace GameItemSystem
{
    public static class AmountHandlerPro
    {
        //Các thao tác liên quan đến AmountNode
        
        #region Get AmountValue From AmountNode

        public static long Get_AmountLong_From_AmountNode(SGNode amountNode)
        {
            return amountNode.LongValue;
        }
        
        public static decimal Get_AmountDecimal_From_AmountNode(SGNode amountNode)
        {
            return amountNode.DecimalValue;
        }
        
        public static BigInteger Get_AmountBigInt_From_AmountNode(SGNode amountNode)
        {
            return amountNode.BigIntValue;
        }

        #endregion



        #region Set AmountValue To AmountNode

        public static void Set_AmountLong_To_AmountNode(long amountValue, SGNode amountNode, AmountType amountType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    amountNode.SetLong(amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    amountNode.SetDecimal(amountValue);
                    break;
                }
                case AmountType.BigInt:
                {
                    amountNode.SetBigInt(amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        
        public static void Set_AmountDecimal_To_AmountNode(decimal amountValue, SGNode amountNode, AmountType amountType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    amountNode.SetLong((long)amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    amountNode.SetDecimal(amountValue);
                    break;
                }
                case AmountType.BigInt:
                {
                    amountNode.SetBigInt((BigInteger)amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        
        public static void Set_AmountBigInt_To_AmountNode(BigInteger amountValue, SGNode amountNode, AmountType amountType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    amountNode.SetLong((long)amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    amountNode.SetDecimal((decimal)amountValue);
                    break;
                }
                case AmountType.BigInt:
                {
                    amountNode.SetBigInt(amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }

        #endregion



        #region Get AmountObject From AmountNode

        public static object Get_AmountObject_From_AmountNode(SGNode amountNode, AmountType amountType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    return amountNode.LongValue;
                    break;
                }
                case AmountType.Decimal:
                {
                    return amountNode.DecimalValue;
                    break;
                }
                case AmountType.BigInt:
                {
                    return amountNode.BigIntValue;
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }

        #endregion



        #region Set AmountObject To AmountNode

        public static void Set_AmountObject_To_AmountNode(object amountObj, AmountType amountType, SGNode amountNode)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    amountNode.SetLong((long)amountObj);
                    break;
                }
                case AmountType.Decimal:
                {
                    amountNode.SetDecimal((decimal)amountObj);
                    break;
                }
                case AmountType.BigInt:
                {
                    amountNode.SetBigInt((BigInteger)amountObj);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }

        #endregion

        
        #region Set AmountJsonNode To AmountData
        
        public static void Set_AmountJsonNode_To_AmountData(ZJsonScalar amountJsonNode, GameItemData gameItemData)
        {
            switch (gameItemData.AmountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    gameItemData.SetAmount(amountJsonNode.AsLong);
                    break;
                }
                case AmountType.Decimal:
                {
                    gameItemData.SetAmount(amountJsonNode.AsDecimal);
                    break;
                }
                case AmountType.BigInt:
                {
                    gameItemData.SetAmount(BigInteger.Parse(amountJsonNode.AsString));
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(gameItemData.AmountType));
                }
            }
        }
        
        #endregion


        #region AmountNode Plus AmountValue

        public static void AmountNode_Plus_AmountLong(SGNode amountNode, AmountType amountType, long amountValue)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    amountNode.SetLong(amountNode.LongValue + amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    amountNode.SetDecimal(amountNode.DecimalValue + amountValue);
                    break;
                }
                case AmountType.BigInt:
                {
                    amountNode.SetBigInt(amountNode.BigIntValue + amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        
        public static void AmountNode_Plus_AmountDecimal(SGNode amountNode, AmountType amountType, decimal amountValue)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    amountNode.SetLong(amountNode.LongValue + (long)amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    amountNode.SetDecimal(amountNode.DecimalValue + amountValue);
                    break;
                }
                case AmountType.BigInt:
                {
                    amountNode.SetBigInt(amountNode.BigIntValue + (BigInteger)amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        
        public static void AmountNode_Plus_AmountBigInt(SGNode amountNode, AmountType amountType, BigInteger amountValue)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    amountNode.SetLong(amountNode.LongValue + (long)amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    amountNode.SetDecimal(amountNode.DecimalValue + (decimal)amountValue);
                    break;
                }
                case AmountType.BigInt:
                {
                    amountNode.SetBigInt(amountNode.BigIntValue + amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }

        #endregion
        
        
        
        #region AmountNode Minus AmountValue

        public static void AmountNode_Minus_AmountLong(SGNode amountNode, AmountType amountType, long amountValue)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    amountNode.SetLong(amountNode.LongValue - amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    amountNode.SetDecimal(amountNode.DecimalValue - amountValue);
                    break;
                }
                case AmountType.BigInt:
                {
                    amountNode.SetBigInt(amountNode.BigIntValue - amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        
        public static void AmountNode_Minus_AmountDecimal(SGNode amountNode, AmountType amountType, decimal amountValue)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    amountNode.SetLong(amountNode.LongValue - (long)amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    amountNode.SetDecimal(amountNode.DecimalValue - amountValue);
                    break;
                }
                case AmountType.BigInt:
                {
                    amountNode.SetBigInt(amountNode.BigIntValue - (BigInteger)amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        
        public static void AmountNode_Minus_AmountBigInt(SGNode amountNode, AmountType amountType, BigInteger amountValue)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    amountNode.SetLong(amountNode.LongValue - (long)amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    amountNode.SetDecimal(amountNode.DecimalValue - (decimal)amountValue);
                    break;
                }
                case AmountType.BigInt:
                {
                    amountNode.SetBigInt(amountNode.BigIntValue - amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }

        #endregion



        #region AmountValue Greater AmountNode

        public static bool AmountLong_Greater_AmountNode(long amountValue, SGNode amountNode, AmountType amountType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    return amountValue > amountNode.LongValue;
                    break;
                }
                case AmountType.Decimal:
                {
                    return amountValue > amountNode.DecimalValue;
                    break;
                }
                case AmountType.BigInt:
                {
                    return amountValue > amountNode.BigIntValue;
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        
        public static bool AmountDecimal_Greater_AmountNode(decimal amountValue, SGNode amountNode, AmountType amountType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    return amountValue > amountNode.LongValue;
                    break;
                }
                case AmountType.Decimal:
                {
                    return amountValue > amountNode.DecimalValue;
                    break;
                }
                case AmountType.BigInt:
                {
                    BigInteger amountNodeBigInt = amountNode.BigIntValue;
                    if (amountNodeBigInt > (BigInteger) decimal.MaxValue) //Nếu max decimal còn nhỏ hơn amountNode thì chắc chắn amountDecimal không thể lớn hơn amountNode -> trả về false
                        return false;
                    
                    return amountValue > (decimal)amountNode.BigIntValue; //AmountNode nhỏ hơn max decimal thì có thể convert về decimal để so sánh được cả phần thập phân
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        
        public static bool AmountBigInt_Greater_AmountNode(BigInteger amountValue, SGNode amountNode, AmountType amountType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    return amountValue > amountNode.LongValue;
                    break;
                }
                case AmountType.Decimal:
                {
                    if (amountValue > (BigInteger) decimal.MaxValue) //Nếu max decimal còn nhỏ hơn amountValue thì amountNode cũng nhỏ hơn amountValue -> return true
                        return true;
                        
                    return (decimal)amountValue > amountNode.DecimalValue; //amountValue nhỏ hơn maxDecimal nên có thể convert về decimal để so sánh cả phần thập phân
                    break;
                }
                case AmountType.BigInt:
                {
                    return amountValue > amountNode.BigIntValue;
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }

        #endregion


        #region AmountNode Greater Zero

        public static bool AmountNode_Greater_Zero(SGNode amountNode, AmountType amountType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    return amountNode.LongValue > 0;
                    break;
                }
                case AmountType.Decimal:
                {
                    return amountNode.DecimalValue > 0;
                    break;
                }
                case AmountType.BigInt:
                {
                    return amountNode.BigIntValue > 0;
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }

        #endregion



        #region -------------------------------
        #endregion
        
        
        //Thao tác giữa amountValue và amountObject
        
        
        #region Get AmountValue From AmountObject

        public static long Get_AmountLong_From_AmountObject(object amountObj, AmountType amountType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    return (long)amountObj;
                    break;
                }
                case AmountType.Decimal:
                {
                    return (long)(decimal)amountObj;
                    break;
                }
                case AmountType.BigInt:
                {
                    return (long)(BigInteger)amountObj;
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        
        public static decimal Get_AmountDecimal_From_AmountObject(object amountObj, AmountType amountType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    return (decimal)(long)amountObj;
                    break;
                }
                case AmountType.Decimal:
                {
                    return (decimal)amountObj;
                    break;
                }
                case AmountType.BigInt:
                {
                    return (decimal)(BigInteger)amountObj;
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }

        public static BigInteger Get_AmountBigInt_From_AmountObject(object amountObj, AmountType amountType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    return (BigInteger)(long)amountObj;
                    break;
                }
                case AmountType.Decimal:
                {
                    return (BigInteger)(decimal)amountObj;
                    break;
                }
                case AmountType.BigInt:
                {
                    return (BigInteger)amountObj;
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        
        #endregion



        #region Set AmountValue To AmountObject

        public static void Set_AmountLong_To_AmountObject(long amountValue, ref object amountObj, AmountType amountType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    amountObj = amountValue;
                    break;
                }
                case AmountType.Decimal:
                {
                    amountObj = (decimal)amountValue;
                    break;
                }
                case AmountType.BigInt:
                {
                    amountObj = (BigInteger)amountValue;
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        
        public static void Set_AmountDecimal_To_AmountObject(decimal amountValue, ref object amountObj, AmountType amountType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    amountObj = (long)amountValue;
                    break;
                }
                case AmountType.Decimal:
                {
                    amountObj = amountValue;
                    break;
                }
                case AmountType.BigInt:
                {
                    amountObj = (BigInteger)amountValue;
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        
        public static void Set_AmountBigInt_To_AmountObject(BigInteger amountValue, ref object amountObj, AmountType amountType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    amountObj = (long)amountValue;
                    break;
                }
                case AmountType.Decimal:
                {
                    amountObj = (decimal)amountValue;
                    break;
                }
                case AmountType.BigInt:
                {
                    amountObj = amountValue;
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }

        #endregion



        #region Set AmountValue Default To AmountObject

        public static void Set_AmountValue_Zero_To_AmountObject(ref object amountObj, AmountType amountType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    amountObj = 0L;
                    break;
                }
                case AmountType.Decimal:
                {
                    amountObj = 0M;
                    break;
                }
                case AmountType.BigInt:
                {
                    amountObj = (BigInteger)0;
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        public static void Set_AmountValue_One_To_AmountObject(ref object amountObj, AmountType amountType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    amountObj = 1L;
                    break;
                }
                case AmountType.Decimal:
                {
                    amountObj = 1M;
                    break;
                }
                case AmountType.BigInt:
                {
                    amountObj = (BigInteger)1;
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }

        #endregion

        
        #region AmountObject Greater Zero

        public static bool AmountObject_Greater_Zero(object amountObj, AmountType amountType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    return (long)amountObj > 0;
                    break;
                }
                case AmountType.Decimal:
                {
                    return (decimal)amountObj > 0;
                    break;
                }
                case AmountType.BigInt:
                {
                    return (BigInteger)amountObj > 0;
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }

        #endregion


        #region AmountObject Plus AmountValue

        public static object AmountObject_Plus_AmountLong(object amountObj, AmountType amountType, long amountValue)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    long amountObjectCasted = (long) amountObj;
                    return (long)(amountObjectCasted + amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    decimal amountObjectCasted = (decimal) amountObj;
                    return (decimal)(amountObjectCasted + amountValue);
                    break;
                }
                case AmountType.BigInt:
                {
                    BigInteger amountObjectCasted = (BigInteger) amountObj;
                    return (BigInteger)(amountObjectCasted + amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        
        public static object AmountObject_Plus_AmountDecimal(object amountObj, AmountType amountType, decimal amountValue)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    long amountObjectCasted = (long) amountObj;
                    return (long)(amountObjectCasted + amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    decimal amountObjectCasted = (decimal) amountObj;
                    return (decimal)(amountObjectCasted + amountValue);
                    break;
                }
                case AmountType.BigInt:
                {
                    BigInteger amountObjectCasted = (BigInteger) amountObj;
                    return (BigInteger)(amountObjectCasted + (BigInteger)amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        
        public static object AmountObject_Plus_AmountBigInt(object amountObj, AmountType amountType, BigInteger amountValue)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    long amountObjectCasted = (long) amountObj;
                    return (long)(amountObjectCasted + amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    decimal amountObjectCasted = (decimal) amountObj;
                    return (decimal)(amountObjectCasted + (decimal)amountValue);
                    break;
                }
                case AmountType.BigInt:
                {
                    BigInteger amountObjectCasted = (BigInteger) amountObj;
                    return (BigInteger)(amountObjectCasted + amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }

        #endregion
        
        
        
        #region AmountObject Minus AmountValue

        public static object AmountObject_Minus_AmountLong(object amountObj, AmountType amountType, long amountValue)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    long amountObjectCasted = (long) amountObj;
                    return (long)(amountObjectCasted - amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    decimal amountObjectCasted = (decimal) amountObj;
                    return (decimal)(amountObjectCasted - amountValue);
                    break;
                }
                case AmountType.BigInt:
                {
                    BigInteger amountObjectCasted = (BigInteger) amountObj;
                    return (BigInteger)(amountObjectCasted - amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        
        public static object AmountObject_Minus_AmountDecimal(object amountObj, AmountType amountType, decimal amountValue)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    long amountObjectCasted = (long) amountObj;
                    return (long)(amountObjectCasted - amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    decimal amountObjectCasted = (decimal) amountObj;
                    return (decimal)(amountObjectCasted - amountValue);
                    break;
                }
                case AmountType.BigInt:
                {
                    BigInteger amountObjectCasted = (BigInteger) amountObj;
                    return (BigInteger)(amountObjectCasted - (BigInteger)amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        
        public static object AmountObject_Minus_AmountBigInt(object amountObj, AmountType amountType, BigInteger amountValue)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    long amountObjectCasted = (long) amountObj;
                    return (long)(amountObjectCasted - amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    decimal amountObjectCasted = (decimal) amountObj;
                    return (decimal)(amountObjectCasted - (decimal)amountValue);
                    break;
                }
                case AmountType.BigInt:
                {
                    BigInteger amountObjectCasted = (BigInteger) amountObj;
                    return (BigInteger)(amountObjectCasted - amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }

        #endregion

        
        
        #region Long Multi

        public static long Long_Multi_Long(long a, long multiplier)
        {
            return a * multiplier;
        }
        
        public static long Long_Multi_Decimal(long a, decimal multiplier, RoundType roundType = RoundType.Default)
        {
            if (roundType == RoundType.Default)
            {
                return (long)Math.Round(a * multiplier, 0);
            }
            
            if (roundType == RoundType.Down)
            {
                return (long)Math.Floor(a * multiplier);
            }
            
            if (roundType == RoundType.Up)
            {
                return (long)Math.Ceiling(a * multiplier);
            }
            
            
            throw new Exception(ErrorMessageUtility.NotSupportRoundType(roundType));
        }
        
        public static long Long_Multi_BigInt(long a, BigInteger multiplier)
        {
            return (long) (a * multiplier);
        }

        #endregion
        
        
        
        #region Decimal Multi

        public static decimal Decimal_Multi_Long(decimal a, long multiplier, RoundType roundType = RoundType.Default)
        {
            if (roundType == RoundType.Default)
            {
                return (decimal)(a * multiplier);
            }
            
            if (roundType == RoundType.Down)
            {
                return (decimal)Math.Floor(a * multiplier);
            }
            
            if (roundType == RoundType.Up)
            {
                return (decimal)Math.Ceiling(a * multiplier);
            }
            
            
            throw new Exception(ErrorMessageUtility.NotSupportRoundType(roundType));
        }

        public static decimal Decimal_Multi_Decimal(decimal a, decimal multiplier, RoundType roundType = RoundType.Default)
        {
            if (roundType == RoundType.Default)
            {
                return (decimal)a * multiplier;
            }
            
            if (roundType == RoundType.Down)
            {
                return (decimal)Math.Floor(a * multiplier);
            }
            
            if (roundType == RoundType.Up)
            {
                return (decimal)Math.Ceiling(a * multiplier);
            }
            
            
            throw new Exception(ErrorMessageUtility.NotSupportRoundType(roundType));
        }
        
        public static decimal Decimal_Multi_BigInt(decimal a, BigInteger multiplier, RoundType roundType = RoundType.Default)
        {
            if (roundType == RoundType.Default)
            {
                return (decimal)(a * (decimal)multiplier);
            }
            
            if (roundType == RoundType.Down)
            {
                return (decimal)Math.Floor(a * (decimal)multiplier);
            }
            
            if (roundType == RoundType.Up)
            {
                return (decimal)Math.Ceiling(a * (decimal)multiplier);
            }
            
            
            throw new Exception(ErrorMessageUtility.NotSupportRoundType(roundType));
        }
        
        #endregion



        #region BigInt Multi

        public static BigInteger BigInt_Multi_Long(BigInteger a, long multiplier)
        {
            return BigInteger.Multiply(a, multiplier);
        }
        
        public static BigInteger BigInt_Multi_Decimal(BigInteger a, decimal multiplier, RoundType roundType = RoundType.Default)
        {
            if (roundType == RoundType.Default)
            {
                return (BigInteger) a.MultiplyRound(multiplier);
            }
            
            if (roundType == RoundType.Down)
            {
                return (BigInteger) a.MultiplyFloor(multiplier);
            }
            
            if (roundType == RoundType.Up)
            {
                return (BigInteger) a.MultiplyCeil(multiplier);
            }
            
            
            throw new Exception(ErrorMessageUtility.NotSupportRoundType(roundType));
        }

        public static BigInteger BigInt_Multi_BigInt(BigInteger a, BigInteger multiplier)
        {
            return a * multiplier;
        }
        
        #endregion



        #region Long Divide

        public static long Long_Divide_Long(long a, long divisor, RoundType roundType = RoundType.Default)
        {
            if (roundType == RoundType.Default)
            {
                return (long) (Math.Round(a / (decimal) divisor, 0));
            }
            
            if (roundType == RoundType.Down)
            {
                return (long) (Math.Floor(a / (decimal) divisor));
            }
            
            if (roundType == RoundType.Up)
            {
                return (long) (Math.Ceiling(a / (decimal) divisor));
            }
            
            throw new Exception(ErrorMessageUtility.NotSupportRoundType(roundType));
        }

        public static long Long_Divide_Decimal(long a, decimal divisor, RoundType roundType = RoundType.Default)
        {
            if (roundType == RoundType.Default)
            {
                return (long) (Math.Round(a / divisor, 0));
            }
            
            if (roundType == RoundType.Down)
            {
                return (long) (Math.Floor(a / divisor));
            }
            
            if (roundType == RoundType.Up)
            {
                return (long) (Math.Ceiling(a / divisor));
            }
            
            throw new Exception(ErrorMessageUtility.NotSupportRoundType(roundType));
        }

        public static long Long_Divide_BigInt(long a, BigInteger divisor, RoundType roundType = RoundType.Default)
        {
            if (roundType == RoundType.Default)
            {
                return (long) (Math.Round(a / (decimal)divisor, 0));
            }
            
            if (roundType == RoundType.Down)
            {
                return (long) (Math.Floor(a / (decimal)divisor));
            }
            
            if (roundType == RoundType.Up)
            {
                return (long) (Math.Ceiling(a / (decimal)divisor));
            }
            
            throw new Exception(ErrorMessageUtility.NotSupportRoundType(roundType));
        }
        
        #endregion

        
        
        #region Decimal Divide

        public static decimal Decimal_Divide_Long(decimal a, long divisor, RoundType roundType = RoundType.Default)
        {
            if (roundType == RoundType.Default)
            {
                return (decimal)(a / divisor);
            }
            
            if (roundType == RoundType.Down)
            { 
                return (decimal)Math.Floor(a / divisor);
            }
            
            if (roundType == RoundType.Up)
            {
                return (decimal)Math.Ceiling(a / divisor);
            }
            
            
            throw new Exception(ErrorMessageUtility.NotSupportRoundType(roundType));
        }
        
        public static decimal Decimal_Divide_Decimal(decimal a, decimal divisor, RoundType roundType = RoundType.Default)
        {
            if (roundType == RoundType.Default)
            {
                return (decimal)a / divisor;
            }
            
            if (roundType == RoundType.Down)
            {
                return (decimal)Math.Floor(a / divisor);
            }
            
            if (roundType == RoundType.Up)
            {
                return (decimal)Math.Ceiling(a / divisor);
            }
            
            
            throw new Exception(ErrorMessageUtility.NotSupportRoundType(roundType));
        }

        public static decimal Decimal_Divide_BigInt(decimal a, BigInteger divisor, RoundType roundType = RoundType.Default)
        {
            if (roundType == RoundType.Default)
            {
                return (decimal)(a / (decimal)divisor);
            }
            
            if (roundType == RoundType.Down)
            {
                return (decimal)Math.Floor(a / (decimal)divisor);
            }
            
            if (roundType == RoundType.Up)
            {
                return (decimal)Math.Ceiling(a / (decimal)divisor);
            }
            
            
            throw new Exception(ErrorMessageUtility.NotSupportRoundType(roundType));
        }
        
        #endregion
        
        
        
        #region BigInt Divide

        public static BigInteger BigInt_Divide_Long(BigInteger a, long divisor, RoundType roundType = RoundType.Default)
        {
            if (roundType == RoundType.Default)
            {
                return a.DivideRound((BigInteger)divisor);
            }
            
            if (roundType == RoundType.Down)
            {
                return a.DivideFloor((BigInteger)divisor);
            }
            
            if (roundType == RoundType.Up)
            {
                return a.DivideCeil((BigInteger)divisor);
            }
            
            throw new Exception(ErrorMessageUtility.NotSupportRoundType(roundType));
        }
        
        public static BigInteger BigInt_Divide_Decimal(BigInteger a, decimal divisor, RoundType roundType = RoundType.Default)
        {
            if (roundType == RoundType.Default)
            {
                return a.DivideRound(divisor);
            }
            
            if (roundType == RoundType.Down)
            {
                return a.DivideFloor(divisor);
            }
            
            if (roundType == RoundType.Up)
            {
                return a.DivideCeil(divisor);
            }
            
            
            throw new Exception(ErrorMessageUtility.NotSupportRoundType(roundType));
        }

        public static BigInteger BigInt_Divide_BigInt(BigInteger a, BigInteger divisor, RoundType roundType = RoundType.Default)
        {
            if (roundType == RoundType.Default)
            {
                return a.DivideRound(divisor);
            }
            
            if (roundType == RoundType.Down)
            {
                return a.DivideFloor(divisor);
            }
            
            if (roundType == RoundType.Up)
            {
                return a.DivideCeil(divisor);
            }
            
            
            throw new Exception(ErrorMessageUtility.NotSupportRoundType(roundType));
        }
        
        #endregion
        

        #region AmountObject Multi AmountValue

        public static object AmountObject_Multi_AmountLong(object amountObj, AmountType amountType, long amountValue, RoundType roundType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    return Long_Multi_Long((long) amountObj, amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    return Decimal_Multi_Long((decimal) amountObj, amountValue, roundType);
                    break;
                }
                case AmountType.BigInt:
                {
                    return BigInt_Multi_Long((BigInteger) amountObj, amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        
        public static object AmountObject_Multi_AmountDecimal(object amountObj, AmountType amountType, decimal amountValue, RoundType roundType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    return Long_Multi_Decimal((long) amountObj, amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    return Decimal_Multi_Decimal((decimal) amountObj, amountValue, roundType);
                    break;
                }
                case AmountType.BigInt:
                {
                    return BigInt_Multi_Decimal((BigInteger) amountObj, amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        
        public static object AmountObject_Multi_AmountBigInt(object amountObj, AmountType amountType, BigInteger amountValue, RoundType roundType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    return Long_Multi_BigInt((long) amountObj, amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    return Decimal_Multi_BigInt((decimal) amountObj, amountValue, roundType);
                    break;
                }
                case AmountType.BigInt:
                {
                    return BigInt_Multi_BigInt((BigInteger) amountObj, amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }

        #endregion
        
        
        
        #region AmountObject Divide AmountValue

        public static object AmountObject_Divide_AmountLong(object amountObj, AmountType amountType, long amountValue, RoundType roundType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    return Long_Divide_Long((long) amountObj, amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    return Decimal_Divide_Long((decimal) amountObj, amountValue, roundType);
                    break;
                }
                case AmountType.BigInt:
                {
                    return BigInt_Divide_Long((BigInteger) amountObj, amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        
        public static object AmountObject_Divide_AmountDecimal(object amountObj, AmountType amountType, decimal amountValue, RoundType roundType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    return Long_Divide_Decimal((long) amountObj, amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    return Decimal_Divide_Decimal((decimal) amountObj, amountValue, roundType);
                    break;
                }
                case AmountType.BigInt:
                {
                    return BigInt_Divide_Decimal((BigInteger) amountObj, amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }
        
        public static object AmountObject_Divide_AmountBigInt(object amountObj, AmountType amountType, BigInteger amountValue, RoundType roundType)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    return Long_Divide_BigInt((long) amountObj, amountValue);
                    break;
                }
                case AmountType.Decimal:
                {
                    return Decimal_Divide_BigInt((decimal) amountObj, amountValue, roundType);
                    break;
                }
                case AmountType.BigInt:
                {
                    return BigInt_Divide_BigInt((BigInteger) amountObj, amountValue);
                    break;
                }
                default:
                {
                    throw new Exception(ErrorMessageUtility.NotSupportAmountType(amountType));
                }
            }
        }

        #endregion
    }
}