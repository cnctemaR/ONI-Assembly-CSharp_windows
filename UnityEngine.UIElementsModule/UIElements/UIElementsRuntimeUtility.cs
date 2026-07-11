using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Profiling;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.UIElements
{
	[NativeHeader("Modules/UIElements/UIElementsRuntimeUtility.h")]
	internal static class UIElementsRuntimeUtility
	{
		public static EventBase CreateEvent(Event systemEvent)
		{
			Debug.Assert(UIElementsRuntimeUtility.s_RuntimeDispatcher != null, "Call UIElementsRuntimeUtility.InitRuntimeEventSystem before sending any event.");
			return UIElementsUtility.CreateEvent(systemEvent, systemEvent.rawType);
		}

		public static IPanel CreateRuntimePanel(ScriptableObject ownerObject)
		{
			return UIElementsRuntimeUtility.FindOrCreateRuntimePanel(ownerObject);
		}

		public static IPanel FindOrCreateRuntimePanel(ScriptableObject ownerObject)
		{
			Panel panel;
			bool flag = !UIElementsUtility.TryGetPanel(ownerObject.GetInstanceID(), out panel);
			if (flag)
			{
				panel = new RuntimePanel(ownerObject, UIElementsRuntimeUtility.s_RuntimeDispatcher)
				{
					IMGUIEventInterests = new EventInterests
					{
						wantsMouseMove = true,
						wantsMouseEnterLeaveWindow = true
					}
				};
				UIElementsRuntimeUtility.RegisterCachedPanelInternal(ownerObject.GetInstanceID(), panel);
			}
			else
			{
				Debug.Assert(panel.contextType == ContextType.Player, "Panel is not a runtime panel.");
			}
			return panel;
		}

		public static void DisposeRuntimePanel(ScriptableObject ownerObject)
		{
			Panel panel;
			bool flag = UIElementsUtility.TryGetPanel(ownerObject.GetInstanceID(), out panel);
			if (flag)
			{
				panel.Dispose();
				UIElementsRuntimeUtility.RemoveCachedPanelInternal(ownerObject.GetInstanceID());
			}
		}

		public static void RegisterCachedPanel(int instanceID, IPanel panel)
		{
			UIElementsRuntimeUtility.RegisterCachedPanelInternal(instanceID, panel);
		}

		private static void RegisterCachedPanelInternal(int instanceID, IPanel panel)
		{
			UIElementsUtility.RegisterCachedPanel(instanceID, panel as Panel);
			bool flag = !UIElementsRuntimeUtility.s_RegisteredPlayerloopCallback;
			if (flag)
			{
				UIElementsRuntimeUtility.s_RegisteredPlayerloopCallback = true;
				UIElementsRuntimeUtility.RegisterPlayerloopCallback();
			}
		}

		public static void RemoveCachedPanel(int instanceID)
		{
			UIElementsRuntimeUtility.RemoveCachedPanelInternal(instanceID);
		}

		private static void RemoveCachedPanelInternal(int instanceID)
		{
			UIElementsUtility.RemoveCachedPanel(instanceID);
			UIElementsUtility.GetAllPanels(UIElementsRuntimeUtility.panelsIteration, ContextType.Player);
			bool flag = UIElementsRuntimeUtility.panelsIteration.Count == 0;
			if (flag)
			{
				UIElementsRuntimeUtility.s_RegisteredPlayerloopCallback = false;
				UIElementsRuntimeUtility.UnregisterPlayerloopCallback();
			}
		}

		[RequiredByNativeCode]
		public static void RepaintOverlayPanels()
		{
			UIElementsUtility.GetAllPanels(UIElementsRuntimeUtility.panelsIteration, ContextType.Player);
			foreach (Panel panel in UIElementsRuntimeUtility.panelsIteration)
			{
				bool flag = (panel as RuntimePanel).targetTexture == null;
				if (flag)
				{
					using (UIElementsRuntimeUtility.s_RepaintProfilerMarker.Auto())
					{
						panel.Repaint(Event.current);
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RegisterPlayerloopCallback();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UnregisterPlayerloopCallback();

		private static EventDispatcher s_RuntimeDispatcher = new EventDispatcher();

		private static bool s_RegisteredPlayerloopCallback = false;

		private static List<Panel> panelsIteration = new List<Panel>();

		internal static readonly string s_RepaintProfilerMarkerName = "UIElementsRuntimeUtility.DoDispatch(Repaint Event)";

		private static readonly ProfilerMarker s_RepaintProfilerMarker = new ProfilerMarker(UIElementsRuntimeUtility.s_RepaintProfilerMarkerName);
	}
}
