using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class MethodAccessException : MemberAccessException
	{
		public MethodAccessException()
			: base(Locale.GetText("Attempt to access a private/protected method failed."))
		{
			base.HResult = -2146233072;
		}

		public MethodAccessException(string message)
			: base(message)
		{
			base.HResult = -2146233072;
		}

		protected MethodAccessException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public MethodAccessException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233072;
		}

		private const int Result = -2146233072;
	}
}
