using System;

namespace UnityEngine.Experimental.Rendering
{
	public interface IRenderPipelineAsset
	{
		/// <summary>
		///   <para>Override this method to destroy RenderPipeline cached state.</para>
		/// </summary>
		void DestroyCreatedInstances();

		/// <summary>
		///   <para>Create a IRenderPipeline specific to this asset.</para>
		/// </summary>
		/// <returns>
		///   <para>Created pipeline.</para>
		/// </returns>
		IRenderPipeline CreatePipeline();
	}
}
