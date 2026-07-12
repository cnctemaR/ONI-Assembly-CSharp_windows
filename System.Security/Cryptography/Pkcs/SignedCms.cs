using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Pkcs.Asn1;
using System.Security.Cryptography.X509Certificates;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class SignedCms
	{
		private static ContentInfo MakeEmptyContentInfo()
		{
			return new ContentInfo(new Oid(SignedCms.s_cmsDataOid), Array.Empty<byte>());
		}

		public SignedCms()
			: this(SubjectIdentifierType.IssuerAndSerialNumber, SignedCms.MakeEmptyContentInfo(), false)
		{
		}

		public SignedCms(SubjectIdentifierType signerIdentifierType)
			: this(signerIdentifierType, SignedCms.MakeEmptyContentInfo(), false)
		{
		}

		public SignedCms(ContentInfo contentInfo)
			: this(SubjectIdentifierType.IssuerAndSerialNumber, contentInfo, false)
		{
		}

		public SignedCms(SubjectIdentifierType signerIdentifierType, ContentInfo contentInfo)
			: this(signerIdentifierType, contentInfo, false)
		{
		}

		public SignedCms(ContentInfo contentInfo, bool detached)
			: this(SubjectIdentifierType.IssuerAndSerialNumber, contentInfo, detached)
		{
		}

		public int Version { get; private set; }

		public ContentInfo ContentInfo { get; private set; }

		public bool Detached { get; private set; }

		public SignedCms(SubjectIdentifierType signerIdentifierType, ContentInfo contentInfo, bool detached)
		{
			if (contentInfo == null)
			{
				throw new ArgumentNullException("contentInfo");
			}
			if (contentInfo.Content == null)
			{
				throw new ArgumentNullException("contentInfo.Content");
			}
			this.ContentInfo = contentInfo;
			this.Detached = detached;
			this.Version = 0;
		}

		public X509Certificate2Collection Certificates
		{
			get
			{
				X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
				if (!this._hasData)
				{
					return x509Certificate2Collection;
				}
				CertificateChoiceAsn[] certificateSet = this._signedData.CertificateSet;
				if (certificateSet == null)
				{
					return x509Certificate2Collection;
				}
				foreach (CertificateChoiceAsn certificateChoiceAsn in certificateSet)
				{
					x509Certificate2Collection.Add(new X509Certificate2(certificateChoiceAsn.Certificate.Value.ToArray()));
				}
				return x509Certificate2Collection;
			}
		}

		public SignerInfoCollection SignerInfos
		{
			get
			{
				if (!this._hasData)
				{
					return new SignerInfoCollection();
				}
				return new SignerInfoCollection(this._signedData.SignerInfos, this);
			}
		}

		public byte[] Encode()
		{
			if (!this._hasData)
			{
				throw new InvalidOperationException("The CMS message is not signed.");
			}
			byte[] array;
			try
			{
				array = Internal.Cryptography.Helpers.EncodeContentInfo<SignedDataAsn>(this._signedData, "1.2.840.113549.1.7.2", AsnEncodingRules.DER);
			}
			catch (CryptographicException)
			{
				if (this.Detached)
				{
					throw;
				}
				SignedDataAsn signedDataAsn = this._signedData;
				signedDataAsn.EncapContentInfo.Content = null;
				using (AsnWriter asnWriter = AsnSerializer.Serialize<SignedDataAsn>(signedDataAsn, AsnEncodingRules.DER))
				{
					signedDataAsn = AsnSerializer.Deserialize<SignedDataAsn>(asnWriter.Encode(), AsnEncodingRules.BER);
				}
				signedDataAsn.EncapContentInfo.Content = this._signedData.EncapContentInfo.Content;
				array = Internal.Cryptography.Helpers.EncodeContentInfo<SignedDataAsn>(signedDataAsn, "1.2.840.113549.1.7.2", AsnEncodingRules.BER);
			}
			return array;
		}

		public void Decode(byte[] encodedMessage)
		{
			if (encodedMessage == null)
			{
				throw new ArgumentNullException("encodedMessage");
			}
			this.Decode(new ReadOnlyMemory<byte>(encodedMessage));
		}

		internal void Decode(ReadOnlyMemory<byte> encodedMessage)
		{
			int num;
			ContentInfoAsn contentInfoAsn = AsnSerializer.Deserialize<ContentInfoAsn>(encodedMessage, AsnEncodingRules.BER, out num);
			if (contentInfoAsn.ContentType != "1.2.840.113549.1.7.2")
			{
				throw new CryptographicException("Invalid cryptographic message type.");
			}
			this._heldData = contentInfoAsn.Content.ToArray();
			this._signedData = AsnSerializer.Deserialize<SignedDataAsn>(this._heldData, AsnEncodingRules.BER);
			this._contentType = this._signedData.EncapContentInfo.ContentType;
			this._hasPkcs7Content = false;
			if (!this.Detached)
			{
				ReadOnlyMemory<byte>? content = this._signedData.EncapContentInfo.Content;
				ReadOnlyMemory<byte> readOnlyMemory;
				if (content != null)
				{
					readOnlyMemory = SignedCms.GetContent(content.Value, this._contentType);
					this._hasPkcs7Content = content.Value.Length == readOnlyMemory.Length;
				}
				else
				{
					readOnlyMemory = ReadOnlyMemory<byte>.Empty;
				}
				this._heldContent = new ReadOnlyMemory<byte>?(readOnlyMemory);
				this.ContentInfo = new ContentInfo(new Oid(this._contentType), readOnlyMemory.ToArray());
			}
			else
			{
				this._heldContent = new ReadOnlyMemory<byte>?(this.ContentInfo.Content.CloneByteArray());
			}
			this.Version = this._signedData.Version;
			this._hasData = true;
		}

		internal static ReadOnlyMemory<byte> GetContent(ReadOnlyMemory<byte> wrappedContent, string contentType)
		{
			byte[] array = null;
			int num = 0;
			try
			{
				AsnReader asnReader = new AsnReader(wrappedContent, AsnEncodingRules.BER);
				ReadOnlyMemory<byte> readOnlyMemory;
				if (asnReader.TryGetPrimitiveOctetStringBytes(out readOnlyMemory))
				{
					return readOnlyMemory;
				}
				array = ArrayPool<byte>.Shared.Rent(wrappedContent.Length);
				if (!asnReader.TryCopyOctetStringBytes(array, out num))
				{
					throw new CryptographicException();
				}
				return array.AsSpan<byte>(0, num).ToArray();
			}
			catch (Exception)
			{
				if (contentType == "1.2.840.113549.1.7.1")
				{
					throw;
				}
			}
			finally
			{
				if (array != null)
				{
					array.AsSpan<byte>(0, num).Clear();
					ArrayPool<byte>.Shared.Return(array, false);
				}
			}
			return wrappedContent;
		}

		public void ComputeSignature()
		{
			throw new PlatformNotSupportedException("No signer certificate was provided. This platform does not implement the certificate picker UI.");
		}

		public void ComputeSignature(CmsSigner signer)
		{
			this.ComputeSignature(signer, true);
		}

		public void ComputeSignature(CmsSigner signer, bool silent)
		{
			if (signer == null)
			{
				throw new ArgumentNullException("signer");
			}
			if (this.ContentInfo.Content.Length == 0)
			{
				throw new CryptographicException("Cannot create CMS signature for empty content.");
			}
			ReadOnlyMemory<byte> readOnlyMemory = this._heldContent ?? this.ContentInfo.Content;
			string text;
			if ((text = this._contentType) == null)
			{
				text = this.ContentInfo.ContentType.Value ?? "1.2.840.113549.1.7.1";
			}
			string text2 = text;
			X509Certificate2Collection x509Certificate2Collection;
			SignerInfoAsn signerInfoAsn = signer.Sign(readOnlyMemory, text2, silent, out x509Certificate2Collection);
			bool flag = false;
			if (!this._hasData)
			{
				flag = true;
				this._signedData = new SignedDataAsn
				{
					DigestAlgorithms = Array.Empty<AlgorithmIdentifierAsn>(),
					SignerInfos = Array.Empty<SignerInfoAsn>(),
					EncapContentInfo = new EncapsulatedContentInfoAsn
					{
						ContentType = text2
					}
				};
				if (!this.Detached)
				{
					using (AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER))
					{
						asnWriter.WriteOctetString(readOnlyMemory.Span);
						this._signedData.EncapContentInfo.Content = new ReadOnlyMemory<byte>?(asnWriter.Encode());
					}
				}
				this._hasData = true;
			}
			int num = this._signedData.SignerInfos.Length;
			Array.Resize<SignerInfoAsn>(ref this._signedData.SignerInfos, num + 1);
			this._signedData.SignerInfos[num] = signerInfoAsn;
			this.UpdateCertificatesFromAddition(x509Certificate2Collection);
			this.ConsiderDigestAddition(signerInfoAsn.DigestAlgorithm);
			this.UpdateMetadata();
			if (flag)
			{
				this.Reencode();
			}
		}

		public void RemoveSignature(int index)
		{
			if (!this._hasData)
			{
				throw new InvalidOperationException("The CMS message is not signed.");
			}
			if (index < 0 || index >= this._signedData.SignerInfos.Length)
			{
				throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			AlgorithmIdentifierAsn digestAlgorithm = this._signedData.SignerInfos[index].DigestAlgorithm;
			Internal.Cryptography.Helpers.RemoveAt<SignerInfoAsn>(ref this._signedData.SignerInfos, index);
			this.ConsiderDigestRemoval(digestAlgorithm);
			this.UpdateMetadata();
		}

		public void RemoveSignature(SignerInfo signerInfo)
		{
			if (signerInfo == null)
			{
				throw new ArgumentNullException("signerInfo");
			}
			int num = this.SignerInfos.FindIndexForSigner(signerInfo);
			if (num < 0)
			{
				throw new CryptographicException("Cannot find the original signer.");
			}
			this.RemoveSignature(num);
		}

		internal ReadOnlySpan<byte> GetHashableContentSpan()
		{
			ReadOnlyMemory<byte> value = this._heldContent.Value;
			if (!this._hasPkcs7Content)
			{
				return value.Span;
			}
			return new AsnReader(value, AsnEncodingRules.BER).PeekContentBytes().Span;
		}

		internal void Reencode()
		{
			ContentInfo contentInfo = this.ContentInfo;
			try
			{
				byte[] array = this.Encode();
				if (this.Detached)
				{
					this._heldContent = null;
				}
				this.Decode(array);
			}
			finally
			{
				this.ContentInfo = contentInfo;
			}
		}

		private void UpdateMetadata()
		{
			int num = 1;
			if ((this._contentType ?? this.ContentInfo.ContentType.Value) != "1.2.840.113549.1.7.1")
			{
				num = 3;
			}
			else if (this._signedData.SignerInfos.Any<SignerInfoAsn>((SignerInfoAsn si) => si.Version == 3))
			{
				num = 3;
			}
			this.Version = num;
			this._signedData.Version = num;
		}

		private void ConsiderDigestAddition(AlgorithmIdentifierAsn candidate)
		{
			int num = this._signedData.DigestAlgorithms.Length;
			for (int i = 0; i < num; i++)
			{
				ref AlgorithmIdentifierAsn ptr = ref this._signedData.DigestAlgorithms[i];
				if (candidate.Equals(ref ptr))
				{
					return;
				}
			}
			Array.Resize<AlgorithmIdentifierAsn>(ref this._signedData.DigestAlgorithms, num + 1);
			this._signedData.DigestAlgorithms[num] = candidate;
		}

		private void ConsiderDigestRemoval(AlgorithmIdentifierAsn candidate)
		{
			bool flag = true;
			for (int i = 0; i < this._signedData.SignerInfos.Length; i++)
			{
				ref AlgorithmIdentifierAsn ptr = ref this._signedData.SignerInfos[i].DigestAlgorithm;
				if (candidate.Equals(ref ptr))
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			for (int j = 0; j < this._signedData.DigestAlgorithms.Length; j++)
			{
				ref AlgorithmIdentifierAsn ptr2 = ref this._signedData.DigestAlgorithms[j];
				if (candidate.Equals(ref ptr2))
				{
					Internal.Cryptography.Helpers.RemoveAt<AlgorithmIdentifierAsn>(ref this._signedData.DigestAlgorithms, j);
					return;
				}
			}
		}

		internal void UpdateCertificatesFromAddition(X509Certificate2Collection newCerts)
		{
			if (newCerts.Count == 0)
			{
				return;
			}
			CertificateChoiceAsn[] certificateSet = this._signedData.CertificateSet;
			int num = ((certificateSet != null) ? certificateSet.Length : 0);
			if (num > 0 || newCerts.Count > 1)
			{
				HashSet<X509Certificate2> hashSet = new HashSet<X509Certificate2>(this.Certificates.OfType<X509Certificate2>());
				for (int i = 0; i < newCerts.Count; i++)
				{
					X509Certificate2 x509Certificate = newCerts[i];
					if (!hashSet.Add(x509Certificate))
					{
						newCerts.RemoveAt(i);
						i--;
					}
				}
			}
			if (newCerts.Count == 0)
			{
				return;
			}
			if (this._signedData.CertificateSet == null)
			{
				this._signedData.CertificateSet = new CertificateChoiceAsn[newCerts.Count];
			}
			else
			{
				Array.Resize<CertificateChoiceAsn>(ref this._signedData.CertificateSet, num + newCerts.Count);
			}
			for (int j = num; j < this._signedData.CertificateSet.Length; j++)
			{
				this._signedData.CertificateSet[j] = new CertificateChoiceAsn
				{
					Certificate = new ReadOnlyMemory<byte>?(newCerts[j - num].RawData)
				};
			}
		}

		public void CheckSignature(bool verifySignatureOnly)
		{
			this.CheckSignature(new X509Certificate2Collection(), verifySignatureOnly);
		}

		public void CheckSignature(X509Certificate2Collection extraStore, bool verifySignatureOnly)
		{
			if (!this._hasData)
			{
				throw new InvalidOperationException("The CMS message is not signed.");
			}
			if (extraStore == null)
			{
				throw new ArgumentNullException("extraStore");
			}
			SignedCms.CheckSignatures(this.SignerInfos, extraStore, verifySignatureOnly);
		}

		private static void CheckSignatures(SignerInfoCollection signers, X509Certificate2Collection extraStore, bool verifySignatureOnly)
		{
			if (signers.Count < 1)
			{
				throw new CryptographicException("The signed cryptographic message does not have a signer for the specified signer index.");
			}
			foreach (SignerInfo signerInfo in signers)
			{
				signerInfo.CheckSignature(extraStore, verifySignatureOnly);
				SignerInfoCollection counterSignerInfos = signerInfo.CounterSignerInfos;
				if (counterSignerInfos.Count > 0)
				{
					SignedCms.CheckSignatures(counterSignerInfos, extraStore, verifySignatureOnly);
				}
			}
		}

		public void CheckHash()
		{
			if (!this._hasData)
			{
				throw new InvalidOperationException("The CMS message is not signed.");
			}
			SignerInfoCollection signerInfos = this.SignerInfos;
			if (signerInfos.Count < 1)
			{
				throw new CryptographicException("The signed cryptographic message does not have a signer for the specified signer index.");
			}
			foreach (SignerInfo signerInfo in signerInfos)
			{
				if (signerInfo.SignerIdentifier.Type == SubjectIdentifierType.NoSignature)
				{
					signerInfo.CheckHash();
				}
			}
		}

		internal ref SignedDataAsn GetRawData()
		{
			return ref this._signedData;
		}

		private static readonly Oid s_cmsDataOid = Oid.FromOidValue("1.2.840.113549.1.7.1", OidGroup.ExtensionOrAttribute);

		private SignedDataAsn _signedData;

		private bool _hasData;

		private Memory<byte> _heldData;

		private ReadOnlyMemory<byte>? _heldContent;

		private bool _hasPkcs7Content;

		private string _contentType;
	}
}
