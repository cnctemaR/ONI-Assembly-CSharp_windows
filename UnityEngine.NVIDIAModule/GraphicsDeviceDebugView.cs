using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.NVIDIA
{
	public class GraphicsDeviceDebugView
	{
		internal GraphicsDeviceDebugView(uint viewId)
		{
			this.m_ViewId = viewId;
		}

		public uint deviceVersion
		{
			get
			{
				return this.m_DeviceVersion;
			}
		}

		public uint ngxVersion
		{
			get
			{
				return this.m_NgxVersion;
			}
		}

		[Obsolete("This property causes garbage collection and is inefficient. Use dlssFeatureInfosSpan and dlssFeatureInfoCount instead.", false)]
		public IEnumerable<DLSSDebugFeatureInfos> dlssFeatureInfos
		{
			get
			{
				return this.m_DlssDebugFeatures.Take<DLSSDebugFeatureInfos>((int)this.m_DlssFeatureValidCount);
			}
		}

		public ReadOnlySpan<DLSSDebugFeatureInfos> dlssFeatureInfosSpan
		{
			get
			{
				return new ReadOnlySpan<DLSSDebugFeatureInfos>(this.m_DlssDebugFeatures, 0, (int)this.m_DlssFeatureValidCount);
			}
		}

		internal const int MaxFeatures = 16;

		internal uint m_ViewId = 0U;

		internal uint m_DeviceVersion = 0U;

		internal uint m_NgxVersion = 0U;

		internal readonly DLSSDebugFeatureInfos[] m_DlssDebugFeatures = new DLSSDebugFeatureInfos[16];

		internal uint m_DlssFeatureValidCount = 0U;
	}
}
