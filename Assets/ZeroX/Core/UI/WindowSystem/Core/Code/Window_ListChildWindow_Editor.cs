#if UNITY_EDITOR


using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZeroX.WindowSystem
{
    public partial class Window
    {
        [ContextMenu("Clear List Child Window Recursive")]
        private void Editor_ClearListChildWindow_Recursive()
        {
            var listChildWindowClone = listChildWindow.ToList();
            
            UnityEditor.Undo.RecordObject(this, "Clear ListChildWindow");
            listChildWindow.Clear();
            UnityEditor.EditorUtility.SetDirty(this);

            foreach (var childWindow in listChildWindowClone)
            {
                childWindow.Editor_ClearListChildWindow_Recursive();
            }
        }
        
        [ContextMenu("Auto Fill List Child Window")]
        private void Editor_AutoFillListChildWindow()
        {
            var childWindows = Editor_GetListChildWindow();
            
            
            UnityEditor.Undo.RecordObject(this, "Edit ListChildWindow");
            listChildWindow.Clear();
            listChildWindow.AddRange(childWindows);
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        [ContextMenu("Auto Fill List Child Window Recursive")]
        private void Editor_AutoFillListChildWindow_Recursive()
        {
            var childWindows = Editor_GetListChildWindow();
            
            
            UnityEditor.Undo.RecordObject(this, "Edit ListChildWindow");
            listChildWindow.Clear();
            listChildWindow.AddRange(childWindows);
            UnityEditor.EditorUtility.SetDirty(this);

            foreach (var childWindow in listChildWindow)
            {
                childWindow.Editor_AutoFillListChildWindow_Recursive();
            }
        }

        

        private List<Window> Editor_GetListChildWindow()
        {
            var childWindows = GetComponentsInChildren<Window>(true).ToList();

            for (int i = 0; i < childWindows.Count; i++)
            {
                var childWindow = childWindows[i];
                if (childWindow == this)
                {
                    childWindows.RemoveAt(i);
                    i--;
                    continue;
                }

                if (childWindow.transform.parent.GetComponentInParent<Window>(true) != this) //Tức là nó nằm trong 1 window khác
                {
                    childWindows.RemoveAt(i);
                    i--;
                    continue;
                }
            }

            return childWindows;
        }
    }
}


#endif