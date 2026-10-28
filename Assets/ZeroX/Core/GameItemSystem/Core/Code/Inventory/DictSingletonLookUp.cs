using System.Collections.Generic;
using UnityEngine;

namespace GameItemSystem
{
    public class DictSingletonLookUp
    {
        private bool initialized = false;
        private Dictionary<string, string> dict_Id_NftId = new Dictionary<string, string>();
        private Dictionary<string, string> dict_NftId_Id = new Dictionary<string, string>();


        public void Init()
        {
            initialized = false;
            InitIfNot();
        }

        public void InitIfNot()
        {
            if(initialized)
                return;
            
            dict_Id_NftId.Clear();
            dict_NftId_Id.Clear();
            
            foreach (var childNode in InventoryDI.InventoryNode)
            {
                string id = childNode[GameItemDataField.id].StringValue;
                if(GameItemTable.IsSingleton(id) == false)
                    continue;

                if (dict_Id_NftId.ContainsKey(id))
                {
                    Debug.LogError("There exist 2 singleton items in inventory, please check again or remove 1 item again");
                    continue;
                }
                
                dict_Id_NftId.Add(id, childNode.Key);
                dict_NftId_Id.Add(childNode.Key, id);
            }

            initialized = true;
        }

        public void Set(string id, string nftId)
        {
            InitIfNot();

            dict_Id_NftId[id] = nftId;
            dict_NftId_Id[nftId] = id;
        }

        public string Get_NftId_By_Id(string id)
        {
            InitIfNot();

            if (dict_Id_NftId.TryGetValue(id, out string nftId))
                return nftId;
            
            return null;
        }
        
        public string Get_Id_By_NftId(string nftId)
        {
            InitIfNot();

            if (dict_NftId_Id.TryGetValue(nftId, out string id))
                return id;
            
            return null;
        }

        public void RemoveByNftId(string nftId)
        {
            InitIfNot();

            if (dict_NftId_Id.TryGetValue(nftId, out string id))
            {
                dict_Id_NftId.Remove(id);
                dict_NftId_Id.Remove(nftId);
            }
        }

        public bool ContainsId(string id)
        {
            InitIfNot();

            return dict_Id_NftId.ContainsKey(id);
        }
        
        public bool ContainsNftId(string nftId)
        {
            InitIfNot();

            return dict_NftId_Id.ContainsKey(nftId);
        }
    }
}