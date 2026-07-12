using System;

namespace UnityEngine.NVIDIA
{
	public class DLSSContext
	{
		public readonly ref DLSSCommandInitializationData initData
		{
			get
			{
				return ref this.m_InitData.Value;
			}
		}

		public ref DLSSCommandExecutionData executeData
		{
			get
			{
				return ref this.m_ExecData.Value;
			}
		}

		internal unsafe uint featureSlot
		{
			get
			{
				DLSSCommandInitializationData dlsscommandInitializationData = *this.initData;
				return dlsscommandInitializationData.featureSlot;
			}
		}

		internal DLSSContext()
		{
		}

		internal void Init(DLSSCommandInitializationData initSettings, uint featureSlot)
		{
			this.m_InitData.Value = initSettings;
			this.m_InitData.Value.featureSlot = featureSlot;
		}

		internal void Reset()
		{
			this.m_InitData.Value = default(DLSSCommandInitializationData);
			this.m_ExecData.Value = default(DLSSCommandExecutionData);
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

		private NativeData<DLSSCommandInitializationData> m_InitData = new NativeData<DLSSCommandInitializationData>();

		private NativeData<DLSSCommandExecutionData> m_ExecData = new NativeData<DLSSCommandExecutionData>();
	}
}
