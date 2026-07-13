using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeType("Runtime/Camera/ProbeSetIndex.h")]
	internal struct ProbeSetIndex
	{
		internal Hash128 m_Hash;

		internal int m_Offset;

		internal int m_Size;
	}
}
