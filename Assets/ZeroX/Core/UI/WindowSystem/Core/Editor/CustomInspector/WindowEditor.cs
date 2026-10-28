using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ZeroX.UI;

namespace ZeroX.WindowSystem.Editors
{
    [CustomEditor(typeof(Window))]
    [CanEditMultipleObjects]
    public class WindowEditor : Editor
    {
        [NonSerialized] private bool isInitialized;
        
        //Transition
        private SerializedProperty transitionConfigModeSp;
        private SerializedProperty globalTransitionIdSp;
        private SerializedProperty transitionConfigSp;
        private List<SerializedProperty> listTransitionSp = new List<SerializedProperty>();
        
        //Global Transition Config
        private GlobalWindowTransitionTable globalWindowTransitionTable;
        private SerializedObject globalWindowTransitionTableSo;
        private SerializedProperty globalWindowTransitionTable_ListRow_So;
        
        //Misc
        //private List<SerializedProperty> listSp_Ref = new List<SerializedProperty>();
        private SerializedProperty contentSp;
        private SerializedProperty canvasGroupSp;
        private SerializedProperty canvasSp;
        
        
        private SerializedProperty actionTypeWhenClosedSp;
        private SerializedProperty useGuiCameraSp;
        private SerializedProperty sortingConfigSp;
        private List<SerializedProperty> listSp_AdvancedConfig = new List<SerializedProperty>();
        
        
        //Temp - Style
        private GUIStyle foldoutStyle_Bold;
        
        
        //Const
        private const int INDENT_SPACING = 15;
        private const string fName_useWindowSorting = "useWindowSorting";

        

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            InitIfNot();
            InitGlobalWindowTransitionTableSo();
            
            //Draw
            DrawUseGuiCamera();
            GUILayout.Space(10);
            
            DrawSortingConfig();
            GUILayout.Space(10);
            
            DrawRefConfig();
            GUILayout.Space(10);
            
            DrawTransitionConfigs();
            GUILayout.Space(10);
            
            DrawGroupConfig("Advanced", listSp_AdvancedConfig, false);
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void InitIfNot()
        {
            if(isInitialized)
                return;

            isInitialized = true;
            
            
            //SP - Transition
            transitionConfigModeSp = serializedObject.FindProperty("transitionConfigMode");
            globalTransitionIdSp = serializedObject.FindProperty("globalTransitionId");
            transitionConfigSp = serializedObject.FindProperty("transitionConfig");
            listTransitionSp = new List<SerializedProperty>(){transitionConfigModeSp, globalTransitionIdSp, transitionConfigSp};
            
            //Ref
            contentSp = serializedObject.FindProperty("content");
            canvasGroupSp = serializedObject.FindProperty("canvasGroup");
            canvasSp = serializedObject.FindProperty("canvas");
            
            //Use GUICamera
            useGuiCameraSp = serializedObject.FindProperty("useGuiCamera");
            
            //Sorting Config
            sortingConfigSp = serializedObject.FindProperty("sortingConfig");
            
            //Misc Config
            actionTypeWhenClosedSp = serializedObject.FindProperty("actionTypeWhenClosed");
            listSp_AdvancedConfig.Add(actionTypeWhenClosedSp);
            listSp_AdvancedConfig.Add(serializedObject.FindProperty("firstAction"));
            listSp_AdvancedConfig.Add(serializedObject.FindProperty("listChildWindow"));
            listSp_AdvancedConfig.Add(serializedObject.FindProperty("windowEvents"));
            
            
            //GUI Camera For Canvas
            AutoSetGuiCameraForCanvas();
            
            
            //Style
            foldoutStyle_Bold = new GUIStyle(EditorStyles.foldout);
            foldoutStyle_Bold.fontStyle = FontStyle.Bold;
        }

        private void InitGlobalWindowTransitionTableSo()
        {
            globalWindowTransitionTable = GlobalWindowTransitionTable.GetInstanceWithoutLog();
            if (globalWindowTransitionTable == null)
            {
                globalWindowTransitionTableSo = null;
                globalWindowTransitionTable_ListRow_So = null;
                return;
            }
            
            if (globalWindowTransitionTableSo != null && globalWindowTransitionTableSo.targetObject == globalWindowTransitionTable)
            {
                return;
            }
            
            
            globalWindowTransitionTableSo = new SerializedObject(globalWindowTransitionTable);
            globalWindowTransitionTable_ListRow_So = globalWindowTransitionTableSo.FindProperty("listRow");
        }

