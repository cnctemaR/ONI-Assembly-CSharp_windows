using System;
using UnityEngine.Scripting;

namespace UnityEngine.VFX
{
	[RequiredByNativeCode]
	internal struct VFXBatchInfo
	{
		public uint capacity;

		public uint activeInstanceCount;
	}
}
