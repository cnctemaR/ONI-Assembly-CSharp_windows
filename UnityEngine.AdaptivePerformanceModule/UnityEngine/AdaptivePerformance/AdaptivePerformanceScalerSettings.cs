using System;

namespace UnityEngine.AdaptivePerformance
{
	[Serializable]
	public class AdaptivePerformanceScalerSettings
	{
		public void ApplySettings(AdaptivePerformanceScalerSettings settings)
		{
			bool flag = settings == null;
			if (!flag)
			{
				this.ApplySettingsBase(this.AdaptiveFramerate, settings.AdaptiveFramerate);
				this.ApplySettingsBase(this.AdaptiveBatching, settings.AdaptiveBatching);
				this.ApplySettingsBase(this.AdaptiveLOD, settings.AdaptiveLOD);
				this.ApplySettingsBase(this.AdaptiveLut, settings.AdaptiveLut);
				this.ApplySettingsBase(this.AdaptiveMSAA, settings.AdaptiveMSAA);
				this.ApplySettingsBase(this.AdaptiveResolution, settings.AdaptiveResolution);
				this.ApplySettingsBase(this.AdaptiveShadowCascade, settings.AdaptiveShadowCascade);
				this.ApplySettingsBase(this.AdaptiveShadowDistance, settings.AdaptiveShadowDistance);
				this.ApplySettingsBase(this.AdaptiveShadowmapResolution, settings.AdaptiveShadowmapResolution);
				this.ApplySettingsBase(this.AdaptiveShadowQuality, settings.AdaptiveShadowQuality);
				this.ApplySettingsBase(this.AdaptiveTransparency, settings.AdaptiveTransparency);
				this.ApplySettingsBase(this.AdaptiveSorting, settings.AdaptiveSorting);
				this.ApplySettingsBase(this.AdaptiveViewDistance, settings.AdaptiveViewDistance);
				this.ApplySettingsBase(this.AdaptivePhysics, settings.AdaptivePhysics);
				this.ApplySettingsBase(this.AdaptiveLayerCulling, settings.AdaptiveLayerCulling);
				this.ApplySettingsBase(this.AdaptiveDecals, settings.AdaptiveDecals);
			}
		}

		private void ApplySettingsBase(AdaptivePerformanceScalerSettingsBase destination, AdaptivePerformanceScalerSettingsBase sources)
		{
			destination.enabled = sources.enabled;
			destination.scale = sources.scale;
			destination.visualImpact = sources.visualImpact;
			destination.target = sources.target;
			destination.minBound = sources.minBound;
			destination.maxBound = sources.maxBound;
			destination.maxLevel = sources.maxLevel;
		}

		public AdaptivePerformanceScalerSettingsBase AdaptiveFramerate
		{
			get
			{
				return this.m_AdaptiveFramerate;
			}
			set
			{
				this.m_AdaptiveFramerate = value;
			}
		}

		public AdaptivePerformanceScalerSettingsBase AdaptiveResolution
		{
			get
			{
				return this.m_AdaptiveResolution;
			}
			set
			{
				this.m_AdaptiveResolution = value;
			}
		}

		public AdaptivePerformanceScalerSettingsBase AdaptiveBatching
		{
			get
			{
				return this.m_AdaptiveBatching;
			}
			set
			{
				this.m_AdaptiveBatching = value;
			}
		}

		public AdaptivePerformanceScalerSettingsBase AdaptiveLOD
		{
			get
			{
				return this.m_AdaptiveLOD;
			}
			set
			{
				this.m_AdaptiveLOD = value;
			}
		}

		public AdaptivePerformanceScalerSettingsBase AdaptiveLut
		{
			get
			{
				return this.m_AdaptiveLut;
			}
			set
			{
				this.m_AdaptiveLut = value;
			}
		}

		public AdaptivePerformanceScalerSettingsBase AdaptiveMSAA
		{
			get
			{
				return this.m_AdaptiveMSAA;
			}
			set
			{
				this.m_AdaptiveMSAA = value;
			}
		}

