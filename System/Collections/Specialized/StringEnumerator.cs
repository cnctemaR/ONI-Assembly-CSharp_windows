using System;

namespace System.Collections.Specialized
{
	public class StringEnumerator
	{
		internal StringEnumerator(StringCollection coll)
		{
			this.enumerable = ((IEnumerable)coll).GetEnumerator();
		}

		public string Current
		{
			get
			{
				return (string)this.enumerable.Current;
			}
		}

		public bool MoveNext()
		{
			return this.enumerable.MoveNext();
		}

		public void Reset()
		{
			this.enumerable.Reset();
		}

		private IEnumerator enumerable;
	}
}
