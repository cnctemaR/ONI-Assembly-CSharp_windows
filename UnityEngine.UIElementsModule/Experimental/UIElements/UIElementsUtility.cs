using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	internal class UIElementsUtility
	{
		static UIElementsUtility()
		{
			GUIUtility.takeCapture = (Action)Delegate.Combine(GUIUtility.takeCapture, new Action(UIElementsUtility.TakeCapture));
			GUIUtility.releaseCapture = (Action)Delegate.Combine(GUIUtility.releaseCapture, new Action(UIElementsUtility.ReleaseCapture));
			GUIUtility.processEvent = (Func<int, IntPtr, bool>)Delegate.Combine(GUIUtility.processEvent, new Func<int, IntPtr, bool>(UIElementsUtility.ProcessEvent));
			GUIUtility.cleanupRoots = (Action)Delegate.Combine(GUIUtility.cleanupRoots, new Action(UIElementsUtility.CleanupRoots));
			GUIUtility.endContainerGUIFromException = (Func<Exception, bool>)Delegate.Combine(GUIUtility.endContainerGUIFromException, new Func<Exception, bool>(UIElementsUtility.EndContainerGUIFromException));
			GUIUtility.enabledStateChanged = (Action)Delegate.Combine(GUIUtility.enabledStateChanged, new Action(UIElementsUtility.EnabledStateChanged));
		}

		private static void EnabledStateChanged()
		{
			if (UIElementsUtility.s_ContainerStack.Count > 0)
			{
				UIElementsUtility.s_ContainerStack.Peek().MarkDirtyLayout();
			}
		}

		private static void TakeCapture()
		{
			if (UIElementsUtility.s_ContainerStack.Count > 0)
			{
				IMGUIContainer imguicontainer = UIElementsUtility.s_ContainerStack.Peek();
				if (MouseCaptureController.IsMouseCaptured() && !imguicontainer.HasMouseCapture())
				{
					Debug.Log("Should not grab hot control with an active capture");
				}
				imguicontainer.CaptureMouse();
			}
		}

		private static void ReleaseCapture()
		{
			MouseCaptureController.ReleaseMouse();
		}

		private static bool ProcessEvent(int instanceID, IntPtr nativeEventPtr)
		{
			Panel panel;
			bool flag;
			if (nativeEventPtr != IntPtr.Zero && UIElementsUtility.s_UIElementsCache.TryGetValue(instanceID, out panel))
			{
				UIElementsUtility.s_EventInstance.CopyFromPtr(nativeEventPtr);
				flag = UIElementsUtility.DoDispatch(panel);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public static void RemoveCachedPanel(int instanceID)
		{
			UIElementsUtility.s_UIElementsCache.Remove(instanceID);
		}

		private static void CleanupRoots()
		{
			UIElementsUtility.s_EventInstance = null;
			EventDispatcher.ClearDispatcher();
			UIElementsUtility.s_UIElementsCache = null;
			UIElementsUtility.s_ContainerStack = null;
		}

		private static bool EndContainerGUIFromException(Exception exception)
		{
			if (UIElementsUtility.s_ContainerStack.Count > 0)
			{
				GUIUtility.EndContainer();
				UIElementsUtility.s_ContainerStack.Pop();
			}
			return GUIUtility.ShouldRethrowException(exception);
		}

		internal static void BeginContainerGUI(GUILayoutUtility.LayoutCache cache, Event evt, IMGUIContainer container)
		{
			if (container.useOwnerObjectGUIState)
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
			if (Event.current == null)
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

		internal static void EndContainerGUI(Event evt)
		{
			if (Event.current.type == EventType.Layout && UIElementsUtility.s_ContainerStack.Count > 0)
			{
				Rect layout = UIElementsUtility.s_ContainerStack.Peek().layout;
				GUILayoutUtility.LayoutFromContainer(layout.width, layout.height);
			}
			GUILayoutUtility.SelectIDList(GUIUtility.s_OriginalID, false);
			GUIContent.ClearStaticCache();
			if (UIElementsUtility.s_ContainerStack.Count > 0)
			{
			}
			evt.CopyFrom(Event.current);
			if (UIElementsUtility.s_ContainerStack.Count > 0)
			{
				GUIUtility.EndContainer();
				UIElementsUtility.s_ContainerStack.Pop();
			}
		}

		internal static ContextType GetGUIContextType()
		{
			return (GUIUtility.s_SkinMode != 0) ? ContextType.Editor : ContextType.Player;
		}

		internal static EventBase CreateEvent(Event systemEvent)
		{
			return UIElementsUtility.CreateEvent(systemEvent, systemEvent.type);
		}

		internal static EventBase CreateEvent(Event systemEvent, EventType eventType)
		{
			switch (eventType)
			{
			case EventType.MouseDown:
				return MouseEventBase<MouseDownEvent>.GetPooled(systemEvent);
			case EventType.MouseUp:
				return MouseEventBase<MouseUpEvent>.GetPooled(systemEvent);
			case EventType.MouseMove:
				return MouseEventBase<MouseMoveEvent>.GetPooled(systemEvent);
			case EventType.MouseDrag:
				return MouseEventBase<MouseMoveEvent>.GetPooled(systemEvent);
			case EventType.KeyDown:
				return KeyboardEventBase<KeyDownEvent>.GetPooled(systemEvent);
			case EventType.KeyUp:
				return KeyboardEventBase<KeyUpEvent>.GetPooled(systemEvent);
			case EventType.ScrollWheel:
				return WheelEvent.GetPooled(systemEvent);
			case EventType.DragUpdated:
				return MouseEventBase<DragUpdatedEvent>.GetPooled(systemEvent);
			case EventType.DragPerform:
				return MouseEventBase<DragPerformEvent>.GetPooled(systemEvent);
			case EventType.ValidateCommand:
				return CommandEventBase<ValidateCommandEvent>.GetPooled(systemEvent);
			case EventType.ExecuteCommand:
				return CommandEventBase<ExecuteCommandEvent>.GetPooled(systemEvent);
			case EventType.DragExited:
				return MouseEventBase<DragExitedEvent>.GetPooled(systemEvent);
			case EventType.ContextClick:
				return MouseEventBase<ContextClickEvent>.GetPooled(systemEvent);
			case EventType.MouseEnterWindow:
				return MouseEventBase<MouseEnterWindowEvent>.GetPooled(systemEvent);
			case EventType.MouseLeaveWindow:
				return MouseEventBase<MouseLeaveWindowEvent>.GetPooled(systemEvent);
			}
			return IMGUIEvent.GetPooled(systemEvent);
		}

		private static bool DoDispatch(BaseVisualElementPanel panel)
		{
			bool flag;
			if (UIElementsUtility.s_EventInstance.type == EventType.Repaint)
			{
				panel.Repaint(UIElementsUtility.s_EventInstance);
				flag = panel.IMGUIContainersCount > 0;
			}
			else
			{
				panel.ValidateLayout();
				using (EventBase eventBase = UIElementsUtility.CreateEvent(UIElementsUtility.s_EventInstance))
				{
					bool flag2 = UIElementsUtility.s_EventInstance.type == EventType.Used || UIElementsUtility.s_EventInstance.type == EventType.Layout || UIElementsUtility.s_EventInstance.type == EventType.ExecuteCommand || UIElementsUtility.s_EventInstance.type == EventType.ValidateCommand;
					panel.SendEvent(eventBase, (!flag2) ? DispatchMode.Default : DispatchMode.Immediate);
					if (eventBase.isPropagationStopped)
					{
						panel.visualTree.IncrementVersion(VersionChangeType.Repaint);
					}
					flag = eventBase.isPropagationStopped;
				}
			}
			return flag;
		}

		internal static Dictionary<int, Panel>.Enumerator GetPanelsIterator()
		{
			return UIElementsUtility.s_UIElementsCache.GetEnumerator();
		}

		internal static Panel FindOrCreatePanel(ScriptableObject ownerObject, ContextType contextType, IDataWatchService dataWatch = null)
		{
			Panel panel;
			if (!UIElementsUtility.s_UIElementsCache.TryGetValue(ownerObject.GetInstanceID(), out panel))
			{
				panel = new Panel(ownerObject, contextType, dataWatch, null);
				UIElementsUtility.s_UIElementsCache.Add(ownerObject.GetInstanceID(), panel);
			}
			else
			{
				Debug.Assert(contextType == panel.contextType, "Context type mismatch");
			}
			return panel;
		}

		internal static Panel FindOrCreatePanel(ScriptableObject ownerObject)
		{
			return UIElementsUtility.FindOrCreatePanel(ownerObject, UIElementsUtility.GetGUIContextType(), null);
		}

		private static Stack<IMGUIContainer> s_ContainerStack = new Stack<IMGUIContainer>();

		private static Dictionary<int, Panel> s_UIElementsCache = new Dictionary<int, Panel>();

		private static Event s_EventInstance = new Event();

		internal static Color editorPlayModeTintColor = Color.white;
	}
}
