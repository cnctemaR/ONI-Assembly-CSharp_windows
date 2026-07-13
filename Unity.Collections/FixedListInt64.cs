using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Obsolete("FixedListInt64 is deprecated, please use FixedList64Bytes<int> instead. (UnityUpgradable) -> FixedList64Bytes<int>", true)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 64)]
	public struct FixedListInt64
	{
	}
}
