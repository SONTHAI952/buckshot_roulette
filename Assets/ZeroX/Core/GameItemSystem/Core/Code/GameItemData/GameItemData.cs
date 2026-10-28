using System.IO;
using System.Numerics;
using UnityEngine;

namespace GameItemSystem
{
    public partial class GameItemData
    {
        public string id;
        public string nftId;
        private object amountObj;
        private readonly AmountType amountType = AmountType.Undefined;

        public object AmountObj => amountObj;
        public AmountType AmountType => amountType;
        
        
        
        //Icon
        public Sprite OriginalIcon => GameItemTable.GetIcon(id);
        public Sprite Icon => OverrideIcon == null ? GameItemTable.GetIcon(id) : OverrideIcon;


        #region Override Icon

        public const string DEFAULT_OVERRIDE_ICON_FOLDER = "Database/Game Item/Override Icon/";
        
        /// <summary>
        /// Nếu overrideIconParentPath null, thì sẽ sử dụng đường dẫn parent mặc định
        /// </summary>
        public string overrideIconParentPath;
        public string overrideIconPath;
        private Sprite overrideIconCached;
        private bool loadedOverrideIcon;
        
        public const string fn_overrideIconFolder = "overrideIconFolder";
        public const string fn_overrideIcon = "overrideIcon";

        public Sprite OverrideIcon
        {
            get
            {
                if (loadedOverrideIcon == false)
                {
                    loadedOverrideIcon = true;
                    
                    if (string.IsNullOrEmpty(overrideIconPath))
                    {
                        return null;
                    }
                    
                    if (string.IsNullOrEmpty(overrideIconParentPath))
                    {
                        overrideIconCached = Resources.Load<Sprite>(DEFAULT_OVERRIDE_ICON_FOLDER + overrideIconPath);
                        if (overrideIconCached == null)
                        {
                            Debug.LogError("Không tìm thấy overrideIcon ở thư mục: " + DEFAULT_OVERRIDE_ICON_FOLDER + overrideIconPath);
                        }
                    }
                    else
                    {
                        overrideIconCached = Resources.Load<Sprite>(Path.Combine(overrideIconParentPath, overrideIconPath));
                        if (overrideIconCached == null)
                        {
                            Debug.LogError("Không tìm thấy overrideIcon ở thư mục: " + Path.Combine(overrideIconParentPath, overrideIconPath));
                        }
                    }
                }

                return overrideIconCached;
            }
        }
        
        #endregion
        
        

        
        #region Constructor

        /// <summary>
        /// Amount sẽ mặc định là 1
        /// </summary>
        public GameItemData(AmountType amountType)
        {
            this.amountType = amountType;
            AmountHandlerPro.Set_AmountValue_One_To_AmountObject(ref amountObj, amountType);
        }
        
        /// <summary>
        /// Amount sẽ mặc định là 1
        /// </summary>
        public GameItemData(AmountType amountType, string id)
        {
            this.amountType = amountType;
            this.id = id;
            AmountHandlerPro.Set_AmountValue_One_To_AmountObject(ref amountObj, amountType);
        }

        /// <summary>
        /// Amount sẽ mặc định là 1
        /// </summary>
        public GameItemData(string id)
        {
            this.id = id;
            this.amountType = GameItemTable.GetAmountType(id);
            AmountHandlerPro.Set_AmountValue_One_To_AmountObject(ref amountObj, amountType);
        }

        #endregion

        
        

        #region Amount
        
        /// <summary>
        /// Mặc định chung là long
        /// </summary>
        public long Amount => AmountHandlerPro.Get_AmountLong_From_AmountObject(amountObj, amountType);

        public long AmountLong => AmountHandlerPro.Get_AmountLong_From_AmountObject(amountObj, amountType);

        public decimal AmountDecimal => AmountHandlerPro.Get_AmountDecimal_From_AmountObject(amountObj, amountType);
        
