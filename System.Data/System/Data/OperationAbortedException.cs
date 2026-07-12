using System;
using System.Runtime.Serialization;
using Unity;

namespace System.Data
{
	[Serializable]
	public sealed class OperationAbortedException : SystemException
	{
		private OperationAbortedException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146232010;
		}

		private OperationAbortedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		internal static OperationAbortedException Aborted(Exception inner)
		{
			OperationAbortedException ex;
			if (inner == null)
			{
				ex = new OperationAbortedException(SR.GetString("Operation aborted."), null);
			}
			else
			{
				ex = new OperationAbortedException(SR.GetString("Operation aborted due to an exception (see InnerException for details)."), inner);
			}
			return ex;
		}

		internal OperationAbortedException()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
