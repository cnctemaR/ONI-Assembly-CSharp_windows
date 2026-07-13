using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace UnityEngine.UIElements
{
	internal class PropagationPaths : IDisposable
	{
		public PropagationPaths()
		{
			this.trickleDownPath = new List<VisualElement>(8);
			this.bubbleUpPath = new List<VisualElement>(8);
		}

		public PropagationPaths(PropagationPaths paths)
			: this()
		{
			bool flag = paths != null;
			if (flag)
			{
				this.trickleDownPath.AddRange(paths.trickleDownPath);
				this.bubbleUpPath.AddRange(paths.bubbleUpPath);
			}
		}

		[NotNull]
		public static PropagationPaths Build(VisualElement elem, EventBase evt, int eventCategories)
		{
			PropagationPaths propagationPaths = PropagationPaths.s_Pool.Get();
			bool flag = elem.HasTrickleDownEventInterests(eventCategories);
			if (flag)
			{
				propagationPaths.trickleDownPath.Add(elem);
			}
			bool flag2 = elem.HasBubbleUpEventInterests(eventCategories);
			if (flag2)
			{
				propagationPaths.bubbleUpPath.Add(elem);
			}
			for (VisualElement visualElement = elem.nextParentWithEventInterests; visualElement != null; visualElement = visualElement.nextParentWithEventInterests)
			{
				bool flag3 = !visualElement.HasParentEventInterests(eventCategories);
				if (flag3)
				{
					break;
				}
				bool flag4 = evt.tricklesDown && visualElement.HasTrickleDownEventInterests(eventCategories);
				if (flag4)
				{
					propagationPaths.trickleDownPath.Add(visualElement);
				}
				bool flag5 = evt.bubbles && visualElement.HasBubbleUpEventInterests(eventCategories);
				if (flag5)
				{
					propagationPaths.bubbleUpPath.Add(visualElement);
				}
			}
			return propagationPaths;
		}

		public void Dispose()
		{
			this.bubbleUpPath.Clear();
			this.trickleDownPath.Clear();
			PropagationPaths.s_Pool.Release(this);
		}

		private static readonly ObjectPool<PropagationPaths> s_Pool = new ObjectPool<PropagationPaths>(() => new PropagationPaths(), 100);

		public readonly List<VisualElement> trickleDownPath;

		public readonly List<VisualElement> bubbleUpPath;

		private const int k_DefaultPropagationDepth = 8;
	}
}
