using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeAsStruct]
	[NativeHeader("TerrainScriptingClasses.h")]
	[UsedByNativeCode]
	[NativeHeader("Modules/Terrain/Public/TerrainDataScriptingInterface.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DetailPrototype
	{
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

		public int noiseSeed
		{
			get
			{
				return this.m_NoiseSeed;
			}
			set
			{
				this.m_NoiseSeed = value;
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

		public float density
		{
			get
			{
				return this.m_Density;
			}
			set
			{
				this.m_Density = value;
			}
		}

		[Obsolete("bendFactor has no effect and is deprecated.", false)]
		public float bendFactor
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		public float holeEdgePadding
		{
			get
			{
				return this.m_HoleEdgePadding;
			}
			set
			{
				this.m_HoleEdgePadding = value;
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
				this.m_UsePrototypeMesh = (value ? 1 : 0);
			}
		}

		public bool useInstancing
		{
			get
			{
				return this.m_UseInstancing != 0;
			}
			set
			{
				this.m_UseInstancing = (value ? 1 : 0);
			}
		}

		public float targetCoverage
		{
			get
			{
				return this.m_TargetCoverage;
			}
			set
			{
				this.m_TargetCoverage = value;
			}
		}

		public bool useDensityScaling
		{
			get
			{
				return this.m_UseDensityScaling != 0;
			}
			set
			{
				this.m_UseDensityScaling = (value ? 1 : 0);
			}
		}

		public float alignToGround
		{
			get
			{
				return this.m_AlignToGround;
			}
			set
			{
				this.m_AlignToGround = value;
			}
		}

		public float positionJitter
		{
			get
			{
				return this.m_PositionJitter;
			}
			set
			{
				this.m_PositionJitter = value;
			}
		}

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
			this.m_NoiseSeed = other.m_NoiseSeed;
			this.m_NoiseSpread = other.m_NoiseSpread;
			this.m_Density = other.m_Density;
			this.m_HoleEdgePadding = other.m_HoleEdgePadding;
			this.m_RenderMode = other.m_RenderMode;
			this.m_UsePrototypeMesh = other.m_UsePrototypeMesh;
			this.m_UseInstancing = other.m_UseInstancing;
			this.m_UseDensityScaling = other.m_UseDensityScaling;
			this.m_AlignToGround = other.m_AlignToGround;
			this.m_PositionJitter = other.m_PositionJitter;
			this.m_TargetCoverage = other.m_TargetCoverage;
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
			bool flag = other == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = other == this;
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					bool flag4 = base.GetType() != other.GetType();
					flag2 = !flag4 && (this.m_Prototype == other.m_Prototype && this.m_PrototypeTexture == other.m_PrototypeTexture && this.m_HealthyColor == other.m_HealthyColor && this.m_DryColor == other.m_DryColor && this.m_MinWidth == other.m_MinWidth && this.m_MaxWidth == other.m_MaxWidth && this.m_MinHeight == other.m_MinHeight && this.m_MaxHeight == other.m_MaxHeight && this.m_NoiseSeed == other.m_NoiseSeed && this.m_NoiseSpread == other.m_NoiseSpread && this.m_Density == other.m_Density && this.m_HoleEdgePadding == other.m_HoleEdgePadding && this.m_RenderMode == other.m_RenderMode && this.m_UsePrototypeMesh == other.m_UsePrototypeMesh && this.m_UseInstancing == other.m_UseInstancing && this.m_TargetCoverage == other.m_TargetCoverage) && this.m_UseDensityScaling == other.m_UseDensityScaling;
				}
			}
			return flag2;
		}

		public bool Validate()
		{
			string text;
			return DetailPrototype.ValidateDetailPrototype(this, out text);
		}

		public bool Validate(out string errorMessage)
		{
			return DetailPrototype.ValidateDetailPrototype(this, out errorMessage);
		}

		[FreeFunction("TerrainDataScriptingInterface::ValidateDetailPrototype")]
		internal static bool ValidateDetailPrototype([NotNull] DetailPrototype prototype, out string errorMessage)
		{
			if (prototype == null)
			{
				ThrowHelper.ThrowArgumentNullException(prototype, "prototype");
			}
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				flag = DetailPrototype.ValidateDetailPrototype_Injected(prototype, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				errorMessage = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return flag;
		}

		internal static bool IsModeSupportedByRenderPipeline(DetailRenderMode renderMode, bool useInstancing, out string errorMessage)
		{
			bool flag = GraphicsSettings.currentRenderPipeline != null;
			if (flag)
			{
				bool flag2 = renderMode == DetailRenderMode.GrassBillboard && GraphicsSettings.GetDefaultShader(DefaultShaderType.TerrainDetailGrassBillboard) == null;
				if (flag2)
				{
					errorMessage = "The current render pipeline does not support Billboard details. Details will not be rendered.";
					return false;
				}
				bool flag3 = renderMode == DetailRenderMode.VertexLit && !useInstancing && GraphicsSettings.GetDefaultShader(DefaultShaderType.TerrainDetailLit) == null;
				if (flag3)
				{
					errorMessage = "The current render pipeline does not support VertexLit details. Details will be rendered using the default shader.";
					return false;
				}
				bool flag4 = renderMode == DetailRenderMode.Grass && GraphicsSettings.GetDefaultShader(DefaultShaderType.TerrainDetailGrass) == null;
				if (flag4)
				{
					errorMessage = "The current render pipeline does not support Grass details. Details will be rendered using the default shader without alpha test and animation.";
					return false;
				}
			}
			errorMessage = string.Empty;
			return true;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ValidateDetailPrototype_Injected(DetailPrototype prototype, out ManagedSpanWrapper errorMessage);

		internal static readonly Color DefaultHealthColor = new Color(0.2627451f, 0.9764706f, 0.16470589f, 1f);

		internal static readonly Color DefaultDryColor = new Color(0.8039216f, 0.7372549f, 0.101960786f, 1f);

		[NativeName("prototype")]
		internal GameObject m_Prototype = null;

		[NativeName("prototypeTexture")]
		internal Texture2D m_PrototypeTexture = null;

		[NativeName("healthyColor")]
		internal Color m_HealthyColor = DetailPrototype.DefaultHealthColor;

		[NativeName("dryColor")]
		internal Color m_DryColor = DetailPrototype.DefaultDryColor;

		[NativeName("minWidth")]
		internal float m_MinWidth = 1f;

		[NativeName("maxWidth")]
		internal float m_MaxWidth = 2f;

		[NativeName("minHeight")]
		internal float m_MinHeight = 1f;

		[NativeName("maxHeight")]
		internal float m_MaxHeight = 2f;

		[NativeName("noiseSeed")]
		internal int m_NoiseSeed = 0;

		[NativeName("noiseSpread")]
		internal float m_NoiseSpread = 0.1f;

		[NativeName("density")]
		internal float m_Density = 1f;

		[NativeName("holeTestRadius")]
		internal float m_HoleEdgePadding = 0f;

		[NativeName("renderMode")]
		internal int m_RenderMode = 2;

		[NativeName("usePrototypeMesh")]
		internal int m_UsePrototypeMesh = 0;

		[NativeName("useInstancing")]
		internal int m_UseInstancing = 0;

		[NativeName("useDensityScaling")]
		internal int m_UseDensityScaling = 0;

		[NativeName("alignToGround")]
		internal float m_AlignToGround = 0f;

		[NativeName("positionJitter")]
		internal float m_PositionJitter = 0f;

		[NativeName("targetCoverage")]
		internal float m_TargetCoverage = 1f;
	}
}
