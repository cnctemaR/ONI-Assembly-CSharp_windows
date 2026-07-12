using System;

namespace UnityEngine.UIElements
{
	internal static class EventDispatchUtilities
	{
		public static void PropagateEvent(EventBase evt)
		{
			Debug.Assert(!evt.dispatch, "Event is being dispatched recursively.");
			evt.dispatch = true;
			bool flag = evt.path == null;
			if (flag)
			{
				CallbackEventHandler callbackEventHandler = evt.target as CallbackEventHandler;
				if (callbackEventHandler != null)
				{
					callbackEventHandler.HandleEventAtTargetPhase(evt);
				}
			}
			else
			{
				bool tricklesDown = evt.tricklesDown;
				if (tricklesDown)
				{
					evt.propagationPhase = PropagationPhase.TrickleDown;
					for (int i = evt.path.trickleDownPath.Count - 1; i >= 0; i--)
					{
						bool isPropagationStopped = evt.isPropagationStopped;
						if (isPropagationStopped)
						{
							break;
						}
						bool flag2 = evt.Skip(evt.path.trickleDownPath[i]);
						if (!flag2)
						{
							evt.currentTarget = evt.path.trickleDownPath[i];
							evt.currentTarget.HandleEvent(evt);
						}
					}
				}
				evt.propagationPhase = PropagationPhase.AtTarget;
				foreach (VisualElement visualElement in evt.path.targetElements)
				{
					bool flag3 = evt.Skip(visualElement);
					if (!flag3)
					{
						evt.target = visualElement;
						evt.currentTarget = evt.target;
						evt.currentTarget.HandleEvent(evt);
					}
				}
				evt.propagationPhase = PropagationPhase.DefaultActionAtTarget;
				foreach (VisualElement visualElement2 in evt.path.targetElements)
				{
					bool flag4 = evt.Skip(visualElement2);
					if (!flag4)
					{
						evt.target = visualElement2;
						evt.currentTarget = evt.target;
						evt.currentTarget.HandleEvent(evt);
					}
				}
				evt.target = evt.leafTarget;
				bool bubbles = evt.bubbles;
				if (bubbles)
				{
					evt.propagationPhase = PropagationPhase.BubbleUp;
					foreach (VisualElement visualElement3 in evt.path.bubbleUpPath)
					{
						bool flag5 = evt.Skip(visualElement3);
						if (!flag5)
						{
							evt.currentTarget = visualElement3;
							evt.currentTarget.HandleEvent(evt);
						}
					}
				}
			}
			evt.dispatch = false;
			evt.propagationPhase = PropagationPhase.None;
			evt.currentTarget = null;
		}

		internal static void PropagateToIMGUIContainer(VisualElement root, EventBase evt)
		{
			bool flag = evt.imguiEvent == null || root.elementPanel.contextType == ContextType.Player;
			if (!flag)
			{
				bool isIMGUIContainer = root.isIMGUIContainer;
				if (isIMGUIContainer)
				{
					IMGUIContainer imguicontainer = root as IMGUIContainer;
					bool flag2 = evt.Skip(imguicontainer);
					if (flag2)
					{
						return;
					}
					Focusable focusable = evt.target as Focusable;
					bool flag3 = focusable != null && focusable.focusable;
					bool flag4 = imguicontainer.SendEventToIMGUI(evt, !flag3, true);
					if (flag4)
					{
						evt.StopPropagation();
						evt.PreventDefault();
					}
					bool flag5 = evt.imguiEvent.rawType == EventType.Used;
					if (flag5)
					{
						Debug.Assert(evt.isPropagationStopped);
					}
				}
				bool flag6 = root.imguiContainerDescendantCount > 0;
				if (flag6)
				{
					int childCount = root.hierarchy.childCount;
					for (int i = 0; i < childCount; i++)
					{
						EventDispatchUtilities.PropagateToIMGUIContainer(root.hierarchy[i], evt);
						bool isPropagationStopped = evt.isPropagationStopped;
						if (isPropagationStopped)
						{
							break;
						}
					}
				}
			}
		}

		public static void ExecuteDefaultAction(EventBase evt, IPanel panel)
		{
			bool flag = evt.target == null && panel != null;
			if (flag)
			{
				evt.target = panel.visualTree;
			}
			bool flag2 = evt.target != null;
			if (flag2)
			{
				evt.dispatch = true;
				evt.currentTarget = evt.target;
				evt.propagationPhase = PropagationPhase.DefaultAction;
				evt.currentTarget.HandleEvent(evt);
				evt.propagationPhase = PropagationPhase.None;
				evt.currentTarget = null;
				evt.dispatch = false;
			}
		}
	}
}
