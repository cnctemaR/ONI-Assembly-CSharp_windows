using System;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Primitives
{
	internal class ComposablePartExceptionDebuggerProxy
	{
		public ComposablePartExceptionDebuggerProxy(ComposablePartException exception)
		{
			Requires.NotNull<ComposablePartException>(exception, "exception");
			this._exception = exception;
		}

		public ICompositionElement Element
		{
			get
			{
				return this._exception.Element;
			}
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

		private readonly ComposablePartException _exception;
	}
}
