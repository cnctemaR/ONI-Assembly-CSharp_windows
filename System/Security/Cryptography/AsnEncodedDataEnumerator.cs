using System;
using System.Collections;
using Unity;

namespace System.Security.Cryptography
{
	public sealed class AsnEncodedDataEnumerator : IEnumerator
	{
		internal AsnEncodedDataEnumerator(AsnEncodedDataCollection collection)
		{
			this._collection = collection;
			this._position = -1;
		}

		public AsnEncodedData Current
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

		public bool MoveNext()
		{
			int num = this._position + 1;
			this._position = num;
			if (num < this._collection.Count)
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

		internal AsnEncodedDataEnumerator()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private AsnEncodedDataCollection _collection;

		private int _position;
	}
}
