using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Obsolete("FixedListByte128 is deprecated, please use FixedList128Bytes<byte> instead. (UnityUpgradable) -> FixedList128Bytes<byte>", true)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 128)]
	public struct FixedListByte128
	{
	}
}
