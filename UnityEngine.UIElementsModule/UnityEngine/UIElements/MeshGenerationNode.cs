using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	[NativeContainer]
	public struct MeshGenerationNode
	{
		internal UnsafeMeshGenerationNode unsafeNode
		{
			get
			{
				return this.m_UnsafeNode;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void Create(GCHandle handle, out MeshGenerationNode node)
		{
			node = default(MeshGenerationNode);
			UnsafeMeshGenerationNode.Create(handle, out node.m_UnsafeNode);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DrawMesh(NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, Texture texture = null)
		{
			this.m_UnsafeNode.DrawMesh(vertices, indices, texture);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Entry GetParentEntry()
		{
			return this.m_UnsafeNode.GetParentEntry();
		}

		private UnsafeMeshGenerationNode m_UnsafeNode;
	}
}
