using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR.Tango
{
	[NativeHeader("ARScriptingClasses.h")]
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	internal struct CoordinateFramePair
	{
		[FieldOffset(0)]
		public CoordinateFrame baseFrame;

		[FieldOffset(4)]
		public CoordinateFrame targetFrame;
	}
}
