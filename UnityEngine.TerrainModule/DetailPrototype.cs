using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DetailPrototype
	{
		public DetailPrototype()
		{
		}

		public DetailPrototype(DetailPrototype other)
		{
			this.m_Prototype = other.m_Prototype;
			this.m_PrototypeTexture = other.m_PrototypeTexture;
			this.m_HealthyColor = other.m_HealthyColor;
			this.m_DryColor = other.m_DryColor;
			this.m_MinWidth = other.m_MinWidth;
			this.m_MaxWidth = other.m_MaxWidth;
			this.m_MinHeight = other.m_MinHeight;
			this.m_MaxHeight = other.m_MaxHeight;
			this.m_NoiseSpread = other.m_NoiseSpread;
			this.m_BendFactor = other.m_BendFactor;
			this.m_RenderMode = other.m_RenderMode;
			this.m_UsePrototypeMesh = other.m_UsePrototypeMesh;
		}

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

		public override bool Equals(object obj)
		{
			return this.Equals(obj as DetailPrototype);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private bool Equals(DetailPrototype other)
		{
			bool flag;
			if (object.ReferenceEquals(other, null))
			{
				flag = false;
			}
			else if (object.ReferenceEquals(other, this))
			{
				flag = true;
			}
			else if (base.GetType() != other.GetType())
			{
				flag = false;
			}
			else
			{
				bool flag2 = this.m_Prototype == other.m_Prototype && this.m_PrototypeTexture == other.m_PrototypeTexture && this.m_HealthyColor == other.m_HealthyColor && this.m_DryColor == other.m_DryColor && this.m_MinWidth == other.m_MinWidth && this.m_MaxWidth == other.m_MaxWidth && this.m_MinHeight == other.m_MinHeight && this.m_MaxHeight == other.m_MaxHeight && this.m_NoiseSpread == other.m_NoiseSpread && this.m_BendFactor == other.m_BendFactor && this.m_RenderMode == other.m_RenderMode && this.m_UsePrototypeMesh == other.m_UsePrototypeMesh;
				flag = flag2;
			}
			return flag;
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
