using System;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal static class ExceptionAggregator
	{
		internal static IEnumerable<TElement> WrapEnumerable<TElement>(IEnumerable<TElement> source, CancellationState cancellationState)
		{
			using (IEnumerator<TElement> enumerator = source.GetEnumerator())
			{
				for (;;)
				{
					TElement telement = default(TElement);
					try
					{
						if (!enumerator.MoveNext())
						{
							yield break;
						}
						telement = enumerator.Current;
					}
					catch (Exception ex)
					{
						ExceptionAggregator.ThrowOCEorAggregateException(ex, cancellationState);
					}
					yield return telement;
				}
			}
			yield break;
			yield break;
		}

		internal static IEnumerable<TElement> WrapQueryEnumerator<TElement, TIgnoreKey>(QueryOperatorEnumerator<TElement, TIgnoreKey> source, CancellationState cancellationState)
		{
			TElement elem = default(TElement);
			TIgnoreKey ignoreKey = default(TIgnoreKey);
			try
			{
				for (;;)
				{
					try
					{
						if (!source.MoveNext(ref elem, ref ignoreKey))
						{
							yield break;
						}
					}
					catch (Exception ex)
					{
						ExceptionAggregator.ThrowOCEorAggregateException(ex, cancellationState);
					}
					yield return elem;
				}
			}
			finally
			{
				source.Dispose();
			}
			yield break;
			yield break;
		}

		internal static void ThrowOCEorAggregateException(Exception ex, CancellationState cancellationState)
		{
			if (ExceptionAggregator.ThrowAnOCE(ex, cancellationState))
			{
				CancellationState.ThrowWithStandardMessageIfCanceled(cancellationState.ExternalCancellationToken);
				return;
			}
			throw new AggregateException(new Exception[] { ex });
		}

		internal static Func<T, U> WrapFunc<T, U>(Func<T, U> f, CancellationState cancellationState)
		{
			return delegate(T t)
			{
				U u = default(U);
				try
				{
					u = f(t);
				}
				catch (Exception ex)
				{
					ExceptionAggregator.ThrowOCEorAggregateException(ex, cancellationState);
				}
				return u;
			};
		}

		private static bool ThrowAnOCE(Exception ex, CancellationState cancellationState)
		{
			OperationCanceledException ex2 = ex as OperationCanceledException;
			return (ex2 != null && ex2.CancellationToken == cancellationState.ExternalCancellationToken && cancellationState.ExternalCancellationToken.IsCancellationRequested) || (ex2 != null && ex2.CancellationToken == cancellationState.MergedCancellationToken && cancellationState.MergedCancellationToken.IsCancellationRequested && cancellationState.ExternalCancellationToken.IsCancellationRequested);
		}
	}
}
