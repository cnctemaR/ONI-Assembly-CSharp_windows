using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[Serializable]
	internal struct BlendShape
	{
		public uint firstVertex
		{
			get
			{
				return this.m_FirstVertex;
			}
			set
			{
				this.m_FirstVertex = value;
			}
		}

		public uint vertexCount
		{
			get
			{
				return this.m_VertexCount;
			}
			set
			{
				this.m_VertexCount = value;
			}
		}

		public bool hasNormals
		{
			get
			{
				return this.m_HasNormals;
			}
			set
			{
				this.m_HasNormals = value;
			}
		}

		public bool hasTangents
		{
			get
			{
				return this.m_HasTangents;
			}
			set
			{
				this.m_HasTangents = value;
			}
		}

		[SerializeField]
		private uint m_FirstVertex;

		[SerializeField]
		private uint m_VertexCount;

		[SerializeField]
		private bool m_HasNormals;

		[SerializeField]
		private bool m_HasTangents;
	}
}
