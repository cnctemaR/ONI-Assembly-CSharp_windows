using System;

namespace System.Configuration
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
	public sealed class SpecialSettingAttribute : Attribute
	{
		public SpecialSettingAttribute(SpecialSetting specialSetting)
		{
			this.setting = specialSetting;
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
