using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Internal;

namespace UnityEngine
{
	public static class AsyncOperationAwaitableExtensions
	{
		[ExcludeFromDocs]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Awaitable.Awaiter GetAwaiter(this AsyncOperation op)
		{
			return Awaitable.FromAsyncOperation(op, default(CancellationToken)).GetAwaiter();
		}
	}
}
