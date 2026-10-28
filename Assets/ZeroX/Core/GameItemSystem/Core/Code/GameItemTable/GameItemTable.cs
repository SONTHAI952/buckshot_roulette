using System;
using System.Collections.Generic;
using UnityEngine;
using ZeroX.DataTableSystem.SoTableSystem;
using ZeroX.JsonSystem;
using ZeroX.SaveGameSystem;

namespace GameItemSystem
{
    [CreateAssetMenu(menuName = "Database/Game Item/Table")]
    public class GameItemTable : SoTableOneId<GameItemTable, GameItemRow, string>
    {
        protected override string ShortMainPath => "Game Item";
        
        protected override string GetRowId(GameItemRow row)
        {
            return row.Id;
        }

        public static GameItemType GetType(string id)
        {
            var row = GetRowById(id);
            if (row == null)
                throw new Exception("Cannot find GameItem row with id: " + id);

            return row.Type;
        }

        public static AmountType GetAmountType(string id)
        {
            var row = GetRowById(id);
            if (row == null)
                throw new Exception("Cannot find GameItem row with id: " + id);

            return row.AmountType;
        }

        public static bool GetAmountIsTime(string id)
        {
            var row = GetRowById(id);
            if (row == null)
                throw new Exception("Cannot find GameItem row with id: " + id);

            return row.AmountIsTime;
        }

        public static bool IsSingleton(string id)
        {
            var row = GetRowByIdWithLog(id);
            if (row == null)
                return false;

            return row.IsSingleton;
        }
        
        public static bool IsNested(string id)
        {
            var row = GetRowByIdWithLog(id);
            if (row == null)
                throw new Exception("Cannot find GameItem row with id: " + id);

            return row.IsNested;
        }

        public static bool IsConsumable(string id)
        {
            var row = GetRowByIdWithLog(id);
            if (row == null)
                throw new Exception("Cannot find GameItem row with id: " + id);

            return row.IsConsumable;
        }

        public static string GetDisplayName(string id)
        {
            var row = GetRowByIdWithLog(id);
            if (row == null)
                return "unknown_game_item_name";

            return row.KeyName;
        }
        
        public static string GetDisplayNameSnakeCase(string id)
        {
            var row = GetRowByIdWithLog(id);
            if (row == null)
                return "unknown_game_item_name";

            return row.KeyName.ToLower().Replace(" ", "_");
        }
        
        public static Sprite GetIcon(string id)
        {
            var row = GetRowByIdWithLog(id);
            if (row == null)
                return null;

            return row.Icon;
        }
        
        public static void SetIcon(string id, Sprite newIcon)
        {
            var row = GetRowByIdWithLog(id);
            if (row == null)
                return;

            row.SetIcon(newIcon);
        }
        
        public static GameItemDataHandler GetDataHandler(string id)
        {
            var row = GetRowByIdWithLog(id);
            return row.DataHandler;
        }

        public static GameItemData CreateDataInstance(string id)
        {
            var row = GetRowByIdWithLog(id);
            return row.DataHandler.CreateGameItemDataInstance();
        }

        public static GameItemData CreateDataInstanceFromJson(string json)
        {
            ZJsonObject jsonNode = ZJson.JsonToNode(json).AsObject;
            string id = jsonNode[GameItemDataField.id].AsString;
            
            var row = GetRowByIdWithLog(id);
            return row.DataHandler.CreateGameItemDataInstance_FromJsonNode(jsonNode);
        }

        public static List<GameItemData> CreateListDataInstanceFromJson(string json)
        {
            List<GameItemData> listData = new List<GameItemData>();
                
            var jsonArray = ZJson.JsonToNode(json).AsList;
            foreach (ZJsonObject jsonNode in jsonArray)
            {
                string id = jsonNode[GameItemDataField.id].AsString;
            
                var row = GetRowByIdWithLog(id);
                var data = row.DataHandler.CreateGameItemDataInstance_FromJsonNode(jsonNode);
                listData.Add(data);
            }

            return listData;
        }



        #if UNITY_EDITOR
        
        [RuntimeInitializeOnLoadMethod]
        public static void CheckDataClassAndDataHandlerClass()
        {
            foreach (var row in Rows)
            {
                try
                {
                    var dataHandler = row.DataHandler;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        
        #endif
    }
}