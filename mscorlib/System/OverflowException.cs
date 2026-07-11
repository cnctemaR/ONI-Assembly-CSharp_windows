using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class OverflowException : ArithmeticException
	{
		public OverflowException()
			: base(Environment.GetResourceString("Arithmetic operation resulted in an overflow."))
		{
			base.SetErrorCode(-2146233066);
		}

		public OverflowException(string message)
			: base(message)
		{
			base.SetErrorCode(-2146233066);
		}

		public OverflowException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.SetErrorCode(-2146233066);
		}

		protected OverflowException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
