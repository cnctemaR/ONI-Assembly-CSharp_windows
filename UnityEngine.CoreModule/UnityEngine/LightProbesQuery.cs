using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine
{
	[NativeHeader("Runtime/Camera/RenderLoops/LightProbeContext.h")]
	[NativeContainer]
	[StaticAccessor("LightProbeContextWrapper", StaticAccessorType.DoubleColon)]
	public struct LightProbesQuery : IDisposable
	{
		public LightProbesQuery(Allocator allocator)
		{
			this.m_LightProbeContextWrapper = LightProbesQuery.Create();
			this.m_AllocatorLabel = allocator;
			UnsafeUtility.LeakRecord(this.m_LightProbeContextWrapper, LeakCategory.LightProbesQuery, 0);
		}

		public void Dispose()
		{
			bool flag = this.m_LightProbeContextWrapper == IntPtr.Zero;
			if (flag)
			{
				throw new ObjectDisposedException("The LightProbesQuery is already disposed.");
			}
			bool flag2 = this.m_AllocatorLabel == Allocator.Invalid;
			if (flag2)
			{
				throw new InvalidOperationException("The LightProbesQuery can not be Disposed because it was not allocated with a valid allocator.");
			}
			bool flag3 = this.m_AllocatorLabel > Allocator.None;
			if (flag3)
			{
				UnsafeUtility.LeakErase(this.m_LightProbeContextWrapper, LeakCategory.LightProbesQuery);
				LightProbesQuery.Destroy(this.m_LightProbeContextWrapper);
				this.m_AllocatorLabel = Allocator.Invalid;
			}
			this.m_LightProbeContextWrapper = IntPtr.Zero;
		}

		public JobHandle Dispose(JobHandle inputDeps)
		{
			bool flag = this.m_AllocatorLabel == Allocator.Invalid;
			if (flag)
			{
				throw new InvalidOperationException("The LightProbesQuery can not be Disposed because it was not allocated with a valid allocator.");
			}
			bool flag2 = this.m_LightProbeContextWrapper == IntPtr.Zero;
			if (flag2)
			{
				throw new InvalidOperationException("The LightProbesQuery is already disposed.");
			}
			bool flag3 = this.m_AllocatorLabel > Allocator.None;
			JobHandle jobHandle2;
			if (flag3)
			{
				JobHandle jobHandle = new LightProbesQuery.LightProbesQueryDisposeJob
				{
					Data = new LightProbesQuery.LightProbesQueryDispose
					{
						m_LightProbeContextWrapper = this.m_LightProbeContextWrapper
					}
				}.Schedule(inputDeps);
				this.m_AllocatorLabel = Allocator.Invalid;
				this.m_LightProbeContextWrapper = IntPtr.Zero;
				jobHandle2 = jobHandle;
			}
			else
			{
				this.m_LightProbeContextWrapper = IntPtr.Zero;
				jobHandle2 = inputDeps;
			}
			return jobHandle2;
		}

		public bool IsCreated
		{
			get
			{
				return this.m_LightProbeContextWrapper != IntPtr.Zero;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create();

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr lightProbeContextWrapper);

		public void CalculateInterpolatedLightAndOcclusionProbe(Vector3 position, ref int tetrahedronIndex, out SphericalHarmonicsL2 lightProbe, out Vector4 occlusionProbe)
		{
			LightProbesQuery.CalculateInterpolatedLightAndOcclusionProbe(this.m_LightProbeContextWrapper, position, ref tetrahedronIndex, out lightProbe, out occlusionProbe);
		}

		public void CalculateInterpolatedLightAndOcclusionProbes(NativeArray<Vector3> positions, NativeArray<int> tetrahedronIndices, NativeArray<SphericalHarmonicsL2> lightProbes, NativeArray<Vector4> occlusionProbes)
		{
			bool flag = tetrahedronIndices.Length < positions.Length;
			if (flag)
			{
				throw new ArgumentException("tetrahedronIndices", "Argument tetrahedronIndices is null or has fewer elements than positions.");
			}
			bool flag2 = lightProbes.Length < positions.Length;
			if (flag2)
			{
				throw new ArgumentException("lightProbes", "Argument lightProbes is null or has fewer elements than positions.");
			}
			bool flag3 = occlusionProbes.Length < positions.Length;
			if (flag3)
			{
				throw new ArgumentException("occlusionProbes", "Argument occlusionProbes is null or has fewer elements than positions.");
			}
			LightProbesQuery.CalculateInterpolatedLightAndOcclusionProbes(this.m_LightProbeContextWrapper, (IntPtr)positions.GetUnsafeReadOnlyPtr<Vector3>(), (IntPtr)tetrahedronIndices.GetUnsafeReadOnlyPtr<int>(), (IntPtr)lightProbes.GetUnsafePtr<SphericalHarmonicsL2>(), (IntPtr)occlusionProbes.GetUnsafePtr<Vector4>(), positions.Length);
		}

		[ThreadSafe]
		private static void CalculateInterpolatedLightAndOcclusionProbe(IntPtr lightProbeContextWrapper, Vector3 position, ref int tetrahedronIndex, out SphericalHarmonicsL2 lightProbe, out Vector4 occlusionProbe)
		{
			LightProbesQuery.CalculateInterpolatedLightAndOcclusionProbe_Injected(lightProbeContextWrapper, ref position, ref tetrahedronIndex, out lightProbe, out occlusionProbe);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CalculateInterpolatedLightAndOcclusionProbes(IntPtr lightProbeContextWrapper, IntPtr positions, IntPtr tetrahedronIndices, IntPtr lightProbes, IntPtr occlusionProbes, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CalculateInterpolatedLightAndOcclusionProbe_Injected(IntPtr lightProbeContextWrapper, ref Vector3 position, ref int tetrahedronIndex, out SphericalHarmonicsL2 lightProbe, out Vector4 occlusionProbe);

		[NativeDisableUnsafePtrRestriction]
		internal IntPtr m_LightProbeContextWrapper;

		internal Allocator m_AllocatorLabel;

		[NativeContainer]
		internal struct LightProbesQueryDispose
		{
			public void Dispose()
			{
				UnsafeUtility.LeakErase(this.m_LightProbeContextWrapper, LeakCategory.LightProbesQuery);
				LightProbesQuery.Destroy(this.m_LightProbeContextWrapper);
			}

			[NativeDisableUnsafePtrRestriction]
			internal IntPtr m_LightProbeContextWrapper;
		}

		internal struct LightProbesQueryDisposeJob : IJob
		{
			public void Execute()
			{
				this.Data.Dispose();
			}

			internal LightProbesQuery.LightProbesQueryDispose Data;
		}
	}
}
