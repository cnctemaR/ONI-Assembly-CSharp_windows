using System;

namespace System.Configuration
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class DefaultSettingValueAttribute : Attribute
	{
		public DefaultSettingValueAttribute(string value)
		{
			this.value = value;
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		private string value;
	}
}
