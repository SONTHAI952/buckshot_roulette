using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using ZeroX.RxSystem;
using ZeroX.SaveGameSystem;

namespace GameItemSystem
{
    public static class InventoryDI
    {
        private const string inventory_key = "inventory";

        
        public static SGNode InventoryNode => SaveGame.Default[inventory_key];

        
        public static Subject<OnGameItemChangedData> OnGameItemChanged = new Subject<OnGameItemChangedData>();


        private static readonly DictSingletonLookUp dictSingletonLookUp = new DictSingletonLookUp();


        #region Event

        public static void EmitOnGameItemChanged(string id, string nftId, GameItemChangedType changedType, GameItemData oldGameItemData, GameItemData newGameItemData, string where, string reason)
        {
            OnGameItemChangedData data = new OnGameItemChangedData();
            data.id = id;
            data.nftId = nftId;
            data.changedType = changedType;
            data.oldGameItemData = oldGameItemData;
            data.newGameItemData = newGameItemData;
            
            data.parameters = new InventoryParameters(where, reason);
            
            OnGameItemChanged.Emit(data);
        }

        #endregion
        
        

        #region Add - Remove - Save Changes
        

        /// <summary>
        /// Return Nft Id
        /// </summary>
        public static string Add(GameItemData gameItemData, string where, string reason)
        {
            if (AmountHandlerPro.AmountObject_Greater_Zero(gameItemData.AmountObj, gameItemData.AmountType) == false)
            {
                Debug.LogError("Cannot add game item data with amount == 0");
                return null;
            }
            
            
            var gameItemRow = GameItemTable.GetRowByIdWithLog(gameItemData.id);
            
            
            
            if (gameItemRow.IsSingleton)
            {
                if (dictSingletonLookUp.ContainsId(gameItemData.id)) //Nếu là singleton và đã đã có id trong inventory thì tăng số lượng thôi
                {
                    PlusAmount(gameItemData.id, gameItemData.AmountObj, gameItemData.AmountType, where, reason);
                    return dictSingletonLookUp.Get_NftId_By_Id(gameItemData.id);
                }
            }


            //Create new
            SGNode gameItemNode = InventoryNode.AddChild();
            
            
            //Essential data
            gameItemNode[GameItemDataField.id].StringValue = gameItemData.id;
            AmountHandlerPro.Set_AmountObject_To_AmountNode(gameItemData.AmountObj, gameItemData.AmountType, gameItemNode[GameItemDataField.amount]);
            


            //Specific data
            gameItemRow.DataHandler.GameItemData_To_GameItemNode(gameItemData, gameItemNode);
            
            
            //Dict Singleton Look Up
            if(gameItemRow.IsSingleton)
                dictSingletonLookUp.Set(gameItemData.id, gameItemNode.Key);
            
            //Event
            GameItemData newGameItemData = GameItemNode_To_GameItemData(gameItemNode);
            EmitOnGameItemChanged(gameItemData.id, gameItemNode.Key, GameItemChangedType.Add, null, newGameItemData, where, reason);
            
            //Result
            return gameItemNode.Key;
        }
        
        
        public static void Remove(string nftId, string where, string reason)
        {
            if(InventoryNode.TryGetChild(nftId, out var gameItemNode) == false)
                return;

            string id = gameItemNode[GameItemDataField.id].StringValue;
            GameItemData oldGameItemData = GameItemNode_To_GameItemData(gameItemNode);
            
            
            InventoryNode.DeleteChild(nftId);
            
            //Dict Singleton Look Up
            dictSingletonLookUp.RemoveByNftId(nftId);
            
            
            //Event
            EmitOnGameItemChanged(id, nftId, GameItemChangedType.Remove, oldGameItemData, null, where, reason);
        }
        
        
        /// <summary>
        /// Sẽ dựa vào nftId để truy cập chính xác node cần lưu. Tuy nhiên id sẽ giữ nguyên
        /// </summary>
        public static bool SaveChanges(GameItemData gameItemData, bool alsoOverrideAmount, string where, string reason)
        {
            //Nếu tồn tại trong inventory mới lưu
            if (InventoryNode.TryGetChild(gameItemData.nftId, out SGNode gameItemNode) == false)
            {
                Debug.LogError("Cannot save changes because game item not exist in Inventory: " + gameItemData.nftId);
                return false;
            }

            
            var handler = GameItemTable.GetDataHandler(gameItemData.id);
            GameItemData oldGameItemData = GameItemNode_To_GameItemData(gameItemNode);
            
            
            //Amount
            if(alsoOverrideAmount)
                AmountHandlerPro.Set_AmountObject_To_AmountNode(gameItemData.AmountObj, gameItemData.AmountType, gameItemNode[GameItemDataField.amount]);
            
            
            //Specific data
            handler.GameItemData_To_GameItemNode(gameItemData, gameItemNode);
            
            
            //Event
            GameItemData newGameItemData = GameItemNode_To_GameItemData(gameItemNode);
            EmitOnGameItemChanged(gameItemData.id, gameItemData.nftId, GameItemChangedType.SaveChanges, oldGameItemData, newGameItemData, where, reason);
            
            
            //Result
            return true;
        }
        

