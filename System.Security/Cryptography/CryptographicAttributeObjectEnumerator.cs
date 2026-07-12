using System;
using System.Collections;
using Unity;

namespace System.Security.Cryptography
{
	public sealed class CryptographicAttributeObjectEnumerator : IEnumerator
	{
		internal CryptographicAttributeObjectEnumerator(CryptographicAttributeObjectCollection attributes)
		{
			this._attributes = attributes;
			this._current = -1;
		}

		public CryptographicAttributeObject Current
		{
			get
			{
				return this._attributes[this._current];
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this._attributes[this._current];
			}
		}

		public bool MoveNext()
		{
			if (this._current >= this._attributes.Count - 1)
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

		internal CryptographicAttributeObjectEnumerator()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private readonly CryptographicAttributeObjectCollection _attributes;

		private int _current;
	}
}
