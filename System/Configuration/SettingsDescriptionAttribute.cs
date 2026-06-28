using System;

namespace System.Configuration
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class SettingsDescriptionAttribute : Attribute
	{
		public SettingsDescriptionAttribute(string description)
		{
			this.desc = description;
		}

		public string Description
		{
			get
			{
				return this.desc;
			}
		}

		private string desc;
	}
}
