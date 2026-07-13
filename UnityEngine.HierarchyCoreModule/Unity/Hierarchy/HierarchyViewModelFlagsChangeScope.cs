using System;

namespace Unity.Hierarchy
{
	public ref struct HierarchyViewModelFlagsChangeScope
	{
		public HierarchyViewModelFlagsChangeScope(HierarchyViewModel hierarchyViewModel)
		{
			this.m_HierarchyViewModel = hierarchyViewModel;
			this.m_Notify = true;
			this.m_HierarchyViewModel.BeginFlagsChange();
		}

		public HierarchyViewModelFlagsChangeScope(HierarchyViewModel hierarchyViewModel, bool notify)
		{
			this.m_HierarchyViewModel = hierarchyViewModel;
			this.m_Notify = notify;
			this.m_HierarchyViewModel.BeginFlagsChange();
		}

		public void Dispose()
		{
			bool flag = this.m_HierarchyViewModel == null || !this.m_HierarchyViewModel.IsCreated;
			if (!flag)
			{
				bool notify = this.m_Notify;
				if (notify)
				{
					this.m_HierarchyViewModel.EndFlagsChange();
				}
				else
				{
					this.m_HierarchyViewModel.EndFlagsChangeWithoutNotify();
				}
			}
		}

		private readonly HierarchyViewModel m_HierarchyViewModel;

		private readonly bool m_Notify;
	}
}
