using System;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32.SafeHandles;
using Mono.Net.Security;
using Mono.Security.Interface;

namespace Mono.Btls
{
	internal class MonoBtlsProvider : MobileTlsProvider
	{
		public override Guid ID
		{
			get
			{
				return Mono.Net.Security.MonoTlsProviderFactory.BtlsId;
			}
		}

		public override string Name
		{
			get
			{
				return "btls";
			}
		}

		internal MonoBtlsProvider()
		{
			if (!Mono.Net.Security.MonoTlsProviderFactory.IsBtlsSupported())
			{
				throw new NotSupportedException("BTLS is not supported in this runtime.");
			}
		}

		public override bool SupportsSslStream
		{
			get
			{
				return true;
			}
		}

		public override bool SupportsMonoExtensions
		{
			get
			{
				return true;
			}
		}

		public override bool SupportsConnectionInfo
		{
			get
			{
				return true;
			}
		}

		internal override bool SupportsCleanShutdown
		{
			get
			{
				return true;
			}
		}

		public override SslProtocols SupportedProtocols
		{
			get
			{
				return SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
			}
		}

		internal override MobileAuthenticatedStream CreateSslStream(SslStream sslStream, Stream innerStream, bool leaveInnerStreamOpen, MonoTlsSettings settings)
		{
			return new MonoBtlsStream(innerStream, leaveInnerStreamOpen, sslStream, settings, this);
		}

		internal override bool HasNativeCertificates
		{
			get
			{
				return true;
			}
		}

		internal X509Certificate2Impl GetNativeCertificate(byte[] data, string password, X509KeyStorageFlags flags)
		{
			X509Certificate2Impl nativeCertificate;
			using (SafePasswordHandle safePasswordHandle = new SafePasswordHandle(password))
			{
				nativeCertificate = this.GetNativeCertificate(data, safePasswordHandle, flags);
			}
			return nativeCertificate;
		}

		internal X509Certificate2Impl GetNativeCertificate(X509Certificate certificate)
		{
			X509CertificateImplBtls x509CertificateImplBtls = certificate.Impl as X509CertificateImplBtls;
			if (x509CertificateImplBtls != null)
			{
				return (X509Certificate2Impl)x509CertificateImplBtls.Clone();
			}
			return new X509CertificateImplBtls(certificate.GetRawCertData(), MonoBtlsX509Format.DER);
		}

		internal X509Certificate2Impl GetNativeCertificate(byte[] data, SafePasswordHandle password, X509KeyStorageFlags flags)
		{
			return new X509CertificateImplBtls(data, password, flags);
		}

		internal static MonoBtlsX509VerifyParam GetVerifyParam(MonoTlsSettings settings, string targetHost, bool serverMode)
		{
			MonoBtlsX509VerifyParam monoBtlsX509VerifyParam;
			if (serverMode)
			{
				monoBtlsX509VerifyParam = MonoBtlsX509VerifyParam.GetSslClient();
			}
			else
			{
				monoBtlsX509VerifyParam = MonoBtlsX509VerifyParam.GetSslServer();
			}
			if (targetHost == null && (settings == null || settings.CertificateValidationTime == null))
			{
				return monoBtlsX509VerifyParam;
			}
			MonoBtlsX509VerifyParam monoBtlsX509VerifyParam3;
			try
			{
				MonoBtlsX509VerifyParam monoBtlsX509VerifyParam2 = monoBtlsX509VerifyParam.Copy();
				if (targetHost != null)
				{
					monoBtlsX509VerifyParam2.SetHost(targetHost);
				}
				if (settings != null && settings.CertificateValidationTime != null)
				{
					monoBtlsX509VerifyParam2.SetTime(settings.CertificateValidationTime.Value);
				}
				monoBtlsX509VerifyParam3 = monoBtlsX509VerifyParam2;
			}
			finally
			{
				monoBtlsX509VerifyParam.Dispose();
			}
			return monoBtlsX509VerifyParam3;
		}

