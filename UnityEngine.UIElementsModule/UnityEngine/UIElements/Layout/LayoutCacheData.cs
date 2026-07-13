using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements.Layout
{
	[NativeHeader("Modules/UIElements/Core/Layout/Native/LayoutNative.h")]
	internal struct LayoutCacheData
	{
		public override readonly string ToString()
		{
			return string.Format("CacheCount: {0}\n", this.MeasurementCacheCount()) + string.Format("CachedLayout: {0}", this.CachedLayout);
		}

		public unsafe readonly int MeasurementCacheCount()
		{
			int num = 0;
			LayoutCachedMeasurement cachedLayout = this.CachedLayout;
			for (LayoutCachedMeasurement* ptr = cachedLayout.NextMeasurementCache; ptr != null; ptr = ptr->NextMeasurementCache)
			{
				num++;
			}
			return num;
		}

		public unsafe void ClearCachedMeasurements()
		{
			bool flag = this.CachedLayout.NextMeasurementCache == null;
			if (!flag)
			{
				fixed (LayoutCachedMeasurement* ptr = &this.CachedLayout)
				{
					void* ptr2 = (void*)ptr;
					LayoutCacheData.ClearCachedMeasurements(ptr2);
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ClearCachedMeasurements(void* LayoutCacheData);

		public static LayoutCacheData Default = new LayoutCacheData
		{
			CachedLayout = LayoutCachedMeasurement.Default
		};

		public LayoutCachedMeasurement CachedLayout;
	}
}
