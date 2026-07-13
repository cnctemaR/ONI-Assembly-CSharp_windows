using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	[NativeContainerIsReadOnly]
	[NativeContainer]
	public struct TempMeshAllocator
	{
		internal static void Create(GCHandle handle, out TempMeshAllocator allocator)
		{
			allocator = new TempMeshAllocator
			{
				m_Handle = handle
			};
		}

		public void AllocateTempMesh(int vertexCount, int indexCount, out NativeSlice<Vertex> vertices, out NativeSlice<ushort> indices)
		{
			TempMeshAllocatorImpl tempMeshAllocatorImpl = this.m_Handle.Target as TempMeshAllocatorImpl;
			Debug.Assert(tempMeshAllocatorImpl != null);
			tempMeshAllocatorImpl.AllocateTempMesh(vertexCount, indexCount, out vertices, out indices);
		}

		private GCHandle m_Handle;
	}
}
