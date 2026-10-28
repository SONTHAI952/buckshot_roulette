using System;
using System.Globalization;
using System.Numerics;
using System.Text;
using UnityEngine;
using ZeroX.JsonSystem;
using ZeroX.SaveGameSystem.Security;

namespace ZeroX.SaveGameSystem
{
    public partial class SGNode
    {
        public static void ImportJChildValueIntoNode(ZJsonScalar jChildValue, SGNode node, NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.Undefined:
                {
                    break;
                }
                case NodeType.String:
                {
                    if (node.e_stringValue == null)
                        node.e_stringValue = new EString(jChildValue.AsString);
                    else
                        node.e_stringValue.Value = jChildValue.AsString;
                    
                    break;
                }
                case NodeType.Int:
                {
                    node.e_numberValue = new EInt(jChildValue.AsInt);
                    break;
                }
                case NodeType.Float:
                {
                    node.e_numberValue = new EFloat(jChildValue.AsFloat);
                    break;
                }
                case NodeType.Long:
                {
                    node.e_numberValue = new ELong(jChildValue.AsLong);
                    break;
                }
                case NodeType.Decimal:
                {
                    node.e_numberValue = new EDecimal(jChildValue.AsDecimal);
                    break;
                }
                case NodeType.Double:
                {
                    node.e_numberValue = new EDouble(jChildValue.AsDouble);
                    break;
                }
                case NodeType.Bool:
                {
                    node.e_numberValue = new EInt(jChildValue.AsInt);
                    break;
                }
                case NodeType.BigInteger:
                {
                    BigInteger value = BigInteger.Parse(jChildValue.AsString);
                    node.e_numberValue = new EBigInteger(value);
                    break;
                }
                default:
                {
                    Debug.LogError("Chưa code cho value type này");
                    break;
                }
            }
        }

        public static void ImportNodeIntoJChild(SGNode node, ZJsonObject jChild)
        {
            switch (node.NodeType)
            {
                case NodeType.String:
                {
                    jChild[KeyDefine.value] = node.e_stringValue == null ? "" : node.e_stringValue.Value;
                    break;
                }
                case NodeType.Int:
                {
                    jChild[KeyDefine.value] = ((EInt)node.e_numberValue).Value;
                    break;
                }
                case NodeType.Float:
                {
                    jChild[KeyDefine.value] = ((EFloat)node.e_numberValue).Value;
                    break;
                }
                case NodeType.Long:
                {
                    jChild[KeyDefine.value] = ((ELong)node.e_numberValue).Value;
                    break;
                }
                case NodeType.Decimal:
                {
                    jChild[KeyDefine.value] = ((EDecimal)node.e_numberValue).Value;
                    break;
                }
                case NodeType.Double:
                {
                    jChild[KeyDefine.value] = ((EDouble)node.e_numberValue).Value;
                    break;
                }
                case NodeType.Bool:
                {
                    jChild[KeyDefine.value] = ((EInt)node.e_numberValue).Value;
                    break;
                }
                case NodeType.BigInteger:
                {
                    jChild[KeyDefine.value] = ((EBigInteger)node.e_numberValue).Value.ToString();
                    break;
                }
                default:
                {
                    Debug.LogError("Chưa code cho value type này: " + node.NodeType);
                    break;
                }
            }
        }



        #region Change Node Type

#if UNITY_EDITOR
        
        /// <summary>
        /// Don't use this method, only for editor
        /// </summary>
        public void Editor_ChangeNodeType(NodeType newNodeType)
        {
            InitializeIfNot();

            if (newNodeType == NodeType.Object) //Nếu muốn chuyển sang Object
            {
                Editor_ChangeNodeType_ToObject();
            }
            else //Nếu muốn chuyển object sang primitive
            {
                if (newNodeType == NodeType.Undefined || newNodeType == NodeType.String)
                {
                    Editor_ChangeNodeType_ToUndefinedOrString(newNodeType);
                }
                else
                {
                    Editor_ChangeNodeType_ToNumber(newNodeType);
                }
            }
        }

        void Editor_ChangeNodeType_ToObject()
        {
            if (HasNodeId) //Nếu đã có nodeId
            {
                NodeType = NodeType.Object;
                    
                //Save
                if(Parent == null)
                    Database.StorageService.SetDatabaseChanged();
                else
                    Database.StorageService.SetNodeChanged(Parent);
            }
            else
            {
                NodeType = NodeType.Object;
                NodeId = Database.NodeIdSeq + 1;
                    
                //Save
                Database.Internal_IncreaseNodeIdSeq(Database);
                if(Parent == null)
                    Database.StorageService.SetDatabaseChanged();
                else
                    Database.StorageService.SetNodeChanged(Parent);
                    
                Database.StorageService.SetNodeChanged(this); 
            }
        }