		internal override bool ValidateCertificate(ChainValidationHelper validator, string targetHost, bool serverMode, X509CertificateCollection certificates, bool wantsChain, ref X509Chain chain, ref SslPolicyErrors errors, ref int status11)
		{
			if (chain != null)
			{
				X509ChainImplBtls x509ChainImplBtls = (X509ChainImplBtls)chain.Impl;
				bool flag = x509ChainImplBtls.StoreCtx.VerifyResult == 1;
				this.CheckValidationResult(validator, targetHost, serverMode, certificates, wantsChain, chain, x509ChainImplBtls.StoreCtx, flag, ref errors, ref status11);
				return flag;
			}
			bool flag3;
			using (MonoBtlsX509Store monoBtlsX509Store = new MonoBtlsX509Store())
			{
				using (MonoBtlsX509Chain nativeChain = MonoBtlsProvider.GetNativeChain(certificates))
				{
					using (MonoBtlsX509VerifyParam verifyParam = MonoBtlsProvider.GetVerifyParam(validator.Settings, targetHost, serverMode))
					{
						using (MonoBtlsX509StoreCtx monoBtlsX509StoreCtx = new MonoBtlsX509StoreCtx())
						{
							MonoBtlsProvider.SetupCertificateStore(monoBtlsX509Store, validator.Settings, serverMode);
							monoBtlsX509StoreCtx.Initialize(monoBtlsX509Store, nativeChain);
							monoBtlsX509StoreCtx.SetVerifyParam(verifyParam);
							bool flag2 = monoBtlsX509StoreCtx.Verify() == 1;
							if (wantsChain && chain == null)
							{
								chain = MonoBtlsProvider.GetManagedChain(nativeChain);
							}
							this.CheckValidationResult(validator, targetHost, serverMode, certificates, wantsChain, null, monoBtlsX509StoreCtx, flag2, ref errors, ref status11);
							flag3 = flag2;
						}
					}
				}
			}
			return flag3;
		}

		internal static bool ValidateCertificate(MonoBtlsX509Chain chain, MonoBtlsX509VerifyParam param)
		{
			bool flag;
			using (MonoBtlsX509Store monoBtlsX509Store = new MonoBtlsX509Store())
			{
				using (MonoBtlsX509StoreCtx monoBtlsX509StoreCtx = new MonoBtlsX509StoreCtx())
				{
					MonoBtlsProvider.SetupCertificateStore(monoBtlsX509Store, MonoTlsSettings.DefaultSettings, false);
					monoBtlsX509StoreCtx.Initialize(monoBtlsX509Store, chain);
					if (param != null)
					{
						monoBtlsX509StoreCtx.SetVerifyParam(param);
					}
					flag = monoBtlsX509StoreCtx.Verify() == 1;
				}
			}
			return flag;
		}

		private void CheckValidationResult(ChainValidationHelper validator, string targetHost, bool serverMode, X509CertificateCollection certificates, bool wantsChain, X509Chain chain, MonoBtlsX509StoreCtx storeCtx, bool success, ref SslPolicyErrors errors, ref int status11)
		{
			status11 = 0;
			if (success)
			{
				return;
			}
			errors = SslPolicyErrors.RemoteCertificateChainErrors;
			if (!wantsChain || storeCtx == null || chain == null)
			{
				status11 = -2146762485;
				return;
			}
			MonoBtlsX509Error error = storeCtx.GetError();
			if (error != MonoBtlsX509Error.OK)
			{
				if (error != MonoBtlsX509Error.CRL_NOT_YET_VALID)
				{
					if (error == MonoBtlsX509Error.HOSTNAME_MISMATCH)
					{
						errors = SslPolicyErrors.RemoteCertificateNameMismatch;
						chain.Impl.AddStatus(X509ChainStatusFlags.UntrustedRoot);
						status11 = -2146762485;
						return;
					}
					chain.Impl.AddStatus(MonoBtlsProvider.MapVerifyErrorToChainStatus(error));
					status11 = -2146762485;
				}
				return;
			}
			errors = SslPolicyErrors.None;
		}

