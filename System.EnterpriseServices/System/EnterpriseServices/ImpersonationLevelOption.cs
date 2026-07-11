using System;

namespace System.EnterpriseServices
{
	[Serializable]
	public enum ImpersonationLevelOption
	{
		Anonymous = 1,
		Default = 0,
		Delegate = 4,
		Identify = 2,
		Impersonate
	}
}
