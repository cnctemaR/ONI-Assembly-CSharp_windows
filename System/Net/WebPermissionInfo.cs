using System;
using System.Text.RegularExpressions;

namespace System.Net
{
	internal class WebPermissionInfo
	{
		public WebPermissionInfo(WebPermissionInfoType type, string info)
		{
			this._type = type;
			this._info = info;
		}

		public WebPermissionInfo(global::System.Text.RegularExpressions.Regex regex)
		{
			this._type = WebPermissionInfoType.InfoRegex;
			this._info = regex;
		}

		public string Info
		{
			get
			{
				if (this._type == WebPermissionInfoType.InfoRegex)
				{
					return null;
				}
				return (string)this._info;
			}
		}

		private WebPermissionInfoType _type;

		private object _info;
	}
}
