using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Struct used to describe meshes to be combined using Mesh.CombineMeshes.</para>
	/// </summary>
	public struct CombineInstance
	{
		/// <summary>
		///   <para>Mesh to combine.</para>
		/// </summary>
		public Mesh mesh
		{
			get
			{
				return Mesh.FromInstanceID(this.m_MeshInstanceID);
			}
			set
			{
				this.m_MeshInstanceID = ((!(value != null)) ? 0 : value.GetInstanceID());
			}
		}

		/// <summary>
		///   <para>Sub-Mesh index of the Mesh.</para>
		/// </summary>
		public int subMeshIndex
		{
			get
			{
				return this.m_SubMeshIndex;
			}
			set
			{
				this.m_SubMeshIndex = value;
			}
		}

		/// <summary>
		///   <para>Matrix to transform the Mesh with before combining.</para>
		/// </summary>
		public Matrix4x4 transform
		{
			get
			{
				return this.m_Transform;
			}
			set
			{
				this.m_Transform = value;
			}
		}

		/// <summary>
		///   <para>The baked lightmap UV scale and offset applied to the Mesh.</para>
		/// </summary>
		public Vector4 lightmapScaleOffset
		{
			get
			{
				return this.m_LightmapScaleOffset;
			}
			set
			{
				this.m_LightmapScaleOffset = value;
			}
		}

		/// <summary>
		///   <para>The realtime lightmap UV scale and offset applied to the Mesh.</para>
		/// </summary>
		public Vector4 realtimeLightmapScaleOffset
		{
			get
			{
				return this.m_RealtimeLightmapScaleOffset;
			}
			set
			{
				this.m_RealtimeLightmapScaleOffset = value;
			}
		}

		private int m_MeshInstanceID;

		private int m_SubMeshIndex;

		private Matrix4x4 m_Transform;

		private Vector4 m_LightmapScaleOffset;

		private Vector4 m_RealtimeLightmapScaleOffset;
	}
}