		internal static X509ChainStatusFlags MapVerifyErrorToChainStatus(MonoBtlsX509Error code)
		{
			switch (code)
			{
			case MonoBtlsX509Error.OK:
				return X509ChainStatusFlags.NoError;
			case MonoBtlsX509Error.UNABLE_TO_GET_ISSUER_CERT:
			case MonoBtlsX509Error.UNABLE_TO_GET_ISSUER_CERT_LOCALLY:
			case MonoBtlsX509Error.UNABLE_TO_VERIFY_LEAF_SIGNATURE:
				return X509ChainStatusFlags.PartialChain;
			case MonoBtlsX509Error.UNABLE_TO_GET_CRL:
			case MonoBtlsX509Error.UNABLE_TO_DECRYPT_CRL_SIGNATURE:
			case MonoBtlsX509Error.CRL_SIGNATURE_FAILURE:
			case MonoBtlsX509Error.CRL_NOT_YET_VALID:
			case MonoBtlsX509Error.ERROR_IN_CRL_LAST_UPDATE_FIELD:
			case MonoBtlsX509Error.ERROR_IN_CRL_NEXT_UPDATE_FIELD:
			case MonoBtlsX509Error.UNABLE_TO_GET_CRL_ISSUER:
			case MonoBtlsX509Error.KEYUSAGE_NO_CRL_SIGN:
			case MonoBtlsX509Error.UNHANDLED_CRITICAL_CRL_EXTENSION:
				return X509ChainStatusFlags.RevocationStatusUnknown;
			case MonoBtlsX509Error.UNABLE_TO_DECODE_ISSUER_PUBLIC_KEY:
			case MonoBtlsX509Error.CERT_SIGNATURE_FAILURE:
				return X509ChainStatusFlags.NotSignatureValid;
			case MonoBtlsX509Error.CERT_NOT_YET_VALID:
			case MonoBtlsX509Error.CERT_HAS_EXPIRED:
			case MonoBtlsX509Error.ERROR_IN_CERT_NOT_BEFORE_FIELD:
			case MonoBtlsX509Error.ERROR_IN_CERT_NOT_AFTER_FIELD:
				return X509ChainStatusFlags.NotTimeValid;
			case MonoBtlsX509Error.CRL_HAS_EXPIRED:
				return X509ChainStatusFlags.OfflineRevocation;
			case MonoBtlsX509Error.OUT_OF_MEM:
				throw new OutOfMemoryException();
			case MonoBtlsX509Error.DEPTH_ZERO_SELF_SIGNED_CERT:
			case MonoBtlsX509Error.SELF_SIGNED_CERT_IN_CHAIN:
			case MonoBtlsX509Error.CERT_UNTRUSTED:
				return X509ChainStatusFlags.UntrustedRoot;
			case MonoBtlsX509Error.CERT_CHAIN_TOO_LONG:
				throw new CryptographicException();
			case MonoBtlsX509Error.CERT_REVOKED:
				return X509ChainStatusFlags.Revoked;
			case MonoBtlsX509Error.INVALID_CA:
			case MonoBtlsX509Error.PATH_LENGTH_EXCEEDED:
			case MonoBtlsX509Error.KEYUSAGE_NO_CERTSIGN:
			case MonoBtlsX509Error.INVALID_NON_CA:
			case MonoBtlsX509Error.KEYUSAGE_NO_DIGITAL_SIGNATURE:
				return X509ChainStatusFlags.InvalidBasicConstraints;
			case MonoBtlsX509Error.INVALID_PURPOSE:
				return X509ChainStatusFlags.NotValidForUsage;
			case MonoBtlsX509Error.CERT_REJECTED:
				return X509ChainStatusFlags.ExplicitDistrust;
			case MonoBtlsX509Error.UNHANDLED_CRITICAL_EXTENSION:
				return X509ChainStatusFlags.HasNotSupportedCriticalExtension;
			case MonoBtlsX509Error.INVALID_EXTENSION:
				return X509ChainStatusFlags.InvalidExtension;
			case MonoBtlsX509Error.INVALID_POLICY_EXTENSION:
			case MonoBtlsX509Error.NO_EXPLICIT_POLICY:
				return X509ChainStatusFlags.InvalidPolicyConstraints;
			case MonoBtlsX509Error.HOSTNAME_MISMATCH:
				return X509ChainStatusFlags.UntrustedRoot;
			}
			throw new CryptographicException("Unrecognized X509VerifyStatusCode:" + code.ToString());
		}

		internal static void SetupCertificateStore(MonoBtlsX509Store store, MonoTlsSettings settings, bool server)
		{
			if (server || ((settings != null) ? settings.CertificateSearchPaths : null) == null)
			{
				MonoBtlsProvider.AddTrustedRoots(store, settings, server);
				if (!server)
				{
					MonoBtlsProvider.SetupDefaultCertificateStore(store);
				}
				return;
			}
			foreach (string text in settings.CertificateSearchPaths)
			{
				if (!(text == "@default"))
				{
					if (!(text == "@trusted"))
					{
						if (!(text == "@user"))
						{
							if (!(text == "@machine"))
							{
								if (text.StartsWith("@pem:"))
								{
									string text2 = text.Substring(5);
									if (Directory.Exists(text2))
									{
										store.AddDirectoryLookup(text2, MonoBtlsX509FileType.PEM);
									}
								}
								else
								{
									if (!text.StartsWith("@der:"))
									{
										throw new NotSupportedException(string.Format("Invalid item `{0}' in MonoTlsSettings.CertificateSearchPaths.", text));
									}
									string text3 = text.Substring(5);
									if (Directory.Exists(text3))
									{
										store.AddDirectoryLookup(text3, MonoBtlsX509FileType.ASN1);
									}
								}
							}
							else
							{
								MonoBtlsProvider.AddMachineStore(store);
							}
						}
						else
						{
							MonoBtlsProvider.AddUserStore(store);
						}
					}
					else
					{
						MonoBtlsProvider.AddTrustedRoots(store, settings, server);
					}
				}
				else
				{
					MonoBtlsProvider.AddTrustedRoots(store, settings, server);
					MonoBtlsProvider.AddUserStore(store);
					MonoBtlsProvider.AddMachineStore(store);
				}
			}
		}

