using System;
using Unity;

namespace System.Collections.Specialized
{
	public class StringEnumerator
	{
		internal StringEnumerator(StringCollection mappings)
		{
			this._temp = mappings;
			this._baseEnumerator = this._temp.GetEnumerator();
		}

		public string Current
		{
			get
			{
				return (string)this._baseEnumerator.Current;
			}
		}

		public bool MoveNext()
		{
			return this._baseEnumerator.MoveNext();
		}

		public void Reset()
		{
			this._baseEnumerator.Reset();
		}

		internal StringEnumerator()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private IEnumerator _baseEnumerator;

		private IEnumerable _temp;
	}
}
