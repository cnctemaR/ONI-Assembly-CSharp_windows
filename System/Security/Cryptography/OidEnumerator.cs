using System;
using System.Collections;

namespace System.Security.Cryptography
{
	public sealed class OidEnumerator : IEnumerator
	{
		internal OidEnumerator(OidCollection collection)
		{
			this._collection = collection;
			this._position = -1;
		}

		object IEnumerator.Current
		{
			get
			{
				if (this._position < 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				return this._collection[this._position];
			}
		}

		public Oid Current
		{
			get
			{
				if (this._position < 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				return this._collection[this._position];
			}
		}

		public bool MoveNext()
		{
			if (++this._position < this._collection.Count)
			{
				return true;
			}
			this._position = this._collection.Count - 1;
			return false;
		}

		public void Reset()
		{
			this._position = -1;
		}

		private OidCollection _collection;

		private int _position;
	}
}
