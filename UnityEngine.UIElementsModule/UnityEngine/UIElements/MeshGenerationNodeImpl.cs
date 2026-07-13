using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	internal class MeshGenerationNodeImpl : IDisposable
	{
		public MeshGenerationNodeImpl()
		{
			this.m_SelfHandle = GCHandle.Alloc(this);
		}

		public void Init(Entry parentEntry, EntryRecorder entryRecorder, bool safe)
		{
			Debug.Assert(this.m_ParentEntry == null);
			Debug.Assert(parentEntry != null);
			Debug.Assert(entryRecorder != null);
			this.m_ParentEntry = parentEntry;
			this.m_EntryRecorder = entryRecorder;
		}

		public void Reset()
		{
			Debug.Assert(this.m_ParentEntry != null);
			Debug.Assert(this.m_EntryRecorder != null);
			this.m_ParentEntry = null;
			this.m_EntryRecorder = null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GetNode(out MeshGenerationNode node)
		{
			MeshGenerationNode.Create(this.m_SelfHandle, out node);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GetUnsafeNode(out UnsafeMeshGenerationNode node)
		{
			UnsafeMeshGenerationNode.Create(this.m_SelfHandle, out node);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entry GetParentEntry()
		{
			return this.m_ParentEntry;
		}

		public void DrawMesh(NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, Texture texture = null, TextureOptions textureOptions = TextureOptions.None)
		{
			bool flag = vertices.Length == 0 || indices.Length == 0;
			if (!flag)
			{
				this.m_EntryRecorder.DrawMesh(this.m_ParentEntry, vertices, indices, texture, textureOptions);
			}
		}

		public void DrawGradients(NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, VectorImage gradientsOwner)
		{
			bool flag = vertices.Length == 0 || indices.Length == 0 || gradientsOwner == null;
			if (!flag)
			{
				this.m_EntryRecorder.DrawGradients(this.m_ParentEntry, vertices, indices, gradientsOwner);
			}
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
					bool flag = this.m_ParentEntry != null;
					if (flag)
					{
						this.Reset();
					}
					this.m_SelfHandle.Free();
				}
				this.disposed = true;
			}
		}

		private GCHandle m_SelfHandle;

		private Entry m_ParentEntry;

		private EntryRecorder m_EntryRecorder;
	}
}
