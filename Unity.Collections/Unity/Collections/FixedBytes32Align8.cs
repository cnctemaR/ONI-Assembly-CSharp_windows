using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	internal struct FixedBytes32Align8
	{
		[SerializeField]
		[FieldOffset(0)]
		internal FixedBytes16Align8 offset0000;

		[SerializeField]
		[FieldOffset(16)]
		internal FixedBytes16Align8 offset0016;
	}
}
