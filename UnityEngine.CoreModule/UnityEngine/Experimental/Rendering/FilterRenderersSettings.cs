using System;

namespace UnityEngine.Experimental.Rendering
{
	public struct FilterRenderersSettings
	{
		public FilterRenderersSettings(bool initializeValues = false)
		{
			this = default(FilterRenderersSettings);
			if (initializeValues)
			{
				this.m_RenderQueueRange = RenderQueueRange.all;
				this.m_LayerMask = -1;
				this.m_RenderingLayerMask = uint.MaxValue;
				this.m_ExcludeMotionVectorObjects = 0;
			}
		}

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

		public bool excludeMotionVectorObjects
		{
			get
			{
				return this.m_ExcludeMotionVectorObjects != 0;
			}
			set
			{
				this.m_ExcludeMotionVectorObjects = ((!value) ? 0 : 1);
			}
		}

		private RenderQueueRange m_RenderQueueRange;

		private int m_LayerMask;

		private uint m_RenderingLayerMask;

		private int m_ExcludeMotionVectorObjects;
	}
}
