using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class VisualTreeTransformClipUpdater : BaseVisualTreeUpdater
	{
		public override string description
		{
			get
			{
				return "Update Transform";
			}
		}

		public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			if ((versionChangeType & (VersionChangeType.Transform | VersionChangeType.Clip)) != (VersionChangeType)0)
			{
				if ((versionChangeType & VersionChangeType.Transform) == VersionChangeType.Transform && (!ve.isWorldTransformDirty || !ve.isWorldClipDirty))
				{
					this.DirtyTransformClipHierarchy(ve);
				}
				else if ((versionChangeType & VersionChangeType.Clip) == VersionChangeType.Clip && !ve.isWorldClipDirty)
				{
					this.DirtyClipHierarchy(ve);
				}
				this.m_Version += 1U;
			}
		}

		private void DirtyTransformClipHierarchy(VisualElement ve)
		{
			ve.isWorldTransformDirty = true;
			ve.isWorldClipDirty = true;
			int childCount = ve.shadow.childCount;
			for (int i = 0; i < childCount; i++)
			{
				VisualElement visualElement = ve.shadow[i];
				if (!visualElement.isWorldTransformDirty || !visualElement.isWorldClipDirty)
				{
					this.DirtyTransformClipHierarchy(visualElement);
				}
			}
		}

		private void DirtyClipHierarchy(VisualElement ve)
		{
			ve.isWorldClipDirty = true;
			int childCount = ve.shadow.childCount;
			for (int i = 0; i < childCount; i++)
			{
				VisualElement visualElement = ve.shadow[i];
				if (!visualElement.isWorldClipDirty)
				{
					this.DirtyClipHierarchy(visualElement);
				}
			}
		}

		public override void Update()
		{
			if (this.m_Version != this.m_LastVersion)
			{
				this.m_LastVersion = this.m_Version;
				base.panel.dispatcher.UpdateElementUnderMouse(base.panel);
			}
		}

		private uint m_Version = 0U;

		private uint m_LastVersion = 0U;
	}
}
