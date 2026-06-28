using System;

namespace System.Configuration
{
	public enum ConfigurationAllowDefinition
	{
		MachineOnly,
		MachineToWebRoot = 100,
		MachineToApplication = 200,
		Everywhere = 300
	}
}
