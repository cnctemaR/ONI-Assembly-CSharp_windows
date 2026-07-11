using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class DivideByZeroException : ArithmeticException
	{
		public DivideByZeroException()
			: base(Environment.GetResourceString("Attempted to divide by zero."))
		{
			base.SetErrorCode(-2147352558);
		}

		public DivideByZeroException(string message)
			: base(message)
		{
			base.SetErrorCode(-2147352558);
		}

		public DivideByZeroException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.SetErrorCode(-2147352558);
		}

		protected DivideByZeroException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
