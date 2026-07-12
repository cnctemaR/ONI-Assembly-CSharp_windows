using System;

namespace System.Net
{
	internal readonly struct SecurityStatusPal
	{
		public SecurityStatusPal(SecurityStatusPalErrorCode errorCode, Exception exception = null)
		{
			this.ErrorCode = errorCode;
			this.Exception = exception;
		}

		public override string ToString()
		{
			if (this.Exception != null)
			{
				return string.Format("{0}={1}, {2}={3}", new object[] { "ErrorCode", this.ErrorCode, "Exception", this.Exception });
			}
			return string.Format("{0}={1}", "ErrorCode", this.ErrorCode);
		}

		public readonly SecurityStatusPalErrorCode ErrorCode;

		public readonly Exception Exception;
	}
}