		[Obsolete("AdaptiveShadowCascades has been renamed. Please use AdaptiveShadowCascade. (UnityUpgradable) -> AdaptiveShadowCascade", false)]
		public AdaptivePerformanceScalerSettingsBase AdaptiveShadowCascades
		{
			get
			{
				return this.AdaptiveShadowCascade;
			}
		}

		public AdaptivePerformanceScalerSettingsBase AdaptiveShadowCascade
		{
			get
			{
				return this.m_AdaptiveShadowCascade;
			}
			set
			{
				this.m_AdaptiveShadowCascade = value;
			}
		}

		public AdaptivePerformanceScalerSettingsBase AdaptiveShadowDistance
		{
			get
			{
				return this.m_AdaptiveShadowDistance;
			}
			set
			{
				this.m_AdaptiveShadowDistance = value;
			}
		}

		public AdaptivePerformanceScalerSettingsBase AdaptiveShadowmapResolution
		{
			get
			{
				return this.m_AdaptiveShadowmapResolution;
			}
			set
			{
				this.m_AdaptiveShadowmapResolution = value;
			}
		}

		public AdaptivePerformanceScalerSettingsBase AdaptiveShadowQuality
		{
			get
			{
				return this.m_AdaptiveShadowQuality;
			}
			set
			{
				this.m_AdaptiveShadowQuality = value;
			}
		}

		public AdaptivePerformanceScalerSettingsBase AdaptiveSorting
		{
			get
			{
				return this.m_AdaptiveSorting;
			}
			set
			{
				this.m_AdaptiveSorting = value;
			}
		}

		public AdaptivePerformanceScalerSettingsBase AdaptiveTransparency
		{
			get
			{
				return this.m_AdaptiveTransparency;
			}
			set
			{
				this.m_AdaptiveTransparency = value;
			}
		}

		public AdaptivePerformanceScalerSettingsBase AdaptiveViewDistance
		{
			get
			{
				return this.m_AdaptiveViewDistance;
			}
			set
			{
				this.m_AdaptiveViewDistance = value;
			}
		}

		public AdaptivePerformanceScalerSettingsBase AdaptivePhysics
		{
			get
			{
				return this.m_AdaptivePhysics;
			}
			set
			{
				this.m_AdaptivePhysics = value;
			}
		}

		public AdaptivePerformanceScalerSettingsBase AdaptiveDecals
		{
			get
			{
				return this.m_AdaptiveDecals;
			}
			set
			{
				this.m_AdaptiveDecals = value;
			}
		}

		public AdaptivePerformanceScalerSettingsBase AdaptiveLayerCulling
		{
			get
			{
				return this.m_AdaptiveLayerCulling;
			}
			set
			{
				this.m_AdaptiveLayerCulling = value;
			}
		}

		[SerializeField]
		[Tooltip("Settings for a scaler used by the Indexer to adjust the application update rate using Application.TargetFramerate")]
		private AdaptivePerformanceScalerSettingsBase m_AdaptiveFramerate = new AdaptivePerformanceScalerSettingsBase
		{
			name = "Adaptive Framerate",
			enabled = false,
			scale = 1f,
			visualImpact = ScalerVisualImpact.High,
			target = (ScalerTarget.CPU | ScalerTarget.GPU | ScalerTarget.FillRate),
			minBound = 15f,
			maxBound = 60f,
			maxLevel = 45
		};

		[SerializeField]
		[Tooltip("Settings for a scaler used by the Indexer to adjust the resolution of all render targets that allow dynamic resolution.")]
		private AdaptivePerformanceScalerSettingsBase m_AdaptiveResolution = new AdaptivePerformanceScalerSettingsBase
		{
			name = "Adaptive Resolution",
			enabled = false,
			scale = 1f,
			visualImpact = ScalerVisualImpact.Low,
			target = (ScalerTarget.GPU | ScalerTarget.FillRate),
			maxLevel = 9,
			minBound = 0.5f,
			maxBound = 1f
		};

