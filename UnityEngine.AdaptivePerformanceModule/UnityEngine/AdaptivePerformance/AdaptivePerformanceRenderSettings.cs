using System;

namespace UnityEngine.AdaptivePerformance
{
	public static class AdaptivePerformanceRenderSettings
	{
		public static float MainLightShadowmapResolutionMultiplier
		{
			get
			{
				return AdaptivePerformanceRenderSettings.s_ShadowResolutionMultiplier;
			}
			set
			{
				AdaptivePerformanceRenderSettings.s_ShadowResolutionMultiplier = Mathf.Clamp01(value);
			}
		}

		public static float DecalsDrawDistance
		{
			get
			{
				return AdaptivePerformanceRenderSettings.s_DecalsMaxDistance;
			}
			set
			{
				AdaptivePerformanceRenderSettings.s_DecalsMaxDistance = value;
			}
		}

		public static int MainLightShadowCascadesCountBias { get; set; }

		public static int ShadowQualityBias { get; set; }

		public static float LutBias { get; set; }

		public static float MaxShadowDistanceMultiplier
		{
			get
			{
				return AdaptivePerformanceRenderSettings.s_MaxShadowDistanceMultiplier;
			}
			set
			{
				AdaptivePerformanceRenderSettings.s_MaxShadowDistanceMultiplier = Mathf.Clamp01(value);
			}
		}

		public static float RenderScaleMultiplier
		{
			get
			{
				return AdaptivePerformanceRenderSettings.s_RenderScaleMultiplier;
			}
			set
			{
				AdaptivePerformanceRenderSettings.s_RenderScaleMultiplier = Mathf.Clamp01(value);
			}
		}

		public static int AntiAliasingQualityBias { get; set; }

		public static bool SkipDynamicBatching { get; set; }

		public static bool SkipFrontToBackSorting { get; set; }

		public static bool SkipTransparentObjects { get; set; }

		private static float s_MaxShadowDistanceMultiplier = 1f;

		private static float s_ShadowResolutionMultiplier = 1f;

		private static float s_RenderScaleMultiplier = 1f;

		private static float s_DecalsMaxDistance = 1000f;
	}
}
