using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Messaging
{
	[ComVisible(true)]
	public class InternalMessageWrapper
	{
		public InternalMessageWrapper(IMessage msg)
		{
			this.WrappedMessage = msg;
		}

		protected IMessage WrappedMessage;
	}
}
