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
			: base("Mutex was abandoned")
		{
		}

		public AbandonedMutexException(string message)
			: base(message)
		{
		}

		public AbandonedMutexException(int location, WaitHandle handle)
			: base("Mutex was abandoned")
		{
			this.mutex_index = location;
			this.mutex = handle as Mutex;
		}

		protected AbandonedMutexException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public AbandonedMutexException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public AbandonedMutexException(string message, int location, WaitHandle handle)
			: base(message)
		{
			this.mutex_index = location;
			this.mutex = handle as Mutex;
		}

		public AbandonedMutexException(string message, Exception inner, int location, WaitHandle handle)
			: base(message, inner)
		{
			this.mutex_index = location;
			this.mutex = handle as Mutex;
		}

		public Mutex Mutex
		{
			get
			{
				return this.mutex;
			}
		}

		public int MutexIndex
		{
			get
			{
				return this.mutex_index;
			}
		}

		private Mutex mutex;

		private int mutex_index = -1;
	}
}
