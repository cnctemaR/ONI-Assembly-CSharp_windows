using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Obsolete("FixedListFloat4096 is deprecated, please use FixedList4096Bytes<float> instead. (UnityUpgradable) -> FixedList4096Bytes<float>", true)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 4096)]
	public struct FixedListFloat4096
	{
	}
}
