using System;

namespace System.Diagnostics
{
	public class SourceSwitch : Switch
	{
		public SourceSwitch(string displayName)
			: this(displayName, null)
		{
		}

		public SourceSwitch(string displayName, string defaultSwitchValue)
			: base(displayName, "Source switch.", defaultSwitchValue)
		{
		}

		public SourceLevels Level
		{
			get
			{
				return (SourceLevels)base.SwitchSetting;
			}
			set
			{
				base.SwitchSetting = (int)value;
			}
		}

		public bool ShouldTrace(TraceEventType eventType)
		{
			switch (eventType)
			{
			case TraceEventType.Critical:
				return (this.Level & SourceLevels.Critical) != SourceLevels.Off;
			case TraceEventType.Error:
				return (this.Level & SourceLevels.Error) != SourceLevels.Off;
			default:
				if (eventType != TraceEventType.Verbose)
				{
					if (eventType != TraceEventType.Start && eventType != TraceEventType.Stop && eventType != TraceEventType.Suspend && eventType != TraceEventType.Resume && eventType != TraceEventType.Transfer)
					{
					}
					return (this.Level & SourceLevels.ActivityTracing) != SourceLevels.Off;
				}
				return (this.Level & SourceLevels.Verbose) != SourceLevels.Off;
			case TraceEventType.Warning:
				return (this.Level & SourceLevels.Warning) != SourceLevels.Off;
			case TraceEventType.Information:
				return (this.Level & SourceLevels.Information) != SourceLevels.Off;
			}
		}

		protected override void OnValueChanged()
		{
			base.SwitchSetting = (int)Enum.Parse(typeof(SourceLevels), base.Value, true);
		}

		private const string description = "Source switch.";
	}
}
