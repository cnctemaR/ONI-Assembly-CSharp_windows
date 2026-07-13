using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine.Bindings;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.Layout;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule", "UnityEditor.UIToolkitAuthoringModule" })]
	internal class UIElementsUtility : IUIElementsUtility
	{
		public static bool isOSXContextualMenuPlatform
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				return platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer || UIElementsUtility.s_EnableOSXContextualMenuEventsOnNonOSXPlatforms;
			}
		}

		internal static void EnableOSXContextualMenuEventsOnNonOSXPlatforms()
		{
			UIElementsUtility.s_EnableOSXContextualMenuEventsOnNonOSXPlatforms = true;
		}

		internal static void ResetOSXContextualMenuEventsOnNonOSXPlatforms()
		{
			UIElementsUtility.s_EnableOSXContextualMenuEventsOnNonOSXPlatforms = false;
		}

		private UIElementsUtility()
		{
			UIEventRegistration.RegisterUIElementSystem(this);
		}

		internal static IMGUIContainer GetCurrentIMGUIContainer()
		{
			bool flag = UIElementsUtility.s_ContainerStack.Count > 0;
			IMGUIContainer imguicontainer;
			if (flag)
			{
				imguicontainer = UIElementsUtility.s_ContainerStack.Peek();
			}
			else
			{
				imguicontainer = null;
			}
			return imguicontainer;
		}

		bool IUIElementsUtility.MakeCurrentIMGUIContainerDirty()
		{
			bool flag = UIElementsUtility.s_ContainerStack.Count > 0;
			bool flag2;
			if (flag)
			{
				UIElementsUtility.s_ContainerStack.Peek().MarkDirtyLayout();
				flag2 = true;
			}
			else
			{
				flag2 = false;
			}
			return flag2;
		}

		bool IUIElementsUtility.TakeCapture()
		{
			bool flag = UIElementsUtility.s_ContainerStack.Count > 0;
			bool flag2;
			if (flag)
			{
				IMGUIContainer imguicontainer = UIElementsUtility.s_ContainerStack.Peek();
				imguicontainer.CaptureMouse();
				flag2 = true;
			}
			else
			{
				flag2 = false;
			}
			return flag2;
		}

		bool IUIElementsUtility.ReleaseCapture()
		{
			return false;
		}

		bool IUIElementsUtility.ProcessEvent(int instanceID, IntPtr nativeEventPtr, ref bool eventHandled)
		{
			Panel panel;
			bool flag = nativeEventPtr != IntPtr.Zero && UIElementsUtility.s_UIElementsCache.TryGetValue(instanceID, out panel);
			bool flag4;
			if (flag)
			{
				bool flag2 = panel.contextType == ContextType.Editor;
				if (flag2)
				{
					UIElementsUtility.s_EventInstance.CopyFromPtr(nativeEventPtr);
					bool flag3 = (EventType)7777 == UIElementsUtility.s_EventInstance.type;
					if (flag3)
					{
						Action action = UIElementsUtility.testFrameUpdateCallback;
						UIElementsUtility.testFrameUpdateCallback = null;
						if (action != null)
						{
							action();
						}
						eventHandled = true;
						return true;
					}
					using (new IMGUIContainer.UITKScope())
					{
						eventHandled = UIElementsUtility.DoDispatch(panel);
					}
				}
				flag4 = true;
			}
			else
			{
				flag4 = false;
			}
			return flag4;
		}

		bool IUIElementsUtility.CleanupRoots()
		{
			UIElementsUtility.s_EventInstance = null;
			UIElementsUtility.s_UIElementsCache = null;
			UIElementsUtility.s_ContainerStack = null;
			return false;
		}

		bool IUIElementsUtility.EndContainerGUIFromException(Exception exception)
		{
			bool flag = UIElementsUtility.s_ContainerStack.Count > 0;
			if (flag)
			{
				GUIUtility.EndContainer();
				UIElementsUtility.s_ContainerStack.Pop();
			}
			return false;
		}

		void IUIElementsUtility.UpdateSchedulers()
		{
			UIElementsUtility.s_PanelsIterationList.Clear();
			UIElementsUtility.GetAllPanels(UIElementsUtility.s_PanelsIterationList, ContextType.Editor);
			bool isSharedManagerCreated = LayoutManager.IsSharedManagerCreated;
			if (isSharedManagerCreated)
			{
				LayoutManager.SharedManager.Collect();
			}
			foreach (Panel panel in UIElementsUtility.s_PanelsIterationList)
			{
				panel.TickSchedulingUpdaters();
			}
		}

		public static Event CreateTestFrameUpdateEvent(Action callback)
		{
			UIElementsUtility.testFrameUpdateCallback = callback;
			return new Event
			{
				type = (EventType)7777
			};
		}

		void IUIElementsUtility.RequestRepaintForPanels(Action<ScriptableObject> repaintCallback)
		{
			Dictionary<int, Panel>.Enumerator panelsIterator = UIElementsUtility.GetPanelsIterator();
			while (panelsIterator.MoveNext())
			{
				KeyValuePair<int, Panel> keyValuePair = panelsIterator.Current;
				Panel value = keyValuePair.Value;
				bool flag = value.contextType != ContextType.Editor;
				if (!flag)
				{
					bool isDirty = value.isDirty;
					if (isDirty)
					{
						repaintCallback(value.ownerObject);
					}
				}
			}
			TextGenerationInfo.OnRepaintEnd();
		}

		public static void RegisterCachedPanel(int instanceID, Panel panel)
		{
			UIElementsUtility.s_UIElementsCache.Add(instanceID, panel);
		}

		public static void RemoveCachedPanel(int instanceID)
		{
			UIElementsUtility.s_UIElementsCache.Remove(instanceID);
		}

		public static bool TryGetPanel(int instanceID, out Panel panel)
		{
			return UIElementsUtility.s_UIElementsCache.TryGetValue(instanceID, out panel);
		}

		internal static void BeginContainerGUI(GUILayoutUtility.LayoutCache cache, Event evt, IMGUIContainer container)
		{
			bool useOwnerObjectGUIState = container.useOwnerObjectGUIState;
			if (useOwnerObjectGUIState)
			{
				GUIUtility.BeginContainerFromOwner(container.elementPanel.ownerObject);
			}
			else
			{
				GUIUtility.BeginContainer(container.guiState);
			}
			UIElementsUtility.s_ContainerStack.Push(container);
			GUIUtility.s_SkinMode = (int)container.contextType;
			GUIUtility.s_OriginalID = container.elementPanel.ownerObject.GetInstanceID();
			bool flag = Event.current == null;
			if (flag)
			{
				Event.current = evt;
			}
			else
			{
				Event.current.CopyFrom(evt);
			}
			GUI.enabled = container.enabledInHierarchy;
			GUILayoutUtility.BeginContainer(cache);
			GUIUtility.ResetGlobalState();
		}

		internal static void EndContainerGUI(Event evt, Rect layoutSize)
		{
			bool flag = Event.current.type == EventType.Layout && UIElementsUtility.s_ContainerStack.Count > 0;
			if (flag)
			{
				GUILayoutUtility.LayoutFromContainer(layoutSize.width, layoutSize.height);
			}
			GUILayoutUtility.SelectIDList(GUIUtility.s_OriginalID, false);
			GUIContent.ClearStaticCache();
			bool flag2 = UIElementsUtility.s_ContainerStack.Count > 0;
			if (flag2)
			{
			}
			evt.CopyFrom(Event.current);
			bool flag3 = UIElementsUtility.s_ContainerStack.Count > 0;
			if (flag3)
			{
				GUIUtility.EndContainer();
				UIElementsUtility.s_ContainerStack.Pop();
			}
		}

		internal static EventBase CreateEvent(Event systemEvent)
		{
			return UIElementsUtility.CreateEvent(systemEvent, systemEvent.rawType);
		}

		internal static EventBase CreateEvent(Event systemEvent, EventType eventType)
		{
			switch (eventType)
			{
			case EventType.MouseDown:
				goto IL_0097;
			case EventType.MouseUp:
				goto IL_00C2;
			case EventType.MouseMove:
				break;
			case EventType.MouseDrag:
				return PointerEventBase<PointerMoveEvent>.GetPooled(systemEvent);
			case EventType.KeyDown:
				return KeyboardEventBase<KeyDownEvent>.GetPooled(systemEvent);
			case EventType.KeyUp:
				return KeyboardEventBase<KeyUpEvent>.GetPooled(systemEvent);
			case EventType.ScrollWheel:
				return WheelEvent.GetPooled(systemEvent);
			case EventType.Repaint:
			case EventType.Layout:
			case EventType.DragUpdated:
			case EventType.DragPerform:
			case EventType.Ignore:
			case EventType.Used:
			case EventType.DragExited:
			case (EventType)17:
			case (EventType)18:
			case (EventType)19:
				goto IL_0134;
			case EventType.ValidateCommand:
				return CommandEventBase<ValidateCommandEvent>.GetPooled(systemEvent);
			case EventType.ExecuteCommand:
				return CommandEventBase<ExecuteCommandEvent>.GetPooled(systemEvent);
			case EventType.ContextClick:
				return MouseEventBase<ContextClickEvent>.GetPooled(systemEvent);
			case EventType.MouseEnterWindow:
				return MouseEventBase<MouseEnterWindowEvent>.GetPooled(systemEvent);
			case EventType.MouseLeaveWindow:
				return MouseLeaveWindowEvent.GetPooled(systemEvent);
			default:
				switch (eventType)
				{
				case EventType.TouchDown:
					goto IL_0097;
				case EventType.TouchUp:
					goto IL_00C2;
				case EventType.TouchMove:
					break;
				default:
					goto IL_0134;
				}
				break;
			}
			return PointerEventBase<PointerMoveEvent>.GetPooled(systemEvent);
			IL_0097:
			bool flag = PointerDeviceState.HasAdditionalPressedButtons(PointerId.mousePointerId, systemEvent.button);
			if (flag)
			{
				return PointerEventBase<PointerMoveEvent>.GetPooled(systemEvent);
			}
			return PointerEventBase<PointerDownEvent>.GetPooled(systemEvent);
			IL_00C2:
			bool flag2 = PointerDeviceState.HasAdditionalPressedButtons(PointerId.mousePointerId, systemEvent.button);
			if (flag2)
			{
				return PointerEventBase<PointerMoveEvent>.GetPooled(systemEvent);
			}
			return PointerEventBase<PointerUpEvent>.GetPooled(systemEvent);
			IL_0134:
			return IMGUIEvent.GetPooled(systemEvent);
		}

		private static bool DoDispatch(BaseVisualElementPanel panel)
		{
			Debug.Assert(panel.contextType == ContextType.Editor, "panel.contextType == ContextType.Editor");
			bool flag = false;
			bool flag2 = UIElementsUtility.s_EventInstance.type == EventType.Repaint;
			if (flag2)
			{
				Camera current = Camera.current;
				RenderTexture active = RenderTexture.active;
				Camera.SetupCurrent(null);
				RenderTexture.active = null;
				using (UIElementsUtility.s_RepaintProfilerMarker.Auto())
				{
					panel.Repaint(UIElementsUtility.s_EventInstance);
					panel.Render();
				}
				flag = panel.IMGUIContainersCount > 0;
				Camera.SetupCurrent(current);
				RenderTexture.active = active;
			}
			else
			{
				panel.ValidateLayout();
				using (EventBase eventBase = UIElementsUtility.CreateEvent(UIElementsUtility.s_EventInstance))
				{
					bool flag3 = UIElementsUtility.s_EventInstance.type == EventType.Used || UIElementsUtility.s_EventInstance.type == EventType.Layout || UIElementsUtility.s_EventInstance.type == EventType.ExecuteCommand || UIElementsUtility.s_EventInstance.type == EventType.ValidateCommand;
					using (UIElementsUtility.s_EventProfilerMarker.Auto())
					{
						panel.SendEvent(eventBase, flag3 ? DispatchMode.Immediate : DispatchMode.Default);
					}
					bool isPropagationStopped = eventBase.isPropagationStopped;
					if (isPropagationStopped)
					{
						panel.visualTree.IncrementVersion(VersionChangeType.Repaint);
						flag = true;
					}
				}
			}
			return flag;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
		internal static void GetAllPanels(List<Panel> panels, ContextType contextType)
		{
			Dictionary<int, Panel>.Enumerator panelsIterator = UIElementsUtility.GetPanelsIterator();
			while (panelsIterator.MoveNext())
			{
				KeyValuePair<int, Panel> keyValuePair = panelsIterator.Current;
				bool flag = keyValuePair.Value.contextType == contextType;
				if (flag)
				{
					keyValuePair = panelsIterator.Current;
					panels.Add(keyValuePair.Value);
				}
			}
		}

		internal static Dictionary<int, Panel>.Enumerator GetPanelsIterator()
		{
			return UIElementsUtility.s_UIElementsCache.GetEnumerator();
		}

		internal static float PixelsPerUnitScaleForElement(VisualElement ve, Sprite sprite)
		{
			bool flag = ve == null || ve.elementPanel == null || sprite == null;
			float num;
			if (flag)
			{
				num = 1f;
			}
			else
			{
				float referenceSpritePixelsPerUnit = ve.elementPanel.referenceSpritePixelsPerUnit;
				float num2 = sprite.pixelsPerUnit;
				num2 = Mathf.Max(0.01f, num2);
				num = referenceSpritePixelsPerUnit / num2;
			}
			return num;
		}

		internal static string ParseMenuName(string menuName)
		{
			bool flag = string.IsNullOrEmpty(menuName);
			string text;
			if (flag)
			{
				text = string.Empty;
			}
			else
			{
				string text2 = menuName.TrimEnd();
				int num = text2.LastIndexOf(' ');
				bool flag2 = num > -1;
				if (flag2)
				{
					int num2 = Array.IndexOf<char>(UIElementsUtility.s_Modifiers, text2[num + 1]);
					bool flag3 = text2.Length > num + 1 && num2 > -1;
					if (flag3)
					{
						text2 = text2.Substring(0, num).TrimEnd();
					}
				}
				text = text2;
			}
			return text;
		}

		internal static void MarkStyleSheetAsChanged(StyleSheet styleSheet)
		{
			bool flag = !styleSheet;
			if (!flag)
			{
				UIElementsUtility.s_StyleSheetsRequiringRebuilding.Add(styleSheet);
			}
		}

		internal static void MarkStyleSheetAsChanged(string styleSheetPath)
		{
			bool flag = string.IsNullOrEmpty(styleSheetPath);
			if (!flag)
			{
				UIElementsUtility.s_ReimportedStyleSheetsPath.Add(styleSheetPath);
			}
		}

		internal static void RebuildDirtyStyleSheets()
		{
			bool flag = UIElementsUtility.s_StyleSheetsRequiringRebuilding.Count == 0;
			if (!flag)
			{
				try
				{
					StyleCache.ClearStyleCache();
					UIElementsUtility.s_StyleSheetsRebuildList.AddRange(UIElementsUtility.s_StyleSheetsRequiringRebuilding);
					UIElementsUtility.s_ReimportedStyleSheetsPathList.AddRange(UIElementsUtility.s_ReimportedStyleSheetsPath);
					foreach (StyleSheet styleSheet in UIElementsUtility.s_StyleSheetsRebuildList)
					{
						styleSheet.RebuildIfNecessary();
					}
				}
				finally
				{
					UIElementsUtility.s_StyleSheetsRebuildList.Clear();
					UIElementsUtility.s_StyleSheetsRequiringRebuilding.Clear();
					UIElementsUtility.s_ReimportedStyleSheetsPathList.Clear();
					UIElementsUtility.s_ReimportedStyleSheetsPath.Clear();
				}
			}
		}

		private static Stack<IMGUIContainer> s_ContainerStack = new Stack<IMGUIContainer>();

		private static Dictionary<int, Panel> s_UIElementsCache = new Dictionary<int, Panel>();

		private static Event s_EventInstance = new Event();

		internal static Color editorPlayModeTintColor = Color.white;

		internal static float singleLineHeight = 18f;

		public const string hiddenClassName = "unity-hidden";

		internal static bool s_EnableOSXContextualMenuEventsOnNonOSXPlatforms;

		private static UIElementsUtility s_Instance = new UIElementsUtility();

		internal static List<Panel> s_PanelsIterationList = new List<Panel>();

		internal const int kTestFrameUpdateEvent = 7777;

		private static Action testFrameUpdateCallback;

		internal static readonly string s_RepaintProfilerMarkerName = "UIElementsUtility.DoDispatch(Repaint Event)";

		internal static readonly string s_EventProfilerMarkerName = "UIElementsUtility.DoDispatch(Non Repaint Event)";

		private static readonly ProfilerMarker s_RepaintProfilerMarker = new ProfilerMarker(UIElementsUtility.s_RepaintProfilerMarkerName);

		private static readonly ProfilerMarker s_EventProfilerMarker = new ProfilerMarker(UIElementsUtility.s_EventProfilerMarkerName);

		internal static char[] s_Modifiers = new char[] { '&', '%', '^', '#', '_' };

		internal static readonly HashSet<StyleSheet> s_StyleSheetsRequiringRebuilding = new HashSet<StyleSheet>();

		internal static readonly HashSet<string> s_ReimportedStyleSheetsPath = new HashSet<string>();

		internal static readonly List<StyleSheet> s_StyleSheetsRebuildList = new List<StyleSheet>();

		internal static readonly List<string> s_ReimportedStyleSheetsPathList = new List<string>();
	}
}
