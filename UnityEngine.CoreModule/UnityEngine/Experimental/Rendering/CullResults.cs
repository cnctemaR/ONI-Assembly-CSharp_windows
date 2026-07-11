using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Culling results (visible objects, lights, reflection probes).</para>
	/// </summary>
	[UsedByNativeCode]
	public struct CullResults
	{
		private void Init()
		{
			this.visibleLights = new List<VisibleLight>();
			this.visibleOffscreenVertexLights = new List<VisibleLight>();
			this.visibleReflectionProbes = new List<VisibleReflectionProbe>();
			this.visibleRenderers = default(FilterResults);
			this.cullResults = IntPtr.Zero;
		}

		public unsafe static bool GetCullingParameters(Camera camera, out ScriptableCullingParameters cullingParameters)
		{
			return CullResults.GetCullingParameters_Internal(camera, false, out cullingParameters, sizeof(ScriptableCullingParameters));
		}

		public unsafe static bool GetCullingParameters(Camera camera, bool stereoAware, out ScriptableCullingParameters cullingParameters)
		{
			return CullResults.GetCullingParameters_Internal(camera, stereoAware, out cullingParameters, sizeof(ScriptableCullingParameters));
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetCullingParameters_Internal(Camera camera, bool stereoAware, out ScriptableCullingParameters cullingParameters, int managedCullingParametersSize);

		internal static void Internal_Cull(ref ScriptableCullingParameters parameters, ScriptableRenderContext renderLoop, ref CullResults results)
		{
			CullResults.INTERNAL_CALL_Internal_Cull(ref parameters, ref renderLoop, ref results);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_Cull(ref ScriptableCullingParameters parameters, ref ScriptableRenderContext renderLoop, ref CullResults results);

		public static CullResults Cull(ref ScriptableCullingParameters parameters, ScriptableRenderContext renderLoop)
		{
			CullResults cullResults = default(CullResults);
			CullResults.Cull(ref parameters, renderLoop, ref cullResults);
			return cullResults;
		}

		public static void Cull(ref ScriptableCullingParameters parameters, ScriptableRenderContext renderLoop, ref CullResults results)
		{
			if (results.visibleLights == null || results.visibleOffscreenVertexLights == null || results.visibleReflectionProbes == null)
			{
				results.Init();
			}
			CullResults.Internal_Cull(ref parameters, renderLoop, ref results);
		}

		public static bool Cull(Camera camera, ScriptableRenderContext renderLoop, out CullResults results)
		{
			results.cullResults = IntPtr.Zero;
			results.visibleLights = null;
			results.visibleOffscreenVertexLights = null;
			results.visibleReflectionProbes = null;
			results.visibleRenderers = default(FilterResults);
			ScriptableCullingParameters scriptableCullingParameters;
			bool flag;
			if (!CullResults.GetCullingParameters(camera, out scriptableCullingParameters))
			{
				flag = false;
			}
			else
			{
				results = CullResults.Cull(ref scriptableCullingParameters, renderLoop);
				flag = true;
			}
			return flag;
		}

		public bool GetShadowCasterBounds(int lightIndex, out Bounds outBounds)
		{
			return CullResults.GetShadowCasterBounds(this.cullResults, lightIndex, out outBounds);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetShadowCasterBounds(IntPtr cullResults, int lightIndex, out Bounds bounds);

		/// <summary>
		///   <para>Gets the number of per-object light indices.</para>
		/// </summary>
		/// <returns>
		///   <para>The number of per-object light indices.</para>
		/// </returns>
		public int GetLightIndicesCount()
		{
			return CullResults.GetLightIndicesCount(this.cullResults);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetLightIndicesCount(IntPtr cullResults);

		/// <summary>
		///   <para>Fills a compute buffer with per-object light indices.</para>
		/// </summary>
		/// <param name="computeBuffer">The compute buffer object to fill.</param>
		public void FillLightIndices(ComputeBuffer computeBuffer)
		{
			CullResults.FillLightIndices(this.cullResults, computeBuffer);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FillLightIndices(IntPtr cullResults, ComputeBuffer computeBuffer);

		/// <summary>
		///   <para>If a RenderPipeline sorts or otherwise modifies the VisibleLight list, an index remap will be necessary to properly make use of per-object light lists.</para>
		/// </summary>
		/// <returns>
		///   <para>Array of indices that map from VisibleLight indices to internal per-object light list indices.</para>
		/// </returns>
		public int[] GetLightIndexMap()
		{
			return CullResults.GetLightIndexMap(this.cullResults);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int[] GetLightIndexMap(IntPtr cullResults);

		/// <summary>
		///   <para>If a RenderPipeline sorts or otherwise modifies the VisibleLight list, an index remap will be necessary to properly make use of per-object light lists.
		/// If an element of the array is set to -1, the light corresponding to that element will be disabled.</para>
		/// </summary>
		/// <param name="mapping">Array with light indices that map from VisibleLight to internal per-object light lists.</param>
		public void SetLightIndexMap(int[] mapping)
		{
			CullResults.SetLightIndexMap(this.cullResults, mapping);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLightIndexMap(IntPtr cullResults, int[] mapping);

		public bool ComputeSpotShadowMatricesAndCullingPrimitives(int activeLightIndex, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData)
		{
			return CullResults.ComputeSpotShadowMatricesAndCullingPrimitives(this.cullResults, activeLightIndex, out viewMatrix, out projMatrix, out shadowSplitData);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ComputeSpotShadowMatricesAndCullingPrimitives(IntPtr cullResults, int activeLightIndex, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData);

		public bool ComputePointShadowMatricesAndCullingPrimitives(int activeLightIndex, CubemapFace cubemapFace, float fovBias, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData)
		{
			return CullResults.ComputePointShadowMatricesAndCullingPrimitives(this.cullResults, activeLightIndex, cubemapFace, fovBias, out viewMatrix, out projMatrix, out shadowSplitData);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ComputePointShadowMatricesAndCullingPrimitives(IntPtr cullResults, int activeLightIndex, CubemapFace cubemapFace, float fovBias, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData);

		public bool ComputeDirectionalShadowMatricesAndCullingPrimitives(int activeLightIndex, int splitIndex, int splitCount, Vector3 splitRatio, int shadowResolution, float shadowNearPlaneOffset, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData)
		{
			return CullResults.ComputeDirectionalShadowMatricesAndCullingPrimitives(this.cullResults, activeLightIndex, splitIndex, splitCount, splitRatio, shadowResolution, shadowNearPlaneOffset, out viewMatrix, out projMatrix, out shadowSplitData);
		}

		private static bool ComputeDirectionalShadowMatricesAndCullingPrimitives(IntPtr cullResults, int activeLightIndex, int splitIndex, int splitCount, Vector3 splitRatio, int shadowResolution, float shadowNearPlaneOffset, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData)
		{
			return CullResults.INTERNAL_CALL_ComputeDirectionalShadowMatricesAndCullingPrimitives(cullResults, activeLightIndex, splitIndex, splitCount, ref splitRatio, shadowResolution, shadowNearPlaneOffset, out viewMatrix, out projMatrix, out shadowSplitData);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_ComputeDirectionalShadowMatricesAndCullingPrimitives(IntPtr cullResults, int activeLightIndex, int splitIndex, int splitCount, ref Vector3 splitRatio, int shadowResolution, float shadowNearPlaneOffset, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData);

		/// <summary>
		///   <para>Array of visible lights.</para>
		/// </summary>
		public List<VisibleLight> visibleLights;

		/// <summary>
		///   <para>Off screen lights that still effect visible scene vertices.</para>
		/// </summary>
		public List<VisibleLight> visibleOffscreenVertexLights;

		/// <summary>
		///   <para>Array of visible reflection probes.</para>
		/// </summary>
		public List<VisibleReflectionProbe> visibleReflectionProbes;

		/// <summary>
		///   <para>Visible renderers.</para>
		/// </summary>
		public FilterResults visibleRenderers;

		internal IntPtr cullResults;
	}
}
