using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Bindings;
using UnityEngine.Pool;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.Layout;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
	internal static class UIElementsRuntimeUtility
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<BaseRuntimePanel> onCreatePanel;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<BaseRuntimePanel> onWillDestroyPanel;

		static UIElementsRuntimeUtility()
		{
			Canvas.externBeginRenderOverlays = new Action<int>(UIElementsRuntimeUtility.BeginRenderOverlays);
			Canvas.externRenderOverlaysBefore = delegate(int displayIndex, int sortOrder)
			{
				UIElementsRuntimeUtility.RenderOverlaysBeforePriority(displayIndex, (float)sortOrder);
			};
			Canvas.externEndRenderOverlays = new Action<int>(UIElementsRuntimeUtility.EndRenderOverlays);
			UIElementsRuntimeUtilityNative.SetUpdateCallback(new Action(UIElementsRuntimeUtility.UpdatePanels));
		}

		public static EventBase CreateEvent(Event systemEvent)
		{
			return UIElementsUtility.CreateEvent(systemEvent, systemEvent.rawType);
		}

		public static BaseRuntimePanel FindOrCreateRuntimePanel(ScriptableObject ownerObject, UIElementsRuntimeUtility.CreateRuntimePanelDelegate createDelegate)
		{
			Panel panel;
			bool flag = UIElementsUtility.TryGetPanel(ownerObject.GetInstanceID(), out panel);
			if (flag)
			{
				BaseRuntimePanel baseRuntimePanel = panel as BaseRuntimePanel;
				bool flag2 = baseRuntimePanel != null;
				if (flag2)
				{
					return baseRuntimePanel;
				}
				UIElementsRuntimeUtility.RemoveCachedPanelInternal(ownerObject.GetInstanceID());
			}
			BaseRuntimePanel baseRuntimePanel2 = createDelegate(ownerObject);
			baseRuntimePanel2.IMGUIEventInterests = new EventInterests
			{
				wantsMouseMove = true,
				wantsMouseEnterLeaveWindow = true
			};
			UIElementsRuntimeUtility.RegisterCachedPanelInternal(ownerObject.GetInstanceID(), baseRuntimePanel2);
			Action<BaseRuntimePanel> action = UIElementsRuntimeUtility.onCreatePanel;
			if (action != null)
			{
				action(baseRuntimePanel2);
			}
			return baseRuntimePanel2;
		}

		public static void DisposeRuntimePanel(ScriptableObject ownerObject)
		{
			Panel panel;
			bool flag = UIElementsUtility.TryGetPanel(ownerObject.GetInstanceID(), out panel);
			if (flag)
			{
				Action<BaseRuntimePanel> action = UIElementsRuntimeUtility.onWillDestroyPanel;
				if (action != null)
				{
					action((BaseRuntimePanel)panel);
				}
				panel.Dispose();
				UIElementsRuntimeUtility.RemoveCachedPanelInternal(ownerObject.GetInstanceID());
			}
		}

		private static void GetPlayerPanelsByRenderMode(List<BaseRuntimePanel> outScreenSpaceOverlayPanels, List<BaseRuntimePanel> outWorldSpacePanels)
		{
			List<Panel> list;
			using (CollectionPool<List<Panel>, Panel>.Get(out list))
			{
				UIElementsUtility.GetAllPanels(list, ContextType.Player);
				foreach (Panel panel in list)
				{
					BaseRuntimePanel baseRuntimePanel = panel as BaseRuntimePanel;
					bool flag = baseRuntimePanel == null;
					if (!flag)
					{
						bool drawsInCameras = baseRuntimePanel.drawsInCameras;
						if (drawsInCameras)
						{
							outWorldSpacePanels.Add(baseRuntimePanel);
						}
						else
						{
							outScreenSpaceOverlayPanels.Add(baseRuntimePanel);
						}
					}
				}
			}
		}

		private static void RegisterCachedPanelInternal(int instanceID, IPanel panel)
		{
			UIElementsUtility.RegisterCachedPanel(instanceID, panel as Panel);
			UIElementsRuntimeUtility.s_PanelOrderingOrDrawInCameraDirty = true;
			bool flag = !UIElementsRuntimeUtility.s_RegisteredPlayerloopCallback;
			if (flag)
			{
				UIElementsRuntimeUtility.s_RegisteredPlayerloopCallback = true;
				UIElementsRuntimeUtility.EnableRenderingAndInputCallbacks();
				Canvas.SetExternalCanvasEnabled(true);
			}
		}

		private static void RemoveCachedPanelInternal(int instanceID)
		{
			UIElementsUtility.RemoveCachedPanel(instanceID);
			UIElementsRuntimeUtility.s_PanelOrderingOrDrawInCameraDirty = true;
			List<Panel> list;
			using (CollectionPool<List<Panel>, Panel>.Get(out list))
			{
				UIElementsUtility.GetAllPanels(list, ContextType.Player);
				bool flag = list.Count == 0;
				if (flag)
				{
					UIElementsRuntimeUtility.SortPanels();
					UIElementsRuntimeUtility.s_RegisteredPlayerloopCallback = false;
					UIElementsRuntimeUtility.DisableRenderingAndInputCallbacks();
					Canvas.SetExternalCanvasEnabled(false);
				}
			}
		}

		public static void RenderOffscreenPanels()
		{
			Camera current = Camera.current;
			RenderTexture active = RenderTexture.active;
			foreach (BaseRuntimePanel baseRuntimePanel in UIElementsRuntimeUtility.GetSortedScreenOverlayPlayerPanels())
			{
				bool flag = baseRuntimePanel.targetTexture != null;
				if (flag)
				{
					UIElementsRuntimeUtility.RenderPanel(baseRuntimePanel, false);
				}
			}
			Camera.SetupCurrent(current);
			RenderTexture.active = active;
		}

		public static void RepaintPanel(BaseRuntimePanel panel)
		{
			Camera current = Camera.current;
			RenderTexture active = RenderTexture.active;
			panel.Repaint(Event.current);
			Camera.SetupCurrent(current);
			RenderTexture.active = active;
		}

		public static void RenderPanel(BaseRuntimePanel panel, bool restoreState = true)
		{
			Debug.Assert(!panel.drawsInCameras);
			Camera current = Camera.current;
			RenderTexture active = RenderTexture.active;
			panel.Render();
			bool flag = !panel.drawsInCameras && restoreState;
			if (flag)
			{
				Camera.SetupCurrent(current);
				RenderTexture.active = active;
			}
		}

		internal static void BeginRenderOverlays(int displayIndex)
		{
			UIElementsRuntimeUtility.currentOverlayIndex = 0;
		}

		internal static void RenderOverlaysBeforePriority(int displayIndex, float maxPriority)
		{
			bool flag = UIElementsRuntimeUtility.currentOverlayIndex < 0;
			if (!flag)
			{
				List<BaseRuntimePanel> sortedScreenOverlayPlayerPanels = UIElementsRuntimeUtility.GetSortedScreenOverlayPlayerPanels();
				while (UIElementsRuntimeUtility.currentOverlayIndex < sortedScreenOverlayPlayerPanels.Count)
				{
					BaseRuntimePanel baseRuntimePanel = sortedScreenOverlayPlayerPanels[UIElementsRuntimeUtility.currentOverlayIndex];
					bool flag2 = baseRuntimePanel.sortingPriority >= maxPriority;
					if (flag2)
					{
						break;
					}
					bool flag3 = baseRuntimePanel.targetDisplay == displayIndex && baseRuntimePanel.targetTexture == null;
					if (flag3)
					{
						UIElementsRuntimeUtility.RenderPanel(baseRuntimePanel, true);
					}
					UIElementsRuntimeUtility.currentOverlayIndex++;
				}
			}
		}

		internal static void EndRenderOverlays(int displayIndex)
		{
			UIElementsRuntimeUtility.RenderOverlaysBeforePriority(displayIndex, float.MaxValue);
			UIElementsRuntimeUtility.currentOverlayIndex = -1;
		}

		public static void RepaintPanels(bool onlyOffscreen)
		{
			foreach (BaseRuntimePanel baseRuntimePanel in UIElementsRuntimeUtility.GetSortedPlayerPanels())
			{
				bool flag = !onlyOffscreen || baseRuntimePanel.targetTexture != null;
				if (flag)
				{
					UIElementsRuntimeUtility.RepaintPanel(baseRuntimePanel);
				}
			}
			TextGenerationInfo.OnRepaintEnd();
		}

		internal static Object activeEventSystem { get; private set; }

		internal static bool useDefaultEventSystem
		{
			get
			{
				return UIElementsRuntimeUtility.overrideUseDefaultEventSystem ?? (UIElementsRuntimeUtility.activeEventSystem == null);
			}
		}

		internal static bool? overrideUseDefaultEventSystem { get; set; }

		internal static bool autoUpdateEventSystem { get; set; } = true;

		public static void RegisterEventSystem(Object eventSystem)
		{
			bool flag = UIElementsRuntimeUtility.activeEventSystem != null && UIElementsRuntimeUtility.activeEventSystem != eventSystem && eventSystem.GetType().Name == "EventSystem";
			if (flag)
			{
				Debug.LogWarning("There can be only one active Event System.");
			}
			UIElementsRuntimeUtility.activeEventSystem = eventSystem;
		}

		public static void UnregisterEventSystem(Object eventSystem)
		{
			bool flag = UIElementsRuntimeUtility.activeEventSystem == eventSystem;
			if (flag)
			{
				UIElementsRuntimeUtility.activeEventSystem = null;
			}
		}

		internal static DefaultEventSystem defaultEventSystem
		{
			get
			{
				DefaultEventSystem defaultEventSystem;
				if ((defaultEventSystem = UIElementsRuntimeUtility.s_DefaultEventSystem) == null)
				{
					defaultEventSystem = (UIElementsRuntimeUtility.s_DefaultEventSystem = new DefaultEventSystem());
				}
				return defaultEventSystem;
			}
		}

		public static void UpdatePanels()
		{
			UIElementsRuntimeUtility.RemoveUnusedPanels();
			UIRenderDevice.ProcessDeviceFreeQueue();
			bool isSharedManagerCreated = LayoutManager.IsSharedManagerCreated;
			if (isSharedManagerCreated)
			{
				LayoutManager.SharedManager.Collect();
			}
			List<BaseRuntimePanel> sortedPlayerPanels = UIElementsRuntimeUtility.GetSortedPlayerPanels();
			bool flag = sortedPlayerPanels.Count == 0;
			if (!flag)
			{
				foreach (BaseRuntimePanel baseRuntimePanel in sortedPlayerPanels)
				{
					baseRuntimePanel.Update();
				}
				UIElementsRuntimeUtility.UpdateEventSystem();
			}
		}

		internal static void UpdateEventSystem()
		{
			bool useDefaultEventSystem = UIElementsRuntimeUtility.useDefaultEventSystem;
			if (useDefaultEventSystem)
			{
				UIElementsRuntimeUtility.defaultEventSystem.isInputReady = true;
				bool autoUpdateEventSystem = UIElementsRuntimeUtility.autoUpdateEventSystem;
				if (autoUpdateEventSystem)
				{
					UIElementsRuntimeUtility.defaultEventSystem.Update(DefaultEventSystem.UpdateMode.IgnoreIfAppNotFocused);
				}
			}
			else
			{
				bool flag = UIElementsRuntimeUtility.s_DefaultEventSystem != null;
				if (flag)
				{
					UIElementsRuntimeUtility.s_DefaultEventSystem.isInputReady = false;
				}
			}
		}

		internal static void MarkPotentiallyEmpty(PanelSettings settings)
		{
			bool flag = !UIElementsRuntimeUtility.s_PotentiallyEmptyPanelSettings.Contains(settings);
			if (flag)
			{
				UIElementsRuntimeUtility.s_PotentiallyEmptyPanelSettings.Add(settings);
			}
		}

		internal static void RemoveUnusedPanels()
		{
			foreach (PanelSettings panelSettings in UIElementsRuntimeUtility.s_PotentiallyEmptyPanelSettings)
			{
				UIDocumentList attachedUIDocumentsList = panelSettings.m_AttachedUIDocumentsList;
				bool flag = attachedUIDocumentsList == null || attachedUIDocumentsList.m_AttachedUIDocuments.Count == 0;
				if (flag)
				{
					panelSettings.DisposePanel();
				}
			}
			UIElementsRuntimeUtility.s_PotentiallyEmptyPanelSettings.Clear();
		}

		public static void EnableRenderingAndInputCallbacks()
		{
			UIElementsRuntimeUtilityNative.SetRenderingCallbacks(new Action<bool>(UIElementsRuntimeUtility.RepaintPanels), new Action(UIElementsRuntimeUtility.RenderOffscreenPanels));
		}

		public static void DisableRenderingAndInputCallbacks()
		{
			UIElementsRuntimeUtilityNative.UnsetRenderingCallbacks();
			bool flag = UIElementsRuntimeUtility.s_DefaultEventSystem != null;
			if (flag)
			{
				UIElementsRuntimeUtility.s_DefaultEventSystem.isInputReady = false;
			}
		}

		internal static void SetPanelOrderingDirty()
		{
			UIElementsRuntimeUtility.s_PanelOrderingOrDrawInCameraDirty = true;
		}

		internal static void SetPanelsDrawInCameraDirty()
		{
			UIElementsRuntimeUtility.s_PanelOrderingOrDrawInCameraDirty = true;
		}

		internal static List<BaseRuntimePanel> GetWorldSpacePlayerPanels()
		{
			bool flag = UIElementsRuntimeUtility.s_PanelOrderingOrDrawInCameraDirty;
			if (flag)
			{
				UIElementsRuntimeUtility.SortPanels();
			}
			return UIElementsRuntimeUtility.s_CachedWorldSpacePanels;
		}

		public static List<BaseRuntimePanel> GetSortedScreenOverlayPlayerPanels()
		{
			bool flag = UIElementsRuntimeUtility.s_PanelOrderingOrDrawInCameraDirty;
			if (flag)
			{
				UIElementsRuntimeUtility.SortPanels();
			}
			return UIElementsRuntimeUtility.s_SortedScreenOverlayPanels;
		}

		public static List<BaseRuntimePanel> GetSortedPlayerPanels()
		{
			bool flag = UIElementsRuntimeUtility.s_PanelOrderingOrDrawInCameraDirty;
			if (flag)
			{
				UIElementsRuntimeUtility.SortPanels();
			}
			return UIElementsRuntimeUtility.s_SortedPlayerPanels;
		}

		internal static List<IPanel> GetSortedPlayerPanelsInternal()
		{
			List<IPanel> list = new List<IPanel>();
			foreach (BaseRuntimePanel baseRuntimePanel in UIElementsRuntimeUtility.GetSortedPlayerPanels())
			{
				list.Add(baseRuntimePanel);
			}
			return list;
		}

		private static void SortPanels()
		{
			UIElementsRuntimeUtility.s_SortedScreenOverlayPanels.Clear();
			UIElementsRuntimeUtility.s_CachedWorldSpacePanels.Clear();
			UIElementsRuntimeUtility.GetPlayerPanelsByRenderMode(UIElementsRuntimeUtility.s_SortedScreenOverlayPanels, UIElementsRuntimeUtility.s_CachedWorldSpacePanels);
			UIElementsRuntimeUtility.s_SortedScreenOverlayPanels.Sort(delegate(BaseRuntimePanel runtimePanelA, BaseRuntimePanel runtimePanelB)
			{
				bool flag = runtimePanelA == null || runtimePanelB == null;
				int num;
				if (flag)
				{
					num = 0;
				}
				else
				{
					float num2 = runtimePanelA.sortingPriority - runtimePanelB.sortingPriority;
					bool flag2 = Mathf.Approximately(0f, num2);
					if (flag2)
					{
						num = runtimePanelA.m_RuntimePanelCreationIndex.CompareTo(runtimePanelB.m_RuntimePanelCreationIndex);
					}
					else
					{
						num = ((num2 < 0f) ? (-1) : 1);
					}
				}
				return num;
			});
			for (int i = 0; i < UIElementsRuntimeUtility.s_SortedScreenOverlayPanels.Count; i++)
			{
				BaseRuntimePanel baseRuntimePanel = UIElementsRuntimeUtility.s_SortedScreenOverlayPanels[i];
				baseRuntimePanel.resolvedSortingIndex = i;
			}
			UIElementsRuntimeUtility.s_ResolvedSortingIndexMax = UIElementsRuntimeUtility.s_SortedScreenOverlayPanels.Count - 1;
			UIElementsRuntimeUtility.s_SortedPlayerPanels.Clear();
			foreach (BaseRuntimePanel baseRuntimePanel2 in UIElementsRuntimeUtility.s_CachedWorldSpacePanels)
			{
				UIElementsRuntimeUtility.s_SortedPlayerPanels.Add(baseRuntimePanel2);
			}
			foreach (BaseRuntimePanel baseRuntimePanel3 in UIElementsRuntimeUtility.s_SortedScreenOverlayPanels)
			{
				UIElementsRuntimeUtility.s_SortedPlayerPanels.Add(baseRuntimePanel3);
			}
			UIElementsRuntimeUtility.s_PanelOrderingOrDrawInCameraDirty = false;
		}

		internal static Vector2 MultiDisplayBottomLeftToPanelPosition(Vector2 position, out int? targetDisplay)
		{
			Vector2 vector = UIElementsRuntimeUtility.MultiDisplayToLocalScreenPosition(position, out targetDisplay);
			return UIElementsRuntimeUtility.ScreenBottomLeftToPanelPosition(vector, targetDisplay.GetValueOrDefault());
		}

		internal static Vector2 MultiDisplayToLocalScreenPosition(Vector2 position, out int? targetDisplay)
		{
			Vector3 vector = Display.RelativeMouseAt(position);
			bool flag = vector != Vector3.zero;
			Vector2 vector2;
			if (flag)
			{
				targetDisplay = new int?((int)vector.z);
				vector2 = vector;
			}
			else
			{
				targetDisplay = null;
				vector2 = position;
			}
			return vector2;
		}

		internal static Vector2 ScreenBottomLeftToPanelPosition(Vector2 position, int targetDisplay)
		{
			return UIElementsRuntimeUtility.FlipY(position, UIElementsRuntimeUtility.GetRuntimeDisplayHeight(targetDisplay));
		}

		internal static Vector2 ScreenBottomLeftToPanelDelta(Vector2 delta)
		{
			return UIElementsRuntimeUtility.FlipDeltaY(delta);
		}

		internal static Vector2 PanelToScreenBottomLeftPosition(Vector2 panelPosition, int targetDisplay)
		{
			return UIElementsRuntimeUtility.FlipY(panelPosition, UIElementsRuntimeUtility.GetRuntimeDisplayHeight(targetDisplay));
		}

		internal static Vector2 FlipY(Vector2 p, float displayHeight)
		{
			p.y = displayHeight - p.y;
			return p;
		}

		private static Vector2 FlipDeltaY(Vector2 delta)
		{
			delta.y = -delta.y;
			return delta;
		}

		private static float GetRuntimeDisplayHeight(int targetDisplay)
		{
			bool flag = targetDisplay > 0 && targetDisplay < Display.displays.Length;
			float num;
			if (flag)
			{
				num = (float)Display.displays[targetDisplay].systemHeight;
			}
			else
			{
				num = (float)Screen.height;
			}
			return num;
		}

		internal static float GetEditorDisplayHeight(int targetDisplay)
		{
			return UIElementsRuntimeUtility.GetRuntimeDisplayHeight(targetDisplay);
		}

		private static bool s_RegisteredPlayerloopCallback = false;

		private static readonly List<BaseRuntimePanel> s_SortedScreenOverlayPanels = new List<BaseRuntimePanel>();

		private static readonly List<BaseRuntimePanel> s_CachedWorldSpacePanels = new List<BaseRuntimePanel>();

		private static readonly List<BaseRuntimePanel> s_SortedPlayerPanels = new List<BaseRuntimePanel>();

		private static bool s_PanelOrderingOrDrawInCameraDirty = true;

		internal static int s_ResolvedSortingIndexMax = 0;

		private static int currentOverlayIndex = -1;

		private static DefaultEventSystem s_DefaultEventSystem;

		private static List<PanelSettings> s_PotentiallyEmptyPanelSettings = new List<PanelSettings>();

		public delegate BaseRuntimePanel CreateRuntimePanelDelegate(ScriptableObject ownerObject);
	}
}
