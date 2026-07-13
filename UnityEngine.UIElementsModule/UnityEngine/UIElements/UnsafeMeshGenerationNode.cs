using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	internal struct UnsafeMeshGenerationNode
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private MeshGenerationNodeImpl GetManaged()
		{
			return (MeshGenerationNodeImpl)this.m_Handle.Target;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void Create(GCHandle handle, out UnsafeMeshGenerationNode node)
		{
			node = new UnsafeMeshGenerationNode
			{
				m_Handle = handle
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DrawMesh(NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, Texture texture = null)
		{
			this.GetManaged().DrawMesh(vertices, indices, texture, TextureOptions.None);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void DrawMeshInternal(NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, Texture texture = null, TextureOptions textureOptions = TextureOptions.None)
		{
			this.GetManaged().DrawMesh(vertices, indices, texture, textureOptions);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void DrawGradientsInternal(NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, VectorImage gradientsOwner)
		{
			this.GetManaged().DrawGradients(vertices, indices, gradientsOwner);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Entry GetParentEntry()
		{
			return this.GetManaged().GetParentEntry();
		}

		private GCHandle m_Handle;
	}
}
