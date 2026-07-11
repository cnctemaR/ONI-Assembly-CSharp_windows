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
			: base(Locale.GetText("Division by zero"))
		{
			base.HResult = -2147352558;
		}

		public DivideByZeroException(string message)
			: base(message)
		{
			base.HResult = -2147352558;
		}

		public DivideByZeroException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2147352558;
		}

		protected DivideByZeroException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private const int Result = -2147352558;
	}
}
