using System;
using System.Collections.Generic;
using Unity.Collections;

namespace UnityEngine.UIElements.UIR
{
	internal class DetachedAllocator : IDisposable
	{
		public List<MeshWriteData> meshes
		{
			get
			{
				return this.m_MeshWriteDataPool.GetRange(0, this.m_MeshWriteDataCount);
			}
		}

		public DetachedAllocator()
		{
			this.m_MeshWriteDataPool = new List<MeshWriteData>(16);
			this.m_MeshWriteDataCount = 0;
			this.m_VertsPool = new TempAllocator<Vertex>(8192, 2048, 65536);
			this.m_IndexPool = new TempAllocator<ushort>(16384, 4096, 131072);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			bool disposed = this.m_Disposed;
			if (!disposed)
			{
				if (disposing)
				{
					this.m_VertsPool.Dispose();
					this.m_IndexPool.Dispose();
				}
				this.m_Disposed = true;
			}
		}

		public MeshWriteData Alloc(int vertexCount, int indexCount)
		{
			bool flag = this.m_MeshWriteDataCount < this.m_MeshWriteDataPool.Count;
			MeshWriteData meshWriteData;
			if (flag)
			{
				meshWriteData = this.m_MeshWriteDataPool[this.m_MeshWriteDataCount];
			}
			else
			{
				meshWriteData = new MeshWriteData();
				this.m_MeshWriteDataPool.Add(meshWriteData);
			}
			this.m_MeshWriteDataCount++;
			bool flag2 = vertexCount == 0 || indexCount == 0;
			MeshWriteData meshWriteData2;
			if (flag2)
			{
				meshWriteData.Reset(default(NativeSlice<Vertex>), default(NativeSlice<ushort>));
				meshWriteData2 = meshWriteData;
			}
			else
			{
				meshWriteData.Reset(this.m_VertsPool.Alloc(vertexCount), this.m_IndexPool.Alloc(indexCount));
				meshWriteData2 = meshWriteData;
			}
			return meshWriteData2;
		}

		public void Clear()
		{
			this.m_VertsPool.Reset();
			this.m_IndexPool.Reset();
			this.m_MeshWriteDataCount = 0;
		}

		private TempAllocator<Vertex> m_VertsPool;

		private TempAllocator<ushort> m_IndexPool;

		private List<MeshWriteData> m_MeshWriteDataPool;

		private int m_MeshWriteDataCount;

		private bool m_Disposed;
	}
}
