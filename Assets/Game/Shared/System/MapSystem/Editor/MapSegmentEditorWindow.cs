using System;
using UnityEditor;
using UnityEngine;

namespace MapSystem.Editors
{
    public class MapSegmentEditorWindow : EditorWindow
    {
        private MapSegment mapSegmentTemplate;
        private MapSegmentData mapSegmentData;
        private MapSegment mapSegment;

        private bool showMapSegmentDataEditor = false;
        private Editor mapSegmentDataEditor;
        private Vector2 mapSegmentDataEditorScrollPos = Vector2.zero;

        private const string map_segment_editor_window_map_segment_template_path_key = "map_segment_editor_window_map_segment_template_path";
        private const string map_segment_editor_window_map_segment_template_guid_key = "map_segment_editor_window_map_segment_template_guid";
        
        [MenuItem("Tools/Map System/Map Segment Editor")]
        public static void OpenWindow()
        {
            MapSegmentEditorWindow window = (MapSegmentEditorWindow)EditorWindow.GetWindow(typeof(MapSegmentEditorWindow), false, "Map Segment Editor");
            window.Show();
            
            window.AutoPickMapSegmentTemplate();
        }

        private void OnGUI()
        {
            DrawMapSegmentTemplate();
            DrawMapSegmentData();

            if (mapSegmentTemplate != null && mapSegmentData != null)
            {
                DrawButtonLoadSave();
                DrawMapSegment();
                
                GUILayout.Space(30);
                DrawMapSegmentDataEditor();
            }
        }

        void DrawMapSegmentTemplate()
        {
            GUILayout.BeginHorizontal();
            var newMapSegmentTemplate = (MapSegment)EditorGUILayout.ObjectField("Map Segment Template", mapSegmentTemplate, typeof(MapSegment), false);
            if (newMapSegmentTemplate != mapSegmentTemplate)
            {
                mapSegmentTemplate = newMapSegmentTemplate;

                if (mapSegmentTemplate != null)
                {
                    string path = AssetDatabase.GetAssetPath(mapSegmentTemplate);
                    string guid = AssetDatabase.AssetPathToGUID(path);
                    EditorPrefs.SetString(map_segment_editor_window_map_segment_template_path_key, path);
                    EditorPrefs.SetString(map_segment_editor_window_map_segment_template_guid_key, guid);
                }
            }
            
            
            
            if (GUILayout.Button("Latest", GUILayout.Width(80)))
            {
                AutoPickMapSegmentTemplate();
            }
            GUILayout.EndHorizontal();
        }

        void AutoPickMapSegmentTemplate()
        {
            //Uư tiên load guid trước
            string latestGuid = EditorPrefs.GetString(map_segment_editor_window_map_segment_template_guid_key);
            if (string.IsNullOrEmpty(latestGuid) == false)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(latestGuid);
                if (string.IsNullOrEmpty(assetPath) == false)
                {
                    this.mapSegmentTemplate = AssetDatabase.LoadAssetAtPath<MapSegment>(assetPath);
                    if(this.mapSegmentTemplate != null)
                    {
                        EditorPrefs.SetString(map_segment_editor_window_map_segment_template_path_key, assetPath);
                        return;
                    }
                }
            }

            
            //Ko tìm thấy thì load từ path
            string latestPath = EditorPrefs.GetString(map_segment_editor_window_map_segment_template_path_key);
            if (string.IsNullOrEmpty(latestPath) == false)
            {
                this.mapSegmentTemplate = AssetDatabase.LoadAssetAtPath<MapSegment>(latestPath);
                if(this.mapSegmentTemplate != null)
                {
                    string guid = AssetDatabase.AssetPathToGUID(latestPath);
                    EditorPrefs.SetString(map_segment_editor_window_map_segment_template_guid_key, guid);
                    return;
                }
            }
        }

        void DrawMapSegmentData()
        {
            var oldMapSegmentData = mapSegmentData;
            mapSegmentData = (MapSegmentData)EditorGUILayout.ObjectField("Map Segment Data", mapSegmentData, typeof(MapSegmentData), false);

            if (oldMapSegmentData != mapSegmentData)
                mapSegmentDataEditor = null;
        }

        void DrawButtonLoadSave()
        {
            //Load
            string buttonLoadText = mapSegment == null ? "Load" : "Reload";
            if (GUILayout.Button(buttonLoadText))
            {
                if (mapSegment != null)
                {
                    bool result = EditorUtility.DisplayDialog("Reload", "Reload will delete current segment\nAre you sure?", "Yes", "No");
                    if(result == false)
                        return;
                    
                    DestroyImmediate(mapSegment.gameObject);
                }
                
                LoadMapSegmentFromData();
                SelectMapSegmentJustLoaded();
            }

            
            //Save
            if (GUILayout.Button("Save"))
            {
                SaveMapSegmentToData();
            }
        }
        
        void DrawMapSegment()
        {
            mapSegment = (MapSegment)EditorGUILayout.ObjectField("Map Segment", mapSegment, typeof(MapSegment), true);
        }

        void LoadMapSegmentFromData()
        {
            mapSegment = (MapSegment)PrefabUtility.InstantiatePrefab(mapSegmentTemplate);
            mapSegment.transform.SetSiblingIndex(int.MaxValue);
            mapSegment.transform.position = Vector3.zero;
            mapSegment.transform.rotation = Quaternion.identity;
            PrefabUtility.UnpackPrefabInstance(mapSegment.gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
            
            mapSegment.Editor_LoadFromData(mapSegmentData);
        }

        void SaveMapSegmentToData()
        {
            mapSegment.Editor_SaveToData(mapSegmentData);
        }
        
        void DrawMapSegmentDataEditor()
        {
            if(mapSegmentData == null)
                return;
            
            //Title
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Bold;
            showMapSegmentDataEditor = EditorGUILayout.Foldout(showMapSegmentDataEditor, "Map Segment Data Inspector", true, foldoutStyle);

            //Content
            mapSegmentDataEditorScrollPos = EditorGUILayout.BeginScrollView(mapSegmentDataEditorScrollPos);
            if (showMapSegmentDataEditor)
            {
                if (mapSegmentDataEditor == null)
                {
                    mapSegmentDataEditor = Editor.CreateEditor(mapSegmentData);
                }
                mapSegmentDataEditor.OnInspectorGUI();
            }
            
            EditorGUILayout.EndScrollView();
        }

        void SelectMapSegmentJustLoaded()
        {
            if (mapSegment.ObjectContainer.childCount == 0)
            {
                Selection.activeObject = mapSegment.ObjectContainer;
            }
            else
            {
                Selection.activeObject = mapSegment.ObjectContainer.GetChild(0);
            }
            
            EditorGUIUtility.PingObject(mapSegment.gameObject);
        }
    }
}