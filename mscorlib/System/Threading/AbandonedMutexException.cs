using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Threading
{
	[ComVisible(false)]
	[Serializable]
	public class AbandonedMutexException : SystemException
	{
		public AbandonedMutexException()
			: base(Environment.GetResourceString("The wait completed due to an abandoned mutex."))
		{
			base.SetErrorCode(-2146233043);
		}

		public AbandonedMutexException(string message)
			: base(message)
		{
			base.SetErrorCode(-2146233043);
		}

		public AbandonedMutexException(string message, Exception inner)
			: base(message, inner)
		{
			base.SetErrorCode(-2146233043);
		}

		public AbandonedMutexException(int location, WaitHandle handle)
			: base(Environment.GetResourceString("The wait completed due to an abandoned mutex."))
		{
			base.SetErrorCode(-2146233043);
			this.SetupException(location, handle);
		}

		public AbandonedMutexException(string message, int location, WaitHandle handle)
			: base(message)
		{
			base.SetErrorCode(-2146233043);
			this.SetupException(location, handle);
		}

		public AbandonedMutexException(string message, Exception inner, int location, WaitHandle handle)
			: base(message, inner)
		{
			base.SetErrorCode(-2146233043);
			this.SetupException(location, handle);
		}

		private void SetupException(int location, WaitHandle handle)
		{
			this.m_MutexIndex = location;
			if (handle != null)
			{
				this.m_Mutex = handle as Mutex;
			}
		}

		protected AbandonedMutexException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public Mutex Mutex
		{
			get
			{
				return this.m_Mutex;
			}
		}

		public int MutexIndex
		{
			get
			{
				return this.m_MutexIndex;
			}
		}

		private int m_MutexIndex = -1;

		private Mutex m_Mutex;
	}
}
