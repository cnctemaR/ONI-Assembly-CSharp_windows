using System;

namespace UnityEngine.NVIDIA
{
	public readonly struct DLSSDebugFeatureInfos
	{
		public bool validFeature
		{
			get
			{
				return this.m_ValidFeature;
			}
		}

		public uint featureSlot
		{
			get
			{
				return this.m_FeatureSlot;
			}
		}

		public DLSSCommandExecutionData execData
		{
			get
			{
				return this.m_ExecData;
			}
		}

		public DLSSCommandInitializationData initData
		{
			get
			{
				return this.m_InitData;
			}
		}

		private readonly bool m_ValidFeature;

		private readonly uint m_FeatureSlot;

		private readonly DLSSCommandExecutionData m_ExecData;

		private readonly DLSSCommandInitializationData m_InitData;
	}
}
