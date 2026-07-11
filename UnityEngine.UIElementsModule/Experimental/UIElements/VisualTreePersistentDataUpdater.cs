using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	internal class VisualTreePersistentDataUpdater : BaseVisualTreeUpdater
	{
		public override string description
		{
			get
			{
				return "Update PersistentData";
			}
		}

		public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			if ((versionChangeType & VersionChangeType.PersistentData) == VersionChangeType.PersistentData)
			{
				this.m_Version += 1U;
				this.m_UpdateList.Add(ve);
				this.PropagateToParents(ve);
			}
		}

		public override void Update()
		{
			if (this.m_Version != this.m_LastVersion)
			{
				int num = 0;
				while (this.m_LastVersion != this.m_Version)
				{
					this.m_LastVersion = this.m_Version;
					this.ValidatePersistentDataOnSubTree(base.visualTree, true);
					num++;
					if (num > 5)
					{
						Debug.LogError("UIElements: Too many children recursively added that rely on persistent data: " + base.visualTree);
						break;
					}
				}
				this.m_UpdateList.Clear();
				this.m_ParentList.Clear();
			}
		}

		private void ValidatePersistentDataOnSubTree(VisualElement ve, bool enablePersistence)
		{
			if (!ve.IsPersitenceSupportedOnChildren())
			{
				enablePersistence = false;
			}
			if (this.m_UpdateList.Contains(ve))
			{
				this.m_UpdateList.Remove(ve);
				ve.OnPersistentDataReady(enablePersistence);
			}
			if (this.m_ParentList.Contains(ve))
			{
				this.m_ParentList.Remove(ve);
				for (int i = 0; i < ve.shadow.childCount; i++)
				{
					this.ValidatePersistentDataOnSubTree(ve.shadow[i], enablePersistence);
				}
			}
		}

		private void PropagateToParents(VisualElement ve)
		{
			for (VisualElement visualElement = ve.shadow.parent; visualElement != null; visualElement = visualElement.shadow.parent)
			{
				if (!this.m_ParentList.Add(visualElement))
				{
					break;
				}
			}
		}

		private HashSet<VisualElement> m_UpdateList = new HashSet<VisualElement>();

		private HashSet<VisualElement> m_ParentList = new HashSet<VisualElement>();

		private const int kMaxValidatePersistentDataCount = 5;

		private uint m_Version = 0U;

		private uint m_LastVersion = 0U;
	}
}
