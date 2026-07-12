using System;
using System.Runtime.Serialization;
using System.Threading;

namespace System
{
	[Serializable]
	public class OperationCanceledException : SystemException
	{
		public CancellationToken CancellationToken
		{
			get
			{
				return this._cancellationToken;
			}
			private set
			{
				this._cancellationToken = value;
			}
		}

		public OperationCanceledException()
			: base("The operation was canceled.")
		{
			base.HResult = -2146233029;
		}

		public OperationCanceledException(string message)
			: base(message)
		{
			base.HResult = -2146233029;
		}

		public OperationCanceledException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146233029;
		}

		public OperationCanceledException(CancellationToken token)
			: this()
		{
			this.CancellationToken = token;
		}

		public OperationCanceledException(string message, CancellationToken token)
			: this(message)
		{
			this.CancellationToken = token;
		}

		public OperationCanceledException(string message, Exception innerException, CancellationToken token)
			: this(message, innerException)
		{
			this.CancellationToken = token;
		}

		protected OperationCanceledException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		[NonSerialized]
		private CancellationToken _cancellationToken;
	}
}
