using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security;
using Microsoft.Internal.Runtime.Serialization;

namespace System.ComponentModel.Composition.Primitives
{
	[DebuggerTypeProxy(typeof(ComposablePartExceptionDebuggerProxy))]
	[DebuggerDisplay("{Message}")]
	[Serializable]
	public class ComposablePartException : Exception
	{
		public ComposablePartException()
			: this(null, null, null)
		{
		}

		public ComposablePartException(string message)
			: this(message, null, null)
		{
		}

		public ComposablePartException(string message, ICompositionElement element)
			: this(message, element, null)
		{
		}

		public ComposablePartException(string message, Exception innerException)
			: this(message, null, innerException)
		{
		}

		public ComposablePartException(string message, ICompositionElement element, Exception innerException)
			: base(message, innerException)
		{
			this._element = element;
		}

		[SecuritySafeCritical]
		protected ComposablePartException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this._element = info.GetValue<ICompositionElement>("Element");
		}

		public ICompositionElement Element
		{
			get
			{
				return this._element;
			}
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Element", this._element.ToSerializableElement());
		}

		private readonly ICompositionElement _element;
	}
}
