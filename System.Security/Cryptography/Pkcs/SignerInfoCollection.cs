using System;
using System.Collections;
using System.Security.Cryptography.Pkcs.Asn1;
using System.Security.Cryptography.Xml;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class SignerInfoCollection : ICollection, IEnumerable
	{
		internal SignerInfoCollection()
		{
			this._signerInfos = Array.Empty<SignerInfo>();
		}

		internal SignerInfoCollection(SignerInfo[] signerInfos)
		{
			this._signerInfos = signerInfos;
		}

		internal SignerInfoCollection(SignerInfoAsn[] signedDataSignerInfos, SignedCms ownerDocument)
		{
			this._signerInfos = new SignerInfo[signedDataSignerInfos.Length];
			for (int i = 0; i < signedDataSignerInfos.Length; i++)
			{
				this._signerInfos[i] = new SignerInfo(ref signedDataSignerInfos[i], ownerDocument);
			}
		}

		public SignerInfo this[int index]
		{
			get
			{
				if (index < 0 || index >= this._signerInfos.Length)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this._signerInfos[index];
			}
		}

		public int Count
		{
			get
			{
				return this._signerInfos.Length;
			}
		}

		public SignerInfoEnumerator GetEnumerator()
		{
			return new SignerInfoEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new SignerInfoEnumerator(this);
		}

		public void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", "array");
			}
			if (index < 0 || index >= array.Length)
			{
				throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (index + this.Count > array.Length)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			for (int i = 0; i < this.Count; i++)
			{
				array.SetValue(this[i], index + i);
			}
		}

		public void CopyTo(SignerInfo[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		internal int FindIndexForSigner(SignerInfo signer)
		{
			SubjectIdentifier signerIdentifier = signer.SignerIdentifier;
			X509IssuerSerial x509IssuerSerial = default(X509IssuerSerial);
			if (signerIdentifier.Type == SubjectIdentifierType.IssuerAndSerialNumber)
			{
				x509IssuerSerial = (X509IssuerSerial)signerIdentifier.Value;
			}
			for (int i = 0; i < this._signerInfos.Length; i++)
			{
				SubjectIdentifier signerIdentifier2 = this._signerInfos[i].SignerIdentifier;
				if (signerIdentifier2.Type == signerIdentifier.Type)
				{
					bool flag = false;
					switch (signerIdentifier.Type)
					{
					case SubjectIdentifierType.IssuerAndSerialNumber:
					{
						X509IssuerSerial x509IssuerSerial2 = (X509IssuerSerial)signerIdentifier2.Value;
						if (x509IssuerSerial2.IssuerName == x509IssuerSerial.IssuerName && x509IssuerSerial2.SerialNumber == x509IssuerSerial.SerialNumber)
						{
							flag = true;
						}
						break;
					}
					case SubjectIdentifierType.SubjectKeyIdentifier:
						if ((string)signerIdentifier.Value == (string)signerIdentifier2.Value)
						{
							flag = true;
						}
						break;
					case SubjectIdentifierType.NoSignature:
						flag = true;
						break;
					default:
						throw new CryptographicException();
					}
					if (flag)
					{
						return i;
					}
				}
			}
			return -1;
		}

		private readonly SignerInfo[] _signerInfos;
	}
}
