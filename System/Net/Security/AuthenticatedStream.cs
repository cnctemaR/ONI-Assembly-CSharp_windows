using System;
using System.IO;

namespace System.Net.Security
{
	public abstract class AuthenticatedStream : Stream
	{
		protected AuthenticatedStream(Stream innerStream, bool leaveInnerStreamOpen)
		{
			if (innerStream == null || innerStream == Stream.Null)
			{
				throw new ArgumentNullException("innerStream");
			}
			if (!innerStream.CanRead || !innerStream.CanWrite)
			{
				throw new ArgumentException(global::SR.GetString("The stream has to be read/write."), "innerStream");
			}
			this._InnerStream = innerStream;
			this._LeaveStreamOpen = leaveInnerStreamOpen;
		}

		public bool LeaveInnerStreamOpen
		{
			get
			{
				return this._LeaveStreamOpen;
			}
		}

		protected Stream InnerStream
		{
			get
			{
				return this._InnerStream;
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (this._LeaveStreamOpen)
					{
						this._InnerStream.Flush();
					}
					else
					{
						this._InnerStream.Close();
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public abstract bool IsAuthenticated { get; }

		public abstract bool IsMutuallyAuthenticated { get; }

		public abstract bool IsEncrypted { get; }

		public abstract bool IsSigned { get; }

		public abstract bool IsServer { get; }

		private Stream _InnerStream;

		private bool _LeaveStreamOpen;
	}
}
