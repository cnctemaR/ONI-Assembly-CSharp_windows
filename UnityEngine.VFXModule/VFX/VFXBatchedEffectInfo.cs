using System;
using UnityEngine.Scripting;

namespace UnityEngine.VFX
{
	[RequiredByNativeCode]
	public struct VFXBatchedEffectInfo
	{
		public VisualEffectAsset vfxAsset;

		public uint activeBatchCount;

		public uint inactiveBatchCount;

		public uint activeInstanceCount;

		public uint unbatchedInstanceCount;

		public uint totalInstanceCapacity;

		public uint maxInstancePerBatchCapacity;

		public ulong totalGPUSizeInBytes;

		public ulong totalCPUSizeInBytes;
	}
}