        private void DrawTransitionConfigs()
        {
            bool isFoldout = BeginGroup("Transition", "Transition", listTransitionSp);
            if (isFoldout == false)
            {
                EndGroup();
                return;
            }
                


            EditorGUILayout.PropertyField(transitionConfigModeSp, new GUIContent("Mode"));
            GUILayout.Space(15);
            
            
            WindowTransitionConfigMode transitionConfigType = (WindowTransitionConfigMode)transitionConfigModeSp.intValue;
            if (transitionConfigType == WindowTransitionConfigMode.Global)
            {
                DrawGlobalTransitionConfig();
            }
            else
            {
                DrawCustomTransitionConfig();
            }
            
            EndGroup();
        }

        private void DrawGlobalTransitionConfig()
        {
            if (globalWindowTransitionTable == null)
            {
                EditorGUILayout.HelpBox("Global Window Transition Table not exist! You can create one in the Tools/ZeroX/Window System menu.", MessageType.Error);
                return;
            }
            
            
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.PropertyField(globalTransitionIdSp, new GUIContent("Global Id"));
            
            if (GUILayout.Button("Open", GUILayout.Width(45)))
            {
                Selection.activeObject = globalWindowTransitionTable;
                EditorGUIUtility.PingObject(globalWindowTransitionTable);
            }
            EditorGUILayout.EndHorizontal();
            
            
            
            //Tìm rowIndex
            GlobalWindowTransitionId id = (GlobalWindowTransitionId)globalTransitionIdSp.intValue;
            int rowIndex = globalWindowTransitionTable.RawRowsI.FindIndex(r => r.id == id);
            if (rowIndex == -1)
            {
                EditorGUILayout.HelpBox("Global Window Transition row not exist with id: " + id, MessageType.Error);
                GUILayout.EndHorizontal();
                return;
            }
            
            
            //Có row nên vẽ
            SerializedProperty rowSp = globalWindowTransitionTable_ListRow_So.GetArrayElementAtIndex(rowIndex);
            SerializedProperty configSp = rowSp.FindPropertyRelative("config");
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(configSp, new GUIContent("Global Config"));
            EditorGUI.EndDisabledGroup();
        }

        private void DrawCustomTransitionConfig()
        {
            EditorGUILayout.PropertyField(transitionConfigSp, new GUIContent("Custom Config"));
        }

        private void DrawUseGuiCamera()
        {
            EditorGUILayout.PropertyField(useGuiCameraSp);
        }

        private void DrawSortingConfig()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Space(10);
            
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(sortingConfigSp);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();
        }

        private bool AnyRefConfigNeedWarning()
        {
            if (contentSp.objectReferenceValue == null)
                return true;

            if (canvasGroupSp.objectReferenceValue == null)
                return true;

            if (canvasSp.objectReferenceValue == null)
            {
                if (actionTypeWhenClosedSp.intValue == (int)ActionTypeWhenClosed.TurnOffCanvas ||
                    sortingConfigSp.FindPropertyRelative(fName_useWindowSorting).boolValue ||
                    useGuiCameraSp.boolValue)
                    return true;
            }

            return false;
        }
        
