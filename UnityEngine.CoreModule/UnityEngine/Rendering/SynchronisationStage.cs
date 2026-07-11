using System;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Broadly describes the stages of processing a draw call on the GPU.</para>
	/// </summary>
	public enum SynchronisationStage
	{
		/// <summary>
		///   <para>All aspects of vertex processing.</para>
		/// </summary>
		VertexProcessing,
		/// <summary>
		///   <para>The process of creating and shading the fragments.</para>
		/// </summary>
		PixelProcessing
	}
}
