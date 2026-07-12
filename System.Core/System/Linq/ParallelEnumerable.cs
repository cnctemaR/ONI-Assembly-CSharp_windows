using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Parallel;
using System.Threading;

namespace System.Linq
{
	public static class ParallelEnumerable
	{
		public static ParallelQuery<TSource> AsParallel<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new ParallelEnumerableWrapper<TSource>(source);
		}

		public static ParallelQuery<TSource> AsParallel<TSource>(this Partitioner<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new PartitionerQueryOperator<TSource>(source);
		}

		public static ParallelQuery<TSource> AsOrdered<TSource>(this ParallelQuery<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!(source is ParallelEnumerableWrapper<TSource>) && !(source is IParallelPartitionable<TSource>))
			{
				PartitionerQueryOperator<TSource> partitionerQueryOperator = source as PartitionerQueryOperator<TSource>;
				if (partitionerQueryOperator == null)
				{
					throw new InvalidOperationException("AsOrdered may only be called on the result of AsParallel, ParallelEnumerable.Range, or ParallelEnumerable.Repeat.");
				}
				if (!partitionerQueryOperator.Orderable)
				{
					throw new InvalidOperationException("AsOrdered may not be used with a partitioner that is not orderable.");
				}
			}
			return new OrderingQueryOperator<TSource>(QueryOperator<TSource>.AsQueryOperator(source), true);
		}

		public static ParallelQuery AsOrdered(this ParallelQuery source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			ParallelEnumerableWrapper parallelEnumerableWrapper = source as ParallelEnumerableWrapper;
			if (parallelEnumerableWrapper == null)
			{
				throw new InvalidOperationException("Non-generic AsOrdered may only be called on the result of the non-generic AsParallel.");
			}
			return new OrderingQueryOperator<object>(QueryOperator<object>.AsQueryOperator(parallelEnumerableWrapper), true);
		}

		public static ParallelQuery<TSource> AsUnordered<TSource>(this ParallelQuery<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new OrderingQueryOperator<TSource>(QueryOperator<TSource>.AsQueryOperator(source), false);
		}

		public static ParallelQuery AsParallel(this IEnumerable source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new ParallelEnumerableWrapper(source);
		}

		public static IEnumerable<TSource> AsSequential<TSource>(this ParallelQuery<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			ParallelEnumerableWrapper<TSource> parallelEnumerableWrapper = source as ParallelEnumerableWrapper<TSource>;
			if (parallelEnumerableWrapper != null)
			{
				return parallelEnumerableWrapper.WrappedEnumerable;
			}
			return source;
		}

		public static ParallelQuery<TSource> WithDegreeOfParallelism<TSource>(this ParallelQuery<TSource> source, int degreeOfParallelism)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (degreeOfParallelism < 1 || degreeOfParallelism > 512)
			{
				throw new ArgumentOutOfRangeException("degreeOfParallelism");
			}
			QuerySettings empty = QuerySettings.Empty;
			empty.DegreeOfParallelism = new int?(degreeOfParallelism);
			return new QueryExecutionOption<TSource>(QueryOperator<TSource>.AsQueryOperator(source), empty);
		}

		public static ParallelQuery<TSource> WithCancellation<TSource>(this ParallelQuery<TSource> source, CancellationToken cancellationToken)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			QuerySettings empty = QuerySettings.Empty;
			empty.CancellationState = new CancellationState(cancellationToken);
			return new QueryExecutionOption<TSource>(QueryOperator<TSource>.AsQueryOperator(source), empty);
		}

		public static ParallelQuery<TSource> WithExecutionMode<TSource>(this ParallelQuery<TSource> source, ParallelExecutionMode executionMode)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (executionMode != ParallelExecutionMode.Default && executionMode != ParallelExecutionMode.ForceParallelism)
			{
				throw new ArgumentException("The executionMode argument contains an invalid value.");
			}
			QuerySettings empty = QuerySettings.Empty;
			empty.ExecutionMode = new ParallelExecutionMode?(executionMode);
			return new QueryExecutionOption<TSource>(QueryOperator<TSource>.AsQueryOperator(source), empty);
		}

		public static ParallelQuery<TSource> WithMergeOptions<TSource>(this ParallelQuery<TSource> source, ParallelMergeOptions mergeOptions)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (mergeOptions != ParallelMergeOptions.Default && mergeOptions != ParallelMergeOptions.AutoBuffered && mergeOptions != ParallelMergeOptions.NotBuffered && mergeOptions != ParallelMergeOptions.FullyBuffered)
			{
				throw new ArgumentException("The mergeOptions argument contains an invalid value.");
			}
			QuerySettings empty = QuerySettings.Empty;
			empty.MergeOptions = new ParallelMergeOptions?(mergeOptions);
			return new QueryExecutionOption<TSource>(QueryOperator<TSource>.AsQueryOperator(source), empty);
		}

		public static ParallelQuery<int> Range(int start, int count)
		{
			if (count < 0 || (count > 0 && 2147483647 - (count - 1) < start))
			{
				throw new ArgumentOutOfRangeException("count");
			}
			return new RangeEnumerable(start, count);
		}

		public static ParallelQuery<TResult> Repeat<TResult>(TResult element, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			return new RepeatEnumerable<TResult>(element, count);
		}

		public static ParallelQuery<TResult> Empty<TResult>()
		{
			return EmptyEnumerable<TResult>.Instance;
		}

		public static void ForAll<TSource>(this ParallelQuery<TSource> source, Action<TSource> action)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			new ForAllOperator<TSource>(source, action).RunSynchronously();
		}

		public static ParallelQuery<TSource> Where<TSource>(this ParallelQuery<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			return new WhereQueryOperator<TSource>(source, predicate);
		}

		public static ParallelQuery<TSource> Where<TSource>(this ParallelQuery<TSource> source, Func<TSource, int, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			return new IndexedWhereQueryOperator<TSource>(source, predicate);
		}

		public static ParallelQuery<TResult> Select<TSource, TResult>(this ParallelQuery<TSource> source, Func<TSource, TResult> selector)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (selector == null)
			{
				throw new ArgumentNullException("selector");
			}
			return new SelectQueryOperator<TSource, TResult>(source, selector);
		}

		public static ParallelQuery<TResult> Select<TSource, TResult>(this ParallelQuery<TSource> source, Func<TSource, int, TResult> selector)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (selector == null)
			{
				throw new ArgumentNullException("selector");
			}
			return new IndexedSelectQueryOperator<TSource, TResult>(source, selector);
		}

		public static ParallelQuery<TResult> Zip<TFirst, TSecond, TResult>(this ParallelQuery<TFirst> first, ParallelQuery<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
		{
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			if (second == null)
			{
				throw new ArgumentNullException("second");
			}
			if (resultSelector == null)
			{
				throw new ArgumentNullException("resultSelector");
			}
			return new ZipQueryOperator<TFirst, TSecond, TResult>(first, second, resultSelector);
		}

		[Obsolete("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.")]
		public static ParallelQuery<TResult> Zip<TFirst, TSecond, TResult>(this ParallelQuery<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
		{
			throw new NotSupportedException("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.");
		}

		public static ParallelQuery<TResult> Join<TOuter, TInner, TKey, TResult>(this ParallelQuery<TOuter> outer, ParallelQuery<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
		{
			return outer.Join<TOuter, TInner, TKey, TResult>(inner, outerKeySelector, innerKeySelector, resultSelector, null);
		}

		[Obsolete("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.")]
		public static ParallelQuery<TResult> Join<TOuter, TInner, TKey, TResult>(this ParallelQuery<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
		{
			throw new NotSupportedException("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.");
		}

		public static ParallelQuery<TResult> Join<TOuter, TInner, TKey, TResult>(this ParallelQuery<TOuter> outer, ParallelQuery<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			if (outer == null)
			{
				throw new ArgumentNullException("outer");
			}
			if (inner == null)
			{
				throw new ArgumentNullException("inner");
			}
			if (outerKeySelector == null)
			{
				throw new ArgumentNullException("outerKeySelector");
			}
			if (innerKeySelector == null)
			{
				throw new ArgumentNullException("innerKeySelector");
			}
			if (resultSelector == null)
			{
				throw new ArgumentNullException("resultSelector");
			}
			return new JoinQueryOperator<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
		}

		[Obsolete("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.")]
		public static ParallelQuery<TResult> Join<TOuter, TInner, TKey, TResult>(this ParallelQuery<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			throw new NotSupportedException("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.");
		}

		public static ParallelQuery<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this ParallelQuery<TOuter> outer, ParallelQuery<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
		{
			return outer.GroupJoin<TOuter, TInner, TKey, TResult>(inner, outerKeySelector, innerKeySelector, resultSelector, null);
		}

		[Obsolete("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.")]
		public static ParallelQuery<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this ParallelQuery<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
		{
			throw new NotSupportedException("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.");
		}

		public static ParallelQuery<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this ParallelQuery<TOuter> outer, ParallelQuery<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			if (outer == null)
			{
				throw new ArgumentNullException("outer");
			}
			if (inner == null)
			{
				throw new ArgumentNullException("inner");
			}
			if (outerKeySelector == null)
			{
				throw new ArgumentNullException("outerKeySelector");
			}
			if (innerKeySelector == null)
			{
				throw new ArgumentNullException("innerKeySelector");
			}
			if (resultSelector == null)
			{
				throw new ArgumentNullException("resultSelector");
			}
			return new GroupJoinQueryOperator<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
		}

		[Obsolete("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.")]
		public static ParallelQuery<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this ParallelQuery<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			throw new NotSupportedException("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.");
		}

		public static ParallelQuery<TResult> SelectMany<TSource, TResult>(this ParallelQuery<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (selector == null)
			{
				throw new ArgumentNullException("selector");
			}
			return new SelectManyQueryOperator<TSource, TResult, TResult>(source, selector, null, null);
		}

		public static ParallelQuery<TResult> SelectMany<TSource, TResult>(this ParallelQuery<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (selector == null)
			{
				throw new ArgumentNullException("selector");
			}
			return new SelectManyQueryOperator<TSource, TResult, TResult>(source, null, selector, null);
		}

		public static ParallelQuery<TResult> SelectMany<TSource, TCollection, TResult>(this ParallelQuery<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (collectionSelector == null)
			{
				throw new ArgumentNullException("collectionSelector");
			}
			if (resultSelector == null)
			{
				throw new ArgumentNullException("resultSelector");
			}
			return new SelectManyQueryOperator<TSource, TCollection, TResult>(source, collectionSelector, null, resultSelector);
		}

		public static ParallelQuery<TResult> SelectMany<TSource, TCollection, TResult>(this ParallelQuery<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (collectionSelector == null)
			{
				throw new ArgumentNullException("collectionSelector");
			}
			if (resultSelector == null)
			{
				throw new ArgumentNullException("resultSelector");
			}
			return new SelectManyQueryOperator<TSource, TCollection, TResult>(source, null, collectionSelector, resultSelector);
		}

		public static OrderedParallelQuery<TSource> OrderBy<TSource, TKey>(this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			return new OrderedParallelQuery<TSource>(new SortQueryOperator<TSource, TKey>(source, keySelector, null, false));
		}

		public static OrderedParallelQuery<TSource> OrderBy<TSource, TKey>(this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			return new OrderedParallelQuery<TSource>(new SortQueryOperator<TSource, TKey>(source, keySelector, comparer, false));
		}

		public static OrderedParallelQuery<TSource> OrderByDescending<TSource, TKey>(this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			return new OrderedParallelQuery<TSource>(new SortQueryOperator<TSource, TKey>(source, keySelector, null, true));
		}

		public static OrderedParallelQuery<TSource> OrderByDescending<TSource, TKey>(this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			return new OrderedParallelQuery<TSource>(new SortQueryOperator<TSource, TKey>(source, keySelector, comparer, true));
		}

		public static OrderedParallelQuery<TSource> ThenBy<TSource, TKey>(this OrderedParallelQuery<TSource> source, Func<TSource, TKey> keySelector)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			return new OrderedParallelQuery<TSource>((QueryOperator<TSource>)source.OrderedEnumerable.CreateOrderedEnumerable<TKey>(keySelector, null, false));
		}

		public static OrderedParallelQuery<TSource> ThenBy<TSource, TKey>(this OrderedParallelQuery<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			return new OrderedParallelQuery<TSource>((QueryOperator<TSource>)source.OrderedEnumerable.CreateOrderedEnumerable<TKey>(keySelector, comparer, false));
		}

		public static OrderedParallelQuery<TSource> ThenByDescending<TSource, TKey>(this OrderedParallelQuery<TSource> source, Func<TSource, TKey> keySelector)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			return new OrderedParallelQuery<TSource>((QueryOperator<TSource>)source.OrderedEnumerable.CreateOrderedEnumerable<TKey>(keySelector, null, true));
		}

		public static OrderedParallelQuery<TSource> ThenByDescending<TSource, TKey>(this OrderedParallelQuery<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			return new OrderedParallelQuery<TSource>((QueryOperator<TSource>)source.OrderedEnumerable.CreateOrderedEnumerable<TKey>(keySelector, comparer, true));
		}

		public static ParallelQuery<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.GroupBy<TSource, TKey>(keySelector, null);
		}

		public static ParallelQuery<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			return new GroupByQueryOperator<TSource, TKey, TSource>(source, keySelector, null, comparer);
		}

		public static ParallelQuery<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return source.GroupBy<TSource, TKey, TElement>(keySelector, elementSelector, null);
		}

		public static ParallelQuery<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			if (elementSelector == null)
			{
				throw new ArgumentNullException("elementSelector");
			}
			return new GroupByQueryOperator<TSource, TKey, TElement>(source, keySelector, elementSelector, comparer);
		}

		public static ParallelQuery<TResult> GroupBy<TSource, TKey, TResult>(this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
		{
			if (resultSelector == null)
			{
				throw new ArgumentNullException("resultSelector");
			}
			return from grouping in source.GroupBy<TSource, TKey>(keySelector)
				select resultSelector(grouping.Key, grouping);
		}

		public static ParallelQuery<TResult> GroupBy<TSource, TKey, TResult>(this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			if (resultSelector == null)
			{
				throw new ArgumentNullException("resultSelector");
			}
			return from grouping in source.GroupBy<TSource, TKey>(keySelector, comparer)
				select resultSelector(grouping.Key, grouping);
		}

		public static ParallelQuery<TResult> GroupBy<TSource, TKey, TElement, TResult>(this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
		{
			if (resultSelector == null)
			{
				throw new ArgumentNullException("resultSelector");
			}
			return from grouping in source.GroupBy<TSource, TKey, TElement>(keySelector, elementSelector)
				select resultSelector(grouping.Key, grouping);
		}

		public static ParallelQuery<TResult> GroupBy<TSource, TKey, TElement, TResult>(this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			if (resultSelector == null)
			{
				throw new ArgumentNullException("resultSelector");
			}
			return from grouping in source.GroupBy<TSource, TKey, TElement>(keySelector, elementSelector, comparer)
				select resultSelector(grouping.Key, grouping);
		}

		private static T PerformAggregation<T>(this ParallelQuery<T> source, Func<T, T, T> reduce, T seed, bool seedIsSpecified, bool throwIfEmpty, QueryAggregationOptions options)
		{
			return new AssociativeAggregationOperator<T, T, T>(source, seed, null, seedIsSpecified, reduce, reduce, (T obj) => obj, throwIfEmpty, options).Aggregate();
		}

		private static TAccumulate PerformSequentialAggregation<TSource, TAccumulate>(this ParallelQuery<TSource> source, TAccumulate seed, bool seedIsSpecified, Func<TAccumulate, TSource, TAccumulate> func)
		{
			TAccumulate taccumulate2;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				TAccumulate taccumulate;
				if (seedIsSpecified)
				{
					taccumulate = seed;
				}
				else
				{
					if (!enumerator.MoveNext())
					{
						throw new InvalidOperationException("Sequence contains no elements");
					}
					taccumulate = (TAccumulate)((object)enumerator.Current);
				}
				while (enumerator.MoveNext())
				{
					TSource tsource = enumerator.Current;
					try
					{
						taccumulate = func(taccumulate, tsource);
					}
					catch (Exception ex)
					{
						throw new AggregateException(new Exception[] { ex });
					}
				}
				taccumulate2 = taccumulate;
			}
			return taccumulate2;
		}

		public static TSource Aggregate<TSource>(this ParallelQuery<TSource> source, Func<TSource, TSource, TSource> func)
		{
			return source.Aggregate<TSource>(func, QueryAggregationOptions.AssociativeCommutative);
		}

		internal static TSource Aggregate<TSource>(this ParallelQuery<TSource> source, Func<TSource, TSource, TSource> func, QueryAggregationOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (func == null)
			{
				throw new ArgumentNullException("func");
			}
			if ((~(QueryAggregationOptions.Associative | QueryAggregationOptions.Commutative) & options) != QueryAggregationOptions.None)
			{
				throw new ArgumentOutOfRangeException("options");
			}
			if ((options & QueryAggregationOptions.Associative) != QueryAggregationOptions.Associative)
			{
				return source.PerformSequentialAggregation(default(TSource), false, func);
			}
			return source.PerformAggregation(func, default(TSource), false, true, options);
		}

		public static TAccumulate Aggregate<TSource, TAccumulate>(this ParallelQuery<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
		{
			return source.Aggregate(seed, func, QueryAggregationOptions.AssociativeCommutative);
		}

		internal static TAccumulate Aggregate<TSource, TAccumulate>(this ParallelQuery<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, QueryAggregationOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (func == null)
			{
				throw new ArgumentNullException("func");
			}
			if ((~(QueryAggregationOptions.Associative | QueryAggregationOptions.Commutative) & options) != QueryAggregationOptions.None)
			{
				throw new ArgumentOutOfRangeException("options");
			}
			return source.PerformSequentialAggregation(seed, true, func);
		}

		public static TResult Aggregate<TSource, TAccumulate, TResult>(this ParallelQuery<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (func == null)
			{
				throw new ArgumentNullException("func");
			}
			if (resultSelector == null)
			{
				throw new ArgumentNullException("resultSelector");
			}
			TAccumulate taccumulate = source.PerformSequentialAggregation(seed, true, func);
			TResult tresult;
			try
			{
				tresult = resultSelector(taccumulate);
			}
			catch (Exception ex)
			{
				throw new AggregateException(new Exception[] { ex });
			}
			return tresult;
		}

		public static TResult Aggregate<TSource, TAccumulate, TResult>(this ParallelQuery<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> updateAccumulatorFunc, Func<TAccumulate, TAccumulate, TAccumulate> combineAccumulatorsFunc, Func<TAccumulate, TResult> resultSelector)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (updateAccumulatorFunc == null)
			{
				throw new ArgumentNullException("updateAccumulatorFunc");
			}
			if (combineAccumulatorsFunc == null)
			{
				throw new ArgumentNullException("combineAccumulatorsFunc");
			}
			if (resultSelector == null)
			{
				throw new ArgumentNullException("resultSelector");
			}
			return new AssociativeAggregationOperator<TSource, TAccumulate, TResult>(source, seed, null, true, updateAccumulatorFunc, combineAccumulatorsFunc, resultSelector, false, QueryAggregationOptions.AssociativeCommutative).Aggregate();
		}

		public static TResult Aggregate<TSource, TAccumulate, TResult>(this ParallelQuery<TSource> source, Func<TAccumulate> seedFactory, Func<TAccumulate, TSource, TAccumulate> updateAccumulatorFunc, Func<TAccumulate, TAccumulate, TAccumulate> combineAccumulatorsFunc, Func<TAccumulate, TResult> resultSelector)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (seedFactory == null)
			{
				throw new ArgumentNullException("seedFactory");
			}
			if (updateAccumulatorFunc == null)
			{
				throw new ArgumentNullException("updateAccumulatorFunc");
			}
			if (combineAccumulatorsFunc == null)
			{
				throw new ArgumentNullException("combineAccumulatorsFunc");
			}
			if (resultSelector == null)
			{
				throw new ArgumentNullException("resultSelector");
			}
			return new AssociativeAggregationOperator<TSource, TAccumulate, TResult>(source, default(TAccumulate), seedFactory, true, updateAccumulatorFunc, combineAccumulatorsFunc, resultSelector, false, QueryAggregationOptions.AssociativeCommutative).Aggregate();
		}

		public static int Count<TSource>(this ParallelQuery<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			ParallelEnumerableWrapper<TSource> parallelEnumerableWrapper = source as ParallelEnumerableWrapper<TSource>;
			if (parallelEnumerableWrapper != null)
			{
				ICollection<TSource> collection = parallelEnumerableWrapper.WrappedEnumerable as ICollection<TSource>;
				if (collection != null)
				{
					return collection.Count;
				}
			}
			return new CountAggregationOperator<TSource>(source).Aggregate();
		}

		public static int Count<TSource>(this ParallelQuery<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			return new CountAggregationOperator<TSource>(source.Where<TSource>(predicate)).Aggregate();
		}

		public static long LongCount<TSource>(this ParallelQuery<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			ParallelEnumerableWrapper<TSource> parallelEnumerableWrapper = source as ParallelEnumerableWrapper<TSource>;
			if (parallelEnumerableWrapper != null)
			{
				ICollection<TSource> collection = parallelEnumerableWrapper.WrappedEnumerable as ICollection<TSource>;
				if (collection != null)
				{
					return (long)collection.Count;
				}
			}
			return new LongCountAggregationOperator<TSource>(source).Aggregate();
		}

		public static long LongCount<TSource>(this ParallelQuery<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			return new LongCountAggregationOperator<TSource>(source.Where<TSource>(predicate)).Aggregate();
		}

		public static int Sum(this ParallelQuery<int> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new IntSumAggregationOperator(source).Aggregate();
		}

		public static int? Sum(this ParallelQuery<int?> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new NullableIntSumAggregationOperator(source).Aggregate();
		}

		public static long Sum(this ParallelQuery<long> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new LongSumAggregationOperator(source).Aggregate();
		}

		public static long? Sum(this ParallelQuery<long?> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new NullableLongSumAggregationOperator(source).Aggregate();
		}

		public static float Sum(this ParallelQuery<float> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new FloatSumAggregationOperator(source).Aggregate();
		}

		public static float? Sum(this ParallelQuery<float?> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new NullableFloatSumAggregationOperator(source).Aggregate();
		}

		public static double Sum(this ParallelQuery<double> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new DoubleSumAggregationOperator(source).Aggregate();
		}

		public static double? Sum(this ParallelQuery<double?> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new NullableDoubleSumAggregationOperator(source).Aggregate();
		}

		public static decimal Sum(this ParallelQuery<decimal> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new DecimalSumAggregationOperator(source).Aggregate();
		}

		public static decimal? Sum(this ParallelQuery<decimal?> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new NullableDecimalSumAggregationOperator(source).Aggregate();
		}

		public static int Sum<TSource>(this ParallelQuery<TSource> source, Func<TSource, int> selector)
		{
			return source.Select<TSource, int>(selector).Sum();
		}

		public static int? Sum<TSource>(this ParallelQuery<TSource> source, Func<TSource, int?> selector)
		{
			return source.Select<TSource, int?>(selector).Sum();
		}

		public static long Sum<TSource>(this ParallelQuery<TSource> source, Func<TSource, long> selector)
		{
			return source.Select<TSource, long>(selector).Sum();
		}

		public static long? Sum<TSource>(this ParallelQuery<TSource> source, Func<TSource, long?> selector)
		{
			return source.Select<TSource, long?>(selector).Sum();
		}

		public static float Sum<TSource>(this ParallelQuery<TSource> source, Func<TSource, float> selector)
		{
			return source.Select<TSource, float>(selector).Sum();
		}

		public static float? Sum<TSource>(this ParallelQuery<TSource> source, Func<TSource, float?> selector)
		{
			return source.Select<TSource, float?>(selector).Sum();
		}

		public static double Sum<TSource>(this ParallelQuery<TSource> source, Func<TSource, double> selector)
		{
			return source.Select<TSource, double>(selector).Sum();
		}

		public static double? Sum<TSource>(this ParallelQuery<TSource> source, Func<TSource, double?> selector)
		{
			return source.Select<TSource, double?>(selector).Sum();
		}

		public static decimal Sum<TSource>(this ParallelQuery<TSource> source, Func<TSource, decimal> selector)
		{
			return source.Select<TSource, decimal>(selector).Sum();
		}

		public static decimal? Sum<TSource>(this ParallelQuery<TSource> source, Func<TSource, decimal?> selector)
		{
			return source.Select<TSource, decimal?>(selector).Sum();
		}

		public static int Min(this ParallelQuery<int> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new IntMinMaxAggregationOperator(source, -1).Aggregate();
		}

		public static int? Min(this ParallelQuery<int?> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new NullableIntMinMaxAggregationOperator(source, -1).Aggregate();
		}

		public static long Min(this ParallelQuery<long> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new LongMinMaxAggregationOperator(source, -1).Aggregate();
		}

		public static long? Min(this ParallelQuery<long?> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new NullableLongMinMaxAggregationOperator(source, -1).Aggregate();
		}

		public static float Min(this ParallelQuery<float> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new FloatMinMaxAggregationOperator(source, -1).Aggregate();
		}

		public static float? Min(this ParallelQuery<float?> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new NullableFloatMinMaxAggregationOperator(source, -1).Aggregate();
		}

		public static double Min(this ParallelQuery<double> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new DoubleMinMaxAggregationOperator(source, -1).Aggregate();
		}

		public static double? Min(this ParallelQuery<double?> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new NullableDoubleMinMaxAggregationOperator(source, -1).Aggregate();
		}

		public static decimal Min(this ParallelQuery<decimal> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new DecimalMinMaxAggregationOperator(source, -1).Aggregate();
		}

		public static decimal? Min(this ParallelQuery<decimal?> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new NullableDecimalMinMaxAggregationOperator(source, -1).Aggregate();
		}

		public static TSource Min<TSource>(this ParallelQuery<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return AggregationMinMaxHelpers<TSource>.ReduceMin(source);
		}

		public static int Min<TSource>(this ParallelQuery<TSource> source, Func<TSource, int> selector)
		{
			return source.Select<TSource, int>(selector).Min<int>();
		}

		public static int? Min<TSource>(this ParallelQuery<TSource> source, Func<TSource, int?> selector)
		{
			return source.Select<TSource, int?>(selector).Min<int?>();
		}

		public static long Min<TSource>(this ParallelQuery<TSource> source, Func<TSource, long> selector)
		{
			return source.Select<TSource, long>(selector).Min<long>();
		}

		public static long? Min<TSource>(this ParallelQuery<TSource> source, Func<TSource, long?> selector)
		{
			return source.Select<TSource, long?>(selector).Min<long?>();
		}

		public static float Min<TSource>(this ParallelQuery<TSource> source, Func<TSource, float> selector)
		{
			return source.Select<TSource, float>(selector).Min<float>();
		}

		public static float? Min<TSource>(this ParallelQuery<TSource> source, Func<TSource, float?> selector)
		{
			return source.Select<TSource, float?>(selector).Min<float?>();
		}

		public static double Min<TSource>(this ParallelQuery<TSource> source, Func<TSource, double> selector)
		{
			return source.Select<TSource, double>(selector).Min<double>();
		}

		public static double? Min<TSource>(this ParallelQuery<TSource> source, Func<TSource, double?> selector)
		{
			return source.Select<TSource, double?>(selector).Min<double?>();
		}

		public static decimal Min<TSource>(this ParallelQuery<TSource> source, Func<TSource, decimal> selector)
		{
			return source.Select<TSource, decimal>(selector).Min<decimal>();
		}

		public static decimal? Min<TSource>(this ParallelQuery<TSource> source, Func<TSource, decimal?> selector)
		{
			return source.Select<TSource, decimal?>(selector).Min<decimal?>();
		}

		public static TResult Min<TSource, TResult>(this ParallelQuery<TSource> source, Func<TSource, TResult> selector)
		{
			return source.Select<TSource, TResult>(selector).Min<TResult>();
		}

		public static int Max(this ParallelQuery<int> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new IntMinMaxAggregationOperator(source, 1).Aggregate();
		}

		public static int? Max(this ParallelQuery<int?> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new NullableIntMinMaxAggregationOperator(source, 1).Aggregate();
		}

		public static long Max(this ParallelQuery<long> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new LongMinMaxAggregationOperator(source, 1).Aggregate();
		}

		public static long? Max(this ParallelQuery<long?> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new NullableLongMinMaxAggregationOperator(source, 1).Aggregate();
		}

		public static float Max(this ParallelQuery<float> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new FloatMinMaxAggregationOperator(source, 1).Aggregate();
		}

		public static float? Max(this ParallelQuery<float?> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new NullableFloatMinMaxAggregationOperator(source, 1).Aggregate();
		}

		public static double Max(this ParallelQuery<double> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new DoubleMinMaxAggregationOperator(source, 1).Aggregate();
		}

		public static double? Max(this ParallelQuery<double?> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new NullableDoubleMinMaxAggregationOperator(source, 1).Aggregate();
		}

		public static decimal Max(this ParallelQuery<decimal> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new DecimalMinMaxAggregationOperator(source, 1).Aggregate();
		}

		public static decimal? Max(this ParallelQuery<decimal?> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new NullableDecimalMinMaxAggregationOperator(source, 1).Aggregate();
		}

		public static TSource Max<TSource>(this ParallelQuery<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return AggregationMinMaxHelpers<TSource>.ReduceMax(source);
		}

		public static int Max<TSource>(this ParallelQuery<TSource> source, Func<TSource, int> selector)
		{
			return source.Select<TSource, int>(selector).Max<int>();
		}

		public static int? Max<TSource>(this ParallelQuery<TSource> source, Func<TSource, int?> selector)
		{
			return source.Select<TSource, int?>(selector).Max<int?>();
		}

		public static long Max<TSource>(this ParallelQuery<TSource> source, Func<TSource, long> selector)
		{
			return source.Select<TSource, long>(selector).Max<long>();
		}

		public static long? Max<TSource>(this ParallelQuery<TSource> source, Func<TSource, long?> selector)
		{
			return source.Select<TSource, long?>(selector).Max<long?>();
		}

		public static float Max<TSource>(this ParallelQuery<TSource> source, Func<TSource, float> selector)
		{
			return source.Select<TSource, float>(selector).Max<float>();
		}

		public static float? Max<TSource>(this ParallelQuery<TSource> source, Func<TSource, float?> selector)
		{
			return source.Select<TSource, float?>(selector).Max<float?>();
		}

		public static double Max<TSource>(this ParallelQuery<TSource> source, Func<TSource, double> selector)
		{
			return source.Select<TSource, double>(selector).Max<double>();
		}

		public static double? Max<TSource>(this ParallelQuery<TSource> source, Func<TSource, double?> selector)
		{
			return source.Select<TSource, double?>(selector).Max<double?>();
		}

		public static decimal Max<TSource>(this ParallelQuery<TSource> source, Func<TSource, decimal> selector)
		{
			return source.Select<TSource, decimal>(selector).Max<decimal>();
		}

		public static decimal? Max<TSource>(this ParallelQuery<TSource> source, Func<TSource, decimal?> selector)
		{
			return source.Select<TSource, decimal?>(selector).Max<decimal?>();
		}

		public static TResult Max<TSource, TResult>(this ParallelQuery<TSource> source, Func<TSource, TResult> selector)
		{
			return source.Select<TSource, TResult>(selector).Max<TResult>();
		}

		public static double Average(this ParallelQuery<int> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new IntAverageAggregationOperator(source).Aggregate();
		}

		public static double? Average(this ParallelQuery<int?> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new NullableIntAverageAggregationOperator(source).Aggregate();
		}

		public static double Average(this ParallelQuery<long> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new LongAverageAggregationOperator(source).Aggregate();
		}

		public static double? Average(this ParallelQuery<long?> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new NullableLongAverageAggregationOperator(source).Aggregate();
		}

		public static float Average(this ParallelQuery<float> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new FloatAverageAggregationOperator(source).Aggregate();
		}

		public static float? Average(this ParallelQuery<float?> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new NullableFloatAverageAggregationOperator(source).Aggregate();
		}

		public static double Average(this ParallelQuery<double> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new DoubleAverageAggregationOperator(source).Aggregate();
		}

		public static double? Average(this ParallelQuery<double?> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new NullableDoubleAverageAggregationOperator(source).Aggregate();
		}

		public static decimal Average(this ParallelQuery<decimal> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new DecimalAverageAggregationOperator(source).Aggregate();
		}

		public static decimal? Average(this ParallelQuery<decimal?> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new NullableDecimalAverageAggregationOperator(source).Aggregate();
		}

		public static double Average<TSource>(this ParallelQuery<TSource> source, Func<TSource, int> selector)
		{
			return source.Select<TSource, int>(selector).Average();
		}

		public static double? Average<TSource>(this ParallelQuery<TSource> source, Func<TSource, int?> selector)
		{
			return source.Select<TSource, int?>(selector).Average();
		}

		public static double Average<TSource>(this ParallelQuery<TSource> source, Func<TSource, long> selector)
		{
			return source.Select<TSource, long>(selector).Average();
		}

		public static double? Average<TSource>(this ParallelQuery<TSource> source, Func<TSource, long?> selector)
		{
			return source.Select<TSource, long?>(selector).Average();
		}

		public static float Average<TSource>(this ParallelQuery<TSource> source, Func<TSource, float> selector)
		{
			return source.Select<TSource, float>(selector).Average();
		}

		public static float? Average<TSource>(this ParallelQuery<TSource> source, Func<TSource, float?> selector)
		{
			return source.Select<TSource, float?>(selector).Average();
		}

		public static double Average<TSource>(this ParallelQuery<TSource> source, Func<TSource, double> selector)
		{
			return source.Select<TSource, double>(selector).Average();
		}

		public static double? Average<TSource>(this ParallelQuery<TSource> source, Func<TSource, double?> selector)
		{
			return source.Select<TSource, double?>(selector).Average();
		}

		public static decimal Average<TSource>(this ParallelQuery<TSource> source, Func<TSource, decimal> selector)
		{
			return source.Select<TSource, decimal>(selector).Average();
		}

		public static decimal? Average<TSource>(this ParallelQuery<TSource> source, Func<TSource, decimal?> selector)
		{
			return source.Select<TSource, decimal?>(selector).Average();
		}

		public static bool Any<TSource>(this ParallelQuery<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			return new AnyAllSearchOperator<TSource>(source, true, predicate).Aggregate();
		}

		public static bool Any<TSource>(this ParallelQuery<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return source.Any<TSource>((TSource x) => true);
		}

		public static bool All<TSource>(this ParallelQuery<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			return new AnyAllSearchOperator<TSource>(source, false, predicate).Aggregate();
		}

		public static bool Contains<TSource>(this ParallelQuery<TSource> source, TSource value)
		{
			return source.Contains(value, null);
		}

		public static bool Contains<TSource>(this ParallelQuery<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new ContainsSearchOperator<TSource>(source, value, comparer).Aggregate();
		}

		public static ParallelQuery<TSource> Take<TSource>(this ParallelQuery<TSource> source, int count)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (count > 0)
			{
				return new TakeOrSkipQueryOperator<TSource>(source, count, true);
			}
			return ParallelEnumerable.Empty<TSource>();
		}

		public static ParallelQuery<TSource> TakeWhile<TSource>(this ParallelQuery<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			return new TakeOrSkipWhileQueryOperator<TSource>(source, predicate, null, true);
		}

		public static ParallelQuery<TSource> TakeWhile<TSource>(this ParallelQuery<TSource> source, Func<TSource, int, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			return new TakeOrSkipWhileQueryOperator<TSource>(source, null, predicate, true);
		}

		public static ParallelQuery<TSource> Skip<TSource>(this ParallelQuery<TSource> source, int count)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (count <= 0)
			{
				return source;
			}
			return new TakeOrSkipQueryOperator<TSource>(source, count, false);
		}

		public static ParallelQuery<TSource> SkipWhile<TSource>(this ParallelQuery<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			return new TakeOrSkipWhileQueryOperator<TSource>(source, predicate, null, false);
		}

		public static ParallelQuery<TSource> SkipWhile<TSource>(this ParallelQuery<TSource> source, Func<TSource, int, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			return new TakeOrSkipWhileQueryOperator<TSource>(source, null, predicate, false);
		}

		public static ParallelQuery<TSource> Concat<TSource>(this ParallelQuery<TSource> first, ParallelQuery<TSource> second)
		{
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			if (second == null)
			{
				throw new ArgumentNullException("second");
			}
			return new ConcatQueryOperator<TSource>(first, second);
		}

		[Obsolete("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.")]
		public static ParallelQuery<TSource> Concat<TSource>(this ParallelQuery<TSource> first, IEnumerable<TSource> second)
		{
			throw new NotSupportedException("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.");
		}

		public static bool SequenceEqual<TSource>(this ParallelQuery<TSource> first, ParallelQuery<TSource> second)
		{
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			if (second == null)
			{
				throw new ArgumentNullException("second");
			}
			return first.SequenceEqual<TSource>(second, null);
		}

		[Obsolete("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.")]
		public static bool SequenceEqual<TSource>(this ParallelQuery<TSource> first, IEnumerable<TSource> second)
		{
			throw new NotSupportedException("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.");
		}

		public static bool SequenceEqual<TSource>(this ParallelQuery<TSource> first, ParallelQuery<TSource> second, IEqualityComparer<TSource> comparer)
		{
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			if (second == null)
			{
				throw new ArgumentNullException("second");
			}
			comparer = comparer ?? EqualityComparer<TSource>.Default;
			ParallelQuery parallelQuery = QueryOperator<TSource>.AsQueryOperator(first);
			QueryOperator<TSource> queryOperator = QueryOperator<TSource>.AsQueryOperator(second);
			QuerySettings querySettings = parallelQuery.SpecifiedQuerySettings.Merge(queryOperator.SpecifiedQuerySettings).WithDefaults().WithPerExecutionSettings(new CancellationTokenSource(), new Shared<bool>(false));
			IEnumerator<TSource> enumerator = first.GetEnumerator();
			try
			{
				IEnumerator<TSource> enumerator2 = second.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator2.MoveNext() || !comparer.Equals(enumerator.Current, enumerator2.Current))
						{
							return false;
						}
					}
					if (enumerator2.MoveNext())
					{
						return false;
					}
				}
				catch (Exception ex)
				{
					ExceptionAggregator.ThrowOCEorAggregateException(ex, querySettings.CancellationState);
				}
				finally
				{
					ParallelEnumerable.DisposeEnumerator<TSource>(enumerator2, querySettings.CancellationState);
				}
			}
			finally
			{
				ParallelEnumerable.DisposeEnumerator<TSource>(enumerator, querySettings.CancellationState);
			}
			return true;
		}

		private static void DisposeEnumerator<TSource>(IEnumerator<TSource> e, CancellationState cancelState)
		{
			try
			{
				e.Dispose();
			}
			catch (Exception ex)
			{
				ExceptionAggregator.ThrowOCEorAggregateException(ex, cancelState);
			}
		}

		[Obsolete("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.")]
		public static bool SequenceEqual<TSource>(this ParallelQuery<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			throw new NotSupportedException("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.");
		}

		public static ParallelQuery<TSource> Distinct<TSource>(this ParallelQuery<TSource> source)
		{
			return source.Distinct<TSource>(null);
		}

		public static ParallelQuery<TSource> Distinct<TSource>(this ParallelQuery<TSource> source, IEqualityComparer<TSource> comparer)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new DistinctQueryOperator<TSource>(source, comparer);
		}

		public static ParallelQuery<TSource> Union<TSource>(this ParallelQuery<TSource> first, ParallelQuery<TSource> second)
		{
			return first.Union<TSource>(second, null);
		}

		[Obsolete("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.")]
		public static ParallelQuery<TSource> Union<TSource>(this ParallelQuery<TSource> first, IEnumerable<TSource> second)
		{
			throw new NotSupportedException("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.");
		}

		public static ParallelQuery<TSource> Union<TSource>(this ParallelQuery<TSource> first, ParallelQuery<TSource> second, IEqualityComparer<TSource> comparer)
		{
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			if (second == null)
			{
				throw new ArgumentNullException("second");
			}
			return new UnionQueryOperator<TSource>(first, second, comparer);
		}

		[Obsolete("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.")]
		public static ParallelQuery<TSource> Union<TSource>(this ParallelQuery<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			throw new NotSupportedException("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.");
		}

		public static ParallelQuery<TSource> Intersect<TSource>(this ParallelQuery<TSource> first, ParallelQuery<TSource> second)
		{
			return first.Intersect<TSource>(second, null);
		}

		[Obsolete("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.")]
		public static ParallelQuery<TSource> Intersect<TSource>(this ParallelQuery<TSource> first, IEnumerable<TSource> second)
		{
			throw new NotSupportedException("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.");
		}

		public static ParallelQuery<TSource> Intersect<TSource>(this ParallelQuery<TSource> first, ParallelQuery<TSource> second, IEqualityComparer<TSource> comparer)
		{
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			if (second == null)
			{
				throw new ArgumentNullException("second");
			}
			return new IntersectQueryOperator<TSource>(first, second, comparer);
		}

		[Obsolete("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.")]
		public static ParallelQuery<TSource> Intersect<TSource>(this ParallelQuery<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			throw new NotSupportedException("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.");
		}

		public static ParallelQuery<TSource> Except<TSource>(this ParallelQuery<TSource> first, ParallelQuery<TSource> second)
		{
			return first.Except<TSource>(second, null);
		}

		[Obsolete("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.")]
		public static ParallelQuery<TSource> Except<TSource>(this ParallelQuery<TSource> first, IEnumerable<TSource> second)
		{
			throw new NotSupportedException("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.");
		}

		public static ParallelQuery<TSource> Except<TSource>(this ParallelQuery<TSource> first, ParallelQuery<TSource> second, IEqualityComparer<TSource> comparer)
		{
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			if (second == null)
			{
				throw new ArgumentNullException("second");
			}
			return new ExceptQueryOperator<TSource>(first, second, comparer);
		}

		[Obsolete("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.")]
		public static ParallelQuery<TSource> Except<TSource>(this ParallelQuery<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			throw new NotSupportedException("The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.");
		}

		public static IEnumerable<TSource> AsEnumerable<TSource>(this ParallelQuery<TSource> source)
		{
			return source.AsSequential<TSource>();
		}

		public static TSource[] ToArray<TSource>(this ParallelQuery<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			QueryOperator<TSource> queryOperator = source as QueryOperator<TSource>;
			if (queryOperator != null)
			{
				return queryOperator.ExecuteAndGetResultsAsArray();
			}
			return source.ToList<TSource>().ToArray<TSource>();
		}

		public static List<TSource> ToList<TSource>(this ParallelQuery<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			List<TSource> list = new List<TSource>();
			QueryOperator<TSource> queryOperator = source as QueryOperator<TSource>;
			IEnumerator<TSource> enumerator;
			if (queryOperator != null)
			{
				if (queryOperator.OrdinalIndexState == OrdinalIndexState.Indexable && queryOperator.OutputOrdered)
				{
					return new List<TSource>(source.ToArray<TSource>());
				}
				enumerator = queryOperator.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered));
			}
			else
			{
				enumerator = source.GetEnumerator();
			}
			using (enumerator)
			{
				while (enumerator.MoveNext())
				{
					TSource tsource = enumerator.Current;
					list.Add(tsource);
				}
			}
			return list;
		}

		public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.ToDictionary<TSource, TKey>(keySelector, EqualityComparer<TKey>.Default);
		}

		public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			Dictionary<TKey, TSource> dictionary = new Dictionary<TKey, TSource>(comparer);
			QueryOperator<TSource> queryOperator = source as QueryOperator<TSource>;
			IEnumerator<TSource> enumerator = ((queryOperator == null) ? source.GetEnumerator() : queryOperator.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered), true));
			using (enumerator)
			{
				while (enumerator.MoveNext())
				{
					TSource tsource = enumerator.Current;
					try
					{
						TKey tkey = keySelector(tsource);
						dictionary.Add(tkey, tsource);
					}
					catch (Exception ex)
					{
						throw new AggregateException(new Exception[] { ex });
					}
				}
			}
			return dictionary;
		}

		public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return source.ToDictionary<TSource, TKey, TElement>(keySelector, elementSelector, EqualityComparer<TKey>.Default);
		}

		public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			if (elementSelector == null)
			{
				throw new ArgumentNullException("elementSelector");
			}
			Dictionary<TKey, TElement> dictionary = new Dictionary<TKey, TElement>(comparer);
			QueryOperator<TSource> queryOperator = source as QueryOperator<TSource>;
			IEnumerator<TSource> enumerator = ((queryOperator == null) ? source.GetEnumerator() : queryOperator.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered), true));
			using (enumerator)
			{
				while (enumerator.MoveNext())
				{
					TSource tsource = enumerator.Current;
					try
					{
						dictionary.Add(keySelector(tsource), elementSelector(tsource));
					}
					catch (Exception ex)
					{
						throw new AggregateException(new Exception[] { ex });
					}
				}
			}
			return dictionary;
		}

		public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.ToLookup<TSource, TKey>(keySelector, EqualityComparer<TKey>.Default);
		}

		public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			comparer = comparer ?? EqualityComparer<TKey>.Default;
			ParallelQuery<IGrouping<TKey, TSource>> parallelQuery = source.GroupBy<TSource, TKey>(keySelector, comparer);
			Lookup<TKey, TSource> lookup = new Lookup<TKey, TSource>(comparer);
			QueryOperator<IGrouping<TKey, TSource>> queryOperator = parallelQuery as QueryOperator<IGrouping<TKey, TSource>>;
			IEnumerator<IGrouping<TKey, TSource>> enumerator = ((queryOperator == null) ? parallelQuery.GetEnumerator() : queryOperator.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered)));
			using (enumerator)
			{
				while (enumerator.MoveNext())
				{
					IGrouping<TKey, TSource> grouping = enumerator.Current;
					lookup.Add(grouping);
				}
			}
			return lookup;
		}

		public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return source.ToLookup<TSource, TKey, TElement>(keySelector, elementSelector, EqualityComparer<TKey>.Default);
		}

		public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			if (elementSelector == null)
			{
				throw new ArgumentNullException("elementSelector");
			}
			comparer = comparer ?? EqualityComparer<TKey>.Default;
			ParallelQuery<IGrouping<TKey, TElement>> parallelQuery = source.GroupBy<TSource, TKey, TElement>(keySelector, elementSelector, comparer);
			Lookup<TKey, TElement> lookup = new Lookup<TKey, TElement>(comparer);
			QueryOperator<IGrouping<TKey, TElement>> queryOperator = parallelQuery as QueryOperator<IGrouping<TKey, TElement>>;
			IEnumerator<IGrouping<TKey, TElement>> enumerator = ((queryOperator == null) ? parallelQuery.GetEnumerator() : queryOperator.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered)));
			using (enumerator)
			{
				while (enumerator.MoveNext())
				{
					IGrouping<TKey, TElement> grouping = enumerator.Current;
					lookup.Add(grouping);
				}
			}
			return lookup;
		}

		public static ParallelQuery<TSource> Reverse<TSource>(this ParallelQuery<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new ReverseQueryOperator<TSource>(source);
		}

		public static ParallelQuery<TResult> OfType<TResult>(this ParallelQuery source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return source.OfType<TResult>();
		}

		public static ParallelQuery<TResult> Cast<TResult>(this ParallelQuery source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return source.Cast<TResult>();
		}

		private static TSource GetOneWithPossibleDefault<TSource>(QueryOperator<TSource> queryOp, bool throwIfTwo, bool defaultIfEmpty)
		{
			using (IEnumerator<TSource> enumerator = queryOp.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered)))
			{
				if (enumerator.MoveNext())
				{
					TSource tsource = enumerator.Current;
					if (throwIfTwo && enumerator.MoveNext())
					{
						throw new InvalidOperationException("Sequence contains more than one matching element");
					}
					return tsource;
				}
			}
			if (defaultIfEmpty)
			{
				return default(TSource);
			}
			throw new InvalidOperationException("Sequence contains no elements");
		}

		public static TSource First<TSource>(this ParallelQuery<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			FirstQueryOperator<TSource> firstQueryOperator = new FirstQueryOperator<TSource>(source, null);
			QuerySettings querySettings = firstQueryOperator.SpecifiedQuerySettings.WithDefaults();
			if (firstQueryOperator.LimitsParallelism)
			{
				ParallelExecutionMode? executionMode = querySettings.ExecutionMode;
				ParallelExecutionMode parallelExecutionMode = ParallelExecutionMode.ForceParallelism;
				if (!((executionMode.GetValueOrDefault() == parallelExecutionMode) & (executionMode != null)))
				{
					return ExceptionAggregator.WrapEnumerable<TSource>(CancellableEnumerable.Wrap<TSource>(firstQueryOperator.Child.AsSequentialQuery(querySettings.CancellationState.ExternalCancellationToken), querySettings.CancellationState.ExternalCancellationToken), querySettings.CancellationState).First<TSource>();
				}
			}
			return ParallelEnumerable.GetOneWithPossibleDefault<TSource>(firstQueryOperator, false, false);
		}

		public static TSource First<TSource>(this ParallelQuery<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			FirstQueryOperator<TSource> firstQueryOperator = new FirstQueryOperator<TSource>(source, predicate);
			QuerySettings querySettings = firstQueryOperator.SpecifiedQuerySettings.WithDefaults();
			if (firstQueryOperator.LimitsParallelism)
			{
				ParallelExecutionMode? executionMode = querySettings.ExecutionMode;
				ParallelExecutionMode parallelExecutionMode = ParallelExecutionMode.ForceParallelism;
				if (!((executionMode.GetValueOrDefault() == parallelExecutionMode) & (executionMode != null)))
				{
					return ExceptionAggregator.WrapEnumerable<TSource>(CancellableEnumerable.Wrap<TSource>(firstQueryOperator.Child.AsSequentialQuery(querySettings.CancellationState.ExternalCancellationToken), querySettings.CancellationState.ExternalCancellationToken), querySettings.CancellationState).First<TSource>(ExceptionAggregator.WrapFunc<TSource, bool>(predicate, querySettings.CancellationState));
				}
			}
			return ParallelEnumerable.GetOneWithPossibleDefault<TSource>(firstQueryOperator, false, false);
		}

		public static TSource FirstOrDefault<TSource>(this ParallelQuery<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			FirstQueryOperator<TSource> firstQueryOperator = new FirstQueryOperator<TSource>(source, null);
			QuerySettings querySettings = firstQueryOperator.SpecifiedQuerySettings.WithDefaults();
			if (firstQueryOperator.LimitsParallelism)
			{
				ParallelExecutionMode? executionMode = querySettings.ExecutionMode;
				ParallelExecutionMode parallelExecutionMode = ParallelExecutionMode.ForceParallelism;
				if (!((executionMode.GetValueOrDefault() == parallelExecutionMode) & (executionMode != null)))
				{
					return ExceptionAggregator.WrapEnumerable<TSource>(CancellableEnumerable.Wrap<TSource>(firstQueryOperator.Child.AsSequentialQuery(querySettings.CancellationState.ExternalCancellationToken), querySettings.CancellationState.ExternalCancellationToken), querySettings.CancellationState).FirstOrDefault<TSource>();
				}
			}
			return ParallelEnumerable.GetOneWithPossibleDefault<TSource>(firstQueryOperator, false, true);
		}

		public static TSource FirstOrDefault<TSource>(this ParallelQuery<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			FirstQueryOperator<TSource> firstQueryOperator = new FirstQueryOperator<TSource>(source, predicate);
			QuerySettings querySettings = firstQueryOperator.SpecifiedQuerySettings.WithDefaults();
			if (firstQueryOperator.LimitsParallelism)
			{
				ParallelExecutionMode? executionMode = querySettings.ExecutionMode;
				ParallelExecutionMode parallelExecutionMode = ParallelExecutionMode.ForceParallelism;
				if (!((executionMode.GetValueOrDefault() == parallelExecutionMode) & (executionMode != null)))
				{
					return ExceptionAggregator.WrapEnumerable<TSource>(CancellableEnumerable.Wrap<TSource>(firstQueryOperator.Child.AsSequentialQuery(querySettings.CancellationState.ExternalCancellationToken), querySettings.CancellationState.ExternalCancellationToken), querySettings.CancellationState).FirstOrDefault<TSource>(ExceptionAggregator.WrapFunc<TSource, bool>(predicate, querySettings.CancellationState));
				}
			}
			return ParallelEnumerable.GetOneWithPossibleDefault<TSource>(firstQueryOperator, false, true);
		}

		public static TSource Last<TSource>(this ParallelQuery<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			LastQueryOperator<TSource> lastQueryOperator = new LastQueryOperator<TSource>(source, null);
			QuerySettings querySettings = lastQueryOperator.SpecifiedQuerySettings.WithDefaults();
			if (lastQueryOperator.LimitsParallelism)
			{
				ParallelExecutionMode? executionMode = querySettings.ExecutionMode;
				ParallelExecutionMode parallelExecutionMode = ParallelExecutionMode.ForceParallelism;
				if (!((executionMode.GetValueOrDefault() == parallelExecutionMode) & (executionMode != null)))
				{
					return ExceptionAggregator.WrapEnumerable<TSource>(CancellableEnumerable.Wrap<TSource>(lastQueryOperator.Child.AsSequentialQuery(querySettings.CancellationState.ExternalCancellationToken), querySettings.CancellationState.ExternalCancellationToken), querySettings.CancellationState).Last<TSource>();
				}
			}
			return ParallelEnumerable.GetOneWithPossibleDefault<TSource>(lastQueryOperator, false, false);
		}

		public static TSource Last<TSource>(this ParallelQuery<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			LastQueryOperator<TSource> lastQueryOperator = new LastQueryOperator<TSource>(source, predicate);
			QuerySettings querySettings = lastQueryOperator.SpecifiedQuerySettings.WithDefaults();
			if (lastQueryOperator.LimitsParallelism)
			{
				ParallelExecutionMode? executionMode = querySettings.ExecutionMode;
				ParallelExecutionMode parallelExecutionMode = ParallelExecutionMode.ForceParallelism;
				if (!((executionMode.GetValueOrDefault() == parallelExecutionMode) & (executionMode != null)))
				{
					return ExceptionAggregator.WrapEnumerable<TSource>(CancellableEnumerable.Wrap<TSource>(lastQueryOperator.Child.AsSequentialQuery(querySettings.CancellationState.ExternalCancellationToken), querySettings.CancellationState.ExternalCancellationToken), querySettings.CancellationState).Last<TSource>(ExceptionAggregator.WrapFunc<TSource, bool>(predicate, querySettings.CancellationState));
				}
			}
			return ParallelEnumerable.GetOneWithPossibleDefault<TSource>(lastQueryOperator, false, false);
		}

		public static TSource LastOrDefault<TSource>(this ParallelQuery<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			LastQueryOperator<TSource> lastQueryOperator = new LastQueryOperator<TSource>(source, null);
			QuerySettings querySettings = lastQueryOperator.SpecifiedQuerySettings.WithDefaults();
			if (lastQueryOperator.LimitsParallelism)
			{
				ParallelExecutionMode? executionMode = querySettings.ExecutionMode;
				ParallelExecutionMode parallelExecutionMode = ParallelExecutionMode.ForceParallelism;
				if (!((executionMode.GetValueOrDefault() == parallelExecutionMode) & (executionMode != null)))
				{
					return ExceptionAggregator.WrapEnumerable<TSource>(CancellableEnumerable.Wrap<TSource>(lastQueryOperator.Child.AsSequentialQuery(querySettings.CancellationState.ExternalCancellationToken), querySettings.CancellationState.ExternalCancellationToken), querySettings.CancellationState).LastOrDefault<TSource>();
				}
			}
			return ParallelEnumerable.GetOneWithPossibleDefault<TSource>(lastQueryOperator, false, true);
		}

		public static TSource LastOrDefault<TSource>(this ParallelQuery<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			LastQueryOperator<TSource> lastQueryOperator = new LastQueryOperator<TSource>(source, predicate);
			QuerySettings querySettings = lastQueryOperator.SpecifiedQuerySettings.WithDefaults();
			if (lastQueryOperator.LimitsParallelism)
			{
				ParallelExecutionMode? executionMode = querySettings.ExecutionMode;
				ParallelExecutionMode parallelExecutionMode = ParallelExecutionMode.ForceParallelism;
				if (!((executionMode.GetValueOrDefault() == parallelExecutionMode) & (executionMode != null)))
				{
					return ExceptionAggregator.WrapEnumerable<TSource>(CancellableEnumerable.Wrap<TSource>(lastQueryOperator.Child.AsSequentialQuery(querySettings.CancellationState.ExternalCancellationToken), querySettings.CancellationState.ExternalCancellationToken), querySettings.CancellationState).LastOrDefault<TSource>(ExceptionAggregator.WrapFunc<TSource, bool>(predicate, querySettings.CancellationState));
				}
			}
			return ParallelEnumerable.GetOneWithPossibleDefault<TSource>(lastQueryOperator, false, true);
		}

		public static TSource Single<TSource>(this ParallelQuery<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return ParallelEnumerable.GetOneWithPossibleDefault<TSource>(new SingleQueryOperator<TSource>(source, null), true, false);
		}

		public static TSource Single<TSource>(this ParallelQuery<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			return ParallelEnumerable.GetOneWithPossibleDefault<TSource>(new SingleQueryOperator<TSource>(source, predicate), true, false);
		}

		public static TSource SingleOrDefault<TSource>(this ParallelQuery<TSource> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return ParallelEnumerable.GetOneWithPossibleDefault<TSource>(new SingleQueryOperator<TSource>(source, null), true, true);
		}

		public static TSource SingleOrDefault<TSource>(this ParallelQuery<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			return ParallelEnumerable.GetOneWithPossibleDefault<TSource>(new SingleQueryOperator<TSource>(source, predicate), true, true);
		}

		public static ParallelQuery<TSource> DefaultIfEmpty<TSource>(this ParallelQuery<TSource> source)
		{
			return source.DefaultIfEmpty(default(TSource));
		}

		public static ParallelQuery<TSource> DefaultIfEmpty<TSource>(this ParallelQuery<TSource> source, TSource defaultValue)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new DefaultIfEmptyQueryOperator<TSource>(source, defaultValue);
		}

		public static TSource ElementAt<TSource>(this ParallelQuery<TSource> source, int index)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			TSource tsource;
			if (new ElementAtQueryOperator<TSource>(source, index).Aggregate(out tsource, false))
			{
				return tsource;
			}
			throw new ArgumentOutOfRangeException("index");
		}

		public static TSource ElementAtOrDefault<TSource>(this ParallelQuery<TSource> source, int index)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			TSource tsource;
			if (index >= 0 && new ElementAtQueryOperator<TSource>(source, index).Aggregate(out tsource, true))
			{
				return tsource;
			}
			return default(TSource);
		}

		private const string RIGHT_SOURCE_NOT_PARALLEL_STR = "The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.";
	}
}
