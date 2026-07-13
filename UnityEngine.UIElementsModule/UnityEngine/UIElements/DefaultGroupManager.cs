using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class DefaultGroupManager : IGroupManager
	{
		public void Init(IGroupBox groupBox)
		{
			this.m_GroupBox = groupBox;
		}

		public IGroupBoxOption GetSelectedOption()
		{
			return this.m_SelectedOption;
		}

		public void OnOptionSelectionChanged(IGroupBoxOption selectedOption)
		{
			bool flag = this.m_SelectedOption == selectedOption;
			if (!flag)
			{
				this.m_SelectedOption = selectedOption;
				foreach (IGroupBoxOption groupBoxOption in this.m_GroupOptions)
				{
					groupBoxOption.SetSelected(groupBoxOption == this.m_SelectedOption);
				}
			}
		}

		public void RegisterOption(IGroupBoxOption option)
		{
			bool flag = !this.m_GroupOptions.Contains(option);
			if (flag)
			{
				this.m_GroupOptions.Add(option);
				this.m_GroupBox.OnOptionAdded(option);
			}
		}

		public void UnregisterOption(IGroupBoxOption option)
		{
			this.m_GroupOptions.Remove(option);
			this.m_GroupBox.OnOptionRemoved(option);
		}

		private List<IGroupBoxOption> m_GroupOptions = new List<IGroupBoxOption>();

		private IGroupBoxOption m_SelectedOption;

		private IGroupBox m_GroupBox;
	}
}
