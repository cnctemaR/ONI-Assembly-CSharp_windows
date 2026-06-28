using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Runtime.Remoting
{
	[ComVisible(true)]
	[Serializable]
	public class RemotingException : SystemException
	{
		public RemotingException()
		{
		}

		public RemotingException(string message)
			: base(message)
		{
		}

		protected RemotingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public RemotingException(string message, Exception InnerException)
			: base(message, InnerException)
		{
		}
	}
}
