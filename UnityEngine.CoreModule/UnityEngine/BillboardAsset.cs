using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>BillboardAsset describes how a billboard is rendered.</para>
	/// </summary>
	public sealed class BillboardAsset : Object
	{
		/// <summary>
		///   <para>Constructs a new BillboardAsset.</para>
		/// </summary>
		public BillboardAsset()
		{
			BillboardAsset.Internal_Create(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] BillboardAsset obj);

		/// <summary>
		///   <para>Width of the billboard.</para>
		/// </summary>
		public extern float width
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Height of the billboard.</para>
		/// </summary>
		public extern float height
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Height of the billboard that is below ground.</para>
		/// </summary>
		public extern float bottom
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Number of pre-rendered images that can be switched when the billboard is viewed from different angles.</para>
		/// </summary>
		public extern int imageCount
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Number of vertices in the billboard mesh.</para>
		/// </summary>
		public extern int vertexCount
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Number of indices in the billboard mesh.</para>
		/// </summary>
		public extern int indexCount
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The material used for rendering.</para>
		/// </summary>
		public extern Material material
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public void GetImageTexCoords(List<Vector4> imageTexCoords)
		{
			if (imageTexCoords == null)
			{
				throw new ArgumentNullException("imageTexCoords");
			}
			this.GetImageTexCoordsInternal(imageTexCoords);
		}

		/// <summary>
		///   <para>Get the array of billboard image texture coordinate data.</para>
		/// </summary>
		/// <param name="imageTexCoords">The list that receives the array.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Vector4[] GetImageTexCoords();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void GetImageTexCoordsInternal(object list);

		public void SetImageTexCoords(List<Vector4> imageTexCoords)
		{
			if (imageTexCoords == null)
			{
				throw new ArgumentNullException("imageTexCoords");
			}
			this.SetImageTexCoordsInternalList(imageTexCoords);
		}

		/// <summary>
		///   <para>Set the array of billboard image texture coordinate data.</para>
		/// </summary>
		/// <param name="imageTexCoords">The array of data to set.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetImageTexCoords(Vector4[] imageTexCoords);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetImageTexCoordsInternalList(object list);

		public void GetVertices(List<Vector2> vertices)
		{
			if (vertices == null)
			{
				throw new ArgumentNullException("vertices");
			}
			this.GetVerticesInternal(vertices);
		}

		/// <summary>
		///   <para>Get the vertices of the billboard mesh.</para>
		/// </summary>
		/// <param name="vertices">The list that receives the array.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Vector2[] GetVertices();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void GetVerticesInternal(object list);

		public void SetVertices(List<Vector2> vertices)
		{
			if (vertices == null)
			{
				throw new ArgumentNullException("vertices");
			}
			this.SetVerticesInternalList(vertices);
		}

		/// <summary>
		///   <para>Set the vertices of the billboard mesh.</para>
		/// </summary>
		/// <param name="vertices">The array of data to set.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetVertices(Vector2[] vertices);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetVerticesInternalList(object list);

		public void GetIndices(List<ushort> indices)
		{
			if (indices == null)
			{
				throw new ArgumentNullException("indices");
			}
			this.GetIndicesInternal(indices);
		}

		/// <summary>
		///   <para>Get the indices of the billboard mesh.</para>
		/// </summary>
		/// <param name="indices">The list that receives the array.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern ushort[] GetIndices();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void GetIndicesInternal(object list);

		public void SetIndices(List<ushort> indices)
		{
			if (indices == null)
			{
				throw new ArgumentNullException("indices");
			}
			this.SetIndicesInternalList(indices);
		}

		/// <summary>
		///   <para>Set the indices of the billboard mesh.</para>
		/// </summary>
		/// <param name="indices">The array of data to set.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetIndices(ushort[] indices);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetIndicesInternalList(object list);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void MakeMaterialProperties(MaterialPropertyBlock properties, Camera camera);
	}
}
