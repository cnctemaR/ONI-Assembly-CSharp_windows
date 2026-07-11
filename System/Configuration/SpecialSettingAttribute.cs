using System;

namespace System.Configuration
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
	public sealed class SpecialSettingAttribute : Attribute
	{
		public SpecialSettingAttribute(SpecialSetting setting)
		{
			this.setting = setting;
		}

		public SpecialSetting SpecialSetting
		{
			get
			{
				return this.setting;
			}
		}

		private SpecialSetting setting;
	}
}
