using System;

namespace UnityEngine.Rendering
{
	public abstract class RenderPipelineAsset<TRenderPipeline> : RenderPipelineAsset where TRenderPipeline : RenderPipeline
	{
		public sealed override Type pipelineType
		{
			get
			{
				return typeof(TRenderPipeline);
			}
		}

		public override string renderPipelineShaderTag
		{
			get
			{
				return typeof(TRenderPipeline).Name;
			}
		}

		[Obsolete("This property is obsolete. Use pipelineType instead. #from(23.2)", false)]
		protected internal sealed override Type renderPipelineType
		{
			get
			{
				return typeof(TRenderPipeline);
			}
		}
	}
}
