using System;

namespace System.Data.SqlClient.SNI
{
	internal class SNIError
	{
		public SNIError(SNIProviders provider, uint nativeError, uint sniErrorCode, string errorMessage)
		{
			this.lineNumber = 0U;
			this.function = string.Empty;
			this.provider = provider;
			this.nativeError = nativeError;
			this.sniError = sniErrorCode;
			this.errorMessage = errorMessage;
			this.exception = null;
		}

		public SNIError(SNIProviders provider, uint sniErrorCode, Exception sniException)
		{
			this.lineNumber = 0U;
			this.function = string.Empty;
			this.provider = provider;
			this.nativeError = 0U;
			this.sniError = sniErrorCode;
			this.errorMessage = string.Empty;
			this.exception = sniException;
		}

		public readonly SNIProviders provider;

		public readonly string errorMessage;

		public readonly uint nativeError;

		public readonly uint sniError;

		public readonly string function;

		public readonly uint lineNumber;

		public readonly Exception exception;
	}
}
