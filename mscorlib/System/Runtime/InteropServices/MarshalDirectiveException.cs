using System;
using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public class MarshalDirectiveException : SystemException
	{
		public MarshalDirectiveException()
			: base(Locale.GetText("Unsupported MarshalAsAttribute found"))
		{
			base.HResult = -2146233035;
		}

		public MarshalDirectiveException(string message)
			: base(message)
		{
			base.HResult = -2146233035;
		}

		public MarshalDirectiveException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233035;
		}

		protected MarshalDirectiveException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private const int ErrorCode = -2146233035;
	}
}
