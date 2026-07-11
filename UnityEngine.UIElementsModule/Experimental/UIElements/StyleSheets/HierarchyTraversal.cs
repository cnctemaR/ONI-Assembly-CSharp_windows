using System;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	internal abstract class HierarchyTraversal
	{
		public virtual void Traverse(VisualElement element)
		{
			this.TraverseRecursive(element, 0);
		}

		public abstract void TraverseRecursive(VisualElement element, int depth);

		protected void Recurse(VisualElement element, int depth)
		{
			int i = 0;
			while (i < element.shadow.childCount)
			{
				VisualElement visualElement = element.shadow[i];
				this.TraverseRecursive(visualElement, depth + 1);
				if (visualElement.shadow.parent == element)
				{
					i++;
				}
			}
		}
	}
}
