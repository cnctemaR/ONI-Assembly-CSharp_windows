using System;

namespace System.EnterpriseServices
{
	[Serializable]
	public enum SynchronizationOption
	{
		Disabled,
		NotSupported,
		Required = 3,
		RequiresNew,
		Supported = 2
	}
}