		[Tooltip("Settings for a scaler used by the Indexer to control if dynamic batching is enabled.")]
		[SerializeField]
		private AdaptivePerformanceScalerSettingsBase m_AdaptiveBatching = new AdaptivePerformanceScalerSettingsBase
		{
			name = "Adaptive Batching",
			enabled = false,
			scale = 1f,
			visualImpact = ScalerVisualImpact.Medium,
			target = ScalerTarget.CPU,
			maxLevel = 1,
			minBound = 0f,
			maxBound = 1f
		};

		[Tooltip("Settings for a scaler used by the Indexer for adjusting at what distance LODs are switched.")]
		[SerializeField]
		private AdaptivePerformanceScalerSettingsBase m_AdaptiveLOD = new AdaptivePerformanceScalerSettingsBase
		{
			name = "Adaptive LOD",
			enabled = false,
			scale = 1f,
			visualImpact = ScalerVisualImpact.High,
			target = ScalerTarget.GPU,
			maxLevel = 3,
			minBound = 0.4f,
			maxBound = 1f
		};

		[Tooltip("Settings for a scaler used by the Indexer to adjust the size of the palette used for color grading in URP.")]
		[SerializeField]
		private AdaptivePerformanceScalerSettingsBase m_AdaptiveLut = new AdaptivePerformanceScalerSettingsBase
		{
			name = "Adaptive Lut",
			enabled = false,
			scale = 1f,
			visualImpact = ScalerVisualImpact.Medium,
			target = (ScalerTarget.CPU | ScalerTarget.GPU),
			maxLevel = 1,
			minBound = 0f,
			maxBound = 1f
		};

		[SerializeField]
		[Tooltip("Settings for a scaler used by the Indexer to adjust the level of antialiasing.")]
		private AdaptivePerformanceScalerSettingsBase m_AdaptiveMSAA = new AdaptivePerformanceScalerSettingsBase
		{
			name = "Adaptive MSAA",
			enabled = false,
			scale = 1f,
			visualImpact = ScalerVisualImpact.Medium,
			target = (ScalerTarget.GPU | ScalerTarget.FillRate),
			maxLevel = 2,
			minBound = 0f,
			maxBound = 1f
		};

		[SerializeField]
		[Tooltip("Settings for a scaler used by the Indexer to adjust the number of shadow cascades to be used.")]
		private AdaptivePerformanceScalerSettingsBase m_AdaptiveShadowCascade = new AdaptivePerformanceScalerSettingsBase
		{
			name = "Adaptive Shadow Cascade",
			enabled = false,
			scale = 1f,
			visualImpact = ScalerVisualImpact.Medium,
			target = (ScalerTarget.CPU | ScalerTarget.GPU),
			maxLevel = 2,
			minBound = 0f,
			maxBound = 1f
		};

		private const string obsoleteMsg = "AdaptiveShadowCascades has been renamed. Please use AdaptiveShadowCascade. (UnityUpgradable) -> AdaptiveShadowCascade";

		[SerializeField]
		[Tooltip("Settings for a scaler used by the Indexer to change the distance at which shadows are rendered.")]
		private AdaptivePerformanceScalerSettingsBase m_AdaptiveShadowDistance = new AdaptivePerformanceScalerSettingsBase
		{
			name = "Adaptive Shadow Distance",
			enabled = false,
			scale = 1f,
			visualImpact = ScalerVisualImpact.Low,
			target = ScalerTarget.GPU,
			maxLevel = 3,
			minBound = 0.15f,
			maxBound = 1f
		};

		[Tooltip("Settings for a scaler used by the Indexer to adjust the resolution of shadow maps.")]
		[SerializeField]
		private AdaptivePerformanceScalerSettingsBase m_AdaptiveShadowmapResolution = new AdaptivePerformanceScalerSettingsBase
		{
			name = "Adaptive Shadowmap Resolution",
			enabled = false,
			scale = 1f,
			visualImpact = ScalerVisualImpact.Low,
			target = ScalerTarget.GPU,
			maxLevel = 3,
			minBound = 0.15f,
			maxBound = 1f
		};

