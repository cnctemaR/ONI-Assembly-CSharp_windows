using System;

namespace System.Configuration.Internal
{
	public sealed class InternalConfigEventArgs : EventArgs
	{
		public InternalConfigEventArgs(string configPath)
		{
			this.configPath = configPath;
		}

		public string ConfigPath
		{
			get
			{
				return this.configPath;
			}
			set
			{
				this.configPath = value;
			}
		}

		private string configPath;
	}
}
