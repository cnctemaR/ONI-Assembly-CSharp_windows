using System;

namespace UnityEngine.NVIDIA
{
	internal struct InitDeviceCmdData
	{
		public IntPtr projectId
		{
			get
			{
				return this.m_ProjectId;
			}
			set
			{
				this.m_ProjectId = value;
			}
		}

		public IntPtr engineVersion
		{
			get
			{
				return this.m_EngineVersion;
			}
			set
			{
				this.m_EngineVersion = value;
			}
		}

		public IntPtr appDir
		{
			get
			{
				return this.m_AppDir;
			}
			set
			{
				this.m_AppDir = value;
			}
		}

		private IntPtr m_ProjectId;

		private IntPtr m_EngineVersion;

		private IntPtr m_AppDir;
	}
}
