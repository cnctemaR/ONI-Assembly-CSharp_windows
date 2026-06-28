using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[Flags]
	[Serializable]
	public enum FileDialogPermissionAccess
	{
		None = 0,
		Open = 1,
		Save = 2,
		OpenSave = 3
	}
}
