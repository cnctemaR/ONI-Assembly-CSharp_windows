using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class FormatException : SystemException
	{
		public FormatException()
			: base(Locale.GetText("Invalid format."))
		{
			base.HResult = -2146233033;
		}

		public FormatException(string message)
			: base(message)
		{
			base.HResult = -2146233033;
		}

		public FormatException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146233033;
		}

		protected FormatException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private const int Result = -2146233033;
	}
}