        public BigInteger AmountBigInt => AmountHandlerPro.Get_AmountBigInt_From_AmountObject(amountObj, amountType);

        #endregion
        

        #region Set Amount

        public void SetAmount(long value)
        {
            AmountHandlerPro.Set_AmountLong_To_AmountObject(value, ref amountObj, amountType);
        }
        
        public void SetAmount(decimal value)
        {
            AmountHandlerPro.Set_AmountDecimal_To_AmountObject(value, ref amountObj, amountType);
        }

        public void SetAmount(BigInteger value)
        {
            AmountHandlerPro.Set_AmountBigInt_To_AmountObject(value, ref amountObj, amountType);
        }

        #endregion


        #region Plus Amount

        public void PlusAmount(long value)
        {
            amountObj = AmountHandlerPro.AmountObject_Plus_AmountLong(amountObj, amountType, value);
        }
        
        public void PlusAmount(decimal value)
        {
            amountObj = AmountHandlerPro.AmountObject_Plus_AmountDecimal(amountObj, amountType, value);
        }
        
        public void PlusAmount(BigInteger value)
        {
            amountObj = AmountHandlerPro.AmountObject_Plus_AmountBigInt(amountObj, amountType, value);
        }

        #endregion


        #region Minus Amount

        public void MinusAmount(long value)
        {
            amountObj = AmountHandlerPro.AmountObject_Minus_AmountLong(amountObj, amountType, value);
        }
        
        public void MinusAmount(decimal value)
        {
            amountObj = AmountHandlerPro.AmountObject_Minus_AmountDecimal(amountObj, amountType, value);
        }
        
        public void MinusAmount(BigInteger value)
        {
            amountObj = AmountHandlerPro.AmountObject_Minus_AmountBigInt(amountObj, amountType, value);
        }

        #endregion
        
        
        #region Multi Amount

        public void MultiAmount(long multiplier, RoundType roundType = RoundType.Default)
        {
            amountObj = AmountHandlerPro.AmountObject_Multi_AmountLong(amountObj, amountType, multiplier, roundType);
        }

        public void MultiAmount(decimal multiplier, RoundType roundType = RoundType.Default)
        {
            amountObj = AmountHandlerPro.AmountObject_Multi_AmountDecimal(amountObj, amountType, multiplier, roundType);
        }
        
        public void MultiAmount(BigInteger multiplier, RoundType roundType = RoundType.Default)
        {
            amountObj = AmountHandlerPro.AmountObject_Multi_AmountBigInt(amountObj, amountType, multiplier, roundType);
        }
        
        #endregion


        #region Divide Amount

        public void DivideAmount(long divisor, RoundType roundType = RoundType.Default)
        {
            amountObj = AmountHandlerPro.AmountObject_Divide_AmountLong(amountObj, amountType, divisor, roundType);
        }
        
        public void DivideAmount(decimal divisor, RoundType roundType = RoundType.Default)
        {
            amountObj = AmountHandlerPro.AmountObject_Divide_AmountDecimal(amountObj, amountType, divisor, roundType);
        }

        public void DivideAmount(BigInteger divisor, RoundType roundType = RoundType.Default)
        {
            amountObj = AmountHandlerPro.AmountObject_Divide_AmountBigInt(amountObj, amountType, divisor, roundType);
        }
        
        #endregion



        public virtual GameItemData Clone()
        {
            var oic = OverrideIcon;//Load overrideIcon trước rồi mới clone để tránh phải load lại
            
            
            
            GameItemData gameItemData = new GameItemData(amountType, id);
            gameItemData.nftId = nftId;
            gameItemData.amountObj = amountObj;
            
            //overrideIcon
            gameItemData.overrideIconParentPath = overrideIconParentPath;
            gameItemData.overrideIconPath = overrideIconPath;
            gameItemData.overrideIconCached = overrideIconCached;
            
            return gameItemData;
        }
    }
}