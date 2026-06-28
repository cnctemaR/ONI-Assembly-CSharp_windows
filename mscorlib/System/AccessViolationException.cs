using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class AccessViolationException : SystemException
	{
		public AccessViolationException()
			: base(Locale.GetText("Attempted to read or write protected memory. This is often an indication that other memory has been corrupted."))
		{
			base.HResult = -2147467261;
		}

		public AccessViolationException(string message)
			: base(message)
		{
			base.HResult = -2147467261;
		}

		public AccessViolationException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2147467261;
		}

		protected AccessViolationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private const int Result = -2147467261;
	}
}
