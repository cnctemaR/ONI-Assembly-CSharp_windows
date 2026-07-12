using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Mono.Net.Security
{
	internal abstract class AsyncProtocolRequest
	{
		public MobileAuthenticatedStream Parent { get; }

		public bool RunSynchronously { get; }

		public int ID
		{
			get
			{
				return ++AsyncProtocolRequest.next_id;
			}
		}

		public string Name
		{
			get
			{
				return base.GetType().Name;
			}
		}

		public int UserResult { get; protected set; }

		public AsyncProtocolRequest(MobileAuthenticatedStream parent, bool sync)
		{
			this.Parent = parent;
			this.RunSynchronously = sync;
		}

		[Conditional("MONO_TLS_DEBUG")]
		protected void Debug(string message, params object[] args)
		{
		}

		internal void RequestRead(int size)
		{
			object obj = this.locker;
			lock (obj)
			{
				this.RequestedSize += size;
			}
		}

		internal void RequestWrite()
		{
			this.WriteRequested = 1;
		}

		internal async Task<AsyncProtocolResult> StartOperation(CancellationToken cancellationToken)
		{
			if (Interlocked.CompareExchange(ref this.Started, 1, 0) != 0)
			{
				throw new InvalidOperationException();
			}
			AsyncProtocolResult asyncProtocolResult;
			try
			{
				await this.ProcessOperation(cancellationToken).ConfigureAwait(false);
				asyncProtocolResult = new AsyncProtocolResult(this.UserResult);
			}
			catch (Exception ex)
			{
				asyncProtocolResult = new AsyncProtocolResult(this.Parent.SetException(ex));
			}
			return asyncProtocolResult;
		}

		private async Task ProcessOperation(CancellationToken cancellationToken)
		{
			AsyncOperationStatus status = AsyncOperationStatus.Initialize;
			while (status != AsyncOperationStatus.Complete)
			{
				cancellationToken.ThrowIfCancellationRequested();
				int? num = await this.InnerRead(cancellationToken).ConfigureAwait(false);
				if (num != null)
				{
					int? num2 = num;
					int num3 = 0;
					if ((num2.GetValueOrDefault() == num3) & (num2 != null))
					{
						status = AsyncOperationStatus.ReadDone;
					}
					else
					{
						num2 = num;
						num3 = 0;
						if ((num2.GetValueOrDefault() < num3) & (num2 != null))
						{
							throw new IOException("Remote prematurely closed connection.");
						}
					}
				}
				if (status <= AsyncOperationStatus.ReadDone)
				{
					AsyncOperationStatus newStatus;
					try
					{
						newStatus = this.Run(status);
						goto IL_011C;
					}
					catch (Exception ex)
					{
						throw MobileAuthenticatedStream.GetSSPIException(ex);
					}
					goto IL_0116;
					IL_011C:
					if (Interlocked.Exchange(ref this.WriteRequested, 0) != 0)
					{
						await this.Parent.InnerWrite(this.RunSynchronously, cancellationToken).ConfigureAwait(false);
					}
					status = newStatus;
					continue;
				}
				IL_0116:
				throw new InvalidOperationException();
			}
		}

		private async Task<int?> InnerRead(CancellationToken cancellationToken)
		{
			int? totalRead = null;
			int num2;
			for (int requestedSize = Interlocked.Exchange(ref this.RequestedSize, 0); requestedSize > 0; requestedSize += num2)
			{
				int num = await this.Parent.InnerRead(this.RunSynchronously, requestedSize, cancellationToken).ConfigureAwait(false);
				if (num <= 0)
				{
					return new int?(num);
				}
				if (num > requestedSize)
				{
					throw new InvalidOperationException();
				}
				totalRead += num;
				requestedSize -= num;
				num2 = Interlocked.Exchange(ref this.RequestedSize, 0);
			}
			return totalRead;
		}

		protected abstract AsyncOperationStatus Run(AsyncOperationStatus status);

		public override string ToString()
		{
			return string.Format("[{0}]", this.Name);
		}

		private int Started;

		private int RequestedSize;

		private int WriteRequested;

		private readonly object locker = new object();

		private static int next_id;
	}
}
