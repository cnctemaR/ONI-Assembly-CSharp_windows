using System;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>How to sort objects during rendering.</para>
	/// </summary>
	[Flags]
	public enum SortFlags
	{
		/// <summary>
		///   <para>Do not sort objects.</para>
		/// </summary>
		None = 0,
		/// <summary>
		///   <para>Sort by renderer sorting layer.</para>
		/// </summary>
		SortingLayer = 1,
		/// <summary>
		///   <para>Sort by material render queue.</para>
		/// </summary>
		RenderQueue = 2,
		/// <summary>
		///   <para>Sort objects back to front.</para>
		/// </summary>
		BackToFront = 4,
		/// <summary>
		///   <para>Sort objects in rough front-to-back buckets.</para>
		/// </summary>
		QuantizedFrontToBack = 8,
		/// <summary>
		///   <para>Sort objects to reduce draw state changes.</para>
		/// </summary>
		OptimizeStateChanges = 16,
		/// <summary>
		///   <para>Sort renderers taking canvas order into account.</para>
		/// </summary>
		CanvasOrder = 32,
		/// <summary>
		///   <para>Typical sorting for opaque objects.</para>
		/// </summary>
		CommonOpaque = 59,
		/// <summary>
		///   <para>Typical sorting for transparencies.</para>
		/// </summary>
		CommonTransparent = 23
	}
}
