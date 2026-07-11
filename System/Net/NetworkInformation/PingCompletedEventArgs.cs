using System;
using System.ComponentModel;
using Unity;

namespace System.Net.NetworkInformation
{
	public class PingCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal PingCompletedEventArgs(Exception ex, bool cancelled, object userState, PingReply reply)
			: base(ex, cancelled, userState)
		{
			this.reply = reply;
		}

		public PingReply Reply
		{
			get
			{
				return this.reply;
			}
		}

		internal PingCompletedEventArgs()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private PingReply reply;
	}
}
