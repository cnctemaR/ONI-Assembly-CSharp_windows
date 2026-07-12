using System;

namespace System.Drawing.Printing
{
	public class QueryPageSettingsEventArgs : PrintEventArgs
	{
		public QueryPageSettingsEventArgs(PageSettings pageSettings)
		{
			this._pageSettings = pageSettings;
		}

		public PageSettings PageSettings
		{
			get
			{
				this.PageSettingsChanged = true;
				return this._pageSettings;
			}
			set
			{
				if (value == null)
				{
					value = new PageSettings();
				}
				this._pageSettings = value;
				this.PageSettingsChanged = true;
			}
		}

		private PageSettings _pageSettings;

		internal bool PageSettingsChanged;
	}
}
