using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 16)]
	internal struct FixedBytes16Align8
	{
		[SerializeField]
		[FieldOffset(0)]
		public ulong byte0000;

		[SerializeField]
		[FieldOffset(8)]
		public ulong byte0008;
	}
}
