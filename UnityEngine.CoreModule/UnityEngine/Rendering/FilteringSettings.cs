using System;
using UnityEngine.Internal;

namespace UnityEngine.Rendering
{
	public struct FilteringSettings : IEquatable<FilteringSettings>
	{
		public static FilteringSettings defaultValue
		{
			get
			{
				return new FilteringSettings(new RenderQueueRange?(RenderQueueRange.all), -1, uint.MaxValue, 0);
			}
		}

		public FilteringSettings([DefaultValue("RenderQueueRange.all")] RenderQueueRange? renderQueueRange = null, int layerMask = -1, uint renderingLayerMask = 4294967295U, int excludeMotionVectorObjects = 0)
		{
			this = default(FilteringSettings);
			this.m_RenderQueueRange = renderQueueRange ?? RenderQueueRange.all;
			this.m_LayerMask = layerMask;
			this.m_RenderingLayerMask = renderingLayerMask;
			this.m_BatchLayerMask = uint.MaxValue;
			this.m_ExcludeMotionVectorObjects = excludeMotionVectorObjects;
			this.m_ForceAllMotionVectorObjects = 0;
			this.m_SortingLayerRange = SortingLayerRange.all;
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

		public uint batchLayerMask
		{
			get
			{
				return this.m_BatchLayerMask;
			}
			set
			{
				this.m_BatchLayerMask = value;
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
				this.m_ExcludeMotionVectorObjects = (value ? 1 : 0);
			}
		}

		public bool forceAllMotionVectorObjects
		{
			get
			{
				return this.m_ForceAllMotionVectorObjects != 0;
			}
			set
			{
				this.m_ForceAllMotionVectorObjects = (value ? 1 : 0);
			}
		}

		public SortingLayerRange sortingLayerRange
		{
			get
			{
				return this.m_SortingLayerRange;
			}
			set
			{
				this.m_SortingLayerRange = value;
			}
		}

		public bool Equals(FilteringSettings other)
		{
			return this.m_RenderQueueRange.Equals(other.m_RenderQueueRange) && this.m_LayerMask == other.m_LayerMask && this.m_RenderingLayerMask == other.m_RenderingLayerMask && this.m_BatchLayerMask == other.m_BatchLayerMask && this.m_ExcludeMotionVectorObjects == other.m_ExcludeMotionVectorObjects && this.m_ForceAllMotionVectorObjects == other.m_ForceAllMotionVectorObjects;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			return !flag && obj is FilteringSettings && this.Equals((FilteringSettings)obj);
		}

		public override int GetHashCode()
		{
			int num = this.m_RenderQueueRange.GetHashCode();
			num = (num * 397) ^ this.m_LayerMask;
			num = (num * 397) ^ (int)this.m_RenderingLayerMask;
			num = (num * 397) ^ (int)this.m_BatchLayerMask;
			num = (num * 397) ^ this.m_ExcludeMotionVectorObjects;
			return (num * 397) ^ this.m_ForceAllMotionVectorObjects;
		}

		public static bool operator ==(FilteringSettings left, FilteringSettings right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(FilteringSettings left, FilteringSettings right)
		{
			return !left.Equals(right);
		}

		private RenderQueueRange m_RenderQueueRange;

		private int m_LayerMask;

		private uint m_RenderingLayerMask;

		private uint m_BatchLayerMask;

		private int m_ExcludeMotionVectorObjects;

		private int m_ForceAllMotionVectorObjects;

		private SortingLayerRange m_SortingLayerRange;
	}
}
