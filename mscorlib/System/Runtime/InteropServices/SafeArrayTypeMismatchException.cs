using System;
using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public class SafeArrayTypeMismatchException : SystemException
	{
		public SafeArrayTypeMismatchException()
			: base(Locale.GetText("The incoming SAVEARRAY does not match the expected managed signature"))
		{
			base.HResult = -2146233037;
		}

		public SafeArrayTypeMismatchException(string message)
			: base(message)
		{
			base.HResult = -2146233037;
		}

		public SafeArrayTypeMismatchException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233037;
		}

		protected SafeArrayTypeMismatchException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private const int ErrorCode = -2146233037;
	}
}
