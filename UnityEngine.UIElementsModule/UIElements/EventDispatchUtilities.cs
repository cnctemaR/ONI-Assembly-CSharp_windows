using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	internal static class EventDispatchUtilities
	{
		public static void PropagateEvent(EventBase evt)
		{
			VisualElement visualElement = evt.target as VisualElement;
			bool flag = visualElement == null;
			if (!flag)
			{
				Debug.Assert(!evt.dispatch, "Event is being dispatched recursively.");
				evt.dispatch = true;
				bool flag2 = !evt.bubblesOrTricklesDown;
				if (flag2)
				{
					bool flag3 = visualElement.HasEventCallbacksOrDefaultActionAtTarget(evt.eventCategory);
					if (flag3)
					{
						visualElement.HandleEventAtTargetPhase(evt);
					}
				}
				else
				{
					bool flag4 = visualElement.HasParentEventCallbacksOrDefaultActionAtTarget(evt.eventCategory);
					if (flag4)
					{
						EventDispatchUtilities.HandleEventAcrossPropagationPath(evt);
					}
				}
				evt.dispatch = false;
			}
		}

		private static void HandleEventAcrossPropagationPath(EventBase evt)
		{
			VisualElement visualElement = (VisualElement)evt.leafTarget;
			PropagationPaths propagationPaths = PropagationPaths.Build(visualElement, evt);
			evt.path = propagationPaths;
			EventDebugger.LogPropagationPaths(evt, propagationPaths);
			IPanel panel = visualElement.panel;
			bool tricklesDown = evt.tricklesDown;
			if (tricklesDown)
			{
				evt.propagationPhase = PropagationPhase.TrickleDown;
				for (int i = propagationPaths.trickleDownPath.Count - 1; i >= 0; i--)
				{
					bool isPropagationStopped = evt.isPropagationStopped;
					if (isPropagationStopped)
					{
						break;
					}
					VisualElement visualElement2 = propagationPaths.trickleDownPath[i];
					bool flag = evt.Skip(visualElement2) || visualElement2.panel != panel;
					if (!flag)
					{
						evt.currentTarget = visualElement2;
						evt.currentTarget.HandleEvent(evt);
					}
				}
			}
			evt.propagationPhase = PropagationPhase.AtTarget;
			foreach (VisualElement visualElement3 in propagationPaths.targetElements)
			{
				bool flag2 = evt.Skip(visualElement3) || visualElement3.panel != panel;
				if (!flag2)
				{
					evt.target = visualElement3;
					evt.currentTarget = evt.target;
					evt.currentTarget.HandleEvent(evt);
				}
			}
			evt.propagationPhase = PropagationPhase.DefaultActionAtTarget;
			foreach (VisualElement visualElement4 in propagationPaths.targetElements)
			{
				bool flag3 = evt.Skip(visualElement4) || visualElement4.panel != panel;
				if (!flag3)
				{
					evt.target = visualElement4;
					evt.currentTarget = evt.target;
					evt.currentTarget.HandleEvent(evt);
				}
			}
			evt.target = evt.leafTarget;
			bool bubbles = evt.bubbles;
			if (bubbles)
			{
				evt.propagationPhase = PropagationPhase.BubbleUp;
				foreach (VisualElement visualElement5 in propagationPaths.bubbleUpPath)
				{
					bool flag4 = evt.Skip(visualElement5) || visualElement5.panel != panel;
					if (!flag4)
					{
						evt.currentTarget = visualElement5;
						evt.currentTarget.HandleEvent(evt);
					}
				}
			}
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
					List<VisualElement> list;
					using (CollectionPool<List<VisualElement>, VisualElement>.Get(out list))
					{
						list.AddRange(root.hierarchy.children);
						foreach (VisualElement visualElement in list)
						{
							bool flag7 = visualElement.hierarchy.parent != root;
							if (!flag7)
							{
								EventDispatchUtilities.PropagateToIMGUIContainer(visualElement, evt);
								bool isPropagationStopped = evt.isPropagationStopped;
								if (isPropagationStopped)
								{
									break;
								}
							}
						}
					}
				}
			}
		}

		public static void ExecuteDefaultAction(EventBase evt)
		{
			VisualElement visualElement = evt.target as VisualElement;
			bool flag = visualElement != null && visualElement.HasDefaultAction(evt.eventCategory);
			if (flag)
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
