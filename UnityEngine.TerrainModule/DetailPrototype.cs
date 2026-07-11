using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Detail prototype used by the Terrain GameObject.</para>
	/// </summary>
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DetailPrototype
	{
		/// <summary>
		///   <para>GameObject used by the DetailPrototype.</para>
		/// </summary>
		public GameObject prototype
		{
			get
			{
				return this.m_Prototype;
			}
			set
			{
				this.m_Prototype = value;
			}
		}

		/// <summary>
		///   <para>Texture used by the DetailPrototype.</para>
		/// </summary>
		public Texture2D prototypeTexture
		{
			get
			{
				return this.m_PrototypeTexture;
			}
			set
			{
				this.m_PrototypeTexture = value;
			}
		}

		/// <summary>
		///   <para>Minimum width of the grass billboards (if render mode is GrassBillboard).</para>
		/// </summary>
		public float minWidth
		{
			get
			{
				return this.m_MinWidth;
			}
			set
			{
				this.m_MinWidth = value;
			}
		}

		/// <summary>
		///   <para>Maximum width of the grass billboards (if render mode is GrassBillboard).</para>
		/// </summary>
		public float maxWidth
		{
			get
			{
				return this.m_MaxWidth;
			}
			set
			{
				this.m_MaxWidth = value;
			}
		}

		/// <summary>
		///   <para>Minimum height of the grass billboards (if render mode is GrassBillboard).</para>
		/// </summary>
		public float minHeight
		{
			get
			{
				return this.m_MinHeight;
			}
			set
			{
				this.m_MinHeight = value;
			}
		}

		/// <summary>
		///   <para>Maximum height of the grass billboards (if render mode is GrassBillboard).</para>
		/// </summary>
		public float maxHeight
		{
			get
			{
				return this.m_MaxHeight;
			}
			set
			{
				this.m_MaxHeight = value;
			}
		}

		/// <summary>
		///   <para>How spread out is the noise for the DetailPrototype.</para>
		/// </summary>
		public float noiseSpread
		{
			get
			{
				return this.m_NoiseSpread;
			}
			set
			{
				this.m_NoiseSpread = value;
			}
		}

		/// <summary>
		///   <para>Bend factor of the detailPrototype.</para>
		/// </summary>
		public float bendFactor
		{
			get
			{
				return this.m_BendFactor;
			}
			set
			{
				this.m_BendFactor = value;
			}
		}

		/// <summary>
		///   <para>Color when the DetailPrototypes are "healthy".</para>
		/// </summary>
		public Color healthyColor
		{
			get
			{
				return this.m_HealthyColor;
			}
			set
			{
				this.m_HealthyColor = value;
			}
		}

		/// <summary>
		///   <para>Color when the DetailPrototypes are "dry".</para>
		/// </summary>
		public Color dryColor
		{
			get
			{
				return this.m_DryColor;
			}
			set
			{
				this.m_DryColor = value;
			}
		}

		/// <summary>
		///   <para>Render mode for the DetailPrototype.</para>
		/// </summary>
		public DetailRenderMode renderMode
		{
			get
			{
				return (DetailRenderMode)this.m_RenderMode;
			}
			set
			{
				this.m_RenderMode = (int)value;
			}
		}

		public bool usePrototypeMesh
		{
			get
			{
				return this.m_UsePrototypeMesh != 0;
			}
			set
			{
				this.m_UsePrototypeMesh = ((!value) ? 0 : 1);
			}
		}

		internal GameObject m_Prototype = null;

		internal Texture2D m_PrototypeTexture = null;

		internal Color m_HealthyColor = new Color(0.2627451f, 0.9764706f, 0.16470589f, 1f);

		internal Color m_DryColor = new Color(0.8039216f, 0.7372549f, 0.101960786f, 1f);

		internal float m_MinWidth = 1f;

		internal float m_MaxWidth = 2f;

		internal float m_MinHeight = 1f;

		internal float m_MaxHeight = 2f;

		internal float m_NoiseSpread = 0.1f;

		internal float m_BendFactor = 0.1f;

		internal int m_RenderMode = 2;

		internal int m_UsePrototypeMesh = 0;
	}
}
