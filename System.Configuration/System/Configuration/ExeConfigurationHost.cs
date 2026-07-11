using System;
using System.Configuration.Internal;

namespace System.Configuration
{
	internal class ExeConfigurationHost : InternalConfigurationHost
	{
		public override void Init(IInternalConfigRoot root, params object[] hostInitParams)
		{
			this.map = (ExeConfigurationFileMap)hostInitParams[0];
			this.level = (ConfigurationUserLevel)hostInitParams[1];
			ExeConfigurationHost.CheckFileMap(this.level, this.map);
		}

		private static void CheckFileMap(ConfigurationUserLevel level, ExeConfigurationFileMap map)
		{
			if (level != ConfigurationUserLevel.None)
			{
				if (level != ConfigurationUserLevel.PerUserRoaming)
				{
					if (level != ConfigurationUserLevel.PerUserRoamingAndLocal)
					{
						return;
					}
					if (string.IsNullOrEmpty(map.LocalUserConfigFilename))
					{
						throw new ArgumentException("The 'LocalUserConfigFilename' argument cannot be null.");
					}
				}
				if (string.IsNullOrEmpty(map.RoamingUserConfigFilename))
				{
					throw new ArgumentException("The 'RoamingUserConfigFilename' argument cannot be null.");
				}
			}
			if (string.IsNullOrEmpty(map.ExeConfigFilename))
			{
				throw new ArgumentException("The 'ExeConfigFilename' argument cannot be null.");
			}
		}

		public override string GetStreamName(string configPath)
		{
			if (configPath == "exe")
			{
				return this.map.ExeConfigFilename;
			}
			if (configPath == "local")
			{
				return this.map.LocalUserConfigFilename;
			}
			if (configPath == "roaming")
			{
				return this.map.RoamingUserConfigFilename;
			}
			if (configPath == "machine")
			{
				return this.map.MachineConfigFilename;
			}
			ConfigurationUserLevel configurationUserLevel = this.level;
			if (configurationUserLevel == ConfigurationUserLevel.None)
			{
				return this.map.ExeConfigFilename;
			}
			if (configurationUserLevel == ConfigurationUserLevel.PerUserRoaming)
			{
				return this.map.RoamingUserConfigFilename;
			}
			if (configurationUserLevel != ConfigurationUserLevel.PerUserRoamingAndLocal)
			{
				return this.map.MachineConfigFilename;
			}
			return this.map.LocalUserConfigFilename;
		}

		public override void InitForConfiguration(ref string locationSubPath, out string configPath, out string locationConfigPath, IInternalConfigRoot root, params object[] hostInitConfigurationParams)
		{
			this.map = (ExeConfigurationFileMap)hostInitConfigurationParams[0];
			if (hostInitConfigurationParams.Length > 1 && hostInitConfigurationParams[1] is ConfigurationUserLevel)
			{
				this.level = (ConfigurationUserLevel)hostInitConfigurationParams[1];
			}
			ExeConfigurationHost.CheckFileMap(this.level, this.map);
			if (locationSubPath == null)
			{
				ConfigurationUserLevel configurationUserLevel = this.level;
				if (configurationUserLevel != ConfigurationUserLevel.PerUserRoaming)
				{
					if (configurationUserLevel == ConfigurationUserLevel.PerUserRoamingAndLocal)
					{
						if (this.map.LocalUserConfigFilename == null)
						{
							throw new ArgumentException("LocalUserConfigFilename must be set correctly");
						}
						locationSubPath = "local";
					}
				}
				else
				{
					if (this.map.RoamingUserConfigFilename == null)
					{
						throw new ArgumentException("RoamingUserConfigFilename must be set correctly");
					}
					locationSubPath = "roaming";
				}
			}
			if (locationSubPath == "exe" || (locationSubPath == null && this.map.ExeConfigFilename != null))
			{
				configPath = "exe";
				locationSubPath = "machine";
				locationConfigPath = this.map.ExeConfigFilename;
				return;
			}
			if (locationSubPath == "local" && this.map.LocalUserConfigFilename != null)
			{
				configPath = "local";
				locationSubPath = "roaming";
				locationConfigPath = this.map.LocalUserConfigFilename;
				return;
			}
			if (locationSubPath == "roaming" && this.map.RoamingUserConfigFilename != null)
			{
				configPath = "roaming";
				locationSubPath = "exe";
				locationConfigPath = this.map.RoamingUserConfigFilename;
				return;
			}
			if (locationSubPath == "machine" && this.map.MachineConfigFilename != null)
			{
				configPath = "machine";
				locationSubPath = null;
				locationConfigPath = null;
				return;
			}
			throw new NotImplementedException();
		}

		private ExeConfigurationFileMap map;

		private ConfigurationUserLevel level;
	}
}
