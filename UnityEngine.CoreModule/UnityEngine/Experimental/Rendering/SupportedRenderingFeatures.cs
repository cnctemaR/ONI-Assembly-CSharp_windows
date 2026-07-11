using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	public class SupportedRenderingFeatures
	{
		public static SupportedRenderingFeatures active
		{
			get
			{
				if (SupportedRenderingFeatures.s_Active == null)
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

		public SupportedRenderingFeatures.ReflectionProbeSupportFlags reflectionProbeSupportFlags { get; set; } = SupportedRenderingFeatures.ReflectionProbeSupportFlags.None;

		public SupportedRenderingFeatures.LightmapMixedBakeMode defaultMixedLightingMode { get; set; } = SupportedRenderingFeatures.LightmapMixedBakeMode.None;

		public SupportedRenderingFeatures.LightmapMixedBakeMode supportedMixedLightingModes { get; set; } = SupportedRenderingFeatures.LightmapMixedBakeMode.IndirectOnly | SupportedRenderingFeatures.LightmapMixedBakeMode.Subtractive | SupportedRenderingFeatures.LightmapMixedBakeMode.Shadowmask;

		public LightmapBakeType supportedLightmapBakeTypes { get; set; } = LightmapBakeType.Realtime | LightmapBakeType.Baked | LightmapBakeType.Mixed;

		public LightmapsMode supportedLightmapsModes { get; set; } = LightmapsMode.CombinedDirectional;

		public bool rendererSupportsLightProbeProxyVolumes { get; set; } = true;

		public bool rendererSupportsMotionVectors { get; set; } = true;

		public bool rendererSupportsReceiveShadows { get; set; } = true;

		public bool rendererSupportsReflectionProbes { get; set; } = true;

		public bool rendererSupportsRendererPriority { get; set; } = false;

		public bool rendererOverridesEnvironmentLighting { get; set; } = false;

		public bool rendererOverridesFog { get; set; } = false;

		public bool rendererOverridesOtherLightingSettings { get; set; } = false;

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
			if (SupportedRenderingFeatures.active.defaultMixedLightingMode != SupportedRenderingFeatures.LightmapMixedBakeMode.None && (SupportedRenderingFeatures.active.supportedMixedLightingModes & SupportedRenderingFeatures.active.defaultMixedLightingMode) == SupportedRenderingFeatures.active.defaultMixedLightingMode)
			{
				SupportedRenderingFeatures.LightmapMixedBakeMode defaultMixedLightingMode = SupportedRenderingFeatures.active.defaultMixedLightingMode;
				if (defaultMixedLightingMode != SupportedRenderingFeatures.LightmapMixedBakeMode.Shadowmask)
				{
					if (defaultMixedLightingMode != SupportedRenderingFeatures.LightmapMixedBakeMode.Subtractive)
					{
						*ptr = MixedLightingMode.IndirectOnly;
					}
					else
					{
						*ptr = MixedLightingMode.Subtractive;
					}
				}
				else
				{
					*ptr = MixedLightingMode.Shadowmask;
				}
			}
			else if (SupportedRenderingFeatures.IsMixedLightingModeSupported(MixedLightingMode.Shadowmask))
			{
				*ptr = MixedLightingMode.Shadowmask;
			}
			else if (SupportedRenderingFeatures.IsMixedLightingModeSupported(MixedLightingMode.Subtractive))
			{
				*ptr = MixedLightingMode.Subtractive;
			}
			else
			{
				*ptr = MixedLightingMode.IndirectOnly;
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
			if (!SupportedRenderingFeatures.IsLightmapBakeTypeSupported(LightmapBakeType.Mixed))
			{
				*ptr = false;
			}
			else
			{
				*ptr = (mixedMode == MixedLightingMode.IndirectOnly && (SupportedRenderingFeatures.active.supportedMixedLightingModes & SupportedRenderingFeatures.LightmapMixedBakeMode.IndirectOnly) == SupportedRenderingFeatures.LightmapMixedBakeMode.IndirectOnly) || (mixedMode == MixedLightingMode.Subtractive && (SupportedRenderingFeatures.active.supportedMixedLightingModes & SupportedRenderingFeatures.LightmapMixedBakeMode.Subtractive) == SupportedRenderingFeatures.LightmapMixedBakeMode.Subtractive) || (mixedMode == MixedLightingMode.Shadowmask && (SupportedRenderingFeatures.active.supportedMixedLightingModes & SupportedRenderingFeatures.LightmapMixedBakeMode.Shadowmask) == SupportedRenderingFeatures.LightmapMixedBakeMode.Shadowmask);
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
			if (bakeType == LightmapBakeType.Mixed)
			{
				bool flag = SupportedRenderingFeatures.IsLightmapBakeTypeSupported(LightmapBakeType.Baked);
				if (!flag || SupportedRenderingFeatures.active.supportedMixedLightingModes == SupportedRenderingFeatures.LightmapMixedBakeMode.None)
				{
					*ptr = false;
					return;
				}
			}
			*ptr = (SupportedRenderingFeatures.active.supportedLightmapBakeTypes & bakeType) == bakeType;
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
			*ptr = (SupportedRenderingFeatures.active.supportedLightmapsModes & mode) == mode;
		}

		private static SupportedRenderingFeatures s_Active = new SupportedRenderingFeatures();

		[Flags]
		public enum ReflectionProbeSupportFlags
		{
			None = 0,
			Rotation = 1
		}

		[Flags]
		public enum LightmapMixedBakeMode
		{
			None = 0,
			IndirectOnly = 1,
			Subtractive = 2,
			Shadowmask = 4
		}
	}
}
