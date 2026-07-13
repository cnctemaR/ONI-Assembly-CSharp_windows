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
			this.m_FillGradientMeshIndices = new List<int>(16);
			this.m_FillGradients = new List<FillGradient>(16);
			this.m_FillTextureMeshIndices = new List<int>(16);
			this.m_FillTextures = new List<Texture>(16);
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

		public void AddGradient(FillGradient gradient)
		{
			bool flag = this.m_FillGradientDataCount >= this.m_FillGradients.Count;
			if (flag)
			{
				this.m_FillGradients.Add(gradient);
				this.m_FillGradientMeshIndices.Add(this.m_MeshWriteDataCount - 1);
			}
			else
			{
				this.m_FillGradients[this.m_FillGradientDataCount] = gradient;
				this.m_FillGradientMeshIndices[this.m_FillGradientDataCount] = this.m_MeshWriteDataCount - 1;
			}
			this.m_FillGradientDataCount++;
		}

		public FillGradient GetGradientFromMeshIndex(int index)
		{
			for (int i = 0; i < this.m_FillGradientDataCount; i++)
			{
				bool flag = this.m_FillGradientMeshIndices[i] == index;
				if (flag)
				{
					return this.m_FillGradients[i];
				}
			}
			throw new ArgumentOutOfRangeException("index", "No gradient found for the specified index.");
		}

		public FillGradient GetGradientAtIndex(int index)
		{
			return this.m_FillGradients[index];
		}

		public bool HasGradientsOrTextures()
		{
			return this.m_FillGradientDataCount > 0 || this.m_FillTextureDataCount > 0;
		}

		public bool HasGradientAtMeshIndex(int index)
		{
			for (int i = 0; i < this.m_FillGradientDataCount; i++)
			{
				bool flag = this.m_FillGradientMeshIndices[i] == index;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		public void AddTexture(Texture fillTexture)
		{
			bool flag = this.m_FillTextureDataCount >= this.m_FillTextures.Count;
			if (flag)
			{
				this.m_FillTextures.Add(fillTexture);
				this.m_FillTextureMeshIndices.Add(this.m_MeshWriteDataCount - 1);
			}
			else
			{
				this.m_FillTextures[this.m_FillTextureDataCount] = fillTexture;
				this.m_FillTextureMeshIndices[this.m_FillTextureDataCount] = this.m_MeshWriteDataCount - 1;
			}
			this.m_FillTextureDataCount++;
		}

		public Texture GetTextureFromMeshIndex(int index)
		{
			for (int i = 0; i < this.m_FillTextureDataCount; i++)
			{
				bool flag = this.m_FillTextureMeshIndices[i] == index;
				if (flag)
				{
					return this.m_FillTextures[i];
				}
			}
			throw new ArgumentOutOfRangeException("index", "No texture found for the specified index.");
		}

		public bool HasTextureAtMeshIndex(int index)
		{
			for (int i = 0; i < this.m_FillTextureDataCount; i++)
			{
				bool flag = this.m_FillTextureMeshIndices[i] == index;
				if (flag)
				{
					return true;
				}
			}
			return false;
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
			this.m_FillGradientDataCount = 0;
			this.m_FillTextureDataCount = 0;
		}

		private TempAllocator<Vertex> m_VertsPool;

		private TempAllocator<ushort> m_IndexPool;

		private List<MeshWriteData> m_MeshWriteDataPool;

		private List<int> m_FillGradientMeshIndices;

		private List<FillGradient> m_FillGradients;

		private int m_FillGradientDataCount;

		private List<int> m_FillTextureMeshIndices;

		private List<Texture> m_FillTextures;

		private int m_FillTextureDataCount;

		private int m_MeshWriteDataCount;

		private bool m_Disposed;
	}
}
