using System;
using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public class InvalidComObjectException : SystemException
	{
		public InvalidComObjectException()
			: base(Locale.GetText("Invalid COM object is used"))
		{
			base.HResult = -2146233049;
		}

		public InvalidComObjectException(string message)
			: base(message)
		{
			base.HResult = -2146233049;
		}

		public InvalidComObjectException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233049;
		}

		protected InvalidComObjectException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private const int ErrorCode = -2146233049;
	}
}
