using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	public class MeshWriteData
	{
		internal MeshWriteData()
		{
		}

		public int vertexCount
		{
			get
			{
				return this.m_Vertices.Length;
			}
		}

		public int indexCount
		{
			get
			{
				return this.m_Indices.Length;
			}
		}

		[Obsolete("Texture coordinates are now automatically remapped by the renderer. You are no longer required to remap the UV coordinates in the provided rectangle.")]
		public Rect uvRegion
		{
			get
			{
				return new Rect(0f, 0f, 1f, 1f);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetNextVertex(Vertex vertex)
		{
			int num = this.currentVertex;
			this.currentVertex = num + 1;
			this.m_Vertices[num] = vertex;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetNextIndex(ushort index)
		{
			int num = this.currentIndex;
			this.currentIndex = num + 1;
			this.m_Indices[num] = index;
		}

		public void SetAllVertices(Vertex[] vertices)
		{
			bool flag = this.currentVertex == 0;
			if (flag)
			{
				this.m_Vertices.CopyFrom(vertices);
				this.currentVertex = this.m_Vertices.Length;
				return;
			}
			throw new InvalidOperationException("SetAllVertices may not be called after using SetNextVertex");
		}

		public void SetAllVertices(NativeSlice<Vertex> vertices)
		{
			bool flag = this.currentVertex == 0;
			if (flag)
			{
				this.m_Vertices.CopyFrom(vertices);
				this.currentVertex = this.m_Vertices.Length;
				return;
			}
			throw new InvalidOperationException("SetAllVertices may not be called after using SetNextVertex");
		}

		public void SetAllIndices(ushort[] indices)
		{
			bool flag = this.currentIndex == 0;
			if (flag)
			{
				this.m_Indices.CopyFrom(indices);
				this.currentIndex = this.m_Indices.Length;
				return;
			}
			throw new InvalidOperationException("SetAllIndices may not be called after using SetNextIndex");
		}

		public void SetAllIndices(NativeSlice<ushort> indices)
		{
			bool flag = this.currentIndex == 0;
			if (flag)
			{
				this.m_Indices.CopyFrom(indices);
				this.currentIndex = this.m_Indices.Length;
				return;
			}
			throw new InvalidOperationException("SetAllIndices may not be called after using SetNextIndex");
		}

		internal void Reset(NativeSlice<Vertex> vertices, NativeSlice<ushort> indices)
		{
			this.m_Vertices = vertices;
			this.m_Indices = indices;
			this.currentIndex = (this.currentVertex = 0);
		}

		internal NativeSlice<Vertex> m_Vertices;

		internal NativeSlice<ushort> m_Indices;

		internal int currentIndex;

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal int currentVertex;
	}
}
