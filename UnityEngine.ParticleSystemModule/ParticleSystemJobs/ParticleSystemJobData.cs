using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.ParticleSystemJobs
{
	public struct ParticleSystemJobData
	{
		public int count { get; }

		public ParticleSystemNativeArray3 positions { get; }

		public ParticleSystemNativeArray3 velocities { get; }

		public ParticleSystemNativeArray3 rotations { get; }

		public ParticleSystemNativeArray3 rotationalSpeeds { get; }

		public ParticleSystemNativeArray3 sizes { get; }

		public NativeArray<Color32> startColors { get; }

		public NativeArray<float> aliveTimePercent { get; }

		public NativeArray<float> inverseStartLifetimes { get; }

		public NativeArray<uint> randomSeeds { get; }

		public ParticleSystemNativeArray4 customData1 { get; }

		public ParticleSystemNativeArray4 customData2 { get; }

		internal ParticleSystemJobData(ref NativeParticleData nativeData)
		{
			this = default(ParticleSystemJobData);
			this.count = nativeData.count;
			this.positions = this.CreateNativeArray3(ref nativeData.positions, this.count);
			this.velocities = this.CreateNativeArray3(ref nativeData.velocities, this.count);
			this.rotations = this.CreateNativeArray3(ref nativeData.rotations, this.count);
			this.rotationalSpeeds = this.CreateNativeArray3(ref nativeData.rotationalSpeeds, this.count);
			this.sizes = this.CreateNativeArray3(ref nativeData.sizes, this.count);
			this.startColors = this.CreateNativeArray<Color32>(nativeData.startColors, this.count);
			this.aliveTimePercent = this.CreateNativeArray<float>(nativeData.aliveTimePercent, this.count);
			this.inverseStartLifetimes = this.CreateNativeArray<float>(nativeData.inverseStartLifetimes, this.count);
			this.randomSeeds = this.CreateNativeArray<uint>(nativeData.randomSeeds, this.count);
			this.customData1 = this.CreateNativeArray4(ref nativeData.customData1, this.count);
			this.customData2 = this.CreateNativeArray4(ref nativeData.customData2, this.count);
		}

		internal unsafe NativeArray<T> CreateNativeArray<T>(void* src, int count) where T : struct
		{
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(src, count, Allocator.Invalid);
		}

		internal unsafe ParticleSystemNativeArray3 CreateNativeArray3(ref NativeParticleData.Array3 ptrs, int count)
		{
			return new ParticleSystemNativeArray3
			{
				x = this.CreateNativeArray<float>((void*)ptrs.x, count),
				y = this.CreateNativeArray<float>((void*)ptrs.y, count),
				z = this.CreateNativeArray<float>((void*)ptrs.z, count)
			};
		}

		internal unsafe ParticleSystemNativeArray4 CreateNativeArray4(ref NativeParticleData.Array4 ptrs, int count)
		{
			return new ParticleSystemNativeArray4
			{
				x = this.CreateNativeArray<float>((void*)ptrs.x, count),
				y = this.CreateNativeArray<float>((void*)ptrs.y, count),
				z = this.CreateNativeArray<float>((void*)ptrs.z, count),
				w = this.CreateNativeArray<float>((void*)ptrs.w, count)
			};
		}
	}
}
