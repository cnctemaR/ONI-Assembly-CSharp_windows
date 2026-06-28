using System;
using System.Runtime.InteropServices;

namespace System.ComponentModel.Design
{
	[Flags]
	[ComVisible(true)]
	public enum SelectionTypes
	{
		Auto = 1,
		[Obsolete("This value has been deprecated. Use SelectionTypes.Auto instead.")]
		Normal = 1,
		Replace = 2,
		[Obsolete("This value has been deprecated. It is no longer supported.")]
		MouseDown = 4,
		[Obsolete("This value has been deprecated. It is no longer supported.")]
		MouseUp = 8,
		[Obsolete("This value has been deprecated. Use SelectionTypes.Primary instead.")]
		Click = 16,
		Primary = 16,
		[Obsolete("This value has been deprecated. It is no longer supported.")]
		Valid = 31,
		Toggle = 32,
		Add = 64,
		Remove = 128
	}
}