        void Editor_ChangeNodeType_ToUndefinedOrString(NodeType newNodeType)
        {
            NodeType = newNodeType;
                    
            //Save
            if(Parent == null)
                Database.StorageService.SetDatabaseChanged();
            else
                Database.StorageService.SetNodeChanged(Parent);
        }

        void Editor_ChangeNodeType_ToNumber(NodeType newNodeType)
        {
            //Sử dụng value dạng string để lm trung gian chuyển đổi
            if(e_numberValue == null)
                e_numberValue = Editor_NodeTypeToEType(newNodeType, "0");
            else
            {
                string value = Editor_GetNumberValue(e_numberValue);
                e_numberValue = Editor_NodeTypeToEType(newNodeType, value);
            }
                    
            NodeType = newNodeType;
                    
            //Save
            if(Parent == null)
                Database.StorageService.SetDatabaseChanged();
            else
                Database.StorageService.SetNodeChanged(Parent);
        }
        
#endif
        
        #endregion
        
        
        
#if UNITY_EDITOR
        public static string Editor_GetNumberValue(object numberValue)
        {
            if (numberValue is EInt)
                return ((EInt)numberValue).Value.ToString();
            
            if (numberValue is EFloat)
                return ((EFloat)numberValue).Value.ToString(CultureInfo.InvariantCulture);
            
            if (numberValue is ELong)
                return ((ELong)numberValue).Value.ToString();
            
            if (numberValue is EDecimal)
                return ((EDecimal)numberValue).Value.ToString(CultureInfo.InvariantCulture);
            
            if (numberValue is EDouble)
                return ((EDouble)numberValue).Value.ToString(CultureInfo.InvariantCulture);

            if (numberValue is EBigInteger)
                return ((EBigInteger) numberValue).Value.ToString();

            Debug.LogError("Chưa code EType này: " + numberValue.GetType().Name);
            return "0";
        }
        
        public static object Editor_NodeTypeToEType(NodeType nodeType, string numberValue)
        {
            switch (nodeType)
            {
                case NodeType.Undefined:
                {
                    return null;
                }
                case NodeType.String:
                {
                    return new EString("");
                }
                case NodeType.Int:
                {
                    numberValue = Editor_ValidateStringInteger(numberValue);
                    try
                    {
                        int value = int.Parse(numberValue);
                        return new EInt(value);
                    }
                    catch (OverflowException e)
                    {
                        return new EInt(int.MaxValue);
                    }
                }
                case NodeType.Float:
                {
                    try
                    {
                        float value = float.Parse(numberValue, CultureInfo.InvariantCulture);
                        return new EFloat(value);
                    }
                    catch (OverflowException e)
                    {
                        return new EFloat(float.MaxValue);
                    }
                }
                case NodeType.Long:
                {
                    numberValue = Editor_ValidateStringInteger(numberValue);
                    try
                    {
                        long value = long.Parse(numberValue);
                        return new ELong(value);
                    }
                    catch (OverflowException e)
                    {
                        return new ELong(long.MaxValue);
                    }
                }
                case NodeType.Decimal:
                {
                    try
                    {
                        decimal value = decimal.Parse(numberValue, CultureInfo.InvariantCulture);
                        return new EDecimal(value);
                    }
                    catch (OverflowException e)
                    {
                        return new EDecimal(decimal.MaxValue);
                    }
                }
                case NodeType.Double:
                {
                    try
                    {
                        double value = double.Parse(numberValue, CultureInfo.InvariantCulture);
                        return new EDouble(value);
                    }
                    catch (OverflowException e)
                    {
                        return new EDouble(double.MaxValue);
                    }
                }
                case NodeType.Bool:
                {
                    numberValue = Editor_ValidateStringInteger(numberValue);
                    try
                    {
                        int value = int.Parse(numberValue);
                        return new EInt(value);
                    }
                    catch (OverflowException e)
                    {
                        return new EInt(int.MaxValue);
                    }
                }
                case NodeType.BigInteger:
                {
                    numberValue = Editor_ValidateStringInteger(numberValue);
                    BigInteger value = BigInteger.Parse(numberValue);
                    return new EBigInteger(value);
                }
                default:
                {
                    Debug.LogError("Chưa code cho value type này");
                    return null;
                }
            }
        }
        
