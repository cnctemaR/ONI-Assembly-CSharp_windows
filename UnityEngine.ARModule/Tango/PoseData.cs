using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR.Tango
{
	[UsedByNativeCode]
	[NativeHeader("ARScriptingClasses.h")]
	[StructLayout(LayoutKind.Explicit, Size = 60)]
	internal struct PoseData
	{
		public Quaternion rotation
		{
			get
			{
				return new Quaternion((float)this.orientation_x, (float)this.orientation_y, (float)this.orientation_z, (float)this.orientation_w);
			}
		}

		public Vector3 position
		{
			get
			{
				return new Vector3((float)this.translation_x, (float)this.translation_y, (float)this.translation_z);
			}
		}

		[FieldOffset(0)]
		public double orientation_x;

		[FieldOffset(8)]
		public double orientation_y;

		[FieldOffset(16)]
		public double orientation_z;

		[FieldOffset(24)]
		public double orientation_w;

		[FieldOffset(32)]
		public double translation_x;

		[FieldOffset(40)]
		public double translation_y;

		[FieldOffset(48)]
		public double translation_z;

		[FieldOffset(56)]
		public PoseStatus statusCode;
	}
}
