using System;
using System.ComponentModel;
using System.Text;

namespace FileHelpers
{
	public sealed class BigFileSorter : BigFileSorter<BigFileSorter.SorterRecord>
	{
		public BigFileSorter()
			: this(0)
		{
		}

		public BigFileSorter(int blockFileSizeInBytes)
			: this(null, null, blockFileSizeInBytes)
		{
		}

		public BigFileSorter(Encoding encoding)
			: this(null, encoding, 0)
		{
		}

		public BigFileSorter(Comparison<string> sorter)
			: this(sorter, 0)
		{
		}

		public BigFileSorter(Comparison<string> sorter, int blockFileSizeInBytes)
			: this(sorter, null, blockFileSizeInBytes)
		{
		}

		public BigFileSorter(Comparison<string> sorter, Encoding encoding, int blockFileSizeInBytes)
			: base(BigFileSorter.CreateSorter(sorter), encoding, blockFileSizeInBytes)
		{
		}

		private static Comparison<BigFileSorter.SorterRecord> CreateSorter(Comparison<string> sorter)
		{
			if (sorter == null)
			{
				return null;
			}
			return (BigFileSorter.SorterRecord x, BigFileSorter.SorterRecord y) => sorter(x.Value, y.Value);
		}

		[DelimitedRecord("\r\n")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public sealed class SorterRecord : IComparable<BigFileSorter.SorterRecord>
		{
			public int CompareTo(BigFileSorter.SorterRecord other)
			{
				return string.Compare(this.Value, other.Value, StringComparison.Ordinal);
			}

			internal string Value;
		}
	}
}
