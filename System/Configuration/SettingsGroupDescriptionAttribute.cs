using System;

namespace System.Configuration
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class SettingsGroupDescriptionAttribute : Attribute
	{
		public SettingsGroupDescriptionAttribute(string description)
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
