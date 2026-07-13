using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.U2D
{
	[NativeType(CodegenOptions.Custom, "ScriptingSpriteBone")]
	[RequiredByNativeCode]
	[MovedFrom("UnityEngine.Experimental.U2D")]
	[NativeHeader("Runtime/2D/Common/SpriteDataAccess.h")]
	[NativeHeader("Runtime/2D/Common/SpriteDataMarshalling.h")]
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

		public string guid
		{
			get
			{
				return this.m_Guid;
			}
			set
			{
				this.m_Guid = value;
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

		public Color32 color
		{
			get
			{
				return this.m_Color;
			}
			set
			{
				this.m_Color = value;
			}
		}

		[SerializeField]
		[NativeName("name")]
		private string m_Name;

		[NativeName("guid")]
		[SerializeField]
		private string m_Guid;

		[SerializeField]
		[NativeName("position")]
		private Vector3 m_Position;

		[SerializeField]
		[NativeName("rotation")]
		private Quaternion m_Rotation;

		[NativeName("length")]
		[SerializeField]
		private float m_Length;

		[NativeName("parentId")]
		[SerializeField]
		private int m_ParentId;

		[NativeName("color")]
		[SerializeField]
		private Color32 m_Color;
	}
}
