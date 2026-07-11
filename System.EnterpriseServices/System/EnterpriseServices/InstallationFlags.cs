using System;

namespace System.EnterpriseServices
{
	[Flags]
	[Serializable]
	public enum InstallationFlags
	{
		Configure = 1024,
		ConfigureComponentsOnly = 16,
		CreateTargetApplication = 2,
		Default = 0,
		ExpectExistingTypeLib = 1,
		FindOrCreateTargetApplication = 4,
		Install = 512,
		ReconfigureExistingApplication = 8,
		Register = 256,
		ReportWarningsToConsole = 32
	}
}
