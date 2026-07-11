using System;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Specifies which parts of the render state that is overriden.</para>
	/// </summary>
	[Flags]
	public enum RenderStateMask
	{
		/// <summary>
		///   <para>No render states are overridden.</para>
		/// </summary>
		Nothing = 0,
		/// <summary>
		///   <para>When set, the blend state is overridden.</para>
		/// </summary>
		Blend = 1,
		/// <summary>
		///   <para>When set, the raster state is overridden.</para>
		/// </summary>
		Raster = 2,
		/// <summary>
		///   <para>When set, the depth state is overridden.</para>
		/// </summary>
		Depth = 4,
		/// <summary>
		///   <para>When set, the stencil state and reference value is overridden.</para>
		/// </summary>
		Stencil = 8,
		/// <summary>
		///   <para>When set, all render states are overridden.</para>
		/// </summary>
		Everything = 15
	}
}
