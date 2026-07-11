using System;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Filter settings for ScriptableRenderContext.DrawRenderers.</para>
	/// </summary>
	public struct FilterRenderersSettings
	{
		/// <summary>
		///   <para></para>
		/// </summary>
		/// <param name="initializeValues">Specifies whether the values of the struct should be initialized.</param>
		public FilterRenderersSettings(bool initializeValues = false)
		{
			this = default(FilterRenderersSettings);
			if (initializeValues)
			{
				this.m_RenderQueueRange = RenderQueueRange.all;
				this.m_LayerMask = -1;
				this.m_RenderingLayerMask = uint.MaxValue;
			}
		}

		/// <summary>
		///   <para>Render objects whose material render queue in inside this range.</para>
		/// </summary>
		public RenderQueueRange renderQueueRange
		{
			get
			{
				return this.m_RenderQueueRange;
			}
			set
			{
				this.m_RenderQueueRange = value;
			}
		}

		/// <summary>
		///   <para>Only render objects in the given layer mask.</para>
		/// </summary>
		public int layerMask
		{
			get
			{
				return this.m_LayerMask;
			}
			set
			{
				this.m_LayerMask = value;
			}
		}

		/// <summary>
		///   <para>The rendering layer mask to use when filtering available renderers for drawing.</para>
		/// </summary>
		public uint renderingLayerMask
		{
			get
			{
				return this.m_RenderingLayerMask;
			}
			set
			{
				this.m_RenderingLayerMask = value;
			}
		}

		private RenderQueueRange m_RenderQueueRange;

		private int m_LayerMask;

		private uint m_RenderingLayerMask;
	}
}
