using System;

namespace System.Data.Sql
{
	public sealed class SqlNotificationRequest
	{
		public SqlNotificationRequest()
		{
		}

		public SqlNotificationRequest(string userData, string options, int timeout)
		{
			this.UserData = userData;
			this.Options = options;
			this.Timeout = timeout;
		}

		public string UserData
		{
			get
			{
				return this.userData;
			}
			set
			{
				if (value != null && value.Length > 65535)
				{
					throw new ArgumentOutOfRangeException("UserData");
				}
				this.userData = value;
			}
		}

		public string Options
		{
			get
			{
				return this.options;
			}
			set
			{
				if (value != null && value.Length > 65535)
				{
					throw new ArgumentOutOfRangeException("Service");
				}
				this.options = value;
			}
		}

		public int Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("Timeout");
				}
				this.timeout = value;
			}
		}

		private string userData;

		private string options;

		private int timeout;
	}
}
