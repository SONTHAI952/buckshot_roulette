using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZeroX.ZGridEditors
{
    public class ZGridEditorWindow : EditorWindow
    {
        private Object targetUnityObject;
        private string gridSpPath;
        
        private SerializedObject serializedObject;
        private GridSp gridSp;
        
        
        //Const
        public const float RowColumnControl_ContainerSize = 35;
        public const float RowColumnControl_ElementSize = RowColumnControl_ContainerSize - GridEditorStyle.BgPadding * 2;
        public const float ButtonInsertSize = 18;
        public const float RowColumnControl_MinSize = 18;
        
        public const float GridContentPaddingLast = 60; //Vùng thừa ra trong content để nhìn cho thoáng

        public const float MinCellSize = ButtonInsertSize + RowColumnControl_MinSize;
        
        //ScrollView
        private Vector2 gridContentScrollPosition = Vector2.zero;
        private DictCellScrollPos dictCellScrollPos = new DictCellScrollPos();
        
        
        //Action
        public readonly List<Action> listEditGridAction = new List<Action>(); //Danh sách các hành động sửa grid cần thực hiện
        
        
        //Order
        private GridOrder gridOrder = new GridOrder();
        
        
        
        public static ZGridEditorWindow GetInstance()
        {
            var window = GetWindow<ZGridEditorWindow>();
            window.titleContent = new GUIContent("Grid Editor");

            return window;
        }

        public void Open(SerializedProperty gridSp)
        {
            targetUnityObject = gridSp.serializedObject.targetObject;
            gridSpPath = gridSp.propertyPath;

            //Init bên dưới sẽ tự tạo ra serializeObject sau
            serializedObject = null;
            this.gridSp = null;
        }
        
        

        #region Unity Method

        private void OnGUI()
        {
            InitializeTarget();
            
            if (gridSp == null)
            {
                DrawGuide();
                return;
            }
            
            
            
            serializedObject.Update();
            
            InitializeEditor();

            EditorGUILayout.BeginHorizontal();
            DrawTargetObject();
            DrawTitle();
            GUILayout.Space(150);
            EditorGUILayout.EndHorizontal();
            
            //GUILayout.Space(15);
            DrawToolbar();
            GUILayout.Space(15);
            DrawGrid();
            
            
            //Order Grid
            gridOrder.Handle();
            
            
            //Edit Grid Action
            ExecuteListEditGridAction();

            
            serializedObject.ApplyModifiedProperties();
            
        }

        #endregion


        #region Utility

        private void InitializeTarget()
        {
            if(gridSp != null)
                return;
            
            if(targetUnityObject == null)
                return;
            
            serializedObject = new SerializedObject(targetUnityObject);
            var gridSpTemp = serializedObject.FindProperty(gridSpPath);
            gridSp = gridSpTemp == null ? null : new GridSp(gridSpTemp);
        }

        private void InitializeEditor()
        { 
            gridOrder.SetContext(this, gridSp);
        }

        private void ExecuteListEditGridAction()
        {
            var listClone = listEditGridAction.ToArray();
            listEditGridAction.Clear();

            foreach (var action in listClone)
            {
                action.Invoke();
            }
        }

        private void ForBaseOnGridPivot(int startIndex, int endIndex, Action<int> action)
        {
            GridPivotType gridPivotType = GridEditorPref.GetGridPivotType(gridSp);

            if (gridPivotType == GridPivotType.TopLeft)
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    action.Invoke(i);
                }
            }
            else
            {
                for (int i = endIndex; i >= startIndex; i--)
                {
                    action.Invoke(i);
                }
            }
        }

        private bool IsRightClickUpInLastRect()
        {
            Rect rect = GUILayoutUtility.GetLastRect();
            Event ev = Event.current;
            return ev.type == EventType.MouseUp && ev.button == 1 && rect.Contains(ev.mousePosition);
        }

        #endregion


        #region Utility Draw

        private void DrawLabelFieldWithContext(SerializedProperty fieldSp, string label, params GUILayoutOption[] options)
        {
            var labelContent = new GUIContent(label);

            var style = fieldSp.prefabOverride ? EditorStyles.boldLabel : EditorStyles.label;
            GUILayout.Label(labelContent, style, options);
            
            //Vẽ vùng thao tác context
            EditorGUI.BeginProperty(GUILayoutUtility.GetLastRect(), labelContent, fieldSp);
            EditorGUI.EndProperty();
        }

        private float FloatFieldWithSlide(string label, float value, float labelWidth, params GUILayoutOption[] options)
        {
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelWidth;
            value = EditorGUILayout.FloatField(label, value, options);
            EditorGUIUtility.labelWidth = oldLabelWidth;

            return value;
        }

        #endregion
        
        


        #region Draw

        private void DrawGuide()
        {
            EditorGUILayout.HelpBox("Hãy chọn một grid để bắt đầu edit", MessageType.Info);
        }

        private void DrawTargetObject()
        {
            bool oldEnabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.ObjectField(targetUnityObject, typeof(Object), true, GUILayout.Width(150));
            GUI.enabled = oldEnabled;
        }
        
        private void DrawTitle()
        {
            GUILayout.Label(gridSp.DisplayName, GridEditorStyle.BigTitleMiddle);
        }
        
        #endregion


        #region Draw Toolbar

        private void DrawToolbar()
        {
            GridGUILayout.BeginVerticalBG();
            
            
            //Row 1
            GUILayout.BeginHorizontal(GUILayout.Height(25));
            DrawGridSize();
            GUILayout.FlexibleSpace();
            DrawCellSize();
            GUILayout.EndHorizontal();
            
            
            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            
            
            //Row 2
            GUILayout.BeginHorizontal(GUILayout.Height(25));
            DrawEnableLabel();
            GUILayout.FlexibleSpace();
            DrawLabelWidth();
            GUILayout.EndHorizontal();
            
            
            GridGUILayout.EndVerticalBG();
        }

        private void DrawGridSize()
        {
            //Label
            GUILayout.Label("Grid Size", GUILayout.Width(80));
            //EditorGUILayout.Space(5);
            
            //Width
            int width = gridSp.Width;
            GUILayout.Label("W", GUILayout.ExpandHeight(true));
            int newWidth = EditorGUILayout.IntField(width, GUILayout.Width(30));
            if(newWidth < 0)
                newWidth = 0;
            if (newWidth != width)
                listEditGridAction.Add(() => gridSp.SetWidth(newWidth));
            
            //X
            GUILayout.Label("x");
            
            
            //Height
            var height = gridSp.Height;
            GUILayout.Label("H", GUILayout.ExpandHeight(true));
            int newHeight = EditorGUILayout.IntField(height, GUILayout.Width(30));
            if (newHeight < 0)
                newHeight = 0;
            if(newHeight != height)
                listEditGridAction.Add(() => gridSp.SetHeight(newHeight));
        }

        private void DrawCellSize()
        {
            Vector2 cellSize = GridEditorPref.GetCellSize(gridSp);
            
            //Label
            GUILayout.Label("Cell Size");
            EditorGUILayout.Space(5);
            
            //Width
            cellSize.x = FloatFieldWithSlide("W", cellSize.x, 14, GUILayout.Width(55));
            
            //X
            GUILayout.Label("x");
            
            
            //Height
            cellSize.y = FloatFieldWithSlide("H", cellSize.y, 12, GUILayout.Width(55));

            
            //Save
            cellSize.x = Mathf.Max(cellSize.x, MinCellSize);
            cellSize.y = Mathf.Max(cellSize.y, MinCellSize);
            GridEditorPref.SetCellSize(gridSp, cellSize);
        }

        private void DrawLabelWidth()
        {
            float cellLabelWidth = GridEditorPref.GetCellLabelWidth(gridSp);
            cellLabelWidth = FloatFieldWithSlide("Label Width", cellLabelWidth, 83, GUILayout.Width(195));
            GridEditorPref.SetCellLabelWidth(gridSp, cellLabelWidth);
        }

        private void DrawEnableLabel()
        {
            //Label
            GUILayout.Label("Enable Label");
            EditorGUILayout.Space(5);
            
            //Vale
            bool enableCellLabel = GridEditorPref.GetEnableCellLabel(gridSp);
            enableCellLabel = EditorGUILayout.Toggle(enableCellLabel, GUILayout.Width(18));
            GridEditorPref.SetEnableCellLabel(gridSp, enableCellLabel);
        }

        #endregion

        #region Draw Grid

        private void DrawGrid()
        {
            //EmptyBox - ColumnControl
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.Height(RowColumnControl_ContainerSize));
            
            DrawButtonChangeGridPivot();
            DrawGrid_AllColumnControl();
            
            GUILayout.EndHorizontal();
            

            
            //RowControl - Content
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            DrawGrid_AllRowControl();
            DrawGrid_Content();
            GUILayout.EndHorizontal();
        }

        private void DrawButtonChangeGridPivot()
        {
            var gridPivotType = GridEditorPref.GetGridPivotType(gridSp);
            Texture2D icon = gridPivotType == GridPivotType.TopLeft ? GridEditorMedia.GridPivot_TopLeft : GridEditorMedia.GridPivot_BottomLeft;
            
            
            
            bool isClicked = GridGUILayout.Button(new GUIContent(icon),
                GUILayout.Width(RowColumnControl_ContainerSize),
                GUILayout.Height(RowColumnControl_ContainerSize));
            
            if(isClicked == false)
                return;
            
            
            if(gridPivotType == GridPivotType.TopLeft)
                GridEditorPref.SetGridPivotType(gridSp, GridPivotType.BottomLeft);
            else
                GridEditorPref.SetGridPivotType(gridSp, GridPivotType.TopLeft);
        }
        
        #endregion
        
        

        #region Grid Column Control

        private void DrawGrid_AllColumnControl()
        {
            //Các data cần thiết
            Vector2 cellSize = GridEditorPref.GetCellSize(gridSp);
            var width = gridSp.Width;
            var height = gridSp.Height;
            
            
            GridGUILayout.BeginHorizontalBG(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            Vector2 scrollPos = new Vector2(gridContentScrollPosition.x, 0);
            GridGUILayout.BeginScrollViewNoScrollBar(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            GUILayout.BeginHorizontal();
            for (int x = 0; x < width; x++)
            {
                DrawButtonInsertColumn(x);
                DrawGrid_ColumnControl(x, cellSize);
            }
            
            //Column mới
            DrawButtonInsertColumn(width);
            GUILayout.Space(GridContentPaddingLast + 100); //100 là phần cộng thêm để luôn dài hơn content để scroll ko bị lệch. Bao gồm cả khi scrollBar hiện lên
            
            GUILayout.EndHorizontal();
            
            
            
            GridGUILayout.EndScrollViewNoScrollBar();
            GridGUILayout.EndHorizontalBG();
        }

        private void DrawButtonInsertColumn(int x)
        {
            bool isClicked = GridGUILayout.Button("+", Color.green, GUILayout.Width(ButtonInsertSize), GUILayout.ExpandHeight(true));
            if(isClicked == false)
                return;
            
            listEditGridAction.Add(() => gridSp.InsertColumn(x));
        }

        private void DrawGrid_ColumnControl(int x, Vector2 cellSize)
        {
            GridGUILayout.BeginHorizontalBG(GUILayout.Width(cellSize.x - ButtonInsertSize), GUILayout.ExpandHeight(true));
            GUILayout.Label(x.ToString(), GridEditorStyle.MiddleLabel,
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true),
                GUILayout.MinWidth(RowColumnControl_MinSize - GridEditorStyle.BgPadding * 2));
            GridGUILayout.EndHorizontalBG();
            
            
            //Xử lý khi nhấn chuột phải vào control
            if (IsRightClickUpInLastRect())
            {
                Event.current.Use();
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Delete Column"), false, () => listEditGridAction.Add(() => gridSp.RemoveColumn(x)));
                menu.ShowAsContext();
                return;
            }
            
            //Xử lý order
            gridOrder.OnAfterDrawColumnControl(x);
        }

        #endregion



        #region Grid Row Control
        
        private void DrawGrid_AllRowControl()
        {
            //Các data cần thiết
            Vector2 cellSize = GridEditorPref.GetCellSize(gridSp);
            var width = gridSp.Width;
            var height = gridSp.Height;
            
            
            
            GridGUILayout.BeginVerticalBG(GUILayout.Width(RowColumnControl_ContainerSize), GUILayout.ExpandHeight(true));
            Vector2 scrollPos = new Vector2(0, gridContentScrollPosition.y);
            GridGUILayout.BeginScrollViewNoScrollBar(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            
            
            var gridPivotType = GridEditorPref.GetGridPivotType(gridSp);
            if (gridPivotType == GridPivotType.TopLeft)
            {
                for (int y = 0; y < height; y++)
                {
                    DrawButtonInsertRow(y);
                    DrawGrid_RowControl(y, cellSize);
                }
                
                //Row mới
                DrawButtonInsertRow(height);
            }
            else
            {
                //Row mới
                DrawButtonInsertRow(height);
                
                for (int y = height - 1; y >= 0; y--)
                {
                    DrawGrid_RowControl(y, cellSize);
                    DrawButtonInsertRow(y);
                }
            }
            
            GUILayout.Space(GridContentPaddingLast + 100); //100 là phần cộng thêm để luôn dài hơn content để scroll ko bị lệch. Bao gồm cả khi scrollBar hiện lên

            
            
            
            GridGUILayout.EndScrollViewNoScrollBar();
            GridGUILayout.EndVerticalBG();
        }
        
        private void DrawButtonInsertRow(int y)
        {
            bool isClicked = GridGUILayout.Button("+", Color.green, GUILayout.Height(ButtonInsertSize), GUILayout.ExpandWidth(true));
            if(isClicked == false)
                return;
            
            listEditGridAction.Add(() => gridSp.InsertRow(y));
        }
        
        private void DrawGrid_RowControl(int y, Vector2 cellSize)
        {
            GridGUILayout.BeginVerticalBG(GUILayout.ExpandWidth(true), GUILayout.Height(cellSize.y - ButtonInsertSize));
            GUILayout.Label(y.ToString(), GridEditorStyle.MiddleLabel,
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true),
                GUILayout.MinHeight(RowColumnControl_MinSize - GridEditorStyle.BgPadding * 2));
            GridGUILayout.EndVerticalBG();
            
            
            //Xử lý khi nhấn chuột phải vào control
            if (IsRightClickUpInLastRect())
            {
                Event.current.Use();
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Delete Row"), false, () => listEditGridAction.Add(() => gridSp.RemoveRow(y)));
                menu.ShowAsContext();
                return;
            }

            
            //Xử lý order
            gridOrder.OnAfterDrawRowControl(y);
        }

        #endregion
        
        
        #region Grid Content
        
        private void DrawGrid_Content()
        {
            //Các data cần thiết
            Vector2 cellSize = GridEditorPref.GetCellSize(gridSp);
            var width = gridSp.Width;
            var height = gridSp.Height;
            
            
            
            
            GridGUILayout.BeginVerticalBG(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)); //Background
            gridContentScrollPosition = EditorGUILayout.BeginScrollView(gridContentScrollPosition, false, false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.Space(ButtonInsertSize / 2f);
            
            
            //Các config khi vẽ cell
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = GridEditorPref.GetCellLabelWidth(gridSp);
            bool enableCellLabel = GridEditorPref.GetEnableCellLabel(gridSp);
            
            //Vẽ các row
            ForBaseOnGridPivot(0, height - 1, y =>
            {
                var listCellSp = gridSp.GetListCellSp(y);
                DrawGrid_Row(listCellSp, width, y, cellSize, enableCellLabel);
            });
            
            EditorGUIUtility.labelWidth = oldLabelWidth;
            
            
            
            //Space so với bottom
            GUILayout.Space(GridContentPaddingLast);
            
            
            
            
            
            EditorGUILayout.EndScrollView();
            gridOrder.OnAfterDrawGridContentScrollView(gridContentScrollPosition, width, height, cellSize);
            GridGUILayout.EndVerticalBG();
        }

        private void DrawGrid_Row(SerializedProperty listCellSp, int width, int y, Vector2 cellSize, bool enableCellLabel)
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Space(ButtonInsertSize / 2f);

            for (int x = 0; x < width; x++)
            {
                var cellSp = listCellSp.GetArrayElementAtIndex(x);
                DrawGrid_Cell(cellSp, x, y, cellSize, enableCellLabel);
            }
                
            //Space ở bên phải
            GUILayout.Space(GridContentPaddingLast);
                
            GUILayout.EndHorizontal();
        }

        private void DrawGrid_Cell(SerializedProperty cellSp, int x, int y, Vector2 cellSize, bool enableLabel)
        {
            GridGUILayout.BeginVerticalBG(GUILayout.Width(cellSize.x), GUILayout.Height(cellSize.y));
            
            
            //ScrollView
            Vector2 cellScrollPos = dictCellScrollPos.Get(x, y);
            cellScrollPos = EditorGUILayout.BeginScrollView(cellScrollPos, false, false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            dictCellScrollPos.Set(x, y, cellScrollPos);
            
            
            //Nội dung cell
            cellSp.isExpanded = true;
            if(enableLabel)
                EditorGUILayout.PropertyField(cellSp, true, GUILayout.Width(cellSize.x - 26));
            else
                EditorGUILayout.PropertyField(cellSp, GUIContent.none, true, GUILayout.Width(cellSize.x - 26));
            
            
            
            EditorGUILayout.EndScrollView();
            GridGUILayout.EndVerticalBG();
        }
        #endregion
    }
}