        #endregion


        #region Add Unpack

        /// <summary>
        /// Unpack and add. Return list added
        /// </summary>
        public static List<GameItemData> AddUnpack(GameItemData data, string where, string reason)
        {
            var gameItemRow = GameItemTable.GetRowByIdWithLog(data.id);

            if (gameItemRow.IsNested == false)
            {
                Add(data, where, reason);
                return new List<GameItemData>(){data};
            }

            List<GameItemData> listUnpacked = gameItemRow.DataHandler.Unpack(data);
            foreach (var d in listUnpacked)
            {
                Add(d, where, reason);
            }
            
            return listUnpacked;
        }
        
        /// <summary>
        /// Unpack until no more nested and add. Return list added
        /// </summary>
        public static List<GameItemData> AddUnpackComplete(GameItemData data, string where, string reason)
        {
            var row = GameItemTable.GetRowByIdWithLog(data.id);

            if (row.IsNested == false)
            {
                Add(data, where, reason);
                return new List<GameItemData>(){data};
            }

            List<GameItemData> listUnpacked = row.DataHandler.UnpackComplete(data);
            foreach (var d in listUnpacked)
            {
                Add(d, where, reason);
            }
            
            return listUnpacked;
        }

        #endregion


        #region Add Bonus

        /// <summary>
        /// Amount sẽ mặc định là 1
        /// </summary>
        public static GameItemData Add(string id, string where, string reason)
        {
            GameItemData gameItemData = new GameItemData(id);
            gameItemData.SetAmount(1);
            Add(gameItemData, where, reason);

            return gameItemData;
        }
        
        public static GameItemData Add(string id, long amount, string where, string reason)
        {
            GameItemData gameItemData = new GameItemData(id);
            gameItemData.SetAmount(amount);
            Add(gameItemData, where, reason);

            return gameItemData;
        }
        
        public static GameItemData Add(string id, decimal amount, string where, string reason)
        {
            GameItemData gameItemData = new GameItemData(id);
            gameItemData.SetAmount(amount);
            Add(gameItemData, where, reason);

            return gameItemData;
        }
        
        public static GameItemData Add(string id, BigInteger amount, string where, string reason)
        {
            GameItemData gameItemData = new GameItemData(id);
            gameItemData.SetAmount(amount);
            Add(gameItemData, where, reason);

            return gameItemData;
        }

        public static List<string> AddList(List<GameItemData> listGameItemData, string where, string reason)
        {
            List<string> listNftId = new List<string>();
            
            foreach (var gameItemData in listGameItemData)
            {
                listNftId.Add(Add(gameItemData, where, reason));
            }

            return listNftId;
        }

        #endregion

        
        #region Get

        private static GameItemData GameItemNode_To_GameItemData(SGNode gameItemNode)
        {
            var handler = GameItemTable.GetDataHandler(gameItemNode[GameItemDataField.id].StringValue);
            
            
            //Essential data
            GameItemData gameItemData = handler.CreateGameItemDataInstance();
            gameItemData.nftId = gameItemNode.Key;
            GameItemData.SetAmountObject(gameItemData, AmountHandlerPro.Get_AmountObject_From_AmountNode(gameItemNode[GameItemDataField.amount], gameItemData.AmountType));


            //Specific data
            handler.GameItemNode_To_GameItemData(gameItemNode, gameItemData);

            return gameItemData;
        }

