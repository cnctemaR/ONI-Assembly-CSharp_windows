using System;
using System.Collections.Generic;

namespace System.Data
{
	public static class TypedTableBaseExtensions
	{
		public static EnumerableRowCollection<TRow> Where<TRow>(this TypedTableBase<TRow> source, Func<TRow, bool> predicate) where TRow : DataRow
		{
			DataSetUtil.CheckArgumentNull<TypedTableBase<TRow>>(source, "source");
			return new EnumerableRowCollection<TRow>(source).Where<TRow>(predicate);
		}

		public static OrderedEnumerableRowCollection<TRow> OrderBy<TRow, TKey>(this TypedTableBase<TRow> source, Func<TRow, TKey> keySelector) where TRow : DataRow
		{
			DataSetUtil.CheckArgumentNull<TypedTableBase<TRow>>(source, "source");
			return new EnumerableRowCollection<TRow>(source).OrderBy<TRow, TKey>(keySelector);
		}

		public static OrderedEnumerableRowCollection<TRow> OrderBy<TRow, TKey>(this TypedTableBase<TRow> source, Func<TRow, TKey> keySelector, IComparer<TKey> comparer) where TRow : DataRow
		{
			DataSetUtil.CheckArgumentNull<TypedTableBase<TRow>>(source, "source");
			return new EnumerableRowCollection<TRow>(source).OrderBy<TRow, TKey>(keySelector, comparer);
		}

		public static OrderedEnumerableRowCollection<TRow> OrderByDescending<TRow, TKey>(this TypedTableBase<TRow> source, Func<TRow, TKey> keySelector) where TRow : DataRow
		{
			DataSetUtil.CheckArgumentNull<TypedTableBase<TRow>>(source, "source");
			return new EnumerableRowCollection<TRow>(source).OrderByDescending<TRow, TKey>(keySelector);
		}

		public static OrderedEnumerableRowCollection<TRow> OrderByDescending<TRow, TKey>(this TypedTableBase<TRow> source, Func<TRow, TKey> keySelector, IComparer<TKey> comparer) where TRow : DataRow
		{
			DataSetUtil.CheckArgumentNull<TypedTableBase<TRow>>(source, "source");
			return new EnumerableRowCollection<TRow>(source).OrderByDescending<TRow, TKey>(keySelector, comparer);
		}

		public static EnumerableRowCollection<S> Select<TRow, S>(this TypedTableBase<TRow> source, Func<TRow, S> selector) where TRow : DataRow
		{
			DataSetUtil.CheckArgumentNull<TypedTableBase<TRow>>(source, "source");
			return new EnumerableRowCollection<TRow>(source).Select<TRow, S>(selector);
		}

		public static EnumerableRowCollection<TRow> AsEnumerable<TRow>(this TypedTableBase<TRow> source) where TRow : DataRow
		{
			DataSetUtil.CheckArgumentNull<TypedTableBase<TRow>>(source, "source");
			return new EnumerableRowCollection<TRow>(source);
		}

		public static TRow ElementAtOrDefault<TRow>(this TypedTableBase<TRow> source, int index) where TRow : DataRow
		{
			if (index >= 0 && index < source.Rows.Count)
			{
				return (TRow)((object)source.Rows[index]);
			}
			return default(TRow);
		}
	}
}
