using System;
using Unity.Profiling;

namespace UnityEngine.UIElements
{
	internal class VisualTreeHierarchyFlagsUpdater : BaseVisualTreeUpdater
	{
		public override ProfilerMarker profilerMarker
		{
			get
			{
				return VisualTreeHierarchyFlagsUpdater.s_ProfilerMarker;
			}
		}

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
					VisualTreeHierarchyFlagsUpdater.DirtyBoundingBoxHierarchy(ve);
				}
				bool flag4 = (versionChangeType & (VersionChangeType.Hierarchy | VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size | VersionChangeType.Picking)) > (VersionChangeType)0;
				if (flag4)
				{
					this.m_Version += 1U;
				}
			}
		}

		protected static VisualElementFlags GetChildrenMustDirtyFlags(VisualElement ve, VersionChangeType versionChangeType)
		{
			VisualElementFlags visualElementFlags = (VisualElementFlags)0;
			bool flag = (versionChangeType & VersionChangeType.Transform) > (VersionChangeType)0;
			if (flag)
			{
				visualElementFlags |= VisualElementFlags.WorldTransformDirty | VisualElementFlags.WorldBoundingBoxDirty;
			}
			bool flag2 = (versionChangeType & (VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size)) > (VersionChangeType)0;
			if (flag2)
			{
				visualElementFlags |= VisualElementFlags.WorldClipDirty;
			}
			bool flag3 = (versionChangeType & (VersionChangeType.Hierarchy | VersionChangeType.EventCallbackCategories)) > (VersionChangeType)0;
			if (flag3)
			{
				visualElementFlags |= VisualElementFlags.EventInterestParentCategoriesDirty;
			}
			return visualElementFlags;
		}

		protected static void DirtyChildrenHierarchy(VisualElement ve, VisualElementFlags mustDirtyFlags)
		{
			VisualElementFlags visualElementFlags = mustDirtyFlags & ~ve.flags;
			bool flag = visualElementFlags == (VisualElementFlags)0;
			if (!flag)
			{
				ve.flags |= visualElementFlags;
				int childCount = ve.hierarchy.childCount;
				for (int i = 0; i < childCount; i++)
				{
					VisualElement visualElement = ve.hierarchy[i];
					VisualTreeHierarchyFlagsUpdater.DirtyChildrenHierarchy(visualElement, visualElementFlags);
				}
			}
		}

		private static void DirtyBoundingBoxHierarchy(VisualElement ve)
		{
			ve.flags |= VisualElementFlags.BoundingBoxDirty | VisualElementFlags.WorldBoundingBoxDirty | VisualElementFlags.BoundingBoxDirtiedSinceLastLayoutPass;
			VisualTreeHierarchyFlagsUpdater.DirtyParentHierarchy(ve.hierarchy.parent, VisualElementFlags.BoundingBoxDirty | VisualElementFlags.WorldBoundingBoxDirty | VisualElementFlags.BoundingBoxDirtiedSinceLastLayoutPass);
		}

		private static void DirtyParentHierarchy(VisualElement ve, VisualElementFlags flags)
		{
			while (ve != null && (ve.flags & flags) != flags)
			{
				ve.flags |= flags;
				ve = ve.hierarchy.parent;
			}
		}

		public override void Update()
		{
			bool flag = this.m_Version == this.m_LastVersion;
			if (!flag)
			{
				this.m_LastVersion = this.m_Version;
				base.panel.visualTree.UpdateBoundingBox();
				bool flag2 = base.panel.UpdateElementUnderPointers();
				bool flag3 = flag2 && base.panel.contextType == ContextType.Editor;
				if (flag3)
				{
					base.panel.ApplyStyles();
				}
			}
		}

		private uint m_Version = 0U;

		private uint m_LastVersion = 0U;

		private static readonly string s_Description = "UIElements.UpdateElementBounds";

		private static readonly ProfilerMarker s_ProfilerMarker = new ProfilerMarker(VisualTreeHierarchyFlagsUpdater.s_Description);

		private const VersionChangeType WorldTransformChanged = VersionChangeType.Transform;

		private const VersionChangeType WorldClipChanged = VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size;

		private const VersionChangeType EventParentCategoriesChanged = VersionChangeType.Hierarchy | VersionChangeType.EventCallbackCategories;

		protected const VersionChangeType BoundingBoxChanged = VersionChangeType.Hierarchy | VersionChangeType.Overflow | VersionChangeType.Transform | VersionChangeType.Size;

		protected const VersionChangeType ChildrenChanged = VersionChangeType.Hierarchy | VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size | VersionChangeType.EventCallbackCategories;

		protected const VersionChangeType VersionChanged = VersionChangeType.Hierarchy | VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size | VersionChangeType.Picking;

		protected const VersionChangeType AnythingChanged = VersionChangeType.Hierarchy | VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size | VersionChangeType.EventCallbackCategories | VersionChangeType.Picking;

		protected const VisualElementFlags BoundingBoxDirtyFlags = VisualElementFlags.BoundingBoxDirty | VisualElementFlags.WorldBoundingBoxDirty | VisualElementFlags.BoundingBoxDirtiedSinceLastLayoutPass;
	}
}
