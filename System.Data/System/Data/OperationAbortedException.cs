using System;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public sealed class OperationAbortedException : SystemException
	{
		internal OperationAbortedException()
			: base(Locale.GetText("An OperationAbortedException has occurred."))
		{
		}

		internal OperationAbortedException(string s)
			: base(s)
		{
		}

		internal OperationAbortedException(string s, Exception innerException)
			: base(s, innerException)
		{
		}

		internal OperationAbortedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
