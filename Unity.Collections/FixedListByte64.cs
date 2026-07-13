using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Obsolete("FixedListByte64 is deprecated, please use FixedList64Bytes<byte> instead. (UnityUpgradable) -> FixedList64Bytes<byte>", true)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 64)]
	public struct FixedListByte64
	{
	}
}
