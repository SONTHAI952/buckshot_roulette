/*using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ZeroX.WindowSystem.Editors
{
    [CustomPropertyDrawer(typeof(WindowSortingConfig))]
    public class WindowSortingConfigEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded == false)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            else
            {
                return 60;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect foldOutRect = new Rect(position.x, position.y, position.width, 15);
            property.isExpanded = EditorGUI.Foldout(foldOutRect, property.isExpanded, label, true);

            if (property.isExpanded)
            {
                //CalculateRect
                var useWindowSortingRect = new Rect(position.x + 15, position.y + 20, position.width - 15, 15);
                var useWindowSortingProperty = property.FindPropertyRelative("useWindowSorting");
                EditorGUI.PropertyField(useWindowSortingRect, useWindowSortingProperty);

                var windowSortingLayerIdRect = new Rect(position.x + 15, position.y + 40, position.width - 15, 15);
                DrawWindowSortingLayerId(windowSortingLayerIdRect, property);
            }
            

            EditorGUI.EndProperty();
        }

        void DrawWindowSortingLayerId(Rect position, SerializedProperty property)
        {
            var windowSortingLayerIdProperty = property.FindPropertyRelative("windowSortingLayerId");
            int selectedId = windowSortingLayerIdProperty.intValue;

            var listWindowSortingLayerData = GetListWindowSortingLayerData();
            List<string> options = listWindowSortingLayerData.Select(d => d.name)
                .Append("Add Window Sorting Layer...")
                .ToList();

            int selectedIndex = -1;
            int newSelectedIndex = -1;
            
            var selectedData = listWindowSortingLayerData.FirstOrDefault(data => data.id == selectedId);
            if (selectedData == null)
            {
                selectedIndex = 0;
                options.Insert(0, string.Format("Missing Window Sorting Layer (id: {0})...", selectedId));

                Color oldBgColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                newSelectedIndex = EditorGUI.Popup(position, "Window Sorting Layer", selectedIndex, options.ToArray());
                GUI.backgroundColor = oldBgColor;
                
                if (newSelectedIndex >= options.Count)
                {
                    EditorGUIUtility.PingObject(WindowSortingTable.Instance);
                    Selection.activeObject = WindowSortingTable.Instance;
                }
                else
                {
                    if (newSelectedIndex > 0)
                    {
                        var data = listWindowSortingLayerData[newSelectedIndex - 1];
                        windowSortingLayerIdProperty.intValue = data.id;
                    }
                }
            }
            else
            {
                selectedIndex = listWindowSortingLayerData.IndexOf(selectedData);
                newSelectedIndex = EditorGUI.Popup(position, "Window Sorting Layer", selectedIndex, options.ToArray());
                
                if (newSelectedIndex >= options.Count - 1)
                {
                    EditorGUIUtility.PingObject(WindowSortingTable.Instance);
                    Selection.activeObject = WindowSortingTable.Instance;
                }
                else
                {
                    var data = listWindowSortingLayerData[newSelectedIndex];
                    windowSortingLayerIdProperty.intValue = data.id;
                }
            }
        }

        List<WindowSortingLayerData> GetListWindowSortingLayerData()
        {
            var instance = WindowSortingTable.Instance;
            if(instance == null)
                return new List<WindowSortingLayerData>();

            BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var fieldInfo = instance.GetType().GetField("listWindowSortingLayerData", bindingFlags);
            if (fieldInfo == null)
            {
                Debug.LogError("Cannot find field with name: listWindowSortingLayerData");
                return new List<WindowSortingLayerData>();
            }

            return (List<WindowSortingLayerData>)fieldInfo.GetValue(instance);
        }
    }
}*/