using System;
using System.Collections;
using Unity;

namespace System.Security.Cryptography
{
	public sealed class OidEnumerator : IEnumerator
	{
		internal OidEnumerator(OidCollection oids)
		{
			this._oids = oids;
			this._current = -1;
		}

		public Oid Current
		{
			get
			{
				return this._oids[this._current];
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		public bool MoveNext()
		{
			if (this._current >= this._oids.Count - 1)
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

		internal OidEnumerator()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private readonly OidCollection _oids;

		private int _current;
	}
}
