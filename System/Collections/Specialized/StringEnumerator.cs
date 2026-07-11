using System;

namespace System.Collections.Specialized
{
	public class StringEnumerator
	{
		internal StringEnumerator(StringCollection mappings)
		{
			this.temp = mappings;
			this.baseEnumerator = this.temp.GetEnumerator();
		}

		public string Current
		{
			get
			{
				return (string)this.baseEnumerator.Current;
			}
		}

		public bool MoveNext()
		{
			return this.baseEnumerator.MoveNext();
		}

		public void Reset()
		{
			this.baseEnumerator.Reset();
		}

		private IEnumerator baseEnumerator;

		private IEnumerable temp;
	}
}
