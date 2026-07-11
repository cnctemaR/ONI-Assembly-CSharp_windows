using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	internal static class SignedXmlDebugLog
	{
		private static bool InformationLoggingEnabled
		{
			get
			{
				if (!SignedXmlDebugLog.s_haveInformationLogging)
				{
					SignedXmlDebugLog.s_informationLogging = SignedXmlDebugLog.s_traceSource.Switch.ShouldTrace(TraceEventType.Information);
					SignedXmlDebugLog.s_haveInformationLogging = true;
				}
				return SignedXmlDebugLog.s_informationLogging;
			}
		}

		private static bool VerboseLoggingEnabled
		{
			get
			{
				if (!SignedXmlDebugLog.s_haveVerboseLogging)
				{
					SignedXmlDebugLog.s_verboseLogging = SignedXmlDebugLog.s_traceSource.Switch.ShouldTrace(TraceEventType.Verbose);
					SignedXmlDebugLog.s_haveVerboseLogging = true;
				}
				return SignedXmlDebugLog.s_verboseLogging;
			}
		}

		private static string FormatBytes(byte[] bytes)
		{
			if (bytes == null)
			{
				return "(null)";
			}
			StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);
			foreach (byte b in bytes)
			{
				stringBuilder.Append(b.ToString("x2", CultureInfo.InvariantCulture));
			}
			return stringBuilder.ToString();
		}

		private static string GetKeyName(object key)
		{
			ICspAsymmetricAlgorithm cspAsymmetricAlgorithm = key as ICspAsymmetricAlgorithm;
			X509Certificate x509Certificate = key as X509Certificate;
			X509Certificate2 x509Certificate2 = key as X509Certificate2;
			string text;
			if (cspAsymmetricAlgorithm != null && cspAsymmetricAlgorithm.CspKeyContainerInfo.KeyContainerName != null)
			{
				text = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", cspAsymmetricAlgorithm.CspKeyContainerInfo.KeyContainerName);
			}
			else if (x509Certificate2 != null)
			{
				text = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", x509Certificate2.GetNameInfo(X509NameType.SimpleName, false));
			}
			else if (x509Certificate != null)
			{
				text = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", x509Certificate.Subject);
			}
			else
			{
				text = key.GetHashCode().ToString("x8", CultureInfo.InvariantCulture);
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}#{1}", key.GetType().Name, text);
		}

		private static string GetObjectId(object o)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}#{1}", o.GetType().Name, o.GetHashCode().ToString("x8", CultureInfo.InvariantCulture));
		}

		private static string GetOidName(Oid oid)
		{
			string text = oid.FriendlyName;
			if (string.IsNullOrEmpty(text))
			{
				text = oid.Value;
			}
			return text;
		}

		internal static void LogBeginCanonicalization(SignedXml signedXml, Transform canonicalizationTransform)
		{
			if (SignedXmlDebugLog.InformationLoggingEnabled)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "Beginning canonicalization using \"{0}\" ({1}).", canonicalizationTransform.Algorithm, canonicalizationTransform.GetType().Name);
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Information, SignedXmlDebugLog.SignedXmlDebugEvent.BeginCanonicalization, text);
			}
			if (SignedXmlDebugLog.VerboseLoggingEnabled)
			{
				string text2 = string.Format(CultureInfo.InvariantCulture, "Canonicalization transform is using resolver {0} and base URI \"{1}\".", canonicalizationTransform.Resolver.GetType(), canonicalizationTransform.BaseURI);
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Verbose, SignedXmlDebugLog.SignedXmlDebugEvent.BeginCanonicalization, text2);
			}
		}

		internal static void LogBeginCheckSignatureFormat(SignedXml signedXml, Func<SignedXml, bool> formatValidator)
		{
			if (SignedXmlDebugLog.InformationLoggingEnabled)
			{
				MethodInfo method = formatValidator.Method;
				string text = string.Format(CultureInfo.InvariantCulture, "Checking signature format using format validator \"[{0}] {1}.{2}\".", method.Module.Assembly.FullName, method.DeclaringType.FullName, method.Name);
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Information, SignedXmlDebugLog.SignedXmlDebugEvent.BeginCheckSignatureFormat, text);
			}
		}

		internal static void LogBeginCheckSignedInfo(SignedXml signedXml, SignedInfo signedInfo)
		{
			if (SignedXmlDebugLog.InformationLoggingEnabled)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "Checking signature on SignedInfo with id \"{0}\".", (signedInfo.Id != null) ? signedInfo.Id : "(null)");
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Information, SignedXmlDebugLog.SignedXmlDebugEvent.BeginCheckSignedInfo, text);
			}
		}

		internal static void LogBeginSignatureComputation(SignedXml signedXml, XmlElement context)
		{
			if (SignedXmlDebugLog.InformationLoggingEnabled)
			{
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Information, SignedXmlDebugLog.SignedXmlDebugEvent.BeginSignatureComputation, "Beginning signature computation.");
			}
			if (SignedXmlDebugLog.VerboseLoggingEnabled)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "Using context: {0}", (context != null) ? context.OuterXml : "(null)");
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Verbose, SignedXmlDebugLog.SignedXmlDebugEvent.BeginSignatureComputation, text);
			}
		}

		internal static void LogBeginSignatureVerification(SignedXml signedXml, XmlElement context)
		{
			if (SignedXmlDebugLog.InformationLoggingEnabled)
			{
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Information, SignedXmlDebugLog.SignedXmlDebugEvent.BeginSignatureVerification, "Beginning signature verification.");
			}
			if (SignedXmlDebugLog.VerboseLoggingEnabled)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "Using context: {0}", (context != null) ? context.OuterXml : "(null)");
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Verbose, SignedXmlDebugLog.SignedXmlDebugEvent.BeginSignatureVerification, text);
			}
		}

		internal static void LogCanonicalizedOutput(SignedXml signedXml, Transform canonicalizationTransform)
		{
			if (SignedXmlDebugLog.VerboseLoggingEnabled)
			{
				using (StreamReader streamReader = new StreamReader(canonicalizationTransform.GetOutput(typeof(Stream)) as Stream))
				{
					string text = string.Format(CultureInfo.InvariantCulture, "Output of canonicalization transform: {0}", streamReader.ReadToEnd());
					SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Verbose, SignedXmlDebugLog.SignedXmlDebugEvent.CanonicalizedData, text);
				}
			}
		}

		internal static void LogFormatValidationResult(SignedXml signedXml, bool result)
		{
			if (SignedXmlDebugLog.InformationLoggingEnabled)
			{
				string text = (result ? "Signature format validation was successful." : "Signature format validation failed.");
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Information, SignedXmlDebugLog.SignedXmlDebugEvent.FormatValidationResult, text);
			}
		}

		internal static void LogUnsafeCanonicalizationMethod(SignedXml signedXml, string algorithm, IEnumerable<string> validAlgorithms)
		{
			if (SignedXmlDebugLog.InformationLoggingEnabled)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string text in validAlgorithms)
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.AppendFormat("\"{0}\"", text);
				}
				string text2 = string.Format(CultureInfo.InvariantCulture, "Canonicalization method \"{0}\" is not on the safe list. Safe canonicalization methods are: {1}.", algorithm, stringBuilder.ToString());
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Information, SignedXmlDebugLog.SignedXmlDebugEvent.UnsafeCanonicalizationMethod, text2);
			}
		}

		internal static void LogUnsafeTransformMethod(SignedXml signedXml, string algorithm, IEnumerable<string> validC14nAlgorithms, IEnumerable<string> validTransformAlgorithms)
		{
			if (SignedXmlDebugLog.InformationLoggingEnabled)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string text in validC14nAlgorithms)
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.AppendFormat("\"{0}\"", text);
				}
				foreach (string text2 in validTransformAlgorithms)
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.AppendFormat("\"{0}\"", text2);
				}
				string text3 = string.Format(CultureInfo.InvariantCulture, "Transform method \"{0}\" is not on the safe list. Safe transform methods are: {1}.", algorithm, stringBuilder.ToString());
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Information, SignedXmlDebugLog.SignedXmlDebugEvent.UnsafeTransformMethod, text3);
			}
		}

		internal static void LogNamespacePropagation(SignedXml signedXml, XmlNodeList namespaces)
		{
			if (SignedXmlDebugLog.InformationLoggingEnabled)
			{
				if (namespaces != null)
				{
					using (IEnumerator enumerator = namespaces.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							XmlAttribute xmlAttribute = (XmlAttribute)obj;
							string text = string.Format(CultureInfo.InvariantCulture, "Propagating namespace {0}=\"{1}\".", xmlAttribute.Name, xmlAttribute.Value);
							SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Information, SignedXmlDebugLog.SignedXmlDebugEvent.NamespacePropagation, text);
						}
						return;
					}
				}
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Information, SignedXmlDebugLog.SignedXmlDebugEvent.NamespacePropagation, "No namespaces are being propagated.");
			}
		}

		internal static Stream LogReferenceData(Reference reference, Stream data)
		{
			if (SignedXmlDebugLog.VerboseLoggingEnabled)
			{
				MemoryStream memoryStream = new MemoryStream();
				byte[] array = new byte[4096];
				int num;
				do
				{
					num = data.Read(array, 0, array.Length);
					memoryStream.Write(array, 0, num);
				}
				while (num == array.Length);
				string text = string.Format(CultureInfo.InvariantCulture, "Transformed reference contents: {0}", Encoding.UTF8.GetString(memoryStream.ToArray()));
				SignedXmlDebugLog.WriteLine(reference, TraceEventType.Verbose, SignedXmlDebugLog.SignedXmlDebugEvent.ReferenceData, text);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				return memoryStream;
			}
			return data;
		}

		internal static void LogSigning(SignedXml signedXml, object key, SignatureDescription signatureDescription, HashAlgorithm hash, AsymmetricSignatureFormatter asymmetricSignatureFormatter)
		{
			if (SignedXmlDebugLog.InformationLoggingEnabled)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "Calculating signature with key {0} using signature description {1}, hash algorithm {2}, and asymmetric signature formatter {3}.", new object[]
				{
					SignedXmlDebugLog.GetKeyName(key),
					signatureDescription.GetType().Name,
					hash.GetType().Name,
					asymmetricSignatureFormatter.GetType().Name
				});
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Information, SignedXmlDebugLog.SignedXmlDebugEvent.Signing, text);
			}
		}

		internal static void LogSigning(SignedXml signedXml, KeyedHashAlgorithm key)
		{
			if (SignedXmlDebugLog.InformationLoggingEnabled)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "Calculating signature using keyed hash algorithm {0}.", key.GetType().Name);
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Information, SignedXmlDebugLog.SignedXmlDebugEvent.Signing, text);
			}
		}

		internal static void LogSigningReference(SignedXml signedXml, Reference reference)
		{
			if (SignedXmlDebugLog.VerboseLoggingEnabled)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "Hashing reference {0}, Uri \"{1}\", Id \"{2}\", Type \"{3}\" with hash algorithm \"{4}\" ({5}).", new object[]
				{
					SignedXmlDebugLog.GetObjectId(reference),
					reference.Uri,
					reference.Id,
					reference.Type,
					reference.DigestMethod,
					CryptoHelpers.CreateFromName(reference.DigestMethod).GetType().Name
				});
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Verbose, SignedXmlDebugLog.SignedXmlDebugEvent.SigningReference, text);
			}
		}

		internal static void LogVerificationFailure(SignedXml signedXml, string failureLocation)
		{
			if (SignedXmlDebugLog.InformationLoggingEnabled)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "Verification failed checking {0}.", failureLocation);
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Information, SignedXmlDebugLog.SignedXmlDebugEvent.VerificationFailure, text);
			}
		}

		internal static void LogVerificationResult(SignedXml signedXml, object key, bool verified)
		{
			if (SignedXmlDebugLog.InformationLoggingEnabled)
			{
				string text = (verified ? "Verification with key {0} was successful." : "Verification with key {0} was not successful.");
				string text2 = string.Format(CultureInfo.InvariantCulture, text, SignedXmlDebugLog.GetKeyName(key));
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Information, SignedXmlDebugLog.SignedXmlDebugEvent.SignatureVerificationResult, text2);
			}
		}

		internal static void LogVerifyKeyUsage(SignedXml signedXml, X509Certificate certificate, X509KeyUsageExtension keyUsages)
		{
			if (SignedXmlDebugLog.InformationLoggingEnabled)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "Found key usages \"{0}\" in extension {1} on certificate {2}.", keyUsages.KeyUsages, SignedXmlDebugLog.GetOidName(keyUsages.Oid), SignedXmlDebugLog.GetKeyName(certificate));
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Verbose, SignedXmlDebugLog.SignedXmlDebugEvent.X509Verification, text);
			}
		}

		internal static void LogVerifyReference(SignedXml signedXml, Reference reference)
		{
			if (SignedXmlDebugLog.InformationLoggingEnabled)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "Processing reference {0}, Uri \"{1}\", Id \"{2}\", Type \"{3}\".", new object[]
				{
					SignedXmlDebugLog.GetObjectId(reference),
					reference.Uri,
					reference.Id,
					reference.Type
				});
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Verbose, SignedXmlDebugLog.SignedXmlDebugEvent.VerifyReference, text);
			}
		}

		internal static void LogVerifyReferenceHash(SignedXml signedXml, Reference reference, byte[] actualHash, byte[] expectedHash)
		{
			if (SignedXmlDebugLog.VerboseLoggingEnabled)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "Reference {0} hashed with \"{1}\" ({2}) has hash value {3}, expected hash value {4}.", new object[]
				{
					SignedXmlDebugLog.GetObjectId(reference),
					reference.DigestMethod,
					CryptoHelpers.CreateFromName(reference.DigestMethod).GetType().Name,
					SignedXmlDebugLog.FormatBytes(actualHash),
					SignedXmlDebugLog.FormatBytes(expectedHash)
				});
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Verbose, SignedXmlDebugLog.SignedXmlDebugEvent.VerifyReference, text);
			}
		}

		internal static void LogVerifySignedInfo(SignedXml signedXml, AsymmetricAlgorithm key, SignatureDescription signatureDescription, HashAlgorithm hashAlgorithm, AsymmetricSignatureDeformatter asymmetricSignatureDeformatter, byte[] actualHashValue, byte[] signatureValue)
		{
			if (SignedXmlDebugLog.InformationLoggingEnabled)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "Verifying SignedInfo using key {0}, signature description {1}, hash algorithm {2}, and asymmetric signature deformatter {3}.", new object[]
				{
					SignedXmlDebugLog.GetKeyName(key),
					signatureDescription.GetType().Name,
					hashAlgorithm.GetType().Name,
					asymmetricSignatureDeformatter.GetType().Name
				});
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Information, SignedXmlDebugLog.SignedXmlDebugEvent.VerifySignedInfo, text);
			}
			if (SignedXmlDebugLog.VerboseLoggingEnabled)
			{
				string text2 = string.Format(CultureInfo.InvariantCulture, "Actual hash value: {0}", SignedXmlDebugLog.FormatBytes(actualHashValue));
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Verbose, SignedXmlDebugLog.SignedXmlDebugEvent.VerifySignedInfo, text2);
				string text3 = string.Format(CultureInfo.InvariantCulture, "Raw signature: {0}", SignedXmlDebugLog.FormatBytes(signatureValue));
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Verbose, SignedXmlDebugLog.SignedXmlDebugEvent.VerifySignedInfo, text3);
			}
		}

		internal static void LogVerifySignedInfo(SignedXml signedXml, KeyedHashAlgorithm mac, byte[] actualHashValue, byte[] signatureValue)
		{
			if (SignedXmlDebugLog.InformationLoggingEnabled)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "Verifying SignedInfo using keyed hash algorithm {0}.", mac.GetType().Name);
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Information, SignedXmlDebugLog.SignedXmlDebugEvent.VerifySignedInfo, text);
			}
			if (SignedXmlDebugLog.VerboseLoggingEnabled)
			{
				string text2 = string.Format(CultureInfo.InvariantCulture, "Actual hash value: {0}", SignedXmlDebugLog.FormatBytes(actualHashValue));
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Verbose, SignedXmlDebugLog.SignedXmlDebugEvent.VerifySignedInfo, text2);
				string text3 = string.Format(CultureInfo.InvariantCulture, "Raw signature: {0}", SignedXmlDebugLog.FormatBytes(signatureValue));
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Verbose, SignedXmlDebugLog.SignedXmlDebugEvent.VerifySignedInfo, text3);
			}
		}

		internal static void LogVerifyX509Chain(SignedXml signedXml, X509Chain chain, X509Certificate certificate)
		{
			if (SignedXmlDebugLog.InformationLoggingEnabled)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "Building and verifying the X509 chain for certificate {0}.", SignedXmlDebugLog.GetKeyName(certificate));
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Information, SignedXmlDebugLog.SignedXmlDebugEvent.X509Verification, text);
			}
			if (SignedXmlDebugLog.VerboseLoggingEnabled)
			{
				string text2 = string.Format(CultureInfo.InvariantCulture, "Revocation mode for chain building: {0}.", chain.ChainPolicy.RevocationFlag);
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Verbose, SignedXmlDebugLog.SignedXmlDebugEvent.X509Verification, text2);
				string text3 = string.Format(CultureInfo.InvariantCulture, "Revocation flag for chain building: {0}.", chain.ChainPolicy.RevocationFlag);
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Verbose, SignedXmlDebugLog.SignedXmlDebugEvent.X509Verification, text3);
				string text4 = string.Format(CultureInfo.InvariantCulture, "Verification flags for chain building: {0}.", chain.ChainPolicy.VerificationFlags);
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Verbose, SignedXmlDebugLog.SignedXmlDebugEvent.X509Verification, text4);
				string text5 = string.Format(CultureInfo.InvariantCulture, "Verification time for chain building: {0}.", chain.ChainPolicy.VerificationTime);
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Verbose, SignedXmlDebugLog.SignedXmlDebugEvent.X509Verification, text5);
				string text6 = string.Format(CultureInfo.InvariantCulture, "URL retrieval timeout for chain building: {0}.", chain.ChainPolicy.UrlRetrievalTimeout);
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Verbose, SignedXmlDebugLog.SignedXmlDebugEvent.X509Verification, text6);
			}
			if (SignedXmlDebugLog.InformationLoggingEnabled)
			{
				foreach (X509ChainStatus x509ChainStatus in chain.ChainStatus)
				{
					if (x509ChainStatus.Status != X509ChainStatusFlags.NoError)
					{
						string text7 = string.Format(CultureInfo.InvariantCulture, "Error building X509 chain: {0}: {1}.", x509ChainStatus.Status, x509ChainStatus.StatusInformation);
						SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Information, SignedXmlDebugLog.SignedXmlDebugEvent.X509Verification, text7);
					}
				}
			}
			if (SignedXmlDebugLog.VerboseLoggingEnabled)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Certificate chain:");
				foreach (X509ChainElement x509ChainElement in chain.ChainElements)
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " {0}", SignedXmlDebugLog.GetKeyName(x509ChainElement.Certificate));
				}
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Verbose, SignedXmlDebugLog.SignedXmlDebugEvent.X509Verification, stringBuilder.ToString());
			}
		}

		internal static void LogSignedXmlRecursionLimit(SignedXml signedXml, Reference reference)
		{
			if (SignedXmlDebugLog.InformationLoggingEnabled)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "Signed xml recursion limit hit while trying to decrypt the key. Reference {0} hashed with \"{1}\" and ({2}).", SignedXmlDebugLog.GetObjectId(reference), reference.DigestMethod, CryptoHelpers.CreateFromName(reference.DigestMethod).GetType().Name);
				SignedXmlDebugLog.WriteLine(signedXml, TraceEventType.Information, SignedXmlDebugLog.SignedXmlDebugEvent.VerifySignedInfo, text);
			}
		}

		private static void WriteLine(object source, TraceEventType eventType, SignedXmlDebugLog.SignedXmlDebugEvent eventId, string data)
		{
		}

		private const string NullString = "(null)";

		private static TraceSource s_traceSource = new TraceSource("System.Security.Cryptography.Xml.SignedXml");

		private static volatile bool s_haveVerboseLogging;

		private static volatile bool s_verboseLogging;

		private static volatile bool s_haveInformationLogging;

		private static volatile bool s_informationLogging;

		internal enum SignedXmlDebugEvent
		{
			BeginCanonicalization,
			BeginCheckSignatureFormat,
			BeginCheckSignedInfo,
			BeginSignatureComputation,
			BeginSignatureVerification,
			CanonicalizedData,
			FormatValidationResult,
			NamespacePropagation,
			ReferenceData,
			SignatureVerificationResult,
			Signing,
			SigningReference,
			VerificationFailure,
			VerifyReference,
			VerifySignedInfo,
			X509Verification,
			UnsafeCanonicalizationMethod,
			UnsafeTransformMethod
		}
	}
}
