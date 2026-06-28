using System;
using System.ComponentModel;

namespace System.Configuration
{
	public class SettingChangingEventArgs : global::System.ComponentModel.CancelEventArgs
	{
		public SettingChangingEventArgs(string settingName, string settingClass, string settingKey, object newValue, bool cancel)
			: base(cancel)
		{
			this.settingName = settingName;
			this.settingClass = settingClass;
			this.settingKey = settingKey;
			this.newValue = newValue;
		}

		public string SettingName
		{
			get
			{
				return this.settingName;
			}
		}

		public string SettingClass
		{
			get
			{
				return this.settingClass;
			}
		}

		public string SettingKey
		{
			get
			{
				return this.settingKey;
			}
		}

		public object NewValue
		{
			get
			{
				return this.newValue;
			}
		}

		private string settingName;

		private string settingClass;

		private string settingKey;

		private object newValue;
	}
}
