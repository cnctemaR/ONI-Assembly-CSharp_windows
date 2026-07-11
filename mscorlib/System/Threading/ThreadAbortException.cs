using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Threading
{
	[ComVisible(true)]
	[Serializable]
	public sealed class ThreadAbortException : SystemException
	{
		private ThreadAbortException()
			: base("Thread was being aborted")
		{
			base.HResult = -2146233040;
		}

		private ThreadAbortException(SerializationInfo info, StreamingContext sc)
			: base(info, sc)
		{
		}

		public object ExceptionState
		{
			get
			{
				return Thread.CurrentThread.GetAbortExceptionState();
			}
		}
	}
}
