using System;
using System.Collections;
using Unity;

namespace System.Security.Cryptography
{
	public sealed class AsnEncodedDataEnumerator : IEnumerator
	{
		internal AsnEncodedDataEnumerator(AsnEncodedDataCollection asnEncodedDatas)
		{
			this._asnEncodedDatas = asnEncodedDatas;
			this._current = -1;
		}

		public AsnEncodedData Current
		{
			get
			{
				return this._asnEncodedDatas[this._current];
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this._asnEncodedDatas[this._current];
			}
		}

		public bool MoveNext()
		{
			if (this._current >= this._asnEncodedDatas.Count - 1)
			{
				return false;
			}
			this._current++;
			return true;
		}

		public void Reset()
		{
			this._current = -1;
		}

		internal AsnEncodedDataEnumerator()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private readonly AsnEncodedDataCollection _asnEncodedDatas;

		private int _current;
	}
}