        public static void Editor_DrawNodeValueField(SGNode node)
        {
            var nodeType = node.NodeType;
            switch (nodeType)
            {
                case NodeType.Undefined:
                {
                    break;
                }
                case NodeType.Object:
                {
                    break;
                }
                case NodeType.String:
                {
                    var newValue = UnityEditor.EditorGUILayout.TextField(node.StringValue);
                    if (newValue != node.StringValue)
                    {
                        node.StringValue = newValue;
                    }
                    break;
                }
                case NodeType.Int:
                {
                    var newValue = UnityEditor.EditorGUILayout.IntField(node.IntValue);
                    if (newValue != node.IntValue)
                    {
                        node.IntValue = newValue;
                    }
                    break;
                }
                case NodeType.Float:
                {
                    var newValue = UnityEditor.EditorGUILayout.FloatField(node.FloatValue);
                    if (newValue != node.FloatValue)
                    {
                        node.FloatValue = newValue;
                    }
                    break;
                }
                case NodeType.Long:
                {
                    var newValue = UnityEditor.EditorGUILayout.LongField(node.LongValue);
                    if (newValue != node.LongValue)
                    {
                        node.LongValue = newValue;
                    }
                    break;
                }
                case NodeType.Decimal:
                {
                    string oldValue = node.DecimalValue.ToString(CultureInfo.InvariantCulture);
                    var newValue = UnityEditor.EditorGUILayout.TextField(oldValue);
                    if (Editor_ValidateStringDecimal(newValue) == false)
                        newValue = oldValue;

                    if (newValue != oldValue)
                    {
                        try
                        {
                            node.DecimalValue = decimal.Parse(newValue, NumberStyles.Any, CultureInfo.InvariantCulture);
                        }
                        catch (OverflowException e)
                        {
                            node.DecimalValue = decimal.MaxValue;
                        }
                    }
                    break;
                }
                case NodeType.Double:
                {
                    string oldValue = node.DoubleValue.ToString(CultureInfo.InvariantCulture);
                    var newValue = UnityEditor.EditorGUILayout.TextField(oldValue);
                    if (Editor_ValidateStringDouble(newValue) == false)
                        newValue = oldValue;
                    
                    if (newValue != oldValue)
                    {
                        try
                        {
                            node.DoubleValue = double.Parse(newValue, NumberStyles.Any, CultureInfo.InvariantCulture);
                        }
                        catch (OverflowException e)
                        {
                            node.DoubleValue = double.MaxValue;
                        }
                    }
                    break;
                }
                case NodeType.Bool:
                {
                    int selectedIndex = node.BoolValue ? 1 : 0;
                    int newSelectedIndex = UnityEditor.EditorGUILayout.Popup(selectedIndex, new []{"False", "True"});
                    if (newSelectedIndex != selectedIndex)
                    {
                        node.BoolValue = newSelectedIndex != 0;
                    }
                    break;
                }
                case NodeType.BigInteger:
                {
                    string oldValue = node.BigIntValue.ToString();
                    var newValue = UnityEditor.EditorGUILayout.TextField(oldValue);
                    newValue = Editor_ValidateStringInteger(newValue);
                    if (newValue != oldValue)
                    {
                        node.BigIntValue = BigInteger.Parse(newValue);
                    }
                    break;
                }
                default:
                {
                    UnityEditor.EditorGUILayout.LabelField("No code for this value type: " + nodeType);
                    break;
                }
            }
        }

        public static void Editor_CopyNodeValue(SGNode fromNode, SGNode toNode)
        {
            switch (fromNode.NodeType)
            {
                case NodeType.Undefined:
                case NodeType.Object:
                {
                    break;
                }
                case NodeType.String:
                {
                    toNode.StringValue = fromNode.StringValue;
                    break;
                }
                case NodeType.Int:
                {
                    toNode.IntValue = fromNode.IntValue;
                    break;
                }
                case NodeType.Float:
                {
                    toNode.FloatValue = fromNode.FloatValue;
                    break;
                }
                case NodeType.Long:
                {
                    toNode.LongValue = fromNode.LongValue;
                    break;
                }
                case NodeType.Decimal:
                {
                    toNode.DecimalValue = fromNode.DecimalValue;
                    break;
                }
                case NodeType.Double:
                {
                    toNode.DoubleValue = fromNode.DoubleValue;
                    break;
                }
                case NodeType.Bool:
                {
                    toNode.BoolValue = fromNode.BoolValue;
                    break;
                }
                case NodeType.BigInteger:
                {
                    toNode.BigIntValue = fromNode.BigIntValue;
                    break;
                }
                default:
                {
                    Debug.LogError("Chưa code cho nodeType này: " + fromNode.NodeType);
                    break;
                }
            }
        }
        
        public static string Editor_ValidateStringInteger(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "0";
            
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if(c == ' ') //Không chấp nhận dấu space
                    continue;
                
                if (c == '-') //Chỉ chấp nhận dấu - ở đầu chuỗi
                {
                    if (i == 0)
                        sb.Append(c);
                    continue;
                }

                if (c == '.') //Loại bỏ phần thập phân
                {
                    break;
                }

                int n = c;
                if(n < 48 || n > 57) //Không chấp nhận ký tự ngoài 0 -> 9
                    continue;

                sb.Append(c);
            }

            value = sb.ToString();
            if (string.IsNullOrEmpty(value) || value == "-")
                return "0";
            
            return value;
        }

        private static bool Editor_ValidateStringDouble(string value)
        {
            return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result);
        }
        
        public static bool Editor_ValidateStringDecimal(string value)
        {
            return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result);
        }
#endif
    }
}