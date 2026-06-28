using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class NullReferenceException : SystemException
	{
		public NullReferenceException()
			: base(Locale.GetText("A null value was found where an object instance was required."))
		{
			base.HResult = -2147467261;
		}

		public NullReferenceException(string message)
			: base(message)
		{
			base.HResult = -2147467261;
		}

		public NullReferenceException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2147467261;
		}

		protected NullReferenceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private const int Result = -2147467261;
	}
}
