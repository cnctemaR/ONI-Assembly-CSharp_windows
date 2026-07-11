using System;

namespace System.Configuration
{
	public enum ConfigurationAllowExeDefinition
	{
		MachineOnly,
		MachineToApplication = 100,
		MachineToLocalUser = 300,
		MachineToRoamingUser = 200
	}
}
