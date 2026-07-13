using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Obsolete("FixedListFloat512 is deprecated, please use FixedList512Bytes<float> instead. (UnityUpgradable) -> FixedList512Bytes<float>", true)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 512)]
	public struct FixedListFloat512
	{
	}
}
