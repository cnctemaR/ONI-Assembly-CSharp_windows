using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Obsolete("FixedListInt32 is deprecated, please use FixedList32Bytes<int> instead. (UnityUpgradable) -> FixedList32Bytes<int>", true)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	public struct FixedListInt32
	{
	}
}
