using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Obsolete("FixedListFloat64 is deprecated, please use FixedList64Bytes<float> instead. (UnityUpgradable) -> FixedList64Bytes<float>", true)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 64)]
	public struct FixedListFloat64
	{
	}
}
