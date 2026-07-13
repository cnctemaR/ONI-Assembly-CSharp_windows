using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Obsolete("FixedListByte32 is deprecated, please use FixedList32Bytes<byte> instead. (UnityUpgradable) -> FixedList32Bytes<byte>", true)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	public struct FixedListByte32
	{
	}
}
