using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Data
{
	public static class EnumerableRowCollectionExtensions
	{
		public static EnumerableRowCollection<TRow> Where<TRow>(this EnumerableRowCollection<TRow> source, Func<TRow, bool> predicate)
		{
			EnumerableRowCollection<TRow> enumerableRowCollection = new EnumerableRowCollection<TRow>(source, source.Where<TRow>(predicate), null);
			enumerableRowCollection.AddPredicate(predicate);
			return enumerableRowCollection;
		}

		public static OrderedEnumerableRowCollection<TRow> OrderBy<TRow, TKey>(this EnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector)
		{
			IEnumerable<TRow> enumerable = source.OrderBy<TRow, TKey>(keySelector);
			OrderedEnumerableRowCollection<TRow> orderedEnumerableRowCollection = new OrderedEnumerableRowCollection<TRow>(source, enumerable);
			orderedEnumerableRowCollection.AddSortExpression<TKey>(keySelector, false, true);
			return orderedEnumerableRowCollection;
		}

		public static OrderedEnumerableRowCollection<TRow> OrderBy<TRow, TKey>(this EnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector, IComparer<TKey> comparer)
		{
			IEnumerable<TRow> enumerable = source.OrderBy<TRow, TKey>(keySelector, comparer);
			OrderedEnumerableRowCollection<TRow> orderedEnumerableRowCollection = new OrderedEnumerableRowCollection<TRow>(source, enumerable);
			orderedEnumerableRowCollection.AddSortExpression<TKey>(keySelector, comparer, false, true);
			return orderedEnumerableRowCollection;
		}

		public static OrderedEnumerableRowCollection<TRow> OrderByDescending<TRow, TKey>(this EnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector)
		{
			IEnumerable<TRow> enumerable = source.OrderByDescending<TRow, TKey>(keySelector);
			OrderedEnumerableRowCollection<TRow> orderedEnumerableRowCollection = new OrderedEnumerableRowCollection<TRow>(source, enumerable);
			orderedEnumerableRowCollection.AddSortExpression<TKey>(keySelector, true, true);
			return orderedEnumerableRowCollection;
		}

		public static OrderedEnumerableRowCollection<TRow> OrderByDescending<TRow, TKey>(this EnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector, IComparer<TKey> comparer)
		{
			IEnumerable<TRow> enumerable = source.OrderByDescending<TRow, TKey>(keySelector, comparer);
			OrderedEnumerableRowCollection<TRow> orderedEnumerableRowCollection = new OrderedEnumerableRowCollection<TRow>(source, enumerable);
			orderedEnumerableRowCollection.AddSortExpression<TKey>(keySelector, comparer, true, true);
			return orderedEnumerableRowCollection;
		}

		public static OrderedEnumerableRowCollection<TRow> ThenBy<TRow, TKey>(this OrderedEnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector)
		{
			IEnumerable<TRow> enumerable = ((IOrderedEnumerable<TRow>)source.EnumerableRows).ThenBy<TRow, TKey>(keySelector);
			OrderedEnumerableRowCollection<TRow> orderedEnumerableRowCollection = new OrderedEnumerableRowCollection<TRow>(source, enumerable);
			orderedEnumerableRowCollection.AddSortExpression<TKey>(keySelector, false, false);
			return orderedEnumerableRowCollection;
		}

		public static OrderedEnumerableRowCollection<TRow> ThenBy<TRow, TKey>(this OrderedEnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector, IComparer<TKey> comparer)
		{
			IEnumerable<TRow> enumerable = ((IOrderedEnumerable<TRow>)source.EnumerableRows).ThenBy<TRow, TKey>(keySelector, comparer);
			OrderedEnumerableRowCollection<TRow> orderedEnumerableRowCollection = new OrderedEnumerableRowCollection<TRow>(source, enumerable);
			orderedEnumerableRowCollection.AddSortExpression<TKey>(keySelector, comparer, false, false);
			return orderedEnumerableRowCollection;
		}

		public static OrderedEnumerableRowCollection<TRow> ThenByDescending<TRow, TKey>(this OrderedEnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector)
		{
			IEnumerable<TRow> enumerable = ((IOrderedEnumerable<TRow>)source.EnumerableRows).ThenByDescending<TRow, TKey>(keySelector);
			OrderedEnumerableRowCollection<TRow> orderedEnumerableRowCollection = new OrderedEnumerableRowCollection<TRow>(source, enumerable);
			orderedEnumerableRowCollection.AddSortExpression<TKey>(keySelector, true, false);
			return orderedEnumerableRowCollection;
		}

		public static OrderedEnumerableRowCollection<TRow> ThenByDescending<TRow, TKey>(this OrderedEnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector, IComparer<TKey> comparer)
		{
			IEnumerable<TRow> enumerable = ((IOrderedEnumerable<TRow>)source.EnumerableRows).ThenByDescending<TRow, TKey>(keySelector, comparer);
			OrderedEnumerableRowCollection<TRow> orderedEnumerableRowCollection = new OrderedEnumerableRowCollection<TRow>(source, enumerable);
			orderedEnumerableRowCollection.AddSortExpression<TKey>(keySelector, comparer, true, false);
			return orderedEnumerableRowCollection;
		}

		public static EnumerableRowCollection<S> Select<TRow, S>(this EnumerableRowCollection<TRow> source, Func<TRow, S> selector)
		{
			IEnumerable<S> enumerable = source.Select<TRow, S>(selector);
			return new EnumerableRowCollection<S>(source as EnumerableRowCollection<S>, enumerable, selector as Func<S, S>);
		}

		public static EnumerableRowCollection<TResult> Cast<TResult>(this EnumerableRowCollection source)
		{
			if (source != null && source.ElementType.Equals(typeof(TResult)))
			{
				return (EnumerableRowCollection<TResult>)source;
			}
			return new EnumerableRowCollection<TResult>(source.Cast<TResult>(), typeof(TResult).IsAssignableFrom(source.ElementType) && typeof(DataRow).IsAssignableFrom(typeof(TResult)), source.Table);
		}
	}
}
