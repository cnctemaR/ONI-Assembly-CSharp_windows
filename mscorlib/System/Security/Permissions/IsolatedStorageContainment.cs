using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[Serializable]
	public enum IsolatedStorageContainment
	{
		None,
		DomainIsolationByUser = 16,
		AssemblyIsolationByUser = 32,
		DomainIsolationByRoamingUser = 80,
		AssemblyIsolationByRoamingUser = 96,
		AdministerIsolatedStorageByUser = 112,
		UnrestrictedIsolatedStorage = 240,
		ApplicationIsolationByUser = 21,
		DomainIsolationByMachine = 48,
		AssemblyIsolationByMachine = 64,
		ApplicationIsolationByMachine = 69,
		ApplicationIsolationByRoamingUser = 101
	}
}
