using System;
using System.Collections.Generic;
using ZeroX.JsonSystem;
using ZeroX.SaveGameSystem.Core;


namespace ZeroX.SaveGameSystem
{
    public static class SGUtility
    {
        public static bool IsNodeIdEmpty(long nodeId)
        {
            return nodeId <= SGConstant.EMPTY_NODE_ID;
        }
        
        /// <summary>
        /// Tách hashSetNodeChanged thành danh sách node edit và node delete để dễ xử lý
        /// </summary>
        public static void ExportNodeChangesToSavable_Split(HashSet<SGNode> hashSetNodeChanged, out Dictionary<long, ZJsonObject> dictJsonNode, out List<long> listNodeIdDelete)
        {
            dictJsonNode = null;
            listNodeIdDelete = null;
            
            
            foreach (var node in hashSetNodeChanged)
            {
                try
                {
                    if (node.IsDeleted)
                    {
                        if(listNodeIdDelete == null)
                            listNodeIdDelete = new List<long>();
                            
                        listNodeIdDelete.Add(node.NodeId);
                    }
                    else
                    {
                        if (dictJsonNode == null)
                            dictJsonNode = new Dictionary<long, ZJsonObject>();
                        
                        dictJsonNode.Add(node.NodeId, node.ToJsonObject());
                    }
                }
                catch (Exception e)
                {
                    SGLog.LogException(e);
                }
            }
        }
        
        
        
        /// <summary>
        /// JsonNode thu được có dạng như 1 dictionary với key là nodeId và data là json
        /// </summary>
        public static ZJsonObject ConvertNodeToJsonNodeIncludeAllChild(SGNode rootNode)
        {
            ZJsonObject jsonNodeResult = new ZJsonObject();
            Recursive(jsonNodeResult, rootNode);
            return jsonNodeResult;


            
            
            
            void Recursive(ZJsonObject result, SGNode nodeInclude)
            {
                if(nodeInclude.NodeType != NodeType.Object)
                    return;
                
                
                
                result.Add(nodeInclude.NodeId.ToString(), nodeInclude.ToJsonObject());
                
                foreach (var child in nodeInclude)
                {
                    Recursive(result, child);
                }
            }
        }
    }
}