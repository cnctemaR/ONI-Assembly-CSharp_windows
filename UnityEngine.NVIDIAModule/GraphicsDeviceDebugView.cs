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

		public IEnumerable<DLSSDebugFeatureInfos> dlssFeatureInfos
		{
			get
			{
				IEnumerable<DLSSDebugFeatureInfos> enumerable;
				if (this.m_DlssDebugFeatures != null)
				{
					IEnumerable<DLSSDebugFeatureInfos> dlssDebugFeatures = this.m_DlssDebugFeatures;
					enumerable = dlssDebugFeatures;
				}
				else
				{
					enumerable = Enumerable.Empty<DLSSDebugFeatureInfos>();
				}
				return enumerable;
			}
		}

		internal uint m_ViewId = 0U;

		internal uint m_DeviceVersion = 0U;

		internal uint m_NgxVersion = 0U;

		internal DLSSDebugFeatureInfos[] m_DlssDebugFeatures = null;
	}
}
