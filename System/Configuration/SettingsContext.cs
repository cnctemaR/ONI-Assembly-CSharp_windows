using System;
using System.Collections;

namespace System.Configuration
{
	[Serializable]
	public class SettingsContext : Hashtable
	{
		internal ApplicationSettingsBase CurrentSettings
		{
			get
			{
				return this.current;
			}
			set
			{
				this.current = value;
			}
		}

		[NonSerialized]
		private ApplicationSettingsBase current;
	}
}
