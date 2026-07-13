using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Obsolete("FixedListInt128 is deprecated, please use FixedList128Bytes<int> instead. (UnityUpgradable) -> FixedList128Bytes<int>", true)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 128)]
	public struct FixedListInt128
	{
	}
}
