using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.U2D
{
	[RequiredByNativeCode]
	[NativeType(CodegenOptions.Custom, "ScriptingSpriteBone")]
	[NativeHeader("Runtime/2D/Common/SpriteDataAccess.h")]
	[NativeHeader("Runtime/2D/Common/SpriteDataMarshalling.h")]
	[MovedFrom("UnityEngine.Experimental.U2D")]
	[Serializable]
	public struct SpriteBone
	{
		public string name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				this.m_Name = value;
			}
		}

		public Vector3 position
		{
			get
			{
				return this.m_Position;
			}
			set
			{
				this.m_Position = value;
			}
		}

		public Quaternion rotation
		{
			get
			{
				return this.m_Rotation;
			}
			set
			{
				this.m_Rotation = value;
			}
		}

		public float length
		{
			get
			{
				return this.m_Length;
			}
			set
			{
				this.m_Length = value;
			}
		}

		public int parentId
		{
			get
			{
				return this.m_ParentId;
			}
			set
			{
				this.m_ParentId = value;
			}
		}

		[NativeName("name")]
		[SerializeField]
		private string m_Name;

		[SerializeField]
		[NativeName("position")]
		private Vector3 m_Position;

		[NativeName("rotation")]
		[SerializeField]
		private Quaternion m_Rotation;

		[NativeName("length")]
		[SerializeField]
		private float m_Length;

		[NativeName("parentId")]
		[SerializeField]
		private int m_ParentId;
	}
}
