using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Tilemaps
{
	[RequiredByNativeCode]
	[NativeType(Header = "Modules/Tilemap/TilemapScripting.h")]
	public struct TileData
	{
		public Sprite sprite
		{
			get
			{
				return Object.ForceLoadFromInstanceID(this.m_Sprite) as Sprite;
			}
			set
			{
				this.m_Sprite = ((value != null) ? value.GetInstanceID() : 0);
			}
		}

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

		public GameObject gameObject
		{
			get
			{
				return Object.ForceLoadFromInstanceID(this.m_GameObject) as GameObject;
			}
			set
			{
				this.m_GameObject = ((value != null) ? value.GetInstanceID() : 0);
			}
		}

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

		private static TileData CreateDefault()
		{
			return new TileData
			{
				color = Color.white,
				transform = Matrix4x4.identity,
				flags = TileFlags.None,
				colliderType = Tile.ColliderType.None
			};
		}

		private int m_Sprite;

		private Color m_Color;

		private Matrix4x4 m_Transform;

		private int m_GameObject;

		private TileFlags m_Flags;

		private Tile.ColliderType m_ColliderType;

		internal static readonly TileData Default = TileData.CreateDefault();
	}
}
