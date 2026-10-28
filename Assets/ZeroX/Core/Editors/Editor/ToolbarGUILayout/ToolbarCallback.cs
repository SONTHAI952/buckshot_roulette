using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;

#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif

namespace ZeroX.Editors
{
	public static class ToolbarCallback
	{
		private const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		
		private static readonly Type type_toolbar = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
		private static readonly Type type_guiView = typeof(Editor).Assembly.GetType("UnityEditor.GUIView");
		private static readonly FieldInfo fi_IMGUIContainer_OnGUIHandler = typeof(IMGUIContainer).GetField("m_OnGUIHandler", bindingFlags);
		
		
		
#if UNITY_2020_1_OR_NEWER
		static Type type_IWindowBackend = typeof(Editor).Assembly.GetType("UnityEditor.IWindowBackend");
		static PropertyInfo pi_windowBackend = type_guiView.GetProperty("windowBackend", bindingFlags);
		static PropertyInfo pi_visualTree = type_IWindowBackend.GetProperty("visualTree", bindingFlags);
#else
		static PropertyInfo pi_visualTree = type_guiView.GetProperty("visualTree", bindingFlags);
#endif
		
		
		//Temp
		private static ScriptableObject currentToolbar;
		

		/// <summary>
		/// Callback for toolbar OnGUI method.
		/// </summary>
		public static Action OnToolbarGUI;
		public static Action OnToolbarGUILeft;
		public static Action OnToolbarGUIRight;
		
		
		
		
		//Constructor
		static ToolbarCallback()
		{
			EditorApplication.update -= OnUpdate;
			EditorApplication.update += OnUpdate;
		}

		static void OnUpdate()
		{
			// Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes
			if (currentToolbar == null)
			{
				// Find toolbar
				var toolbars = Resources.FindObjectsOfTypeAll(type_toolbar);
				currentToolbar = toolbars.Length > 0 ? (ScriptableObject) toolbars[0] : null;
				if (currentToolbar != null)
				{ 
#if UNITY_2021_1_OR_NEWER
					var root = currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
					var rawRoot = root.GetValue(currentToolbar);
					var mRoot = rawRoot as VisualElement;
					RegisterCallback("ToolbarZoneLeftAlign", OnToolbarGUILeft);
					RegisterCallback("ToolbarZoneRightAlign", OnToolbarGUIRight);

					void RegisterCallback(string root, Action cb) {
						var toolbarZone = mRoot.Q(root);

						var parent = new VisualElement()
						{
							style = {
								flexGrow = 1,
								flexDirection = FlexDirection.Row,
							}
						};
						var container = new IMGUIContainer();
						container.style.flexGrow = 1;
						container.onGUIHandler += () => { 
							cb?.Invoke();
						}; 
						parent.Add(container);
						toolbarZone.Add(parent);
					}
#else
#if UNITY_2020_1_OR_NEWER
					var windowBackend = pi_windowBackend.GetValue(currentToolbar);

					// Get it's visual tree
					var visualTree = (VisualElement) pi_visualTree.GetValue(windowBackend, null);
#else
					// Get it's visual tree
					var visualTree = (VisualElement) pi_visualTree.GetValue(currentToolbar, null);
#endif

					// Get first child which 'happens' to be toolbar IMGUIContainer
					var container = (IMGUIContainer) visualTree[0];

					// (Re)attach handler
					var handler = (Action) fi_IMGUIContainer_OnGUIHandler.GetValue(container);
					handler -= OnGUI;
					handler += OnGUI;
					fi_IMGUIContainer_OnGUIHandler.SetValue(container, handler);
					
#endif
				}
			}
		}

		static void OnGUI()
		{
			var handler = OnToolbarGUI;
			if (handler != null) handler();
		}
	}
}