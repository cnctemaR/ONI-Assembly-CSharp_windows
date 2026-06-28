using System;
using System.Collections.Generic;
using System.Globalization;

namespace TiledSharp
{
	public class Tuple<T1, T2>
	{
		public T1 Item1 { get; private set; }

		public T2 Item2 { get; private set; }

		internal Tuple(T1 first, T2 second)
		{
			this.Item1 = first;
			this.Item2 = second;
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (!object.ReferenceEquals(this.Item1, null))
			{
				num = Tuple<T1, T2>.Item1Comparer.GetHashCode(this.Item1);
			}
			if (!object.ReferenceEquals(this.Item2, null))
			{
				num = (num << 3) ^ Tuple<T1, T2>.Item2Comparer.GetHashCode(this.Item2);
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			Tuple<T1, T2> tuple = obj as Tuple<T1, T2>;
			return !object.ReferenceEquals(tuple, null) && Tuple<T1, T2>.Item1Comparer.Equals(this.Item1, tuple.Item1) && Tuple<T1, T2>.Item2Comparer.Equals(this.Item2, tuple.Item2);
		}

		public override string ToString()
		{
			return this.ToString(null, CultureInfo.CurrentCulture);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format(formatProvider, format ?? "{0},{1}", new object[] { this.Item1, this.Item2 });
		}

		private static readonly IEqualityComparer<T1> Item1Comparer = EqualityComparer<T1>.Default;

		private static readonly IEqualityComparer<T2> Item2Comparer = EqualityComparer<T2>.Default;
	}
}
