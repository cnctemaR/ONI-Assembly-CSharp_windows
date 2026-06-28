using System;

namespace System.EnterpriseServices
{
	[Serializable]
	public enum TransactionOption
	{
		Disabled,
		NotSupported,
		Supported,
		Required,
		RequiresNew
	}
}
