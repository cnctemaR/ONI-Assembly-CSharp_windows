using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Pkcs.Asn1;
using System.Security.Cryptography.X509Certificates;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs
{
	internal abstract class CmsSignature
	{
		static CmsSignature()
		{
			CmsSignature.PrepareRegistrationRsa(CmsSignature.s_lookup);
			CmsSignature.PrepareRegistrationDsa(CmsSignature.s_lookup);
			CmsSignature.PrepareRegistrationECDsa(CmsSignature.s_lookup);
		}

		private static void PrepareRegistrationRsa(Dictionary<string, CmsSignature> lookup)
		{
			lookup.Add("1.2.840.113549.1.1.1", new CmsSignature.RSAPkcs1CmsSignature(null, null));
			lookup.Add("1.2.840.113549.1.1.5", new CmsSignature.RSAPkcs1CmsSignature("1.2.840.113549.1.1.5", new HashAlgorithmName?(HashAlgorithmName.SHA1)));
			lookup.Add("1.2.840.113549.1.1.11", new CmsSignature.RSAPkcs1CmsSignature("1.2.840.113549.1.1.11", new HashAlgorithmName?(HashAlgorithmName.SHA256)));
			lookup.Add("1.2.840.113549.1.1.12", new CmsSignature.RSAPkcs1CmsSignature("1.2.840.113549.1.1.12", new HashAlgorithmName?(HashAlgorithmName.SHA384)));
			lookup.Add("1.2.840.113549.1.1.13", new CmsSignature.RSAPkcs1CmsSignature("1.2.840.113549.1.1.13", new HashAlgorithmName?(HashAlgorithmName.SHA512)));
			lookup.Add("1.2.840.113549.1.1.10", new CmsSignature.RSAPssCmsSignature());
		}

		private static void PrepareRegistrationDsa(Dictionary<string, CmsSignature> lookup)
		{
			lookup.Add("1.2.840.10040.4.3", new CmsSignature.DSACmsSignature("1.2.840.10040.4.3", HashAlgorithmName.SHA1));
			lookup.Add("2.16.840.1.101.3.4.3.2", new CmsSignature.DSACmsSignature("2.16.840.1.101.3.4.3.2", HashAlgorithmName.SHA256));
			lookup.Add("2.16.840.1.101.3.4.3.3", new CmsSignature.DSACmsSignature("2.16.840.1.101.3.4.3.3", HashAlgorithmName.SHA384));
			lookup.Add("2.16.840.1.101.3.4.3.4", new CmsSignature.DSACmsSignature("2.16.840.1.101.3.4.3.4", HashAlgorithmName.SHA512));
			lookup.Add("1.2.840.10040.4.1", new CmsSignature.DSACmsSignature(null, default(HashAlgorithmName)));
		}

		private static void PrepareRegistrationECDsa(Dictionary<string, CmsSignature> lookup)
		{
			lookup.Add("1.2.840.10045.4.1", new CmsSignature.ECDsaCmsSignature("1.2.840.10045.4.1", HashAlgorithmName.SHA1));
			lookup.Add("1.2.840.10045.4.3.2", new CmsSignature.ECDsaCmsSignature("1.2.840.10045.4.3.2", HashAlgorithmName.SHA256));
			lookup.Add("1.2.840.10045.4.3.3", new CmsSignature.ECDsaCmsSignature("1.2.840.10045.4.3.3", HashAlgorithmName.SHA384));
			lookup.Add("1.2.840.10045.4.3.4", new CmsSignature.ECDsaCmsSignature("1.2.840.10045.4.3.4", HashAlgorithmName.SHA512));
			lookup.Add("1.2.840.10045.2.1", new CmsSignature.ECDsaCmsSignature(null, default(HashAlgorithmName)));
		}

		internal abstract bool VerifySignature(byte[] valueHash, byte[] signature, string digestAlgorithmOid, HashAlgorithmName digestAlgorithmName, ReadOnlyMemory<byte>? signatureParameters, X509Certificate2 certificate);

		protected abstract bool Sign(byte[] dataHash, HashAlgorithmName hashAlgorithmName, X509Certificate2 certificate, bool silent, out Oid signatureAlgorithm, out byte[] signatureValue);

		internal static CmsSignature Resolve(string signatureAlgorithmOid)
		{
			CmsSignature cmsSignature;
			if (CmsSignature.s_lookup.TryGetValue(signatureAlgorithmOid, out cmsSignature))
			{
				return cmsSignature;
			}
			return null;
		}

		internal static bool Sign(byte[] dataHash, HashAlgorithmName hashAlgorithmName, X509Certificate2 certificate, bool silent, out Oid oid, out ReadOnlyMemory<byte> signatureValue)
		{
			CmsSignature cmsSignature = CmsSignature.Resolve(certificate.GetKeyAlgorithm());
			if (cmsSignature == null)
			{
				oid = null;
				signatureValue = default(ReadOnlyMemory<byte>);
				return false;
			}
			byte[] array;
			bool flag = cmsSignature.Sign(dataHash, hashAlgorithmName, certificate, silent, out oid, out array);
			signatureValue = array;
			return flag;
		}

		private unsafe static bool DsaDerToIeee(ReadOnlyMemory<byte> derSignature, Span<byte> ieeeSignature)
		{
			int num = ieeeSignature.Length / 2;
			bool flag;
			try
			{
				AsnReader asnReader = new AsnReader(derSignature, AsnEncodingRules.DER);
				AsnReader asnReader2 = asnReader.ReadSequence();
				if (asnReader.HasData)
				{
					flag = false;
				}
				else
				{
					ieeeSignature.Clear();
					ReadOnlySpan<byte> readOnlySpan = asnReader2.GetIntegerBytes().Span;
					if (readOnlySpan.Length > num && *readOnlySpan[0] == 0)
					{
						readOnlySpan = readOnlySpan.Slice(1);
					}
					if (readOnlySpan.Length <= num)
					{
						readOnlySpan.CopyTo(ieeeSignature.Slice(num - readOnlySpan.Length, readOnlySpan.Length));
					}
					readOnlySpan = asnReader2.GetIntegerBytes().Span;
					if (readOnlySpan.Length > num && *readOnlySpan[0] == 0)
					{
						readOnlySpan = readOnlySpan.Slice(1);
					}
					if (readOnlySpan.Length <= num)
					{
						readOnlySpan.CopyTo(ieeeSignature.Slice(num + num - readOnlySpan.Length, readOnlySpan.Length));
					}
					flag = !asnReader2.HasData;
				}
			}
			catch (CryptographicException)
			{
				flag = false;
			}
			return flag;
		}

		private static byte[] DsaIeeeToDer(ReadOnlySpan<byte> ieeeSignature)
		{
			int num = ieeeSignature.Length / 2;
			byte[] array2;
			using (AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER))
			{
				asnWriter.PushSequence();
				byte[] array = new byte[num + 1];
				Span<byte> span = new Span<byte>(array, 1, num);
				ieeeSignature.Slice(0, num).CopyTo(span);
				Array.Reverse<byte>(array);
				BigInteger bigInteger = new BigInteger(array);
				asnWriter.WriteInteger(bigInteger);
				array[0] = 0;
				ieeeSignature.Slice(num, num).CopyTo(span);
				Array.Reverse<byte>(array);
				bigInteger = new BigInteger(array);
				asnWriter.WriteInteger(bigInteger);
				asnWriter.PopSequence();
				array2 = asnWriter.Encode();
			}
			return array2;
		}

		private static readonly Dictionary<string, CmsSignature> s_lookup = new Dictionary<string, CmsSignature>();

		private class DSACmsSignature : CmsSignature
		{
			internal DSACmsSignature(string signatureAlgorithm, HashAlgorithmName expectedDigest)
			{
				this._signatureAlgorithm = signatureAlgorithm;
				this._expectedDigest = expectedDigest;
			}

			internal override bool VerifySignature(byte[] valueHash, byte[] signature, string digestAlgorithmOid, HashAlgorithmName digestAlgorithmName, ReadOnlyMemory<byte>? signatureParameters, X509Certificate2 certificate)
			{
				if (this._expectedDigest != digestAlgorithmName)
				{
					throw new CryptographicException(SR.Format("SignerInfo digest algorithm '{0}' is not valid for signature algorithm '{1}'.", digestAlgorithmOid, this._signatureAlgorithm));
				}
				DSA dsapublicKey = certificate.GetDSAPublicKey();
				if (dsapublicKey == null)
				{
					return false;
				}
				DSAParameters dsaparameters = dsapublicKey.ExportParameters(false);
				byte[] array = new byte[2 * dsaparameters.Q.Length];
				return CmsSignature.DsaDerToIeee(signature, array) && dsapublicKey.VerifySignature(valueHash, array);
			}

			protected override bool Sign(byte[] dataHash, HashAlgorithmName hashAlgorithmName, X509Certificate2 certificate, bool silent, out Oid signatureAlgorithm, out byte[] signatureValue)
			{
				DSA dsa = PkcsPal.Instance.GetPrivateKeyForSigning<DSA>(certificate, silent) ?? certificate.GetDSAPublicKey();
				if (dsa == null)
				{
					signatureAlgorithm = null;
					signatureValue = null;
					return false;
				}
				string text = ((hashAlgorithmName == HashAlgorithmName.SHA1) ? "1.2.840.10040.4.3" : ((hashAlgorithmName == HashAlgorithmName.SHA256) ? "2.16.840.1.101.3.4.3.2" : ((hashAlgorithmName == HashAlgorithmName.SHA384) ? "2.16.840.1.101.3.4.3.3" : ((hashAlgorithmName == HashAlgorithmName.SHA512) ? "2.16.840.1.101.3.4.3.4" : null))));
				if (text == null)
				{
					signatureAlgorithm = null;
					signatureValue = null;
					return false;
				}
				signatureAlgorithm = new Oid(text, text);
				byte[] array = dsa.CreateSignature(dataHash);
				signatureValue = CmsSignature.DsaIeeeToDer(new ReadOnlySpan<byte>(array));
				return true;
			}

			private readonly HashAlgorithmName _expectedDigest;

			private readonly string _signatureAlgorithm;
		}

		private class ECDsaCmsSignature : CmsSignature
		{
			internal ECDsaCmsSignature(string signatureAlgorithm, HashAlgorithmName expectedDigest)
			{
				this._signatureAlgorithm = signatureAlgorithm;
				this._expectedDigest = expectedDigest;
			}

			internal override bool VerifySignature(byte[] valueHash, byte[] signature, string digestAlgorithmOid, HashAlgorithmName digestAlgorithmName, ReadOnlyMemory<byte>? signatureParameters, X509Certificate2 certificate)
			{
				if (this._expectedDigest != digestAlgorithmName)
				{
					throw new CryptographicException(SR.Format("SignerInfo digest algorithm '{0}' is not valid for signature algorithm '{1}'.", digestAlgorithmOid, this._signatureAlgorithm));
				}
				ECDsa ecdsaPublicKey = certificate.GetECDsaPublicKey();
				if (ecdsaPublicKey == null)
				{
					return false;
				}
				byte[] array = new byte[ecdsaPublicKey.KeySize / 4];
				return CmsSignature.DsaDerToIeee(signature, array) && ecdsaPublicKey.VerifyHash(valueHash, array);
			}

			protected override bool Sign(byte[] dataHash, HashAlgorithmName hashAlgorithmName, X509Certificate2 certificate, bool silent, out Oid signatureAlgorithm, out byte[] signatureValue)
			{
				ECDsa ecdsa = PkcsPal.Instance.GetPrivateKeyForSigning<ECDsa>(certificate, silent) ?? certificate.GetECDsaPublicKey();
				if (ecdsa == null)
				{
					signatureAlgorithm = null;
					signatureValue = null;
					return false;
				}
				string text = ((hashAlgorithmName == HashAlgorithmName.SHA1) ? "1.2.840.10045.4.1" : ((hashAlgorithmName == HashAlgorithmName.SHA256) ? "1.2.840.10045.4.3.2" : ((hashAlgorithmName == HashAlgorithmName.SHA384) ? "1.2.840.10045.4.3.3" : ((hashAlgorithmName == HashAlgorithmName.SHA512) ? "1.2.840.10045.4.3.4" : null))));
				if (text == null)
				{
					signatureAlgorithm = null;
					signatureValue = null;
					return false;
				}
				signatureAlgorithm = new Oid(text, text);
				signatureValue = CmsSignature.DsaIeeeToDer(ecdsa.SignHash(dataHash));
				return true;
			}

			private readonly HashAlgorithmName _expectedDigest;

			private readonly string _signatureAlgorithm;
		}

		private abstract class RSACmsSignature : CmsSignature
		{
			protected RSACmsSignature(string signatureAlgorithm, HashAlgorithmName? expectedDigest)
			{
				this._signatureAlgorithm = signatureAlgorithm;
				this._expectedDigest = expectedDigest;
			}

			internal override bool VerifySignature(byte[] valueHash, byte[] signature, string digestAlgorithmOid, HashAlgorithmName digestAlgorithmName, ReadOnlyMemory<byte>? signatureParameters, X509Certificate2 certificate)
			{
				if (this._expectedDigest != null && this._expectedDigest.Value != digestAlgorithmName)
				{
					throw new CryptographicException(SR.Format("SignerInfo digest algorithm '{0}' is not valid for signature algorithm '{1}'.", digestAlgorithmOid, this._signatureAlgorithm));
				}
				RSASignaturePadding signaturePadding = this.GetSignaturePadding(signatureParameters, digestAlgorithmOid, digestAlgorithmName, valueHash.Length);
				RSA rsapublicKey = certificate.GetRSAPublicKey();
				return rsapublicKey != null && rsapublicKey.VerifyHash(valueHash, signature, digestAlgorithmName, signaturePadding);
			}

			protected abstract RSASignaturePadding GetSignaturePadding(ReadOnlyMemory<byte>? signatureParameters, string digestAlgorithmOid, HashAlgorithmName digestAlgorithmName, int digestValueLength);

			private readonly string _signatureAlgorithm;

			private readonly HashAlgorithmName? _expectedDigest;
		}

		private sealed class RSAPkcs1CmsSignature : CmsSignature.RSACmsSignature
		{
			public RSAPkcs1CmsSignature(string signatureAlgorithm, HashAlgorithmName? expectedDigest)
				: base(signatureAlgorithm, expectedDigest)
			{
			}

			protected unsafe override RSASignaturePadding GetSignaturePadding(ReadOnlyMemory<byte>? signatureParameters, string digestAlgorithmOid, HashAlgorithmName digestAlgorithmName, int digestValueLength)
			{
				if (signatureParameters == null)
				{
					return RSASignaturePadding.Pkcs1;
				}
				Span<byte> span = new Span<byte>(stackalloc byte[(UIntPtr)2], 2);
				*span[0] = 5;
				*span[1] = 0;
				if (span.SequenceEqual<byte>(signatureParameters.Value.Span))
				{
					return RSASignaturePadding.Pkcs1;
				}
				throw new CryptographicException("Invalid signature paramters.");
			}

			protected override bool Sign(byte[] dataHash, HashAlgorithmName hashAlgorithmName, X509Certificate2 certificate, bool silent, out Oid signatureAlgorithm, out byte[] signatureValue)
			{
				RSA rsa = PkcsPal.Instance.GetPrivateKeyForSigning<RSA>(certificate, silent) ?? certificate.GetRSAPublicKey();
				if (rsa == null)
				{
					signatureAlgorithm = null;
					signatureValue = null;
					return false;
				}
				signatureAlgorithm = new Oid("1.2.840.113549.1.1.1", "1.2.840.113549.1.1.1");
				signatureValue = rsa.SignHash(dataHash, hashAlgorithmName, RSASignaturePadding.Pkcs1);
				return true;
			}
		}

		private class RSAPssCmsSignature : CmsSignature.RSACmsSignature
		{
			public RSAPssCmsSignature()
				: base(null, null)
			{
			}

			protected override RSASignaturePadding GetSignaturePadding(ReadOnlyMemory<byte>? signatureParameters, string digestAlgorithmOid, HashAlgorithmName digestAlgorithmName, int digestValueLength)
			{
				if (signatureParameters == null)
				{
					throw new CryptographicException("PSS parameters were not present.");
				}
				PssParamsAsn pssParamsAsn = AsnSerializer.Deserialize<PssParamsAsn>(signatureParameters.Value, AsnEncodingRules.DER);
				if (pssParamsAsn.HashAlgorithm.Algorithm.Value != digestAlgorithmOid)
				{
					throw new CryptographicException(SR.Format("This platform requires that the PSS hash algorithm ({0}) match the data digest algorithm ({1}).", pssParamsAsn.HashAlgorithm.Algorithm.Value, digestAlgorithmOid));
				}
				if (pssParamsAsn.TrailerField != 1)
				{
					throw new CryptographicException("Invalid signature paramters.");
				}
				if (pssParamsAsn.SaltLength != digestValueLength)
				{
					throw new CryptographicException(SR.Format("PSS salt size {0} is not supported by this platform with hash algorithm {1}.", pssParamsAsn.SaltLength, digestAlgorithmName.Name));
				}
				if (pssParamsAsn.MaskGenAlgorithm.Algorithm.Value != "1.2.840.113549.1.1.8")
				{
					throw new CryptographicException("Mask generation function '{0}' is not supported by this platform.", pssParamsAsn.MaskGenAlgorithm.Algorithm.Value);
				}
				if (pssParamsAsn.MaskGenAlgorithm.Parameters == null)
				{
					throw new CryptographicException("Invalid signature paramters.");
				}
				AlgorithmIdentifierAsn algorithmIdentifierAsn = AsnSerializer.Deserialize<AlgorithmIdentifierAsn>(pssParamsAsn.MaskGenAlgorithm.Parameters.Value, AsnEncodingRules.DER);
				if (algorithmIdentifierAsn.Algorithm.Value != digestAlgorithmOid)
				{
					throw new CryptographicException(SR.Format("This platform does not support the MGF hash algorithm ({0}) being different from the signature hash algorithm ({1}).", algorithmIdentifierAsn.Algorithm.Value, digestAlgorithmOid));
				}
				return RSASignaturePadding.Pss;
			}

			protected override bool Sign(byte[] dataHash, HashAlgorithmName hashAlgorithmName, X509Certificate2 certificate, bool silent, out Oid signatureAlgorithm, out byte[] signatureValue)
			{
				throw new CryptographicException();
			}
		}
	}
}
