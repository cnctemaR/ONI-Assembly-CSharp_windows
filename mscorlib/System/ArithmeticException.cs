using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class ArithmeticException : SystemException
	{
		public ArithmeticException()
			: base(Environment.GetResourceString("Overflow or underflow in the arithmetic operation."))
		{
			base.SetErrorCode(-2147024362);
		}

		public ArithmeticException(string message)
			: base(message)
		{
			base.SetErrorCode(-2147024362);
		}

		public ArithmeticException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.SetErrorCode(-2147024362);
		}

		protected ArithmeticException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