        /// <summary>
        /// Có bao gồm cả amount
        /// </summary>
        public static GameItemData GetDataByNftId(string nftId)
        {
            //Nếu tồn tại trong inventory mới get
            if (InventoryNode.TryGetChild(nftId, out SGNode gameItemNode) == false)
            {
                return null;
            }


            return GameItemNode_To_GameItemData(gameItemNode);
        }

        public static T GetDataByNftId<T>(string nftId) where T : GameItemData
        {
            //Nếu tồn tại trong inventory mới get
            if (InventoryNode.TryGetChild(nftId, out SGNode gameItemNode) == false)
            {
                return null;
            }


            var gameItemData = GameItemNode_To_GameItemData(gameItemNode);
            return (T) gameItemData;
        }

        public static GameItemData GetDataById(string id)
        {
            string nftId = dictSingletonLookUp.Get_NftId_By_Id(id);
            if (string.IsNullOrEmpty(nftId))
            {
                return null;
            }


            //Nếu tồn tại trong inventory mới get
            if (InventoryNode.TryGetChild(nftId, out SGNode gameItemNode) == false)
            {
                return null;
            }


            return GameItemNode_To_GameItemData(gameItemNode);
        }
        
        public static T GetDataById<T>(string id) where T : GameItemData
        {
            string nftId = dictSingletonLookUp.Get_NftId_By_Id(id);
            if (string.IsNullOrEmpty(nftId))
            {
                return null;
            }


            //Nếu tồn tại trong inventory mới get
            if (InventoryNode.TryGetChild(nftId, out SGNode gameItemNode) == false)
            {
                return null;
            }


            return (T)GameItemNode_To_GameItemData(gameItemNode);
        }

        public static List<GameItemData> GetListDataByType(GameItemType gameItemType)
        {
            List<GameItemData> listGameItemData = new List<GameItemData>();
            
            
            foreach (var gameItemNode in InventoryNode)
            {
                string id = gameItemNode[GameItemDataField.id].StringValue;
                if(GameItemTable.GetType(id) != gameItemType)
                    continue;
                
                GameItemData gameItemData = GameItemNode_To_GameItemData(gameItemNode);
                listGameItemData.Add(gameItemData);
            }

            return listGameItemData;
        }

        #endregion


        #region Check

        /// <summary>
        /// True khi trong Inventory có item với id truyền vào và amount > 0
        /// </summary>
        public static bool HasInInventory(string id)
        {
            if (GameItemTable.IsSingleton(id))
            {
                string nftId = dictSingletonLookUp.Get_NftId_By_Id(id);
                if (string.IsNullOrEmpty(nftId))
                    return false;

                var gameItemRow = GameItemTable.GetRowByIdWithLog(id);
                return AmountHandlerPro.AmountNode_Greater_Zero(InventoryNode[nftId][GameItemDataField.amount], gameItemRow.AmountType);
            }


            foreach (var gameItemNode in InventoryNode)
            {
                if (gameItemNode[GameItemDataField.id].StringValue != id)
                    continue;
                
                var gameItemRow = GameItemTable.GetRowByIdWithLog(id);
                if (AmountHandlerPro.AmountNode_Greater_Zero(gameItemNode[GameItemDataField.amount], gameItemRow.AmountType))
                    return true;
            }

            return false;
        }
        
