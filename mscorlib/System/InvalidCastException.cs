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
			: base(Locale.GetText("Cannot cast from source type to destination type."))
		{
			base.HResult = -2147467262;
		}

		public InvalidCastException(string message)
			: base(message)
		{
			base.HResult = -2147467262;
		}

		public InvalidCastException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2147467262;
		}

		public InvalidCastException(string message, int errorCode)
			: base(message)
		{
			base.HResult = errorCode;
		}

		protected InvalidCastException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private const int Result = -2147467262;
	}
}
