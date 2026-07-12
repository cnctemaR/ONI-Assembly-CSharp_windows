using System;
using Unity.Collections;

namespace UnityEngine.Rendering.RendererUtils
{
	public struct RendererListDesc
	{
		internal CullingResults cullingResult { readonly get; private set; }

		internal Camera camera { readonly get; set; }

		internal ShaderTagId passName { readonly get; private set; }

		internal ShaderTagId[] passNames { readonly get; private set; }

		public RendererListDesc(ShaderTagId passName, CullingResults cullingResult, Camera camera)
		{
			this = default(RendererListDesc);
			this.passName = passName;
			this.passNames = null;
			this.cullingResult = cullingResult;
			this.camera = camera;
			this.layerMask = -1;
			this.renderingLayerMask = uint.MaxValue;
			this.overrideMaterialPassIndex = 0;
			this.overrideShaderPassIndex = 0;
		}

		public RendererListDesc(ShaderTagId[] passNames, CullingResults cullingResult, Camera camera)
		{
			this = default(RendererListDesc);
			this.passNames = passNames;
			this.passName = ShaderTagId.none;
			this.cullingResult = cullingResult;
			this.camera = camera;
			this.layerMask = -1;
			this.renderingLayerMask = uint.MaxValue;
			this.overrideMaterialPassIndex = 0;
		}

		public bool IsValid()
		{
			bool flag = this.camera == null || (this.passName == ShaderTagId.none && (this.passNames == null || this.passNames.Length == 0));
			return !flag;
		}

		public static RendererListParams ConvertToParameters(in RendererListDesc desc)
		{
			RendererListDesc rendererListDesc = desc;
			bool flag = !rendererListDesc.IsValid();
			RendererListParams rendererListParams;
			if (flag)
			{
				rendererListParams = RendererListParams.Invalid;
			}
			else
			{
				RendererListParams rendererListParams2 = default(RendererListParams);
				SortingSettings sortingSettings = new SortingSettings(desc.camera)
				{
					criteria = desc.sortingCriteria
				};
				DrawingSettings drawingSettings = new DrawingSettings(RendererListDesc.s_EmptyName, sortingSettings)
				{
					perObjectData = desc.rendererConfiguration
				};
				bool flag2 = desc.passName != ShaderTagId.none;
				if (flag2)
				{
					Debug.Assert(desc.passNames == null);
					drawingSettings.SetShaderPassName(0, desc.passName);
				}
				else
				{
					for (int i = 0; i < desc.passNames.Length; i++)
					{
						drawingSettings.SetShaderPassName(i, desc.passNames[i]);
					}
				}
				bool flag3 = desc.overrideShader != null;
				if (flag3)
				{
					drawingSettings.overrideShader = desc.overrideShader;
					drawingSettings.overrideShaderPassIndex = desc.overrideShaderPassIndex;
				}
				bool flag4 = desc.overrideMaterial != null;
				if (flag4)
				{
					drawingSettings.overrideMaterial = desc.overrideMaterial;
					drawingSettings.overrideMaterialPassIndex = desc.overrideMaterialPassIndex;
				}
				FilteringSettings filteringSettings = new FilteringSettings(new RenderQueueRange?(desc.renderQueueRange), desc.layerMask, desc.renderingLayerMask, 0)
				{
					excludeMotionVectorObjects = desc.excludeObjectMotionVectors
				};
				rendererListParams2.cullingResults = desc.cullingResult;
				rendererListParams2.drawSettings = drawingSettings;
				rendererListParams2.filteringSettings = filteringSettings;
				rendererListParams2.tagName = ShaderTagId.none;
				rendererListParams2.isPassTagName = false;
				bool flag5 = desc.stateBlock != null && desc.stateBlock != null;
				if (flag5)
				{
					NativeArray<RenderStateBlock> nativeArray = new NativeArray<RenderStateBlock>(1, Allocator.Temp, NativeArrayOptions.ClearMemory);
					nativeArray[0] = desc.stateBlock.Value;
					rendererListParams2.stateBlocks = new NativeArray<RenderStateBlock>?(nativeArray);
					NativeArray<ShaderTagId> nativeArray2 = new NativeArray<ShaderTagId>(1, Allocator.Temp, NativeArrayOptions.ClearMemory);
					nativeArray2[0] = ShaderTagId.none;
					rendererListParams2.tagValues = new NativeArray<ShaderTagId>?(nativeArray2);
				}
				rendererListParams = rendererListParams2;
			}
			return rendererListParams;
		}

		public SortingCriteria sortingCriteria;

		public PerObjectData rendererConfiguration;

		public RenderQueueRange renderQueueRange;

		public RenderStateBlock? stateBlock;

		public Shader overrideShader;

		public Material overrideMaterial;

		public bool excludeObjectMotionVectors;

		public int layerMask;

		public uint renderingLayerMask;

		public int overrideMaterialPassIndex;

		public int overrideShaderPassIndex;

		private static readonly ShaderTagId s_EmptyName = new ShaderTagId("");
	}
}
