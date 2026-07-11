using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Describes the rendering features supported by a given render pipeline.</para>
	/// </summary>
	public class SupportedRenderingFeatures
	{
		/// <summary>
		///   <para>Get / Set a SupportedRenderingFeatures.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Flags for supported reflection probes.</para>
		/// </summary>
		public SupportedRenderingFeatures.ReflectionProbeSupportFlags reflectionProbeSupportFlags { get; set; } = SupportedRenderingFeatures.ReflectionProbeSupportFlags.None;

		/// <summary>
		///   <para>This is the fallback mode if the mode the user had previously selected is no longer available. See SupportedRenderingFeatures.supportedMixedLightingModes.</para>
		/// </summary>
		public SupportedRenderingFeatures.LightmapMixedBakeMode defaultMixedLightingMode { get; set; } = SupportedRenderingFeatures.LightmapMixedBakeMode.None;

		/// <summary>
		///   <para>Specifies what LightmapMixedBakeMode that are supported. Please define a SupportedRenderingFeatures.defaultMixedLightingMode in case multiple modes are supported.</para>
		/// </summary>
		public SupportedRenderingFeatures.LightmapMixedBakeMode supportedMixedLightingModes { get; set; } = SupportedRenderingFeatures.LightmapMixedBakeMode.IndirectOnly | SupportedRenderingFeatures.LightmapMixedBakeMode.Subtractive | SupportedRenderingFeatures.LightmapMixedBakeMode.Shadowmask;

		/// <summary>
		///   <para>What baking types are supported. The unsupported ones will be hidden from the UI. See LightmapBakeType.</para>
		/// </summary>
		public LightmapBakeType supportedLightmapBakeTypes { get; set; } = LightmapBakeType.Realtime | LightmapBakeType.Baked | LightmapBakeType.Mixed;

		/// <summary>
		///   <para>Specifies what modes are supported. Has to be at least one. See LightmapsMode.</para>
		/// </summary>
		public LightmapsMode supportedLightmapsModes { get; set; } = LightmapsMode.CombinedDirectional;

		/// <summary>
		///   <para>Are light probe proxy volumes supported?</para>
		/// </summary>
		public bool rendererSupportsLightProbeProxyVolumes { get; set; } = true;

		/// <summary>
		///   <para>Are motion vectors supported?</para>
		/// </summary>
		public bool rendererSupportsMotionVectors { get; set; } = true;

		/// <summary>
		///   <para>Can renderers support receiving shadows?</para>
		/// </summary>
		public bool rendererSupportsReceiveShadows { get; set; } = true;

		/// <summary>
		///   <para>Are reflection probes supported?</para>
		/// </summary>
		public bool rendererSupportsReflectionProbes { get; set; } = true;

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

		/// <summary>
		///   <para>Supported modes for ReflectionProbes.</para>
		/// </summary>
		[Flags]
		public enum ReflectionProbeSupportFlags
		{
			/// <summary>
			///   <para>Default reflection probe support.</para>
			/// </summary>
			None = 0,
			/// <summary>
			///   <para>Rotated reflection probes are supported.</para>
			/// </summary>
			Rotation = 1
		}

		/// <summary>
		///   <para>Same as MixedLightingMode for baking, but is used to determine what is supported by the pipeline.</para>
		/// </summary>
		[Flags]
		public enum LightmapMixedBakeMode
		{
			/// <summary>
			///   <para>No mode is supported.</para>
			/// </summary>
			None = 0,
			/// <summary>
			///   <para>Same as MixedLightingMode.IndirectOnly but determines if it is supported by the pipeline.</para>
			/// </summary>
			IndirectOnly = 1,
			/// <summary>
			///   <para>Same as MixedLightingMode.Subtractive but determines if it is supported by the pipeline.</para>
			/// </summary>
			Subtractive = 2,
			/// <summary>
			///   <para>Same as MixedLightingMode.Shadowmask but determines if it is supported by the pipeline.</para>
			/// </summary>
			Shadowmask = 4
		}
	}
}
