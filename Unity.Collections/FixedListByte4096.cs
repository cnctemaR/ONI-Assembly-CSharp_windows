using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Obsolete("FixedListByte4096 is deprecated, please use FixedList4096Bytes<byte> instead. (UnityUpgradable) -> FixedList4096Bytes<byte>", true)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 4096)]
	public struct FixedListByte4096
	{
	}
}
