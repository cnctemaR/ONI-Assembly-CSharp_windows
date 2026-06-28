using System;

namespace System.Configuration
{
	public class SettingsLoadedEventArgs : EventArgs
	{
		public SettingsLoadedEventArgs(SettingsProvider provider)
		{
			this.provider = provider;
		}

		public SettingsProvider Provider
		{
			get
			{
				return this.provider;
			}
		}

		private SettingsProvider provider;
	}
}
