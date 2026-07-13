using System;

namespace UnityEngine.UIElements
{
	internal enum HierarchyChangeType
	{
		AddedToParent,
		RemovedFromParent,
		ChildrenReordered,
		AttachedToPanel,
		DetachedFromPanel
	}
}
