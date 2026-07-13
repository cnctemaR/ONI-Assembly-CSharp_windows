using System;

namespace UnityEngine.UIElements
{
	internal class VisualTreeWorldSpaceHierarchyFlagsUpdater : VisualTreeHierarchyFlagsUpdater
	{
		public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			bool flag = (versionChangeType & (VersionChangeType.Hierarchy | VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size | VersionChangeType.EventCallbackCategories | VersionChangeType.Picking)) == (VersionChangeType)0;
			if (!flag)
			{
				bool flag2 = (versionChangeType & (VersionChangeType.Hierarchy | VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size | VersionChangeType.EventCallbackCategories)) > (VersionChangeType)0;
				if (flag2)
				{
					VisualTreeHierarchyFlagsUpdater.DirtyChildrenHierarchy(ve, VisualTreeHierarchyFlagsUpdater.GetChildrenMustDirtyFlags(ve, versionChangeType));
				}
				bool flag3 = (versionChangeType & (VersionChangeType.Hierarchy | VersionChangeType.Overflow | VersionChangeType.Transform | VersionChangeType.Size)) > (VersionChangeType)0;
				if (flag3)
				{
					VisualTreeWorldSpaceHierarchyFlagsUpdater.DirtyBoundingBoxHierarchy(ve);
				}
			}
		}

		private static VisualElementFlags GetParentMustDirtyFlags(VisualElement ve)
		{
			VisualElementFlags visualElementFlags = VisualElementFlags.BoundingBoxDirty | VisualElementFlags.WorldBoundingBoxDirty | VisualElementFlags.LocalBounds3DDirty | VisualElementFlags.LocalBoundsWithoutNested3DDirty | VisualElementFlags.BoundingBoxDirtiedSinceLastLayoutPass;
			bool has3DTransform = ve.has3DTransform;
			if (has3DTransform)
			{
				visualElementFlags |= VisualElementFlags.Needs3DBounds;
			}
			return visualElementFlags;
		}

		private static void DirtyBoundingBoxHierarchy(VisualElement ve)
		{
			VisualElementFlags visualElementFlags = VisualTreeWorldSpaceHierarchyFlagsUpdater.GetParentMustDirtyFlags(ve);
			ve.flags |= visualElementFlags;
			bool flag = ve is UIDocumentRootElement;
			if (flag)
			{
				visualElementFlags &= ~VisualElementFlags.LocalBoundsWithoutNested3DDirty;
			}
			VisualTreeWorldSpaceHierarchyFlagsUpdater.DirtyParentHierarchy(ve.hierarchy.parent, visualElementFlags);
		}

		private static void DirtyParentHierarchy(VisualElement ve, VisualElementFlags flags)
		{
			while (ve != null && (ve.flags & flags) != flags)
			{
				ve.flags |= flags;
				bool flag = ve is UIDocumentRootElement;
				if (flag)
				{
					flags &= ~VisualElementFlags.LocalBoundsWithoutNested3DDirty;
				}
				ve = ve.hierarchy.parent;
			}
		}

		public override void Update()
		{
		}

		private new const VisualElementFlags BoundingBoxDirtyFlags = VisualElementFlags.BoundingBoxDirty | VisualElementFlags.WorldBoundingBoxDirty | VisualElementFlags.LocalBounds3DDirty | VisualElementFlags.LocalBoundsWithoutNested3DDirty | VisualElementFlags.BoundingBoxDirtiedSinceLastLayoutPass;
	}
}
