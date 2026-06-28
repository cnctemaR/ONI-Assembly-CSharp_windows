using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace FileHelpers
{
	[DelimitedRecord(",")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public sealed class RecordIndexer : IEnumerable<string>, IEnumerable
	{
		internal RecordIndexer()
		{
		}

		public int FieldCount
		{
			get
			{
				return this.values.Length;
			}
		}

		public string this[int index]
		{
			get
			{
				return this.values[index];
			}
		}

		IEnumerator<string> IEnumerable<string>.GetEnumerator()
		{
			return new RecordIndexer.ArrayEnumerator(this.values);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<string>)this).GetEnumerator();
		}

		[FieldQuoted(QuoteMode.OptionalForRead, MultilineMode.AllowForRead)]
		private readonly string[] values;

		private sealed class ArrayEnumerator : IEnumerator<string>, IDisposable, IEnumerator
		{
			public ArrayEnumerator(string[] values)
			{
				this.mValues = values;
				this.i = -1;
			}

			string IEnumerator<string>.Current
			{
				get
				{
					return this.mValues[this.i];
				}
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				this.i++;
				return this.i < this.mValues.Length;
			}

			public void Reset()
			{
				this.i = -1;
			}

			public object Current
			{
				get
				{
					return this.mValues[this.i];
				}
			}

			private readonly string[] mValues;

			private int i;
		}
	}
}
