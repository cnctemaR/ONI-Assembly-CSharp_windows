using System;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Globalization;

namespace System.ComponentModel.Composition
{
	[DebuggerTypeProxy(typeof(CompositionErrorDebuggerProxy))]
	[Serializable]
	public class CompositionError
	{
		public CompositionError(string message)
			: this(CompositionErrorId.Unknown, message, null, null)
		{
		}

		public CompositionError(string message, ICompositionElement element)
			: this(CompositionErrorId.Unknown, message, element, null)
		{
		}

		public CompositionError(string message, Exception exception)
			: this(CompositionErrorId.Unknown, message, null, exception)
		{
		}

		public CompositionError(string message, ICompositionElement element, Exception exception)
			: this(CompositionErrorId.Unknown, message, element, exception)
		{
		}

		internal CompositionError(CompositionErrorId id, string description, ICompositionElement element, Exception exception)
		{
			this._id = id;
			this._description = description ?? string.Empty;
			this._element = element;
			this._exception = exception;
		}

		public ICompositionElement Element
		{
			get
			{
				return this._element;
			}
		}

		public string Description
		{
			get
			{
				return this._description;
			}
		}

		public Exception Exception
		{
			get
			{
				return this._exception;
			}
		}

		internal CompositionErrorId Id
		{
			get
			{
				return this._id;
			}
		}

		internal Exception InnerException
		{
			get
			{
				return this.Exception;
			}
		}

		public override string ToString()
		{
			return this.Description;
		}

		internal static CompositionError Create(CompositionErrorId id, string format, params object[] parameters)
		{
			return CompositionError.Create(id, null, null, format, parameters);
		}

		internal static CompositionError Create(CompositionErrorId id, ICompositionElement element, string format, params object[] parameters)
		{
			return CompositionError.Create(id, element, null, format, parameters);
		}

		internal static CompositionError Create(CompositionErrorId id, ICompositionElement element, Exception exception, string format, params object[] parameters)
		{
			return new CompositionError(id, string.Format(CultureInfo.CurrentCulture, format, parameters), element, exception);
		}

		private readonly CompositionErrorId _id;

		private readonly string _description;

		private readonly Exception _exception;

		private readonly ICompositionElement _element;
	}
}
