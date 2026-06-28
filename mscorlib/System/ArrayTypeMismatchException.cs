using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class ArrayTypeMismatchException : SystemException
	{
		public ArrayTypeMismatchException()
			: base(Locale.GetText("Source array type cannot be assigned to destination array type."))
		{
			base.HResult = -2146233085;
		}

		public ArrayTypeMismatchException(string message)
			: base(message)
		{
			base.HResult = -2146233085;
		}

		public ArrayTypeMismatchException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146233085;
		}

		protected ArrayTypeMismatchException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private const int Result = -2146233085;
	}
}
