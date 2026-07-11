using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal static class CancellableEnumerable
	{
		internal static IEnumerable<TElement> Wrap<TElement>(IEnumerable<TElement> source, CancellationToken token)
		{
			int count = 0;
			foreach (TElement telement in source)
			{
				int num = count;
				count = num + 1;
				if ((num & 63) == 0)
				{
					CancellationState.ThrowIfCanceled(token);
				}
				yield return telement;
			}
			IEnumerator<TElement> enumerator = null;
			yield break;
			yield break;
		}
	}
}
