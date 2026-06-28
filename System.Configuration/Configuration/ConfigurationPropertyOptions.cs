using System;

namespace System.Configuration
{
	[Flags]
	public enum ConfigurationPropertyOptions
	{
		None = 0,
		IsDefaultCollection = 1,
		IsRequired = 2,
		IsKey = 4
	}
}
