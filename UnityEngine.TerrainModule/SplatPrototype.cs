using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>A Splat prototype is just a texture that is used by the TerrainData.</para>
	/// </summary>
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class SplatPrototype
	{
		/// <summary>
		///   <para>Texture of the splat applied to the Terrain.</para>
		/// </summary>
		public Texture2D texture
		{
			get
			{
				return this.m_Texture;
			}
			set
			{
				this.m_Texture = value;
			}
		}

		/// <summary>
		///   <para>Normal map of the splat applied to the Terrain.</para>
		/// </summary>
		public Texture2D normalMap
		{
			get
			{
				return this.m_NormalMap;
			}
			set
			{
				this.m_NormalMap = value;
			}
		}

		/// <summary>
		///   <para>Size of the tile used in the texture of the SplatPrototype.</para>
		/// </summary>
		public Vector2 tileSize
		{
			get
			{
				return this.m_TileSize;
			}
			set
			{
				this.m_TileSize = value;
			}
		}

		/// <summary>
		///   <para>Offset of the tile texture of the SplatPrototype.</para>
		/// </summary>
		public Vector2 tileOffset
		{
			get
			{
				return this.m_TileOffset;
			}
			set
			{
				this.m_TileOffset = value;
			}
		}

		public Color specular
		{
			get
			{
				return new Color(this.m_SpecularMetallic.x, this.m_SpecularMetallic.y, this.m_SpecularMetallic.z);
			}
			set
			{
				this.m_SpecularMetallic.x = value.r;
				this.m_SpecularMetallic.y = value.g;
				this.m_SpecularMetallic.z = value.b;
			}
		}

		/// <summary>
		///   <para>The metallic value of the splat layer.</para>
		/// </summary>
		public float metallic
		{
			get
			{
				return this.m_SpecularMetallic.w;
			}
			set
			{
				this.m_SpecularMetallic.w = value;
			}
		}

		/// <summary>
		///   <para>The smoothness value of the splat layer when the main texture has no alpha channel.</para>
		/// </summary>
		public float smoothness
		{
			get
			{
				return this.m_Smoothness;
			}
			set
			{
				this.m_Smoothness = value;
			}
		}

		internal Texture2D m_Texture;

		internal Texture2D m_NormalMap;

		internal Vector2 m_TileSize = new Vector2(15f, 15f);

		internal Vector2 m_TileOffset = new Vector2(0f, 0f);

		internal Vector4 m_SpecularMetallic = new Vector4(0f, 0f, 0f, 0f);

		internal float m_Smoothness = 0f;
	}
}
