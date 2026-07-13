using System;

namespace UnityEngine.UIElements.Layout
{
	internal struct LayoutCachedMeasurement
	{
		public unsafe LayoutCachedMeasurement* NextMeasurementCache
		{
			get
			{
				return (LayoutCachedMeasurement*)this.m_NextMeasurementCachePtr;
			}
		}

		public override readonly string ToString()
		{
			return string.Format("Available: {0}/{1}   Parent: {2}/{3}   MeasureMode: {4}/{5},   Computed: {6}/{7}", new object[] { this.AvailableWidth, this.AvailableHeight, this.ParentWidth, this.ParentHeight, this.WidthMeasureMode, this.HeightMeasureMode, this.ComputedWidth, this.ComputedHeight });
		}

		public static LayoutCachedMeasurement Default = new LayoutCachedMeasurement
		{
			AvailableWidth = 0f,
			AvailableHeight = 0f,
			ParentWidth = 0f,
			ParentHeight = 0f,
			WidthMeasureMode = LayoutMeasureMode.Invalid,
			HeightMeasureMode = LayoutMeasureMode.Invalid,
			ComputedWidth = -1f,
			ComputedHeight = -1f,
			m_NextMeasurementCachePtr = null
		};

		public float AvailableWidth;

		public float AvailableHeight;

		public float ParentWidth;

		public float ParentHeight;

		public LayoutMeasureMode WidthMeasureMode;

		public LayoutMeasureMode HeightMeasureMode;

		public float ComputedWidth;

		public float ComputedHeight;

		private unsafe void* m_NextMeasurementCachePtr;
	}
}
