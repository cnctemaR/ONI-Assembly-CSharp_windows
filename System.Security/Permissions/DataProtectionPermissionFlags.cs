using System;

namespace System.Security.Permissions
{
	[Flags]
	[Serializable]
	public enum DataProtectionPermissionFlags
	{
		NoFlags = 0,
		ProtectData = 1,
		UnprotectData = 2,
		ProtectMemory = 4,
		UnprotectMemory = 8,
		AllFlags = 15
	}
}
