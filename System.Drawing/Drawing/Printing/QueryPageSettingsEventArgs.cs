using System;

namespace System.Drawing.Printing
{
	public class QueryPageSettingsEventArgs : PrintEventArgs
	{
		public QueryPageSettingsEventArgs(PageSettings pageSettings)
		{
			this.pageSettings = pageSettings;
		}

		public PageSettings PageSettings
		{
			get
			{
				return this.pageSettings;
			}
			set
			{
				this.pageSettings = value;
			}
		}

		private PageSettings pageSettings;
	}
}
