using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Tilemaps
{
	[RequiredByNativeCode]
	[NativeType(Header = "Modules/Tilemap/TilemapScripting.h")]
	[Serializable]
	public struct TileChangeData
	{
		public Vector3Int position
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

		public TileBase tile
		{
			get
			{
				return (TileBase)this.m_TileAsset;
			}
			set
			{
				this.m_TileAsset = value;
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

		public TileChangeData(Vector3Int position, TileBase tile, Color color, Matrix4x4 transform)
		{
			this.m_Position = position;
			this.m_TileAsset = tile;
			this.m_Color = color;
			this.m_Transform = transform;
		}

		[SerializeField]
		private Vector3Int m_Position;

		[SerializeField]
		private Object m_TileAsset;

		[SerializeField]
		private Color m_Color;

		[SerializeField]
		private Matrix4x4 m_Transform;
	}
}
