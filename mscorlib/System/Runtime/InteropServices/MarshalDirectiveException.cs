using System;
using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public class MarshalDirectiveException : SystemException
	{
		public MarshalDirectiveException()
			: base(Environment.GetResourceString("Marshaling directives are invalid."))
		{
			base.SetErrorCode(-2146233035);
		}

		public MarshalDirectiveException(string message)
			: base(message)
		{
			base.SetErrorCode(-2146233035);
		}

		public MarshalDirectiveException(string message, Exception inner)
			: base(message, inner)
		{
			base.SetErrorCode(-2146233035);
		}

		protected MarshalDirectiveException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
