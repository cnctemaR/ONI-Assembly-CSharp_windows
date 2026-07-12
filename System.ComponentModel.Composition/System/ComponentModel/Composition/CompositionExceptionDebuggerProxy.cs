using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition
{
	internal class CompositionExceptionDebuggerProxy
	{
		public CompositionExceptionDebuggerProxy(CompositionException exception)
		{
			Requires.NotNull<CompositionException>(exception, "exception");
			this._exception = exception;
		}

		public ReadOnlyCollection<Exception> Exceptions
		{
			get
			{
				List<Exception> list = new List<Exception>();
				foreach (CompositionError compositionError in this._exception.Errors)
				{
					if (compositionError.Exception != null)
					{
						list.Add(compositionError.Exception);
					}
				}
				return list.ToReadOnlyCollection<Exception>();
			}
		}

		public string Message
		{
			get
			{
				return this._exception.Message;
			}
		}

		public ReadOnlyCollection<Exception> RootCauses
		{
			get
			{
				List<Exception> list = new List<Exception>();
				foreach (CompositionError compositionError in this._exception.Errors)
				{
					if (compositionError.Exception != null)
					{
						CompositionException ex = compositionError.Exception as CompositionException;
						if (ex != null)
						{
							CompositionExceptionDebuggerProxy compositionExceptionDebuggerProxy = new CompositionExceptionDebuggerProxy(ex);
							if (compositionExceptionDebuggerProxy.RootCauses.Count > 0)
							{
								list.AddRange(compositionExceptionDebuggerProxy.RootCauses);
								continue;
							}
						}
						list.Add(compositionError.Exception);
					}
				}
				return list.ToReadOnlyCollection<Exception>();
			}
		}

		private readonly CompositionException _exception;
	}
}
