using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.U2D
{
	/// <summary>
	///   <para>A struct that holds a rich set of information that describes the bind pose of this Sprite.</para>
	/// </summary>
	[NativeType(CodegenOptions.Custom, "ScriptingSpriteBone")]
	[NativeHeader("Runtime/2D/Common/SpriteDataAccess.h")]
	[RequiredByNativeCode]
	[Serializable]
	public struct SpriteBone
	{
		/// <summary>
		///   <para>The name of the bone. This is useful when recreating bone hierarchy at editor or runtime. You can also use this as a way of resolving the bone path when a Sprite is bound to a more complex or richer hierarchy.</para>
		/// </summary>
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

		/// <summary>
		///   <para>The position in local space of this bone.</para>
		/// </summary>
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

		/// <summary>
		///   <para>The rotation of this bone in local space.</para>
		/// </summary>
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

		/// <summary>
		///   <para>The length of the bone. This is important for the leaf bones to describe their length without needing another bone as the terminal bone.</para>
		/// </summary>
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

		/// <summary>
		///   <para>The ID of the parent of this bone.</para>
		/// </summary>
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

		[SerializeField]
		[NativeName("name")]
		private string m_Name;

		[SerializeField]
		[NativeName("position")]
		private Vector3 m_Position;

		[SerializeField]
		[NativeName("rotation")]
		private Quaternion m_Rotation;

		[SerializeField]
		[NativeName("length")]
		private float m_Length;

		[SerializeField]
		[NativeName("parentId")]
		private int m_ParentId;
	}
}
