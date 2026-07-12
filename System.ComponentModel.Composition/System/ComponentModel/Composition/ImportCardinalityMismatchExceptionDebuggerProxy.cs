using System;
using Microsoft.Internal;

namespace System.ComponentModel.Composition
{
	internal class ImportCardinalityMismatchExceptionDebuggerProxy
	{
		public ImportCardinalityMismatchExceptionDebuggerProxy(ImportCardinalityMismatchException exception)
		{
			Requires.NotNull<ImportCardinalityMismatchException>(exception, "exception");
			this._exception = exception;
		}

		public Exception InnerException
		{
			get
			{
				return this._exception.InnerException;
			}
		}

		public string Message
		{
			get
			{
				return this._exception.Message;
			}
		}

		private readonly ImportCardinalityMismatchException _exception;
	}
}