        public static bool HasGameItemWithTypeInInventory(GameItemType gameItemType)
        {
            foreach (var gameItemNode in InventoryNode)
            {
                GameItemRow gameItemRow = GameItemTable.GetRowByIdWithLog(gameItemNode[GameItemDataField.id].StringValue);
                
                if (gameItemRow.Type != gameItemType)
                    continue;
                
                if (AmountHandlerPro.AmountNode_Greater_Zero(gameItemNode[GameItemDataField.amount], gameItemRow.AmountType))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// True khi trong Inventory có item với id truyền vào. Khác với HasInInventory, hàm này không quan tâm đến amount
        /// </summary>
        public static bool ContainsId(string id)
        {
            if (GameItemTable.IsSingleton(id))
            {
                return dictSingletonLookUp.ContainsId(id);
            }


            foreach (var gameItemNode in InventoryNode)
            {
                if (gameItemNode[GameItemDataField.id].StringValue == id)
                    return true;
            }

            return false;
        }
        
        #endregion


        #region Get Amount

        public static long GetAmount(string id)
        {
            return GetAmountLong(id);
        }

        public static long GetAmountLong(string id)
        {
            //Singleton
            if (GameItemTable.IsSingleton(id))
            {
                string nftId = dictSingletonLookUp.Get_NftId_By_Id(id);
                if (string.IsNullOrEmpty(nftId))
                    return 0L;

                return InventoryNode[nftId][GameItemDataField.amount].LongValue;
            }
            else
            {
                //Non-Singleton - Total Amount
                long totalAmount = 0;
                foreach (var gameItemNode in InventoryNode)
                {
                    if(gameItemNode[GameItemDataField.id].StringValue != id)
                        continue;
                
                    totalAmount += gameItemNode[GameItemDataField.amount].LongValue;
                }

                return totalAmount;
            }
        }
        
        public static decimal GetAmountDecimal(string id)
        {
            //Singleton
            if (GameItemTable.IsSingleton(id))
            {
                string nftId = dictSingletonLookUp.Get_NftId_By_Id(id);
                if (string.IsNullOrEmpty(nftId))
                    return 0L;

                return InventoryNode[nftId][GameItemDataField.amount].DecimalValue;
            }
            else
            {
                //Non-Singleton - Total Amount
                decimal totalAmount = 0;
                foreach (var gameItemNode in InventoryNode)
                {
                    if(gameItemNode[GameItemDataField.id].StringValue != id)
                        continue;
                
                    totalAmount += gameItemNode[GameItemDataField.amount].DecimalValue;
                }

                return totalAmount;
            }
        }
        
        public static BigInteger GetAmountBigInt(string id)
        {
            //Singleton
            if (GameItemTable.IsSingleton(id))
            {
                string nftId = dictSingletonLookUp.Get_NftId_By_Id(id);
                if (string.IsNullOrEmpty(nftId))
                    return 0L;

                return InventoryNode[nftId][GameItemDataField.amount].BigIntValue;
            }
            else
            {
                //Non-Singleton - Total Amount
                BigInteger totalAmount = 0;
                foreach (var gameItemNode in InventoryNode)
                {
                    if(gameItemNode[GameItemDataField.id].StringValue != id)
                        continue;
                
                    totalAmount = BigInteger.Add(totalAmount, gameItemNode[GameItemDataField.amount].BigIntValue);
                }

                return totalAmount;
            }
        }

        #endregion
        

        #region Plus Amount


        private static void PlusAmount(string id, object amountObj, AmountType amountType, string where, string reason)
        {
            switch (amountType)
            {
                case AmountType.Undefined:
                {
                    throw new Exception(ErrorMessageUtility.AmountTypeIsUndefined());
                }
                case AmountType.Long:
                {
                    PlusAmount(id, (long)amountObj, where, reason);
                    return;
                }
                case AmountType.Decimal:
                {
                    PlusAmount(id, (decimal)amountObj, where, reason);
                    return;
                }
                case AmountType.BigInt:
                {
                    PlusAmount(id, (BigInteger)amountObj, where, reason);
                    return;
                }
                default:
                {
                    Debug.LogError("Not code for amount type: " + amountType);
                    return;
                }
            }
        }
        

        /// <summary>
        /// Chỉ hoạt động với Singleton Item, vì việc plus mà chỉ truyền Id sẽ là ko đủ thông tin để truy cập chính xác NonSingleton Item
        /// </summary>
        private static void PlusAmount(string id, long amount, string where, string reason)
        {
            if (amount < 0)
            {
                Debug.LogError(ErrorMessageUtility.CannotPlusAmountSmallerZero());
                return;
            }
            
            
            var gameItemRow = GameItemTable.GetRowByIdWithLog(id);
            if (gameItemRow.IsSingleton == false)
            {
                Debug.LogError(ErrorMessageUtility.PlusAmountOnlyWorkWithSingletonItem());
                return;
            }


            string nftId = dictSingletonLookUp.Get_NftId_By_Id(id);
            if (string.IsNullOrEmpty(nftId)) //Nếu chưa tồn tại thì add
            {
                SGNode gameItemNode = InventoryNode.AddChild();
                gameItemNode[GameItemDataField.id].StringValue = id;
                
                AmountHandlerPro.Set_AmountLong_To_AmountNode(amount, gameItemNode[GameItemDataField.amount], gameItemRow.AmountType);
                
                //Dict Singleton Look Up
                dictSingletonLookUp.Set(id, gameItemNode.Key); //Vì là add nên cần cập nhật dictSingletonLookUp
                
                
                //Event
                GameItemData newGameItemData = GameItemNode_To_GameItemData(gameItemNode);
                EmitOnGameItemChanged(id, nftId, GameItemChangedType.Add, null, newGameItemData, where, reason);
            }
            else //Nếu đã tồn tại thì chỉ cần thay đổi amount
            {
                SGNode gameItemNode = InventoryNode[nftId];
                GameItemData oldGameItemData = GameItemNode_To_GameItemData(gameItemNode);
                
                AmountHandlerPro.AmountNode_Plus_AmountLong(gameItemNode[GameItemDataField.amount], gameItemRow.AmountType, amount);
                
                //Dict Singleton Look Up
                dictSingletonLookUp.Set(id, gameItemNode.Key); //Vì cập nhật dictSingletonLookUp cho yên tâm
                
                
                //Event
                GameItemData newGameItemData = GameItemNode_To_GameItemData(gameItemNode);
                EmitOnGameItemChanged(id, nftId, GameItemChangedType.Amount, oldGameItemData, newGameItemData, where, reason);
            }
        }
        
        /// <summary>
        /// Chỉ hoạt động với Singleton Item, vì việc plus mà chỉ truyền Id sẽ là ko đủ thông tin để truy cập chính xác NonSingleton Item
        /// </summary>
        private static void PlusAmount(string id, decimal amount, string where, string reason)
        {
            if (amount < 0)
            {
                Debug.LogError(ErrorMessageUtility.CannotPlusAmountSmallerZero());
                return;
            }
            
            
            var gameItemRow = GameItemTable.GetRowByIdWithLog(id);
            if (gameItemRow.IsSingleton == false)
            {
                Debug.LogError(ErrorMessageUtility.PlusAmountOnlyWorkWithSingletonItem());
                return;
            }


            string nftId = dictSingletonLookUp.Get_NftId_By_Id(id);
            if (string.IsNullOrEmpty(nftId)) //Nếu chưa tồn tại thì add
            {
                SGNode gameItemNode = InventoryNode.AddChild();
                gameItemNode[GameItemDataField.id].StringValue = id;
                
                AmountHandlerPro.Set_AmountDecimal_To_AmountNode(amount, gameItemNode[GameItemDataField.amount], gameItemRow.AmountType);
                
                //Dict Singleton Look Up
                dictSingletonLookUp.Set(id, gameItemNode.Key); //Vì là add nên cần cập nhật dictSingletonLookUp
                
                
                //Event
                GameItemData newGameItemData = GameItemNode_To_GameItemData(gameItemNode);
                EmitOnGameItemChanged(id, nftId, GameItemChangedType.Add, null, newGameItemData, where, reason);
            }
            else //Nếu đã tồn tại thì chỉ cần thay đổi amount
            {
                SGNode gameItemNode = InventoryNode[nftId];
                GameItemData oldGameItemData = GameItemNode_To_GameItemData(gameItemNode);
                
                AmountHandlerPro.AmountNode_Plus_AmountDecimal(gameItemNode[GameItemDataField.amount], gameItemRow.AmountType, amount);
                
                //Dict Singleton Look Up
                dictSingletonLookUp.Set(id, gameItemNode.Key); //Vì cập nhật dictSingletonLookUp cho yên tâm
                
                
                //Event
                GameItemData newGameItemData = GameItemNode_To_GameItemData(gameItemNode);
                EmitOnGameItemChanged(id, nftId, GameItemChangedType.Amount, oldGameItemData, newGameItemData, where, reason);
            }
        }
        
        /// <summary>
        /// Chỉ hoạt động với Singleton Item, vì việc plus mà chỉ truyền Id sẽ là ko đủ thông tin để truy cập chính xác NonSingleton Item
        /// </summary>
        private static void PlusAmount(string id, BigInteger amount, string where, string reason)
        {
            if (amount < 0)
            {
                Debug.LogError(ErrorMessageUtility.CannotPlusAmountSmallerZero());
                return;
            }
            
            
            var gameItemRow = GameItemTable.GetRowByIdWithLog(id);
            if (gameItemRow.IsSingleton == false)
            {
                Debug.LogError(ErrorMessageUtility.PlusAmountOnlyWorkWithSingletonItem());
                return;
            }


            string nftId = dictSingletonLookUp.Get_NftId_By_Id(id);
            if (string.IsNullOrEmpty(nftId)) //Nếu chưa tồn tại thì add
            {
                SGNode gameItemNode = InventoryNode.AddChild();
                gameItemNode[GameItemDataField.id].StringValue = id;
                
                AmountHandlerPro.Set_AmountBigInt_To_AmountNode(amount, gameItemNode[GameItemDataField.amount], gameItemRow.AmountType);
                
                //Dict Singleton Look Up
                dictSingletonLookUp.Set(id, gameItemNode.Key); //Vì là add nên cần cập nhật dictSingletonLookUp
                
                
                //Event
                GameItemData newGameItemData = GameItemNode_To_GameItemData(gameItemNode);
                EmitOnGameItemChanged(id, nftId, GameItemChangedType.Add, null, newGameItemData, where, reason);
            }
            else //Nếu đã tồn tại thì chỉ cần thay đổi amount
            {
                SGNode gameItemNode = InventoryNode[nftId];
                GameItemData oldGameItemData = GameItemNode_To_GameItemData(gameItemNode);
                
                AmountHandlerPro.AmountNode_Plus_AmountBigInt(gameItemNode[GameItemDataField.amount], gameItemRow.AmountType, amount);
                
                //Dict Singleton Look Up
                dictSingletonLookUp.Set(id, gameItemNode.Key); //Vì cập nhật dictSingletonLookUp cho yên tâm
                
                
                //Event
                GameItemData newGameItemData = GameItemNode_To_GameItemData(gameItemNode);
                EmitOnGameItemChanged(id, nftId, GameItemChangedType.Amount, oldGameItemData, newGameItemData, where, reason);
            }
        }

        #endregion


        #region Minus Amount

        public static bool MinusAmount(string id, long value, string where, string reason)
        {
            if (value < 0)
            {
                Debug.LogError(ErrorMessageUtility.CannotMinusAmountSmallerZero());
                return false;
            }

            if (value == 0)
                return true;
            
            
            var gameItemRow = GameItemTable.GetRowByIdWithLog(id);

            if (gameItemRow.IsSingleton == false)
            {
                Debug.LogError(ErrorMessageUtility.MinusAmountOnlyWorkWithSingletonItem());
                return false;
            }


            string nftId = dictSingletonLookUp.Get_NftId_By_Id(id);
            if (string.IsNullOrEmpty(nftId)) //Item không tồn tại thì đương nhiên không thể trừ
                return false;


            SGNode gameItemNode = InventoryNode[nftId];
            SGNode amountNode = gameItemNode[GameItemDataField.amount];
            
            
            if (AmountHandlerPro.AmountLong_Greater_AmountNode(value, amountNode, gameItemRow.AmountType)) //Nếu số muốn trừ lớn hơn số trong Inventory thì không thể trừ
                return false;
            
            
            GameItemData oldGameItemData = GameItemNode_To_GameItemData(gameItemNode);
            
            AmountHandlerPro.AmountNode_Minus_AmountLong(amountNode, gameItemRow.AmountType, value);
            
            
            //Event
            GameItemData newGameItemData = GameItemNode_To_GameItemData(gameItemNode);
            EmitOnGameItemChanged(id, nftId, GameItemChangedType.Amount, oldGameItemData, newGameItemData, where, reason);
            
            
            return true;
        }
        
        public static bool MinusAmount(string id, decimal value, string where, string reason)
        {
            if (value < 0)
            {
                Debug.LogError(ErrorMessageUtility.CannotMinusAmountSmallerZero());
                return false;
            }

            if (value == 0)
                return true;
            
            
            var gameItemRow = GameItemTable.GetRowByIdWithLog(id);

            if (gameItemRow.IsSingleton == false)
            {
                Debug.LogError(ErrorMessageUtility.MinusAmountOnlyWorkWithSingletonItem());
                return false;
            }


            string nftId = dictSingletonLookUp.Get_NftId_By_Id(id);
            if (string.IsNullOrEmpty(nftId)) //Item không tồn tại thì đương nhiên không thể trừ
                return false;


            SGNode gameItemNode = InventoryNode[nftId];
            SGNode amountNode = gameItemNode[GameItemDataField.amount];
            
            
            if (AmountHandlerPro.AmountDecimal_Greater_AmountNode(value, amountNode, gameItemRow.AmountType)) //Nếu số muốn trừ lớn hơn số trong Inventory thì không thể trừ
                return false;
            
            
            GameItemData oldGameItemData = GameItemNode_To_GameItemData(gameItemNode);
            
            AmountHandlerPro.AmountNode_Minus_AmountDecimal(amountNode, gameItemRow.AmountType, value);
            
            
            //Event
            GameItemData newGameItemData = GameItemNode_To_GameItemData(gameItemNode);
            EmitOnGameItemChanged(id, nftId, GameItemChangedType.Amount, oldGameItemData, newGameItemData, where, reason);
            
            
            return true;
        }
        
        public static bool MinusAmount(string id, BigInteger value, string where, string reason)
        {
            if (value < 0)
            {
                Debug.LogError(ErrorMessageUtility.CannotMinusAmountSmallerZero());
                return false;
            }

            if (value == 0)
                return true;
            
            
            var gameItemRow = GameItemTable.GetRowByIdWithLog(id);

            if (gameItemRow.IsSingleton == false)
            {
                Debug.LogError(ErrorMessageUtility.MinusAmountOnlyWorkWithSingletonItem());
                return false;
            }


            string nftId = dictSingletonLookUp.Get_NftId_By_Id(id);
            if (string.IsNullOrEmpty(nftId)) //Item không tồn tại thì đương nhiên không thể trừ
                return false;


            SGNode gameItemNode = InventoryNode[nftId];
            SGNode amountNode = gameItemNode[GameItemDataField.amount];
            
            
            if (AmountHandlerPro.AmountBigInt_Greater_AmountNode(value, amountNode, gameItemRow.AmountType)) //Nếu số muốn trừ lớn hơn số trong Inventory thì không thể trừ
                return false;
            
            
            GameItemData oldGameItemData = GameItemNode_To_GameItemData(gameItemNode);
            
            AmountHandlerPro.AmountNode_Minus_AmountBigInt(amountNode, gameItemRow.AmountType, value);
            
            
            //Event
            GameItemData newGameItemData = GameItemNode_To_GameItemData(gameItemNode);
            EmitOnGameItemChanged(id, nftId, GameItemChangedType.Amount, oldGameItemData, newGameItemData, where, reason);
            
            
            return true;
        }

        #endregion


        #region Set Amount

        public static void SetAmount(string id, long amount, string where, string reason)
        {
            if (amount < 0)
            {
                Debug.LogError(ErrorMessageUtility.CannotSetAmountSmallerZero());
                return;
            }
                
            var gameItemRow = GameItemTable.GetRowByIdWithLog(id);

            
            if (gameItemRow.IsSingleton == false)
            {
                Debug.LogError(ErrorMessageUtility.SetAmountOnlyWorkWithSingletonItem());
                return;
            }

            string nftId = dictSingletonLookUp.Get_NftId_By_Id(id);
            if (string.IsNullOrEmpty(nftId)) //Nếu chưa có thì a
            {
                SGNode gameItemNode = InventoryNode.AddChild();
                
                gameItemNode[GameItemDataField.id].StringValue = id;
                AmountHandlerPro.Set_AmountLong_To_AmountNode(amount, gameItemNode[GameItemDataField.amount], gameItemRow.AmountType);
                
                //Dict Singleton Look Up
                dictSingletonLookUp.Set(id, gameItemNode.Key);
                
                
                //Event
                GameItemData newGameItemData = GameItemNode_To_GameItemData(gameItemNode);
                EmitOnGameItemChanged(id, nftId, GameItemChangedType.Add, null, newGameItemData, where, reason);
            }
            else
            {
                SGNode gameItemNode = InventoryNode[nftId];
                GameItemData oldGameItemData = GameItemNode_To_GameItemData(gameItemNode);
                
                AmountHandlerPro.Set_AmountLong_To_AmountNode(amount, gameItemNode[GameItemDataField.amount], gameItemRow.AmountType);
                
                
                //Event
                GameItemData newGameItemData = GameItemNode_To_GameItemData(gameItemNode);
                EmitOnGameItemChanged(id, nftId, GameItemChangedType.Amount, oldGameItemData, newGameItemData, where, reason);
            }
        }

        public static void SetAmount(string id, decimal amount, string where, string reason)
        {
            if (amount < 0)
            {
                Debug.LogError(ErrorMessageUtility.CannotSetAmountSmallerZero());
                return;
            }
                
            var gameItemRow = GameItemTable.GetRowByIdWithLog(id);

            
            if (gameItemRow.IsSingleton == false)
            {
                Debug.LogError(ErrorMessageUtility.SetAmountOnlyWorkWithSingletonItem());
                return;
            }

            string nftId = dictSingletonLookUp.Get_NftId_By_Id(id);
            if (string.IsNullOrEmpty(nftId)) //Nếu chưa có thì a
            {
                SGNode gameItemNode = InventoryNode.AddChild();
                
                gameItemNode[GameItemDataField.id].StringValue = id;
                AmountHandlerPro.Set_AmountDecimal_To_AmountNode(amount, gameItemNode[GameItemDataField.amount], gameItemRow.AmountType);
                
                //Dict Singleton Look Up
                dictSingletonLookUp.Set(id, gameItemNode.Key);
                
                
                //Event
                GameItemData newGameItemData = GameItemNode_To_GameItemData(gameItemNode);
                EmitOnGameItemChanged(id, nftId, GameItemChangedType.Add, null, newGameItemData, where, reason);
            }
            else
            {
                SGNode gameItemNode = InventoryNode[nftId];
                GameItemData oldGameItemData = GameItemNode_To_GameItemData(gameItemNode);
                
                AmountHandlerPro.Set_AmountDecimal_To_AmountNode(amount, gameItemNode[GameItemDataField.amount], gameItemRow.AmountType);
                
                
                //Event
                GameItemData newGameItemData = GameItemNode_To_GameItemData(gameItemNode);
                EmitOnGameItemChanged(id, nftId, GameItemChangedType.Amount, oldGameItemData, newGameItemData, where, reason);
            }
        }
        
        public static void SetAmount(string id, BigInteger amount, string where, string reason)
        {
            if (amount < 0)
            {
                Debug.LogError(ErrorMessageUtility.CannotSetAmountSmallerZero());
                return;
            }
                
            var gameItemRow = GameItemTable.GetRowByIdWithLog(id);

            
            if (gameItemRow.IsSingleton == false)
            {
                Debug.LogError(ErrorMessageUtility.SetAmountOnlyWorkWithSingletonItem());
                return;
            }

            string nftId = dictSingletonLookUp.Get_NftId_By_Id(id);
            if (string.IsNullOrEmpty(nftId)) //Nếu chưa có thì a
            {
                SGNode gameItemNode = InventoryNode.AddChild();
                
                gameItemNode[GameItemDataField.id].StringValue = id;
                AmountHandlerPro.Set_AmountBigInt_To_AmountNode(amount, gameItemNode[GameItemDataField.amount], gameItemRow.AmountType);
                
                //Dict Singleton Look Up
                dictSingletonLookUp.Set(id, gameItemNode.Key);
                
                
                //Event
                GameItemData newGameItemData = GameItemNode_To_GameItemData(gameItemNode);
                EmitOnGameItemChanged(id, nftId, GameItemChangedType.Add, null, newGameItemData, where, reason);
            }
            else
            {
                SGNode gameItemNode = InventoryNode[nftId];
                GameItemData oldGameItemData = GameItemNode_To_GameItemData(gameItemNode);
                
                AmountHandlerPro.Set_AmountBigInt_To_AmountNode(amount, gameItemNode[GameItemDataField.amount], gameItemRow.AmountType);
                
                
                //Event
                GameItemData newGameItemData = GameItemNode_To_GameItemData(gameItemNode);
                EmitOnGameItemChanged(id, nftId, GameItemChangedType.Amount, oldGameItemData, newGameItemData, where, reason);
            }
        }
        
        #endregion
    }
}