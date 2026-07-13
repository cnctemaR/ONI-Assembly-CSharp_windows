using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 64)]
	internal struct FixedBytes64Align8
	{
		[SerializeField]
		[FieldOffset(0)]
		internal FixedBytes16Align8 offset0000;

		[SerializeField]
		[FieldOffset(16)]
		internal FixedBytes16Align8 offset0016;

		[SerializeField]
		[FieldOffset(32)]
		internal FixedBytes16Align8 offset0032;

		[SerializeField]
		[FieldOffset(48)]
		internal FixedBytes16Align8 offset0048;
	}
}
