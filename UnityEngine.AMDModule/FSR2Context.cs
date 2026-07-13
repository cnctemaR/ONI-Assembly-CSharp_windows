using System;

namespace UnityEngine.AMD
{
	public class FSR2Context
	{
		public readonly ref FSR2CommandInitializationData initData
		{
			get
			{
				return ref this.m_InitData.Value;
			}
		}

		public ref FSR2CommandExecutionData executeData
		{
			get
			{
				return ref this.m_ExecData.Value;
			}
		}

		internal uint featureSlot
		{
			get
			{
				return this.initData.featureSlot;
			}
		}

		internal FSR2Context()
		{
		}

		internal void Init(FSR2CommandInitializationData initSettings, uint featureSlot)
		{
			this.m_InitData.Value = initSettings;
			this.m_InitData.Value.featureSlot = featureSlot;
		}

		internal void Reset()
		{
			this.m_InitData.Value = default(FSR2CommandInitializationData);
			this.m_ExecData.Value = default(FSR2CommandExecutionData);
		}

		internal IntPtr GetInitCmdPtr()
		{
			return this.m_InitData.Ptr;
		}

		internal IntPtr GetExecuteCmdPtr()
		{
			this.m_ExecData.Value.featureSlot = this.featureSlot;
			return this.m_ExecData.Ptr;
		}

		private NativeData<FSR2CommandInitializationData> m_InitData = new NativeData<FSR2CommandInitializationData>();

		private NativeData<FSR2CommandExecutionData> m_ExecData = new NativeData<FSR2CommandExecutionData>();
	}
}
