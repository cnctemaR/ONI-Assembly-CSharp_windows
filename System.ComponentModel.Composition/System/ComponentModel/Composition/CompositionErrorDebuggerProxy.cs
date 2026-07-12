using System;
using System.ComponentModel.Composition.Primitives;
using Microsoft.Internal;

namespace System.ComponentModel.Composition
{
	internal class CompositionErrorDebuggerProxy
	{
		public CompositionErrorDebuggerProxy(CompositionError error)
		{
			Requires.NotNull<CompositionError>(error, "error");
			this._error = error;
		}

		public string Description
		{
			get
			{
				return this._error.Description;
			}
		}

		public Exception Exception
		{
			get
			{
				return this._error.Exception;
			}
		}

		public ICompositionElement Element
		{
			get
			{
				return this._error.Element;
			}
		}

		private readonly CompositionError _error;
	}
}
