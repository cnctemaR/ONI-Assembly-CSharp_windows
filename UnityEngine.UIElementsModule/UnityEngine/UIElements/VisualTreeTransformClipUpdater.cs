using System;
using Unity.Profiling;

namespace UnityEngine.UIElements
{
	internal class VisualTreeTransformClipUpdater : BaseVisualTreeUpdater
	{
		public override ProfilerMarker profilerMarker
		{
			get
			{
				return VisualTreeTransformClipUpdater.s_ProfilerMarker;
			}
		}

		public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			bool flag = (versionChangeType & (VersionChangeType.Hierarchy | VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size)) == (VersionChangeType)0;
			if (!flag)
			{
				bool flag2 = (versionChangeType & VersionChangeType.Transform) > (VersionChangeType)0;
				bool flag3 = (versionChangeType & (VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size)) > (VersionChangeType)0;
				flag2 = flag2 && !ve.isWorldTransformDirty;
				flag3 = flag3 && !ve.isWorldClipDirty;
				bool flag4 = flag2 || flag3;
				if (flag4)
				{
					VisualTreeTransformClipUpdater.DirtyHierarchy(ve, flag2, flag3);
				}
				VisualTreeTransformClipUpdater.DirtyBoundingBoxHierarchy(ve);
				this.m_Version += 1U;
			}
		}

		private static void DirtyHierarchy(VisualElement ve, bool mustDirtyWorldTransform, bool mustDirtyWorldClip)
		{
			if (mustDirtyWorldTransform)
			{
				ve.isWorldTransformDirty = true;
				ve.isWorldBoundingBoxDirty = true;
			}
			if (mustDirtyWorldClip)
			{
				ve.isWorldClipDirty = true;
			}
			int childCount = ve.hierarchy.childCount;
			for (int i = 0; i < childCount; i++)
			{
				VisualElement visualElement = ve.hierarchy[i];
				bool flag = (mustDirtyWorldTransform && !visualElement.isWorldTransformDirty) || (mustDirtyWorldClip && !visualElement.isWorldClipDirty);
				if (flag)
				{
					VisualTreeTransformClipUpdater.DirtyHierarchy(visualElement, mustDirtyWorldTransform, mustDirtyWorldClip);
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

		private static readonly string s_Description = "Update Transform";

		private static readonly ProfilerMarker s_ProfilerMarker = new ProfilerMarker(VisualTreeTransformClipUpdater.s_Description);
	}
}
