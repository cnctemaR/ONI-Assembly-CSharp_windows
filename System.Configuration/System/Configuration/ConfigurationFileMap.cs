using System;
using System.Runtime.InteropServices;

namespace System.Configuration
{
	public class ConfigurationFileMap : ICloneable
	{
		public ConfigurationFileMap()
		{
			this.machineConfigFilename = RuntimeEnvironment.SystemConfigurationFile;
		}

		public ConfigurationFileMap(string machineConfigFilename)
		{
			this.machineConfigFilename = machineConfigFilename;
		}

		public string MachineConfigFilename
		{
			get
			{
				return this.machineConfigFilename;
			}
			set
			{
				this.machineConfigFilename = value;
			}
		}

		public virtual object Clone()
		{
			return new ConfigurationFileMap(this.machineConfigFilename);
		}

		private string machineConfigFilename;
	}
}