        private void DrawRefConfig()
        {
            Color oldBgColor = GUI.backgroundColor;
            
            
            //Nếu đang collapse mà có bất kỳ ref nào cần warning thì sẽ tô đỏ cả group
            bool isFoldout = IsFoldout("Reference");
            if (isFoldout == false)
            {
                if (AnyRefConfigNeedWarning())
                {
                    GUI.backgroundColor = Color.red;
                }
            }
            
            //Begin Group
            bool isBold = contentSp.prefabOverride || canvasGroupSp.prefabOverride || canvasSp.prefabOverride;
            isFoldout = BeginGroup("Reference", "Reference", isBold);
            GUI.backgroundColor = oldBgColor;
            
            //Nếu group collapse thì ko vẽ phần phía dưới
            if (isFoldout == false)
            {
                EndGroup();
                return;
            }
            
            
            
            
            
            //Content
            GUI.backgroundColor = contentSp.objectReferenceValue == null ? Color.red : oldBgColor;
            EditorGUILayout.PropertyField(contentSp);
            
            
            //Canvas Group
            GUI.backgroundColor = canvasGroupSp.objectReferenceValue == null ? Color.red : oldBgColor;
            EditorGUILayout.PropertyField(canvasGroupSp);

            
            //Canvas
            GUI.backgroundColor = oldBgColor;
            if (canvasSp.objectReferenceValue == null)
            {
                if (actionTypeWhenClosedSp.intValue == (int)ActionTypeWhenClosed.TurnOffCanvas ||
                    sortingConfigSp.FindPropertyRelative(fName_useWindowSorting).boolValue ||
                    useGuiCameraSp.boolValue)
                    GUI.backgroundColor = Color.red;
            }
            
            EditorGUILayout.PropertyField(canvasSp);

            GUI.backgroundColor = oldBgColor;
            
            EndGroup();
        }
        

        #region Utility

        private bool IsFoldout(string saveKeyFoldout)
        {
            saveKeyFoldout = "ZeroX.Window.Foldout." + saveKeyFoldout;
            return EditorPrefs.GetBool(saveKeyFoldout);
        }

        private void SetIsFoldout(string saveKeyFoldout, bool value)
        {
            saveKeyFoldout = "ZeroX.Window.Foldout." + saveKeyFoldout;
            EditorPrefs.SetBool(saveKeyFoldout, value);
        }
        
        private bool BeginGroup(string label, string saveKeyFoldout, bool isBold)
        {
            var foldout = IsFoldout(saveKeyFoldout);
            
            
            
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Space(10);
            EditorGUILayout.BeginVertical();
            GUILayout.Space(3);
            
            
            foldout = EditorGUILayout.Foldout(foldout, label, true, isBold ? foldoutStyle_Bold : EditorStyles.foldout);
            SetIsFoldout(saveKeyFoldout, foldout);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(INDENT_SPACING);
            EditorGUILayout.BeginVertical();
            
            return foldout;
        }

        //Trả về có foldout hay không
        private bool BeginGroup(string label, string saveKeyFoldout, List<SerializedProperty> listSp)
        {
            bool isBold = listSp.Any(s => s.prefabOverride);
            return BeginGroup(label, saveKeyFoldout, isBold);
        }

        private void EndGroup()
        {
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
                
            GUILayout.Space(3);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawGroupConfig(string groupName, List<SerializedProperty> listSp, bool addSpacing)
        {
            bool isFoldout = BeginGroup(groupName, groupName, listSp);

            if (isFoldout)
            {
                foreach (var sp in listSp)
                {
                    if(addSpacing)
                        GUILayout.Space(7);
                    
                    EditorGUILayout.PropertyField(sp);
                    
                    if(addSpacing)
                        GUILayout.Space(7);
                }
            }

            EndGroup();
        }

        #endregion



        private void AutoSetGuiCameraForCanvas()
        {
            if(EditorApplication.isPlaying)
                return;
            
            foreach (var t in targets)
            {
                Window window = (Window)t;
                AutoSetGuiCameraForCanvas(window);
            }
        }

        private void AutoSetGuiCameraForCanvas(Window window)
        {
            //Nếu được mở trong prefab edit mode thì đừng set gui camera
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if(prefabStage != null && prefabStage.IsPartOfPrefabContents(window.gameObject))
                return;
            
            //Nếu là prefab asset thì đừng set gui camera
            if(PrefabUtility.IsPartOfPrefabAsset(window.gameObject))
                return;
            
            if(window.UseGuiCamera == false)
                return;
            
            if (window.Canvas == null)
                return;
            
            if(window.Canvas.renderMode == RenderMode.WorldSpace)
                return;
            
            var guiCamera = FindObjectOfType<GUICamera>();
            if (guiCamera == null)
                return;
           
            window.Canvas.worldCamera = guiCamera.GetComponentInChildren<Camera>();
            EditorUtility.SetDirty(window.Canvas);
        }
    }
}