using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine.UIElements.UIR
{
	internal class OpacityIdAccelerator : IDisposable
	{
		public void CreateJob(NativeSlice<Vertex> oldVerts, NativeSlice<Vertex> newVerts, Color32 opacityData, int vertexCount)
		{
			JobHandle jobHandle = new OpacityIdAccelerator.OpacityIdUpdateJob
			{
				oldVerts = oldVerts,
				newVerts = newVerts,
				opacityData = opacityData
			}.Schedule(vertexCount, 128, default(JobHandle));
			bool flag = this.m_NextJobIndex == this.m_Jobs.Length;
			if (flag)
			{
				this.m_Jobs[0] = JobHandle.CombineDependencies(this.m_Jobs);
				this.m_NextJobIndex = 1;
				JobHandle.ScheduleBatchedJobs();
			}
			int nextJobIndex = this.m_NextJobIndex;
			this.m_NextJobIndex = nextJobIndex + 1;
			this.m_Jobs[nextJobIndex] = jobHandle;
		}

		public void CompleteJobs()
		{
			bool flag = this.m_NextJobIndex > 0;
			if (flag)
			{
				bool flag2 = this.m_NextJobIndex > 1;
				if (flag2)
				{
					JobHandle.CombineDependencies(this.m_Jobs.Slice<JobHandle>(0, this.m_NextJobIndex)).Complete();
				}
				else
				{
					this.m_Jobs[0].Complete();
				}
			}
			this.m_NextJobIndex = 0;
		}

		private protected bool disposed { protected get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					this.m_Jobs.Dispose();
				}
				this.disposed = true;
			}
		}

		private const int k_VerticesPerBatch = 128;

		private const int k_JobLimit = 256;

		private static readonly MemoryLabel k_MemoryLabel = new MemoryLabel("UIElements", "Renderer.OpacityIdAccelerator", Allocator.Persistent);

		private NativeArray<JobHandle> m_Jobs = new NativeArray<JobHandle>(256, OpacityIdAccelerator.k_MemoryLabel, NativeArrayOptions.UninitializedMemory);

		private int m_NextJobIndex;

		private struct OpacityIdUpdateJob : IJobParallelFor
		{
			public void Execute(int i)
			{
				Vertex vertex = this.oldVerts[i];
				vertex.opacityColorPages.r = this.opacityData.r;
				vertex.opacityColorPages.g = this.opacityData.g;
				vertex.ids.b = this.opacityData.b;
				this.newVerts[i] = vertex;
			}

			[NativeDisableContainerSafetyRestriction]
			public NativeSlice<Vertex> oldVerts;

			[NativeDisableContainerSafetyRestriction]
			public NativeSlice<Vertex> newVerts;

			public Color32 opacityData;
		}
	}
}
