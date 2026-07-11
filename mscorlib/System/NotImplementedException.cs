using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class NotImplementedException : SystemException
	{
		public NotImplementedException()
			: base(Environment.GetResourceString("The method or operation is not implemented."))
		{
			base.SetErrorCode(-2147467263);
		}

		public NotImplementedException(string message)
			: base(message)
		{
			base.SetErrorCode(-2147467263);
		}

		public NotImplementedException(string message, Exception inner)
			: base(message, inner)
		{
			base.SetErrorCode(-2147467263);
		}

		protected NotImplementedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
