using System;
using System.Security.Permissions;

namespace System.Diagnostics
{
	[SwitchLevel(typeof(TraceLevel))]
	public class TraceSwitch : Switch
	{
		public TraceSwitch(string displayName, string description)
			: base(displayName, description)
		{
		}

		public TraceSwitch(string displayName, string description, string defaultSwitchValue)
			: base(displayName, description, defaultSwitchValue)
		{
		}

		public TraceLevel Level
		{
			get
			{
				return (TraceLevel)base.SwitchSetting;
			}
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			set
			{
				if (value < TraceLevel.Off || value > TraceLevel.Verbose)
				{
					throw new ArgumentException(global::SR.GetString("The Level must be set to a value in the enumeration TraceLevel."));
				}
				base.SwitchSetting = (int)value;
			}
		}

		public bool TraceError
		{
			get
			{
				return this.Level >= TraceLevel.Error;
			}
		}

		public bool TraceWarning
		{
			get
			{
				return this.Level >= TraceLevel.Warning;
			}
		}

		public bool TraceInfo
		{
			get
			{
				return this.Level >= TraceLevel.Info;
			}
		}

		public bool TraceVerbose
		{
			get
			{
				return this.Level == TraceLevel.Verbose;
			}
		}

		protected override void OnSwitchSettingChanged()
		{
			int switchSetting = base.SwitchSetting;
			if (switchSetting < 0)
			{
				base.SwitchSetting = 0;
				return;
			}
			if (switchSetting > 4)
			{
				base.SwitchSetting = 4;
			}
		}

		protected override void OnValueChanged()
		{
			base.SwitchSetting = (int)Enum.Parse(typeof(TraceLevel), base.Value, true);
		}
	}
}
