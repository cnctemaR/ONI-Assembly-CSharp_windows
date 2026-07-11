using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Obsolete("this type is obsoleted in 2.0 profile")]
	[ComVisible(true)]
	[Serializable]
	public class ContextMarshalException : SystemException
	{
		public ContextMarshalException()
			: base(Locale.GetText("Attempt to marshal and object across a context failed."))
		{
			base.HResult = -2146233084;
		}

		public ContextMarshalException(string message)
			: base(message)
		{
			base.HResult = -2146233084;
		}

		protected ContextMarshalException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public ContextMarshalException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233084;
		}

		private const int Result = -2146233084;
	}
}
