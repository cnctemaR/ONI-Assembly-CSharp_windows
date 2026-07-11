using System;

namespace System.Security.Permissions
{
	[Flags]
	[Serializable]
	public enum TypeDescriptorPermissionFlags
	{
		NoFlags = 0,
		RestrictedRegistrationAccess = 1
	}
}
