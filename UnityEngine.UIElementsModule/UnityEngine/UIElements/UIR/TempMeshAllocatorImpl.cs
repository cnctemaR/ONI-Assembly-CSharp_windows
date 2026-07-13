using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;

namespace UnityEngine.UIElements.UIR
{
	internal class TempMeshAllocatorImpl : IDisposable
	{
		public TempMeshAllocatorImpl()
		{
			this.m_GCHandle = GCHandle.Alloc(this);
			this.m_ThreadData = new TempMeshAllocatorImpl.ThreadData[JobsUtility.ThreadIndexCount];
			for (int i = 0; i < JobsUtility.ThreadIndexCount; i++)
			{
				this.m_ThreadData[i].allocations = new List<IntPtr>();
			}
		}

		public void CreateNativeHandle(out TempMeshAllocator allocator)
		{
			TempMeshAllocator.Create(this.m_GCHandle, out allocator);
		}

		private unsafe NativeSlice<T> Allocate<T>(int count, int alignment) where T : struct
		{
			ref TempMeshAllocatorImpl.ThreadData ptr = ref this.m_ThreadData[UIRUtility.GetThreadIndex()];
			Debug.Assert(count > 0);
			long num = (long)(UnsafeUtility.SizeOf<T>() * count);
			void* ptr2 = UnsafeUtility.Malloc(num, UnsafeUtility.AlignOf<T>(), Allocator.TempJob);
			ptr.allocations.Add((IntPtr)ptr2);
			NativeArray<T> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(ptr2, count, Allocator.Invalid);
			return nativeArray;
		}

		public void AllocateTempMesh(int vertexCount, int indexCount, out NativeSlice<Vertex> vertices, out NativeSlice<ushort> indices)
		{
			bool flag = (long)vertexCount > (long)((ulong)UIRenderDevice.maxVerticesPerPage);
			if (flag)
			{
				throw new ArgumentOutOfRangeException("vertexCount", string.Format("Attempting to allocate {0} vertices which exceeds the limit of {1}.", vertexCount, UIRenderDevice.maxVerticesPerPage));
			}
			bool flag2 = !JobsUtility.IsExecutingJob;
			if (flag2)
			{
				bool disposed = this.disposed;
				if (disposed)
				{
					DisposeHelper.NotifyDisposedUsed(this);
					vertices = default(NativeSlice<Vertex>);
					indices = default(NativeSlice<ushort>);
				}
				else
				{
					vertices = ((vertexCount > 0) ? this.m_VertexPool.Alloc(vertexCount) : default(NativeSlice<Vertex>));
					indices = ((indexCount > 0) ? this.m_IndexPool.Alloc(indexCount) : default(NativeSlice<ushort>));
				}
			}
			else
			{
				vertices = ((vertexCount > 0) ? this.Allocate<Vertex>(vertexCount, 4) : default(NativeSlice<Vertex>));
				indices = ((indexCount > 0) ? this.Allocate<ushort>(indexCount, 2) : default(NativeSlice<ushort>));
			}
		}

		public void Clear()
		{
			for (int i = 0; i < this.m_ThreadData.Length; i++)
			{
				foreach (IntPtr intPtr in this.m_ThreadData[i].allocations)
				{
					UnsafeUtility.Free(intPtr.ToPointer(), Allocator.TempJob);
				}
				this.m_ThreadData[i].allocations.Clear();
			}
			this.m_VertexPool.Reset();
			this.m_IndexPool.Reset();
		}

		private protected bool disposed { protected get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					this.Clear();
					this.m_GCHandle.Free();
					this.m_VertexPool.Dispose();
					this.m_IndexPool.Dispose();
				}
				this.disposed = true;
			}
		}

		private GCHandle m_GCHandle;

		private TempMeshAllocatorImpl.ThreadData[] m_ThreadData;

		private TempAllocator<Vertex> m_VertexPool = new TempAllocator<Vertex>(8192, 2048, 65536);

		private TempAllocator<ushort> m_IndexPool = new TempAllocator<ushort>(16384, 4096, 131072);

		private struct ThreadData
		{
			public List<IntPtr> allocations;
		}
	}
}
