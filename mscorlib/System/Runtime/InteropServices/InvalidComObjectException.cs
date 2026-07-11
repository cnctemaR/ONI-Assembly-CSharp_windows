using System;
using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public class InvalidComObjectException : SystemException
	{
		public InvalidComObjectException()
			: base(Environment.GetResourceString("Attempt has been made to use a COM object that does not have a backing class factory."))
		{
			base.SetErrorCode(-2146233049);
		}

		public InvalidComObjectException(string message)
			: base(message)
		{
			base.SetErrorCode(-2146233049);
		}

		public InvalidComObjectException(string message, Exception inner)
			: base(message, inner)
		{
			base.SetErrorCode(-2146233049);
		}

		protected InvalidComObjectException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
