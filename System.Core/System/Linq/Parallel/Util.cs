using System;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal static class Util
	{
		internal static int Sign(int x)
		{
			if (x < 0)
			{
				return -1;
			}
			if (x != 0)
			{
				return 1;
			}
			return 0;
		}

		internal static Comparer<TKey> GetDefaultComparer<TKey>()
		{
			if (typeof(TKey) == typeof(int))
			{
				return (Comparer<TKey>)Util.s_fastIntComparer;
			}
			if (typeof(TKey) == typeof(long))
			{
				return (Comparer<TKey>)Util.s_fastLongComparer;
			}
			if (typeof(TKey) == typeof(float))
			{
				return (Comparer<TKey>)Util.s_fastFloatComparer;
			}
			if (typeof(TKey) == typeof(double))
			{
				return (Comparer<TKey>)Util.s_fastDoubleComparer;
			}
			if (typeof(TKey) == typeof(DateTime))
			{
				return (Comparer<TKey>)Util.s_fastDateTimeComparer;
			}
			return Comparer<TKey>.Default;
		}

		private static Util.FastIntComparer s_fastIntComparer = new Util.FastIntComparer();

		private static Util.FastLongComparer s_fastLongComparer = new Util.FastLongComparer();

		private static Util.FastFloatComparer s_fastFloatComparer = new Util.FastFloatComparer();

		private static Util.FastDoubleComparer s_fastDoubleComparer = new Util.FastDoubleComparer();

		private static Util.FastDateTimeComparer s_fastDateTimeComparer = new Util.FastDateTimeComparer();

		private class FastIntComparer : Comparer<int>
		{
			public override int Compare(int x, int y)
			{
				return x.CompareTo(y);
			}
		}

		private class FastLongComparer : Comparer<long>
		{
			public override int Compare(long x, long y)
			{
				return x.CompareTo(y);
			}
		}

		private class FastFloatComparer : Comparer<float>
		{
			public override int Compare(float x, float y)
			{
				return x.CompareTo(y);
			}
		}

		private class FastDoubleComparer : Comparer<double>
		{
			public override int Compare(double x, double y)
			{
				return x.CompareTo(y);
			}
		}

		private class FastDateTimeComparer : Comparer<DateTime>
		{
			public override int Compare(DateTime x, DateTime y)
			{
				return x.CompareTo(y);
			}
		}
	}
}
