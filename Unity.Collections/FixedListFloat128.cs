using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Obsolete("FixedListFloat128 is deprecated, please use FixedList128Bytes<float> instead. (UnityUpgradable) -> FixedList128Bytes<float>", true)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 128)]
	public struct FixedListFloat128
	{
	}
}
