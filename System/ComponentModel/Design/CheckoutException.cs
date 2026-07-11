using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.ComponentModel.Design
{
	[Serializable]
	public class CheckoutException : ExternalException
	{
		public CheckoutException()
		{
		}

		public CheckoutException(string message)
			: base(message)
		{
		}

		public CheckoutException(string message, int errorCode)
			: base(message, errorCode)
		{
		}

		public CheckoutException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected CheckoutException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public static readonly CheckoutException Canceled = new CheckoutException("The user canceled the checkout.", -2147467260);
	}
}
