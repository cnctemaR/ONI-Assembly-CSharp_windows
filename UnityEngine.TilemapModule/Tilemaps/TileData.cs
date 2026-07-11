using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Tilemaps
{
	/// <summary>
	///   <para>A Struct for the required data for rendering a Tile.</para>
	/// </summary>
	[RequiredByNativeCode]
	[NativeType(Header = "Modules/Tilemap/TilemapScripting.h")]
	public struct TileData
	{
		/// <summary>
		///   <para>Sprite to be rendered at the Tile.</para>
		/// </summary>
		public Sprite sprite
		{
			get
			{
				return this.m_Sprite;
			}
			set
			{
				this.m_Sprite = value;
			}
		}

		/// <summary>
		///   <para>Color of the Tile.</para>
		/// </summary>
		public Color color
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

		/// <summary>
		///   <para>Matrix4x4|Transform matrix of the Tile.</para>
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
		///   <para>GameObject of the Tile.</para>
		/// </summary>
		public GameObject gameObject
		{
			get
			{
				return this.m_GameObject;
			}
			set
			{
				this.m_GameObject = value;
			}
		}

		/// <summary>
		///   <para>TileFlags of the Tile.</para>
		/// </summary>
		public TileFlags flags
		{
			get
			{
				return this.m_Flags;
			}
			set
			{
				this.m_Flags = value;
			}
		}

		public Tile.ColliderType colliderType
		{
			get
			{
				return this.m_ColliderType;
			}
			set
			{
				this.m_ColliderType = value;
			}
		}

		private Sprite m_Sprite;

		private Color m_Color;

		private Matrix4x4 m_Transform;

		private GameObject m_GameObject;

		private TileFlags m_Flags;

		private Tile.ColliderType m_ColliderType;
	}
}
