using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	[Serializable]
	public class SerializationException : SystemException
	{
		public SerializationException()
			: base(SerializationException._nullMessage)
		{
			base.SetErrorCode(-2146233076);
		}

		public SerializationException(string message)
			: base(message)
		{
			base.SetErrorCode(-2146233076);
		}

		public SerializationException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.SetErrorCode(-2146233076);
		}

		protected SerializationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private static string _nullMessage = Environment.GetResourceString("Serialization error.");
	}
}
