using System;
using System.Collections;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class SignerInfoEnumerator : IEnumerator
	{
		private SignerInfoEnumerator()
		{
		}

		internal SignerInfoEnumerator(SignerInfoCollection signerInfos)
		{
			this._signerInfos = signerInfos;
			this._position = -1;
		}

		public SignerInfo Current
		{
			get
			{
				return this._signerInfos[this._position];
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this._signerInfos[this._position];
			}
		}

		public bool MoveNext()
		{
			int num = this._position + 1;
			if (num >= this._signerInfos.Count)
			{
				return false;
			}
			this._position = num;
			return true;
		}

		public void Reset()
		{
			this._position = -1;
		}

		private readonly SignerInfoCollection _signerInfos;

		private int _position;
	}
}
