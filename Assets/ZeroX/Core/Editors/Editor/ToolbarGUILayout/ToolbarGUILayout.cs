using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ZeroX.Editors
{
	[InitializeOnLoad]
	public static class ToolbarGUILayout
	{
		static int m_toolCount;
		static GUIStyle m_commandStyle = null;

		public static readonly List<Action> LeftToolbarGUI_AlignLeft = new List<Action>();
		public static readonly List<Action> LeftToolbarGUI_AlignRight = new List<Action>();
		
		public static readonly List<Action> RightToolbarGUI_AlignLeft = new List<Action>();
		public static readonly List<Action> RightToolbarGUI_AlignRight = new List<Action>();
		
		

		static ToolbarGUILayout()
		{
			Type toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
			
#if UNITY_2019_1_OR_NEWER
			string fieldName = "k_ToolCount";
#else
			string fieldName = "s_ShownToolIcons";
#endif
			
			FieldInfo toolIcons = toolbarType.GetField(fieldName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			
#if UNITY_2019_3_OR_NEWER
			m_toolCount = toolIcons != null ? ((int) toolIcons.GetValue(null)) : 8;
#elif UNITY_2019_1_OR_NEWER
			m_toolCount = toolIcons != null ? ((int) toolIcons.GetValue(null)) : 7;
#elif UNITY_2018_1_OR_NEWER
			m_toolCount = toolIcons != null ? ((Array) toolIcons.GetValue(null)).Length : 6;
#else
			m_toolCount = toolIcons != null ? ((Array) toolIcons.GetValue(null)).Length : 5;
#endif
	
			ToolbarCallback.OnToolbarGUI = OnGUI;
			ToolbarCallback.OnToolbarGUILeft = GUILeft;
			ToolbarCallback.OnToolbarGUIRight = GUIRight;
		}

#if UNITY_2019_3_OR_NEWER
		public const float space = 8;
		public const float totalHeight = 22;
#else
		public const float space = 10;
		public const float totalHeight = 24;
#endif
		public const float largeSpace = 20;
		public const float buttonWidth = 32;
		public const float dropdownWidth = 80;
#if UNITY_2019_1_OR_NEWER
		public const float playPauseStopWidth = 140;
#else
		public const float playPauseStopWidth = 100;
#endif

		static void OnGUI()
		{
			Debug.Log("lạ");
			// Create two containers, left and right
			// Screen is whole toolbar

			if (m_commandStyle == null)
			{
				m_commandStyle = new GUIStyle("CommandLeft");
			}

			var screenWidth = EditorGUIUtility.currentViewWidth;

			// Following calculations match code reflected from Toolbar.OldOnGUI()
			float playButtonsPosition = Mathf.RoundToInt ((screenWidth - playPauseStopWidth) / 2);

			Rect leftRect = new Rect(0, 0, screenWidth, Screen.height);
			leftRect.xMin += space; // Spacing left
			leftRect.xMin += buttonWidth * m_toolCount; // Tool buttons
#if UNITY_2019_3_OR_NEWER
			leftRect.xMin += space; // Spacing between tools and pivot
#else
			leftRect.xMin += largeSpace; // Spacing between tools and pivot
#endif
			leftRect.xMin += 64 * 2; // Pivot buttons
			leftRect.xMax = playButtonsPosition;

			Rect rightRect = new Rect(0, 0, screenWidth, Screen.height);
			rightRect.xMin = playButtonsPosition;
			rightRect.xMin += m_commandStyle.fixedWidth * 3; // Play buttons
			rightRect.xMax = screenWidth;
			rightRect.xMax -= space; // Spacing right
			rightRect.xMax -= dropdownWidth; // Layout
			rightRect.xMax -= space; // Spacing between layout and layers
			rightRect.xMax -= dropdownWidth; // Layers
#if UNITY_2019_3_OR_NEWER
			rightRect.xMax -= space; // Spacing between layers and account
#else
			rightRect.xMax -= largeSpace; // Spacing between layers and account
#endif
			rightRect.xMax -= dropdownWidth; // Account
			rightRect.xMax -= space; // Spacing between account and cloud
			rightRect.xMax -= buttonWidth; // Cloud
			rightRect.xMax -= space; // Spacing between cloud and collab
			rightRect.xMax -= 78; // Colab

			// Add spacing around existing controls
			leftRect.xMin += space;
			leftRect.xMax -= space;
			rightRect.xMin += space;
			rightRect.xMax -= space;

			// Add top and bottom margins
#if UNITY_2019_3_OR_NEWER
			leftRect.y = 4;
			leftRect.height = totalHeight;
			rightRect.y = 4;
			rightRect.height = totalHeight;
#else
			leftRect.y = 5;
			leftRect.height = totalHeight;
			rightRect.y = 5;
			rightRect.height = totalHeight;
#endif
			
			if (leftRect.width > 0)
			{
				//Align Left
				GUILayout.BeginArea(leftRect);
				GUILayout.BeginHorizontal();
				foreach (var handler in LeftToolbarGUI_AlignLeft)
				{
					handler();
				}
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
				
				
				//Align Right
				GUILayout.BeginArea(leftRect);
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				foreach (var handler in LeftToolbarGUI_AlignRight)
				{
					handler();
				}
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
			}

			if (rightRect.width > 0)
			{
				//Align Left
				GUILayout.BeginArea(rightRect);
				GUILayout.BeginHorizontal();
				foreach (var handler in RightToolbarGUI_AlignLeft)
				{
					handler();
				}

				GUILayout.EndHorizontal();
				GUILayout.EndArea();
				
				
				//Align Right
				GUILayout.BeginArea(rightRect);
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				foreach (var handler in RightToolbarGUI_AlignRight)
				{
					handler();
				}

				GUILayout.EndHorizontal();
				GUILayout.EndArea();
			}
		}
		
		public static void GUILeft() 
		{
			//Align Left
			GUILayout.BeginHorizontal(GUILayout.Height(totalHeight));
			GUILayout.Label("", GUILayout.Width(0), GUILayout.Height(totalHeight)); //Có ít nhất 1 gui để tạo area, như vậy Space phía dưới mới đúng
			foreach (var handler in LeftToolbarGUI_AlignLeft)
			{
				handler();
			}
			GUILayout.EndHorizontal();
			
			
			//Align Right
			GUILayout.Space(-totalHeight - EditorGUIUtility.standardVerticalSpacing);
			GUILayout.BeginHorizontal(GUILayout.Height(totalHeight));
			GUILayout.FlexibleSpace();
			foreach (var handler in LeftToolbarGUI_AlignRight)
			{
				handler();
			}
			GUILayout.EndHorizontal();
		}
		
		public static void GUIRight()
		{
			//Align Left
			GUILayout.BeginHorizontal(GUILayout.Height(totalHeight));
			GUILayout.Label("", GUILayout.Width(0), GUILayout.Height(totalHeight)); //Có ít nhất 1 gui để tạo area, như vậy Space phía dưới mới đúng
			foreach (var handler in RightToolbarGUI_AlignLeft)
			{
				handler();
			}
			GUILayout.EndHorizontal();
			
			
			//Align Right
			GUILayout.Space(-totalHeight - EditorGUIUtility.standardVerticalSpacing);
			GUILayout.BeginHorizontal(GUILayout.Height(totalHeight));
			GUILayout.FlexibleSpace();
			foreach (var handler in RightToolbarGUI_AlignRight)
			{
				handler();
			}
			GUILayout.EndHorizontal();
		}
	}
}