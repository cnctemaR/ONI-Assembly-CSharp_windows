using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 128)]
	internal struct FixedBytes128Align8
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

		[SerializeField]
		[FieldOffset(64)]
		internal FixedBytes16Align8 offset0064;

		[SerializeField]
		[FieldOffset(80)]
		internal FixedBytes16Align8 offset0080;

		[SerializeField]
		[FieldOffset(96)]
		internal FixedBytes16Align8 offset0096;

		[SerializeField]
		[FieldOffset(112)]
		internal FixedBytes16Align8 offset0112;
	}
}
