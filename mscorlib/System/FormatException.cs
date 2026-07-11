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
			: base(Environment.GetResourceString("One of the identified items was in an invalid format."))
		{
			base.SetErrorCode(-2146233033);
		}

		public FormatException(string message)
			: base(message)
		{
			base.SetErrorCode(-2146233033);
		}

		public FormatException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.SetErrorCode(-2146233033);
		}

		protected FormatException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
