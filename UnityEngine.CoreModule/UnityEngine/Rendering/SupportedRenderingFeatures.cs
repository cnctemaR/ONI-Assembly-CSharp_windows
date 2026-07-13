using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	public class SupportedRenderingFeatures
	{
		public static SupportedRenderingFeatures active
		{
			get
			{
				bool flag = SupportedRenderingFeatures.s_Active == null;
				if (flag)
				{
					SupportedRenderingFeatures.s_Active = new SupportedRenderingFeatures();
				}
				return SupportedRenderingFeatures.s_Active;
			}
			set
			{
				SupportedRenderingFeatures.s_Active = value;
			}
		}

		public SupportedRenderingFeatures.ReflectionProbeModes reflectionProbeModes { get; set; } = SupportedRenderingFeatures.ReflectionProbeModes.None;

		public SupportedRenderingFeatures.LightmapMixedBakeModes defaultMixedLightingModes { get; set; } = SupportedRenderingFeatures.LightmapMixedBakeModes.None;

		public SupportedRenderingFeatures.LightmapMixedBakeModes mixedLightingModes { get; set; } = SupportedRenderingFeatures.LightmapMixedBakeModes.IndirectOnly | SupportedRenderingFeatures.LightmapMixedBakeModes.Subtractive | SupportedRenderingFeatures.LightmapMixedBakeModes.Shadowmask;

		public LightmapBakeType lightmapBakeTypes { get; set; } = LightmapBakeType.Realtime | LightmapBakeType.Baked | LightmapBakeType.Mixed;

		public LightmapsMode lightmapsModes { get; set; } = LightmapsMode.CombinedDirectional;

		[Obsolete("Bake with the Progressive Lightmapper. The backend that uses Enlighten to bake is obsolete.", true)]
		public bool enlightenLightmapper { get; set; } = false;

		public bool enlighten { get; set; } = true;

		public bool skyOcclusion { get; set; } = false;

		public bool lightProbeProxyVolumes { get; set; } = true;

		public bool motionVectors { get; set; } = true;

		public bool receiveShadows { get; set; } = true;

		public bool reflectionProbes { get; set; } = true;

		public bool reflectionProbesBlendDistance { get; set; } = true;

		public bool rendererPriority { get; set; } = false;

		public bool rendersUIOverlay { get; set; } = false;

		public bool overridesEnvironmentLighting { get; set; } = false;

		public bool overridesFog { get; set; } = false;

		public bool overridesRealtimeReflectionProbes { get; set; } = false;

		public bool overridesOtherLightingSettings { get; set; } = false;

		public bool editableMaterialRenderQueue { get; set; } = true;

		public bool overridesLODBias { get; set; } = false;

		public bool overridesMaximumLODLevel { get; set; } = false;

		public bool overridesEnableLODCrossFade { get; set; } = false;

		public bool rendererProbes { get; set; } = true;

		public bool particleSystemInstancing { get; set; } = true;

		[Obsolete("autoAmbientProbeBaking is obsolete. To enable or disable baking of the ambient probe, use ambientProbeBaking instead. (UnityUpgradable) -> ambientProbeBaking", false)]
		public bool autoAmbientProbeBaking
		{
			get
			{
				return this.ambientProbeBaking;
			}
			set
			{
				this.ambientProbeBaking = value;
			}
		}

		[Obsolete("autoDefaultReflectionProbeBaking is obsolete. To enable or disable baking of the default reflection probe, use defaultReflectionProbeBaking instead. (UnityUpgradable) -> defaultReflectionProbeBaking", false)]
		public bool autoDefaultReflectionProbeBaking
		{
			get
			{
				return this.defaultReflectionProbeBaking;
			}
			set
			{
				this.defaultReflectionProbeBaking = value;
			}
		}

		public bool ambientProbeBaking { get; set; } = true;

		public bool defaultReflectionProbeBaking { get; set; } = true;

		public bool overridesShadowmask { get; set; } = false;

		public bool overridesLightProbeSystem { get; set; } = false;

		public bool supportsHDR { get; set; } = false;

		public bool supportsClouds { get; set; } = false;

		public string overridesLightProbeSystemWarningMessage { get; set; } = "Light Probe Groups are unavailable as Probe Volumes have been enabled by the current Render Pipeline.";

		internal unsafe static MixedLightingMode FallbackMixedLightingMode()
		{
			MixedLightingMode mixedLightingMode;
			SupportedRenderingFeatures.FallbackMixedLightingModeByRef(new IntPtr((void*)(&mixedLightingMode)));
			return mixedLightingMode;
		}

		[RequiredByNativeCode]
		internal unsafe static void FallbackMixedLightingModeByRef(IntPtr fallbackModePtr)
		{
			MixedLightingMode* ptr = (MixedLightingMode*)(void*)fallbackModePtr;
			bool flag = SupportedRenderingFeatures.active.defaultMixedLightingModes != SupportedRenderingFeatures.LightmapMixedBakeModes.None && (SupportedRenderingFeatures.active.mixedLightingModes & SupportedRenderingFeatures.active.defaultMixedLightingModes) == SupportedRenderingFeatures.active.defaultMixedLightingModes;
			if (flag)
			{
				SupportedRenderingFeatures.LightmapMixedBakeModes defaultMixedLightingModes = SupportedRenderingFeatures.active.defaultMixedLightingModes;
				SupportedRenderingFeatures.LightmapMixedBakeModes lightmapMixedBakeModes = defaultMixedLightingModes;
				if (lightmapMixedBakeModes != SupportedRenderingFeatures.LightmapMixedBakeModes.Subtractive)
				{
					if (lightmapMixedBakeModes != SupportedRenderingFeatures.LightmapMixedBakeModes.Shadowmask)
					{
						*ptr = MixedLightingMode.IndirectOnly;
					}
					else
					{
						*ptr = MixedLightingMode.Shadowmask;
					}
				}
				else
				{
					*ptr = MixedLightingMode.Subtractive;
				}
			}
			else
			{
				bool flag2 = SupportedRenderingFeatures.IsMixedLightingModeSupported(MixedLightingMode.Shadowmask);
				if (flag2)
				{
					*ptr = MixedLightingMode.Shadowmask;
				}
				else
				{
					bool flag3 = SupportedRenderingFeatures.IsMixedLightingModeSupported(MixedLightingMode.Subtractive);
					if (flag3)
					{
						*ptr = MixedLightingMode.Subtractive;
					}
					else
					{
						*ptr = MixedLightingMode.IndirectOnly;
					}
				}
			}
		}

		internal unsafe static bool IsMixedLightingModeSupported(MixedLightingMode mixedMode)
		{
			bool flag;
			SupportedRenderingFeatures.IsMixedLightingModeSupportedByRef(mixedMode, new IntPtr((void*)(&flag)));
			return flag;
		}

		[RequiredByNativeCode]
		internal unsafe static void IsMixedLightingModeSupportedByRef(MixedLightingMode mixedMode, IntPtr isSupportedPtr)
		{
			bool* ptr = (bool*)(void*)isSupportedPtr;
			bool flag = !SupportedRenderingFeatures.IsLightmapBakeTypeSupported(LightmapBakeType.Mixed);
			if (flag)
			{
				*ptr = false;
			}
			else
			{
				*ptr = (mixedMode == MixedLightingMode.IndirectOnly && (SupportedRenderingFeatures.active.mixedLightingModes & SupportedRenderingFeatures.LightmapMixedBakeModes.IndirectOnly) == SupportedRenderingFeatures.LightmapMixedBakeModes.IndirectOnly) || (mixedMode == MixedLightingMode.Subtractive && (SupportedRenderingFeatures.active.mixedLightingModes & SupportedRenderingFeatures.LightmapMixedBakeModes.Subtractive) == SupportedRenderingFeatures.LightmapMixedBakeModes.Subtractive) || (mixedMode == MixedLightingMode.Shadowmask && (SupportedRenderingFeatures.active.mixedLightingModes & SupportedRenderingFeatures.LightmapMixedBakeModes.Shadowmask) == SupportedRenderingFeatures.LightmapMixedBakeModes.Shadowmask);
			}
		}

		internal unsafe static bool IsLightmapBakeTypeSupported(LightmapBakeType bakeType)
		{
			bool flag;
			SupportedRenderingFeatures.IsLightmapBakeTypeSupportedByRef(bakeType, new IntPtr((void*)(&flag)));
			return flag;
		}

		[RequiredByNativeCode]
		internal unsafe static void IsLightmapBakeTypeSupportedByRef(LightmapBakeType bakeType, IntPtr isSupportedPtr)
		{
			bool* ptr = (bool*)(void*)isSupportedPtr;
			bool flag = bakeType == LightmapBakeType.Mixed;
			if (flag)
			{
				bool flag2 = SupportedRenderingFeatures.IsLightmapBakeTypeSupported(LightmapBakeType.Baked);
				bool flag3 = !flag2 || SupportedRenderingFeatures.active.mixedLightingModes == SupportedRenderingFeatures.LightmapMixedBakeModes.None;
				if (flag3)
				{
					*ptr = false;
					return;
				}
			}
			*ptr = (SupportedRenderingFeatures.active.lightmapBakeTypes & bakeType) == bakeType;
			bool flag4 = bakeType == LightmapBakeType.Realtime && !SupportedRenderingFeatures.active.enlighten;
			if (flag4)
			{
				*ptr = false;
			}
		}

		internal unsafe static bool IsLightmapsModeSupported(LightmapsMode mode)
		{
			bool flag;
			SupportedRenderingFeatures.IsLightmapsModeSupportedByRef(mode, new IntPtr((void*)(&flag)));
			return flag;
		}

		[RequiredByNativeCode]
		internal unsafe static void IsLightmapsModeSupportedByRef(LightmapsMode mode, IntPtr isSupportedPtr)
		{
			bool* ptr = (bool*)(void*)isSupportedPtr;
			*ptr = (SupportedRenderingFeatures.active.lightmapsModes & mode) == mode;
		}

		internal unsafe static bool IsLightmapperSupported(int lightmapper)
		{
			bool flag;
			SupportedRenderingFeatures.IsLightmapperSupportedByRef(lightmapper, new IntPtr((void*)(&flag)));
			return flag;
		}

		[RequiredByNativeCode]
		internal unsafe static void IsLightmapperSupportedByRef(int lightmapper, IntPtr isSupportedPtr)
		{
			bool* ptr = (bool*)(void*)isSupportedPtr;
			*ptr = lightmapper != 0;
		}

		[RequiredByNativeCode]
		internal unsafe static void IsUIOverlayRenderedBySRP(IntPtr isSupportedPtr)
		{
			bool* ptr = (bool*)(void*)isSupportedPtr;
			*ptr = SupportedRenderingFeatures.active.rendersUIOverlay;
		}

		[RequiredByNativeCode]
		internal unsafe static void IsAmbientProbeBakingSupported(IntPtr isSupportedPtr)
		{
			bool* ptr = (bool*)(void*)isSupportedPtr;
			*ptr = SupportedRenderingFeatures.active.ambientProbeBaking;
		}

		[RequiredByNativeCode]
		internal unsafe static void IsDefaultReflectionProbeBakingSupported(IntPtr isSupportedPtr)
		{
			bool* ptr = (bool*)(void*)isSupportedPtr;
			*ptr = SupportedRenderingFeatures.active.defaultReflectionProbeBaking;
		}

		[RequiredByNativeCode]
		internal unsafe static void OverridesLightProbeSystem(IntPtr overridesPtr)
		{
			bool* ptr = (bool*)(void*)overridesPtr;
			*ptr = SupportedRenderingFeatures.active.overridesLightProbeSystem;
		}

		internal unsafe static int FallbackLightmapper()
		{
			int num;
			SupportedRenderingFeatures.FallbackLightmapperByRef(new IntPtr((void*)(&num)));
			return num;
		}

		[RequiredByNativeCode]
		internal unsafe static void FallbackLightmapperByRef(IntPtr lightmapperPtr)
		{
			int* ptr = (int*)(void*)lightmapperPtr;
			*ptr = 1;
		}

		[RequiredByNativeCode]
		internal unsafe static void IsRotatingReflectionProbesSupported(IntPtr isSupportedPtr)
		{
			bool* ptr = (bool*)(void*)isSupportedPtr;
			*ptr = (SupportedRenderingFeatures.active.reflectionProbeModes & SupportedRenderingFeatures.ReflectionProbeModes.Rotation) > SupportedRenderingFeatures.ReflectionProbeModes.None;
		}

		[Obsolete("terrainDetailUnsupported is deprecated.")]
		public bool terrainDetailUnsupported
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		private static SupportedRenderingFeatures s_Active = new SupportedRenderingFeatures();

		[Flags]
		public enum ReflectionProbeModes
		{
			None = 0,
			Rotation = 1
		}

		[Flags]
		public enum LightmapMixedBakeModes
		{
			None = 0,
			IndirectOnly = 1,
			Subtractive = 2,
			Shadowmask = 4
		}
	}
}
