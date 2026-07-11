using System;
using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public class ExternalException : SystemException
	{
		public ExternalException()
			: base(Locale.GetText("External exception"))
		{
			base.HResult = -2147467259;
		}

		public ExternalException(string message)
			: base(message)
		{
			base.HResult = -2147467259;
		}

		protected ExternalException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public ExternalException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2147467259;
		}

		public ExternalException(string message, int errorCode)
			: base(message)
		{
			base.HResult = errorCode;
		}

		public virtual int ErrorCode
		{
			get
			{
				return base.HResult;
			}
		}
	}
}
