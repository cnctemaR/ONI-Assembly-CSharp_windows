using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class ContextMarshalException : SystemException
	{
		public ContextMarshalException()
			: base(Environment.GetResourceString("Attempted to marshal an object across a context boundary."))
		{
			base.SetErrorCode(-2146233084);
		}

		public ContextMarshalException(string message)
			: base(message)
		{
			base.SetErrorCode(-2146233084);
		}

		public ContextMarshalException(string message, Exception inner)
			: base(message, inner)
		{
			base.SetErrorCode(-2146233084);
		}

		protected ContextMarshalException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
