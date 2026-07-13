using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Obsolete("FixedListFloat32 is deprecated, please use FixedList32Bytes<float> instead. (UnityUpgradable) -> FixedList32Bytes<float>", true)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	public struct FixedListFloat32
	{
	}
}