		private static void SetupDefaultCertificateStore(MonoBtlsX509Store store)
		{
			MonoBtlsProvider.AddUserStore(store);
			MonoBtlsProvider.AddMachineStore(store);
		}

		private static void AddUserStore(MonoBtlsX509Store store)
		{
			string storePath = MonoBtlsX509StoreManager.GetStorePath(MonoBtlsX509StoreType.UserTrustedRoots);
			if (Directory.Exists(storePath))
			{
				store.AddDirectoryLookup(storePath, MonoBtlsX509FileType.PEM);
			}
		}

		private static void AddMachineStore(MonoBtlsX509Store store)
		{
			string storePath = MonoBtlsX509StoreManager.GetStorePath(MonoBtlsX509StoreType.MachineTrustedRoots);
			if (Directory.Exists(storePath))
			{
				store.AddDirectoryLookup(storePath, MonoBtlsX509FileType.PEM);
			}
		}

		private static void AddTrustedRoots(MonoBtlsX509Store store, MonoTlsSettings settings, bool server)
		{
			if (((settings != null) ? settings.TrustAnchors : null) == null)
			{
				return;
			}
			MonoBtlsX509TrustKind monoBtlsX509TrustKind = (server ? MonoBtlsX509TrustKind.TRUST_CLIENT : MonoBtlsX509TrustKind.TRUST_SERVER);
			store.AddCollection(settings.TrustAnchors, monoBtlsX509TrustKind);
		}

		public static string GetSystemStoreLocation()
		{
			return MonoBtlsX509StoreManager.GetStorePath(MonoBtlsX509StoreType.MachineTrustedRoots);
		}

		public static X509Certificate2 CreateCertificate(byte[] data, MonoBtlsX509Format format)
		{
			X509Certificate2 x509Certificate;
			using (X509CertificateImplBtls x509CertificateImplBtls = new X509CertificateImplBtls(data, format))
			{
				x509Certificate = new X509Certificate2(x509CertificateImplBtls);
			}
			return x509Certificate;
		}

		public static X509Certificate2 CreateCertificate(byte[] data, string password, bool disallowFallback = false)
		{
			X509Certificate2 x509Certificate;
			using (SafePasswordHandle safePasswordHandle = new SafePasswordHandle(password))
			{
				using (X509CertificateImplBtls x509CertificateImplBtls = new X509CertificateImplBtls(data, safePasswordHandle, X509KeyStorageFlags.DefaultKeySet))
				{
					x509Certificate = new X509Certificate2(x509CertificateImplBtls);
				}
			}
			return x509Certificate;
		}

		public static X509Certificate2 CreateCertificate(MonoBtlsX509 x509)
		{
			X509Certificate2 x509Certificate;
			using (X509CertificateImplBtls x509CertificateImplBtls = new X509CertificateImplBtls(x509))
			{
				x509Certificate = new X509Certificate2(x509CertificateImplBtls);
			}
			return x509Certificate;
		}

		public static X509Chain CreateChain()
		{
			X509Chain x509Chain;
			using (X509ChainImplBtls x509ChainImplBtls = new X509ChainImplBtls())
			{
				x509Chain = new X509Chain(x509ChainImplBtls);
			}
			return x509Chain;
		}

		public static X509Chain GetManagedChain(MonoBtlsX509Chain chain)
		{
			return new X509Chain(new X509ChainImplBtls(chain));
		}

		public static MonoBtlsX509 GetBtlsCertificate(X509Certificate certificate)
		{
			X509CertificateImplBtls x509CertificateImplBtls = certificate.Impl as X509CertificateImplBtls;
			if (x509CertificateImplBtls != null)
			{
				return x509CertificateImplBtls.X509.Copy();
			}
			return MonoBtlsX509.LoadFromData(certificate.GetRawCertData(), MonoBtlsX509Format.DER);
		}

		public static MonoBtlsX509Chain GetNativeChain(X509CertificateCollection certificates)
		{
			MonoBtlsX509Chain monoBtlsX509Chain = new MonoBtlsX509Chain();
			MonoBtlsX509Chain monoBtlsX509Chain2;
			try
			{
				foreach (X509Certificate x509Certificate in certificates)
				{
					using (MonoBtlsX509 btlsCertificate = MonoBtlsProvider.GetBtlsCertificate(x509Certificate))
					{
						monoBtlsX509Chain.AddCertificate(btlsCertificate);
					}
				}
				monoBtlsX509Chain2 = monoBtlsX509Chain;
			}
			catch
			{
				monoBtlsX509Chain.Dispose();
				throw;
			}
			return monoBtlsX509Chain2;
		}
	}
}
