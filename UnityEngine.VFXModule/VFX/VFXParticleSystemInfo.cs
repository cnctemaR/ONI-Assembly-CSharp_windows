using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.VFX
{
	[NativeHeader("Modules/VFX/Public/Systems/VFXParticleSystem.h")]
	[UsedByNativeCode]
	public struct VFXParticleSystemInfo
	{
		public VFXParticleSystemInfo(uint aliveCount, uint capacity, bool sleeping, Bounds bounds)
		{
			this.aliveCount = aliveCount;
			this.capacity = capacity;
			this.sleeping = sleeping;
			this.bounds = bounds;
		}

		public uint aliveCount;

		public uint capacity;

		public bool sleeping;

		public Bounds bounds;
	}
}
