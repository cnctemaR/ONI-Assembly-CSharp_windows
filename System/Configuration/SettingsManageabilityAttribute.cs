using System;

namespace System.Configuration
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
	public sealed class SettingsManageabilityAttribute : Attribute
	{
		public SettingsManageabilityAttribute(SettingsManageability manageability)
		{
			this.manageability = manageability;
		}

		public SettingsManageability Manageability
		{
			get
			{
				return this.manageability;
			}
		}

		private SettingsManageability manageability;
	}
}
