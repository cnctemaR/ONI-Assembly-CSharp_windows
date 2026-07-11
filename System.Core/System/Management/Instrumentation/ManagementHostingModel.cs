using System;

namespace System.Management.Instrumentation
{
	public enum ManagementHostingModel
	{
		Decoupled,
		LocalService = 2,
		LocalSystem,
		NetworkService = 1
	}
}
