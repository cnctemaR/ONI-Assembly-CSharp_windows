using System;
using System.ComponentModel;

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

		private PingReply reply;
	}
}
