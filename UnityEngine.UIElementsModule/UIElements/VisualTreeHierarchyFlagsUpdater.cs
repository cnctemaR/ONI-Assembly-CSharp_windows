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
				bool flag2 = (versionChangeType & VersionChangeType.Transform) > (VersionChangeType)0;
				bool flag3 = (versionChangeType & (VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size)) > (VersionChangeType)0;
				bool flag4 = (versionChangeType & (VersionChangeType.Hierarchy | VersionChangeType.EventCallbackCategories)) > (VersionChangeType)0;
				VisualElementFlags visualElementFlags = (flag2 ? (VisualElementFlags.WorldTransformDirty | VisualElementFlags.WorldBoundingBoxDirty) : ((VisualElementFlags)0)) | (flag3 ? VisualElementFlags.WorldClipDirty : ((VisualElementFlags)0)) | (flag4 ? VisualElementFlags.EventCallbackParentCategoriesDirty : ((VisualElementFlags)0));
				VisualElementFlags visualElementFlags2 = visualElementFlags & ~ve.m_Flags;
				bool flag5 = visualElementFlags2 > (VisualElementFlags)0;
				if (flag5)
				{
					VisualTreeHierarchyFlagsUpdater.DirtyHierarchy(ve, visualElementFlags2);
				}
				VisualTreeHierarchyFlagsUpdater.DirtyBoundingBoxHierarchy(ve);
				this.m_Version += 1U;
			}
		}

		private static void DirtyHierarchy(VisualElement ve, VisualElementFlags mustDirtyFlags)
		{
			ve.m_Flags |= mustDirtyFlags;
			int childCount = ve.hierarchy.childCount;
			for (int i = 0; i < childCount; i++)
			{
				VisualElement visualElement = ve.hierarchy[i];
				VisualElementFlags visualElementFlags = mustDirtyFlags & ~visualElement.m_Flags;
				bool flag = visualElementFlags > (VisualElementFlags)0;
				if (flag)
				{
					VisualTreeHierarchyFlagsUpdater.DirtyHierarchy(visualElement, visualElementFlags);
				}
			}
		}

		private static void DirtyBoundingBoxHierarchy(VisualElement ve)
		{
			ve.isBoundingBoxDirty = true;
			ve.isWorldBoundingBoxDirty = true;
			VisualElement visualElement = ve.hierarchy.parent;
			while (visualElement != null && !visualElement.isBoundingBoxDirty)
			{
				visualElement.isBoundingBoxDirty = true;
				visualElement.isWorldBoundingBoxDirty = true;
				visualElement = visualElement.hierarchy.parent;
			}
		}

		public override void Update()
		{
			bool flag = this.m_Version == this.m_LastVersion;
			if (!flag)
			{
				this.m_LastVersion = this.m_Version;
				base.panel.UpdateElementUnderPointers();
				base.panel.visualTree.UpdateBoundingBox();
			}
		}

		private uint m_Version = 0U;

		private uint m_LastVersion = 0U;

		private static readonly string s_Description = "Update Hierarchy Flags";

		private static readonly ProfilerMarker s_ProfilerMarker = new ProfilerMarker(VisualTreeHierarchyFlagsUpdater.s_Description);
	}
}
