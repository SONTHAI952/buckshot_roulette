using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZeroX.JsonSystem;
using ZeroX.SaveGameSystem;

namespace GameItemSystem
{
    public class GameItemDataHandler
    {
        private AmountType amountTypeToHandle;
        private string idToHandle;
        
        public AmountType AmountTypeToHandle => amountTypeToHandle;
        public string IdToHandle => idToHandle;
        
        
        
        
        
        /// <summary>
        /// Được set bởi GameItemDataRow. Và instance của GameItemDataHandler cũng được tạo bởi GameItemDataRow
        /// </summary>
        public static void SetInfoToHandle(GameItemDataHandler dataHandler, AmountType amountType, string id)
        {
            dataHandler.amountTypeToHandle = amountType;
            dataHandler.idToHandle = id;
        }
        
        
        /// <summary>
        /// Object trả về đã set sẵn amountType và id
        /// </summary>
        public virtual GameItemData CreateGameItemDataInstance()
        {
            return new GameItemData(amountTypeToHandle, idToHandle);
        }
        
        
        public GameItemData CreateGameItemDataInstance_FromJsonNode(ZJsonObject jsonObject)
        {
            GameItemData gameItemData = CreateGameItemDataInstance();
            
            AmountHandlerPro.Set_AmountJsonNode_To_AmountData(jsonObject[GameItemDataField.amount].AsScalar, gameItemData);
            SetOverrideIconPath_FromJsonNode(jsonObject, gameItemData);
            JsonNode_To_GameItemData(jsonObject, gameItemData);
            
            return gameItemData;
        }


        public void SetOverrideIconPath_FromJsonNode(ZJsonObject jsonNode, GameItemData gameItemData)
        {
            if (jsonNode.HasKey(GameItemData.fn_overrideIcon))
            {
                gameItemData.overrideIconPath = jsonNode[GameItemData.fn_overrideIcon].AsString;
            }
            
            if (jsonNode.HasKey(GameItemData.fn_overrideIconFolder))
            {
                gameItemData.overrideIconParentPath = jsonNode[GameItemData.fn_overrideIconFolder].AsString;
            }
        }


        #region GameItemData - SGNode
        
        /// <summary>
        /// Không bao gồm phần id, nftId, amount. Những phần này đã được xử lý ở nơi khác
        /// </summary>
        public virtual void GameItemData_To_GameItemNode(GameItemData gameItemData, SGNode gameItemNode)
        {
            
        }
        
        
        
        /// <summary>
        /// Không bao gồm phần id, nftId, amount. Những phần này đã được xử lý ở nơi khác
        /// </summary>
        public virtual void GameItemNode_To_GameItemData(SGNode gameItemNode, GameItemData gameItemData)
        {
            
        }
        
        #endregion
        
        
        #region GameItemData - JsonObject
        
        /// <summary>
        /// Không bao gồm phần id, nftId, amount, overrideIcon. Những phần này đã được xử lý ở nơi khác
        /// </summary>
        public virtual void JsonNode_To_GameItemData(ZJsonObject jsonObject, GameItemData gameItemData)
        {
            
        }
        
        #endregion


        #region Unpack
        
        public virtual List<GameItemData> ExecuteUnpack(GameItemData data)
        {
            return new List<GameItemData>() {data};
        }
        
        
        public List<GameItemData> Unpack(GameItemData gameItemData)
        {
            var row = GameItemTable.GetRowByIdWithLog(gameItemData.id);
            
            if (row.IsNested == false)
            {
                Debug.LogErrorFormat("Cannot unpack because GameItem '{0}' is not nested", gameItemData.id);
            }

            return ExecuteUnpack(gameItemData);
        }
        
        public List<GameItemData> Unpack(List<GameItemData> listData)
        {
            List<GameItemData> listUnPacked = new List<GameItemData>();
            foreach (var data in listData)
            {
                listUnPacked.AddRange(Unpack(data));
            }

            return listUnPacked;
        }

        
        /// <summary>
        /// Unpack until no more nested
        /// </summary>
        public List<GameItemData> UnpackComplete(GameItemData data)
        {
            List<GameItemData> listUnPacked = Unpack(data);
            
            while (listUnPacked.Any(g => GameItemTable.IsNested(g.id)))
            {
                listUnPacked = Unpack(listUnPacked);
            }

            return listUnPacked;
        }
        
        
        #endregion
    }
}