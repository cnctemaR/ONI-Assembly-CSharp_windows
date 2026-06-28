using System;
using System.Configuration.Internal;

namespace System.Configuration
{
	internal class MachineConfigurationHost : InternalConfigurationHost
	{
		public override void Init(IInternalConfigRoot root, params object[] hostInitParams)
		{
			this.map = (ConfigurationFileMap)hostInitParams[0];
		}

		public override string GetStreamName(string configPath)
		{
			return this.map.MachineConfigFilename;
		}

		public override void InitForConfiguration(ref string locationSubPath, out string configPath, out string locationConfigPath, IInternalConfigRoot root, params object[] hostInitConfigurationParams)
		{
			this.map = (ConfigurationFileMap)hostInitConfigurationParams[0];
			locationSubPath = null;
			configPath = null;
			locationConfigPath = null;
		}

		public override bool IsDefinitionAllowed(string configPath, ConfigurationAllowDefinition allowDefinition, ConfigurationAllowExeDefinition allowExeDefinition)
		{
			return true;
		}

		private ConfigurationFileMap map;
	}
}
