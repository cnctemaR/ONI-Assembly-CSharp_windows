using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Threading
{
	[ComVisible(false)]
	[Serializable]
	public class WaitHandleCannotBeOpenedException : ApplicationException
	{
		public WaitHandleCannotBeOpenedException()
			: base(Locale.GetText("Named handle doesn't exists."))
		{
		}

		public WaitHandleCannotBeOpenedException(string message)
			: base(message)
		{
		}

		public WaitHandleCannotBeOpenedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected WaitHandleCannotBeOpenedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
