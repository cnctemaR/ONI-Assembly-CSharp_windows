using System;
using System.IO;

namespace System.Net.Security
{
	public abstract class AuthenticatedStream : Stream
	{
		protected AuthenticatedStream(Stream innerStream, bool leaveInnerStreamOpen)
		{
			this.innerStream = innerStream;
			this.leaveStreamOpen = leaveInnerStreamOpen;
		}

		protected Stream InnerStream
		{
			get
			{
				return this.innerStream;
			}
		}

		public abstract bool IsAuthenticated { get; }

		public abstract bool IsEncrypted { get; }

		public abstract bool IsMutuallyAuthenticated { get; }

		public abstract bool IsServer { get; }

		public abstract bool IsSigned { get; }

		public bool LeaveInnerStreamOpen
		{
			get
			{
				return this.leaveStreamOpen;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.innerStream != null)
			{
				if (!this.leaveStreamOpen)
				{
					this.innerStream.Close();
				}
				this.innerStream = null;
			}
		}

		private Stream innerStream;

		private bool leaveStreamOpen;
	}
}
