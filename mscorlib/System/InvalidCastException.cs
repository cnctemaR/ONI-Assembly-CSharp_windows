using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class InvalidCastException : SystemException
	{
		public InvalidCastException()
			: base(Environment.GetResourceString("Specified cast is not valid."))
		{
			base.SetErrorCode(-2147467262);
		}

		public InvalidCastException(string message)
			: base(message)
		{
			base.SetErrorCode(-2147467262);
		}

		public InvalidCastException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.SetErrorCode(-2147467262);
		}

		protected InvalidCastException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public InvalidCastException(string message, int errorCode)
			: base(message)
		{
			base.SetErrorCode(errorCode);
		}
	}
}
