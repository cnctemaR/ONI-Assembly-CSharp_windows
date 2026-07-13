using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Obsolete("FixedListByte512 is deprecated, please use FixedList512Bytes<byte> instead. (UnityUpgradable) -> FixedList512Bytes<byte>", true)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 512)]
	public struct FixedListByte512
	{
	}
}
