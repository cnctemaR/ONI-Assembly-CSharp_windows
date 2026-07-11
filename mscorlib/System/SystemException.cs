using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class SystemException : Exception
	{
		public SystemException()
			: base(Locale.GetText("A system exception has occurred."))
		{
			base.HResult = -2146233087;
		}

		public SystemException(string message)
			: base(message)
		{
			base.HResult = -2146233087;
		}

		protected SystemException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public SystemException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146233087;
		}

		private const int Result = -2146233087;
	}
}
