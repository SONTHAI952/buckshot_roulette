using System;
using UnityEditor;
using UnityEngine;
using ZeroX.Editors;
using ZeroX.Extensions;
using ZeroX.MiniFsmSystem;
using ZeroX.MiniFsmSystem.Demo;

namespace ZeroX.Core.MiniFsmSystem.Editors
{
    [CustomPropertyDrawer(typeof(MiniFsm<>), true)]
    public class MiniFsmEditor : PropertyDrawer
    {
        [NonSerialized] private bool isInitialized = false;
        
        //Temp
        private Rect originRect;
        private Rect remainingRect;
        
        private SerializedProperty originSp;
        private object originObj;
        private Type originObjType;
        
        private SerializedProperty entryStatIdSp;

        
        
        

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUI.GetPropertyHeight(property, label, true);

            if (property.isExpanded && EditorApplication.isPlaying)
            {
                height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2; //Thêm 2 dòng cho status và currentState
            }
            
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            originRect = EditorGUI.IndentedRect(position);
            remainingRect = originRect;
            
            originSp = property;
            entryStatIdSp = originSp.FindPropertyRelative("entryStateId");
            
            
            
            //Label Foldout
            Rect justDrawRect = DrawLabel(label);
            
            
            //Expand thì mới vẽ tiếp
            if(property.isExpanded == false)
                return;

            
            //Xuống dòng và thụt vào
            remainingRect = remainingRect.ShiftDown(justDrawRect);
            remainingRect = remainingRect.ShiftRight(15);


            
            if (EditorApplication.isPlaying)
            {
                originObj = property.GetObject();
                originObjType = originObj.GetType();
                
                justDrawRect = DrawStatus();
                remainingRect = remainingRect.ShiftDown(justDrawRect);
                
                justDrawRect = DrawCurrentState();
                remainingRect = remainingRect.ShiftDown(justDrawRect);
            }
            
            
            //EntryState
            justDrawRect = DrawEntryState();
            remainingRect = remainingRect.ShiftDown(justDrawRect);
            
            DrawOtherFields();
        }

        private Rect DrawLabel(GUIContent label)
        {
            Rect rect = remainingRect;
            rect.height = EditorGUIUtility.singleLineHeight;

            GUIStyle style = new GUIStyle(EditorStyles.foldout);
            style.fontStyle = originSp.prefabOverride ? FontStyle.Bold : FontStyle.Normal;
            originSp.isExpanded = EditorGUI.Foldout(rect, originSp.isExpanded, label, true, style);
            
            return rect;
        }

        private Rect DrawEntryState()
        {
            Rect rect = remainingRect;
            rect.height = EditorGUIUtility.singleLineHeight;
            
            EditorGUI.PropertyField(rect, entryStatIdSp);
            return rect;
        }

        private Rect DrawStatus()
        {
            MiniFsmStatus status = originObjType.GetFieldValueDeep<MiniFsmStatus>("status", originObj);
            
            Rect rect = remainingRect;
            rect.height = EditorGUIUtility.singleLineHeight;
            
            
            Rect valueRect = EditorGUI.PrefixLabel(rect, new GUIContent("Status"));
            EditorGUI.LabelField(valueRect, status.ToString());
            
            return rect;
        }

        private Rect DrawCurrentState()
        {
            Rect rect = remainingRect;
            rect.height = EditorGUIUtility.singleLineHeight;
            Rect valueRect = EditorGUI.PrefixLabel(rect, new GUIContent("Current State"));
            
            
            var HasCurrentStateId = originObjType.GetPropertyValueDeep<bool>("HasCurrentStateId", originObj);
            if (HasCurrentStateId == false)
            {
                EditorGUI.LabelField(valueRect, "None");
            }
            else
            {
                Enum CurrentStateId = (Enum)originObjType.GetPropertyValueDeep("CurrentStateId", originObj);
                EditorGUI.LabelField(valueRect, CurrentStateId.ToString());
            }
            
            return rect;
        }
        
        

        private void DrawOtherFields()
        {
            SerializedProperty currentSp = entryStatIdSp;
            SerializedProperty endSp = originSp.GetEndProperty(true);

            while (currentSp.Next(false) && SerializedProperty.EqualContents(currentSp, endSp) == false)
            {
                float height = EditorGUI.GetPropertyHeight(currentSp);
                Rect rect = remainingRect;
                rect.height = height;
                
                EditorGUI.PropertyField(rect, currentSp, true);
                
                remainingRect = remainingRect.ShiftDown(rect);
            }
        }
    }
}