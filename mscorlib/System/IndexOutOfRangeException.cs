using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public sealed class IndexOutOfRangeException : SystemException
	{
		public IndexOutOfRangeException()
			: base(Locale.GetText("Array index is out of range."))
		{
		}

		public IndexOutOfRangeException(string message)
			: base(message)
		{
		}

		public IndexOutOfRangeException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		internal IndexOutOfRangeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
