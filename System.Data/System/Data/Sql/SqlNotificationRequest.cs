using System;
using System.Data.Common;

namespace System.Data.Sql
{
	public sealed class SqlNotificationRequest
	{
		public SqlNotificationRequest()
			: this(null, null, 0)
		{
		}

		public SqlNotificationRequest(string userData, string options, int timeout)
		{
			this.UserData = userData;
			this.Timeout = timeout;
			this.Options = options;
		}

		public string Options
		{
			get
			{
				return this._options;
			}
			set
			{
				if (value != null && 65535 < value.Length)
				{
					throw ADP.ArgumentOutOfRange(string.Empty, "Options");
				}
				this._options = value;
			}
		}

		public int Timeout
		{
			get
			{
				return this._timeout;
			}
			set
			{
				if (0 > value)
				{
					throw ADP.ArgumentOutOfRange(string.Empty, "Timeout");
				}
				this._timeout = value;
			}
		}

		public string UserData
		{
			get
			{
				return this._userData;
			}
			set
			{
				if (value != null && 65535 < value.Length)
				{
					throw ADP.ArgumentOutOfRange(string.Empty, "UserData");
				}
				this._userData = value;
			}
		}

		private string _userData;

		private string _options;

		private int _timeout;
	}
}
