using System;

namespace System.Configuration
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class SettingsGroupNameAttribute : Attribute
	{
		public SettingsGroupNameAttribute(string groupName)
		{
			this.group_name = groupName;
		}

		public string GroupName
		{
			get
			{
				return this.group_name;
			}
		}

		private string group_name;
	}
}
