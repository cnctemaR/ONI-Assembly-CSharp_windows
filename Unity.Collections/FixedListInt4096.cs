using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Obsolete("FixedListInt4096 is deprecated, please use FixedList4096Bytes<int> instead. (UnityUpgradable) -> FixedList4096Bytes<int>", true)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 4096)]
	public struct FixedListInt4096
	{
	}
}
