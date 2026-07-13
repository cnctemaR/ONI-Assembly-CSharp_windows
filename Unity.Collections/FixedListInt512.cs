using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Obsolete("FixedListInt512 is deprecated, please use FixedList512Bytes<int> instead. (UnityUpgradable) -> FixedList512Bytes<int>", true)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 512)]
	public struct FixedListInt512
	{
	}
}