		[SerializeField]
		[Tooltip("Settings for a scaler used by the Indexer to adjust the quality of shadows.")]
		private AdaptivePerformanceScalerSettingsBase m_AdaptiveShadowQuality = new AdaptivePerformanceScalerSettingsBase
		{
			name = "Adaptive Shadow Quality",
			enabled = false,
			scale = 1f,
			visualImpact = ScalerVisualImpact.High,
			target = (ScalerTarget.CPU | ScalerTarget.GPU),
			maxLevel = 3,
			minBound = 0f,
			maxBound = 1f
		};

		[Tooltip("Settings for a scaler used by the Indexer to change if objects in the scene are sorted by depth before rendering to reduce overdraw.")]
		[SerializeField]
		private AdaptivePerformanceScalerSettingsBase m_AdaptiveSorting = new AdaptivePerformanceScalerSettingsBase
		{
			name = "Adaptive Sorting",
			enabled = false,
			scale = 1f,
			visualImpact = ScalerVisualImpact.Medium,
			target = ScalerTarget.CPU,
			maxLevel = 1,
			minBound = 0f,
			maxBound = 1f
		};

		[SerializeField]
		[Tooltip("Settings for a scaler used by the Indexer to disable transparent objects rendering")]
		private AdaptivePerformanceScalerSettingsBase m_AdaptiveTransparency = new AdaptivePerformanceScalerSettingsBase
		{
			name = "Adaptive Transparency",
			enabled = false,
			scale = 1f,
			visualImpact = ScalerVisualImpact.High,
			target = ScalerTarget.GPU,
			maxLevel = 1,
			minBound = 0f,
			maxBound = 1f
		};

		[SerializeField]
		[Tooltip("Settings for a scaler used by the Indexer to change the view distance")]
		private AdaptivePerformanceScalerSettingsBase m_AdaptiveViewDistance = new AdaptivePerformanceScalerSettingsBase
		{
			name = "Adaptive View Distance",
			enabled = false,
			scale = 1f,
			visualImpact = ScalerVisualImpact.High,
			target = ScalerTarget.GPU,
			maxLevel = 40,
			minBound = 50f,
			maxBound = 1000f
		};

		[SerializeField]
		[Tooltip("Settings for a scaler used by the Indexer to change physics properties")]
		private AdaptivePerformanceScalerSettingsBase m_AdaptivePhysics = new AdaptivePerformanceScalerSettingsBase
		{
			name = "Adaptive Physics",
			enabled = false,
			scale = 1f,
			visualImpact = ScalerVisualImpact.Low,
			target = ScalerTarget.CPU,
			maxLevel = 5,
			minBound = 0.5f,
			maxBound = 1f
		};

		[SerializeField]
		[Tooltip("Settings for a scaler used by the Indexer to change decal properties")]
		private AdaptivePerformanceScalerSettingsBase m_AdaptiveDecals = new AdaptivePerformanceScalerSettingsBase
		{
			name = "Adaptive Decals",
			enabled = false,
			scale = 1f,
			visualImpact = ScalerVisualImpact.Medium,
			target = ScalerTarget.GPU,
			maxLevel = 20,
			minBound = 0.01f,
			maxBound = 1f
		};

		[Tooltip("Settings for a scaler used by the Indexer to change the layer culling distance")]
		[SerializeField]
		private AdaptivePerformanceScalerSettingsBase m_AdaptiveLayerCulling = new AdaptivePerformanceScalerSettingsBase
		{
			name = "Adaptive Layer Culling",
			enabled = false,
			scale = 1f,
			visualImpact = ScalerVisualImpact.Medium,
			target = ScalerTarget.CPU,
			maxLevel = 40,
			minBound = 0.01f,
			maxBound = 1f
		};
	}
}
