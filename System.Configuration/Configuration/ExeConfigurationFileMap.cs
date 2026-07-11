using System;

namespace System.Configuration
{
	public sealed class ExeConfigurationFileMap : ConfigurationFileMap
	{
		public ExeConfigurationFileMap()
		{
			this.exeConfigFilename = "";
			this.localUserConfigFilename = "";
			this.roamingUserConfigFilename = "";
		}

		public string ExeConfigFilename
		{
			get
			{
				return this.exeConfigFilename;
			}
			set
			{
				this.exeConfigFilename = value;
			}
		}

		public string LocalUserConfigFilename
		{
			get
			{
				return this.localUserConfigFilename;
			}
			set
			{
				this.localUserConfigFilename = value;
			}
		}

		public string RoamingUserConfigFilename
		{
			get
			{
				return this.roamingUserConfigFilename;
			}
			set
			{
				this.roamingUserConfigFilename = value;
			}
		}

		public override object Clone()
		{
			return new ExeConfigurationFileMap
			{
				exeConfigFilename = this.exeConfigFilename,
				localUserConfigFilename = this.localUserConfigFilename,
				roamingUserConfigFilename = this.roamingUserConfigFilename,
				MachineConfigFilename = base.MachineConfigFilename
			};
		}

		private string exeConfigFilename;

		private string localUserConfigFilename;

		private string roamingUserConfigFilename;
	}
}
