using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.ParticleSystemJobs
{
	public struct ParticleSystemJobData
	{
		public readonly int count { get; }

		public readonly ParticleSystemNativeArray3 positions { get; }

		public readonly ParticleSystemNativeArray3 velocities { get; }

		public readonly ParticleSystemNativeArray3 axisOfRotations { get; }

		public readonly ParticleSystemNativeArray3 rotations { get; }

		public readonly ParticleSystemNativeArray3 rotationalSpeeds { get; }

		public readonly ParticleSystemNativeArray3 sizes { get; }

		public readonly NativeArray<Color32> startColors { get; }

		public readonly NativeArray<float> aliveTimePercent { get; }

		public readonly NativeArray<float> inverseStartLifetimes { get; }

		public readonly NativeArray<uint> randomSeeds { get; }

		public readonly ParticleSystemNativeArray4 customData1 { get; }

		public readonly ParticleSystemNativeArray4 customData2 { get; }

		public readonly NativeArray<int> meshIndices { get; }

		internal ParticleSystemJobData(ref NativeParticleData nativeData)
		{
			this = default(ParticleSystemJobData);
			this.count = nativeData.count;
			this.positions = this.CreateNativeArray3(ref nativeData.positions, this.count);
			this.velocities = this.CreateNativeArray3(ref nativeData.velocities, this.count);
			this.axisOfRotations = this.CreateNativeArray3(ref nativeData.axisOfRotations, this.count);
			this.rotations = this.CreateNativeArray3(ref nativeData.rotations, this.count);
			this.rotationalSpeeds = this.CreateNativeArray3(ref nativeData.rotationalSpeeds, this.count);
			this.sizes = this.CreateNativeArray3(ref nativeData.sizes, this.count);
			this.startColors = this.CreateNativeArray<Color32>(nativeData.startColors, this.count);
			this.aliveTimePercent = this.CreateNativeArray<float>(nativeData.aliveTimePercent, this.count);
			this.inverseStartLifetimes = this.CreateNativeArray<float>(nativeData.inverseStartLifetimes, this.count);
			this.randomSeeds = this.CreateNativeArray<uint>(nativeData.randomSeeds, this.count);
			this.customData1 = this.CreateNativeArray4(ref nativeData.customData1, this.count);
			this.customData2 = this.CreateNativeArray4(ref nativeData.customData2, this.count);
			this.meshIndices = this.CreateNativeArray<int>(nativeData.meshIndices, this.count);
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
