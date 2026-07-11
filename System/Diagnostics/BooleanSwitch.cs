using System;

namespace System.Diagnostics
{
	[SwitchLevel(typeof(bool))]
	public class BooleanSwitch : Switch
	{
		public BooleanSwitch(string displayName, string description)
			: base(displayName, description)
		{
		}

		public BooleanSwitch(string displayName, string description, string defaultSwitchValue)
			: base(displayName, description, defaultSwitchValue)
		{
		}

		public bool Enabled
		{
			get
			{
				return base.SwitchSetting != 0;
			}
			set
			{
				base.SwitchSetting = Convert.ToInt32(value);
			}
		}

		protected override void OnValueChanged()
		{
			int num;
			if (int.TryParse(base.Value, out num))
			{
				this.Enabled = num != 0;
			}
			else
			{
				this.Enabled = Convert.ToBoolean(base.Value);
			}
		}
	}
}
