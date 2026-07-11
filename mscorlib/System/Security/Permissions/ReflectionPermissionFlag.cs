using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[Flags]
	[Serializable]
	public enum ReflectionPermissionFlag
	{
		NoFlags = 0,
		[Obsolete("not used anymore")]
		TypeInformation = 1,
		MemberAccess = 2,
		ReflectionEmit = 4,
		AllFlags = 7,
		[ComVisible(false)]
		RestrictedMemberAccess = 8
	}
}
