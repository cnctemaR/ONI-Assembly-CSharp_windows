using System;
using System.Threading;

namespace System.Linq.Parallel
{
	internal class CancellationState
	{
		internal CancellationToken MergedCancellationToken
		{
			get
			{
				if (this.MergedCancellationTokenSource != null)
				{
					return this.MergedCancellationTokenSource.Token;
				}
				return new CancellationToken(false);
			}
		}

		internal CancellationState(CancellationToken externalCancellationToken)
		{
			this.ExternalCancellationToken = externalCancellationToken;
			this.TopLevelDisposedFlag = new Shared<bool>(false);
		}

		internal static void ThrowIfCanceled(CancellationToken token)
		{
			if (token.IsCancellationRequested)
			{
				throw new OperationCanceledException(token);
			}
		}

		internal static void ThrowWithStandardMessageIfCanceled(CancellationToken externalCancellationToken)
		{
			if (externalCancellationToken.IsCancellationRequested)
			{
				throw new OperationCanceledException("The query has been canceled via the token supplied to WithCancellation.", externalCancellationToken);
			}
		}

		internal CancellationTokenSource InternalCancellationTokenSource;

		internal CancellationToken ExternalCancellationToken;

		internal CancellationTokenSource MergedCancellationTokenSource;

		internal Shared<bool> TopLevelDisposedFlag;

		internal const int POLL_INTERVAL = 63;
	}
}
