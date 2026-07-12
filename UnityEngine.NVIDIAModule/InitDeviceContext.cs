using System;

namespace UnityEngine.NVIDIA
{
	internal class InitDeviceContext
	{
		public InitDeviceContext(string projectId, string engineVersion, string appDir)
		{
			this.m_ProjectId.Str = projectId;
			this.m_EngineVersion.Str = engineVersion;
			this.m_AppDir.Str = appDir;
		}

		internal IntPtr GetInitCmdPtr()
		{
			this.m_InitData.Value.projectId = this.m_ProjectId.Ptr;
			this.m_InitData.Value.engineVersion = this.m_EngineVersion.Ptr;
			this.m_InitData.Value.appDir = this.m_AppDir.Ptr;
			return this.m_InitData.Ptr;
		}

		private NativeStr m_ProjectId = new NativeStr();

		private NativeStr m_EngineVersion = new NativeStr();

		private NativeStr m_AppDir = new NativeStr();

		private NativeData<InitDeviceCmdData> m_InitData = new NativeData<InitDeviceCmdData>();
	}
}
