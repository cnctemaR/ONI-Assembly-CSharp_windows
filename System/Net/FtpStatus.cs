using System;

namespace System.Net
{
	internal class FtpStatus
	{
		public FtpStatus(FtpStatusCode statusCode, string statusDescription)
		{
			this.statusCode = statusCode;
			this.statusDescription = statusDescription;
		}

		public FtpStatusCode StatusCode
		{
			get
			{
				return this.statusCode;
			}
		}

		public string StatusDescription
		{
			get
			{
				return this.statusDescription;
			}
		}

		private readonly FtpStatusCode statusCode;

		private readonly string statusDescription;
	}
}
