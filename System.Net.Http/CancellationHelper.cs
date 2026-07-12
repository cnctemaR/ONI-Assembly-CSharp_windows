using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
	internal static class CancellationHelper
	{
		internal static bool ShouldWrapInOperationCanceledException(Exception exception, CancellationToken cancellationToken)
		{
			return !(exception is OperationCanceledException) && cancellationToken.IsCancellationRequested;
		}

		internal static Exception CreateOperationCanceledException(Exception innerException, CancellationToken cancellationToken)
		{
			return new TaskCanceledException(CancellationHelper.s_cancellationMessage, innerException, cancellationToken);
		}

		private static void ThrowOperationCanceledException(Exception innerException, CancellationToken cancellationToken)
		{
			throw CancellationHelper.CreateOperationCanceledException(innerException, cancellationToken);
		}

		internal static void ThrowIfCancellationRequested(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				CancellationHelper.ThrowOperationCanceledException(null, cancellationToken);
			}
		}

		private static readonly string s_cancellationMessage = new OperationCanceledException().Message;
	}
}
