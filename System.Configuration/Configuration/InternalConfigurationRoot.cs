using System;
using System.Configuration.Internal;

namespace System.Configuration
{
	internal class InternalConfigurationRoot : IInternalConfigRoot
	{
		[MonoTODO]
		public IInternalConfigRecord GetConfigRecord(string configPath)
		{
			throw new NotImplementedException();
		}

		public object GetSection(string section, string configPath)
		{
			return this.GetConfigRecord(configPath).GetSection(section);
		}

		[MonoTODO]
		public string GetUniqueConfigPath(string configPath)
		{
			return configPath;
		}

		[MonoTODO]
		public IInternalConfigRecord GetUniqueConfigRecord(string configPath)
		{
			return this.GetConfigRecord(this.GetUniqueConfigPath(configPath));
		}

		public void Init(IInternalConfigHost host, bool isDesignTime)
		{
			this.host = host;
			this.isDesignTime = isDesignTime;
		}

		[MonoTODO]
		public void RemoveConfig(string configPath)
		{
			this.host.DeleteStream(configPath);
			if (this.ConfigRemoved != null)
			{
				this.ConfigRemoved(this, new InternalConfigEventArgs(configPath));
			}
		}

		public bool IsDesignTime
		{
			get
			{
				return this.isDesignTime;
			}
		}

		public event InternalConfigEventHandler ConfigChanged;

		public event InternalConfigEventHandler ConfigRemoved;

		private IInternalConfigHost host;

		private bool isDesignTime;
	}
}
