using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class PropagationPaths
	{
		public PropagationPaths()
		{
			this.trickleDownPath = new List<VisualElement>(16);
			this.targetElements = new List<VisualElement>(4);
			this.bubbleUpPath = new List<VisualElement>(16);
		}

		public PropagationPaths(PropagationPaths paths)
		{
			this.trickleDownPath = new List<VisualElement>(paths.trickleDownPath);
			this.targetElements = new List<VisualElement>(paths.targetElements);
			this.bubbleUpPath = new List<VisualElement>(paths.bubbleUpPath);
		}

		internal static PropagationPaths Copy(PropagationPaths paths)
		{
			PropagationPaths propagationPaths = PropagationPaths.s_Pool.Get();
			propagationPaths.trickleDownPath.AddRange(paths.trickleDownPath);
			propagationPaths.targetElements.AddRange(paths.targetElements);
			propagationPaths.bubbleUpPath.AddRange(paths.bubbleUpPath);
			return propagationPaths;
		}

		public static PropagationPaths Build(VisualElement elem, EventBase evt)
		{
			PropagationPaths propagationPaths = PropagationPaths.s_Pool.Get();
			EventCategory eventCategory = evt.eventCategory;
			bool flag = elem.HasEventCallbacksOrDefaultActions(eventCategory);
			if (flag)
			{
				propagationPaths.targetElements.Add(elem);
			}
			for (VisualElement visualElement = elem.nextParentWithEventCallback; visualElement != null; visualElement = visualElement.nextParentWithEventCallback)
			{
				bool flag2 = visualElement.isCompositeRoot && !evt.ignoreCompositeRoots;
				if (flag2)
				{
					bool flag3 = visualElement.HasEventCallbacksOrDefaultActions(eventCategory);
					if (flag3)
					{
						propagationPaths.targetElements.Add(visualElement);
					}
				}
				else
				{
					bool flag4 = visualElement.HasEventCallbacks(eventCategory);
					if (flag4)
					{
						bool flag5 = evt.tricklesDown && visualElement.HasTrickleDownHandlers();
						if (flag5)
						{
							propagationPaths.trickleDownPath.Add(visualElement);
						}
						bool flag6 = evt.bubbles && visualElement.HasBubbleUpHandlers();
						if (flag6)
						{
							propagationPaths.bubbleUpPath.Add(visualElement);
						}
					}
				}
			}
			return propagationPaths;
		}

		public void Release()
		{
			this.bubbleUpPath.Clear();
			this.targetElements.Clear();
			this.trickleDownPath.Clear();
			PropagationPaths.s_Pool.Release(this);
		}

		private static readonly ObjectPool<PropagationPaths> s_Pool = new ObjectPool<PropagationPaths>(() => new PropagationPaths(), 100);

		public readonly List<VisualElement> trickleDownPath;

		public readonly List<VisualElement> targetElements;

		public readonly List<VisualElement> bubbleUpPath;

		private const int k_DefaultPropagationDepth = 16;

		private const int k_DefaultTargetCount = 4;

		[Flags]
		public enum Type
		{
			None = 0,
			TrickleDown = 1,
			BubbleUp = 2
		}
	}
}
