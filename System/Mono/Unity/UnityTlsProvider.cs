using System;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Mono.Net.Security;
using Mono.Security.Interface;

namespace Mono.Unity
{
	internal class UnityTlsProvider : MonoTlsProvider
	{
		public override string Name
		{
			get
			{
				return "unitytls";
			}
		}

		public override Guid ID
		{
			get
			{
				return Mono.Net.Security.MonoTlsProviderFactory.UnityTlsId;
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

		public override IMonoSslStream CreateSslStream(Stream innerStream, bool leaveInnerStreamOpen, MonoTlsSettings settings = null)
		{
			return SslStream.CreateMonoSslStream(innerStream, leaveInnerStreamOpen, this, settings);
		}

		internal override IMonoSslStream CreateSslStreamInternal(SslStream sslStream, Stream innerStream, bool leaveInnerStreamOpen, MonoTlsSettings settings)
		{
			return new UnityTlsStream(innerStream, leaveInnerStreamOpen, sslStream, settings, this);
		}

		internal unsafe override bool ValidateCertificate(ICertificateValidator2 validator, string targetHost, bool serverMode, X509CertificateCollection certificates, bool wantsChain, ref X509Chain chain, ref MonoSslPolicyErrors errors, ref int status11)
		{
			UnityTls.unitytls_errorstate unitytls_errorstate = UnityTls.NativeInterface.unitytls_errorstate_create();
			X509ChainImplUnityTls x509ChainImplUnityTls = chain.Impl as X509ChainImplUnityTls;
			if (x509ChainImplUnityTls == null)
			{
				if (certificates == null || certificates.Count == 0)
				{
					errors |= MonoSslPolicyErrors.RemoteCertificateNotAvailable;
					return false;
				}
				if (wantsChain)
				{
					chain = SystemCertificateValidator.CreateX509Chain(certificates);
				}
			}
			else if (UnityTls.NativeInterface.unitytls_x509list_get_x509(x509ChainImplUnityTls.NativeCertificateChain, (IntPtr)0, &unitytls_errorstate).handle == UnityTls.NativeInterface.UNITYTLS_INVALID_HANDLE)
			{
				errors |= MonoSslPolicyErrors.RemoteCertificateNotAvailable;
				return false;
			}
			if (!string.IsNullOrEmpty(targetHost))
			{
				int num = targetHost.IndexOf(':');
				if (num > 0)
				{
					targetHost = targetHost.Substring(0, num);
				}
			}
			UnityTls.unitytls_x509verify_result unitytls_x509verify_result = (UnityTls.unitytls_x509verify_result)2147483648U;
			UnityTls.unitytls_x509list* ptr = null;
			try
			{
				UnityTls.unitytls_x509list_ref unitytls_x509list_ref;
				if (x509ChainImplUnityTls == null)
				{
					ptr = UnityTls.NativeInterface.unitytls_x509list_create(&unitytls_errorstate);
					CertHelper.AddCertificatesToNativeChain(ptr, certificates, &unitytls_errorstate);
					unitytls_x509list_ref = UnityTls.NativeInterface.unitytls_x509list_get_ref(ptr, &unitytls_errorstate);
				}
				else
				{
					unitytls_x509list_ref = x509ChainImplUnityTls.NativeCertificateChain;
				}
				byte[] bytes = Encoding.UTF8.GetBytes(targetHost);
				if (validator.Settings.TrustAnchors != null)
				{
					UnityTls.unitytls_x509list* ptr2 = null;
					try
					{
						ptr2 = UnityTls.NativeInterface.unitytls_x509list_create(&unitytls_errorstate);
						CertHelper.AddCertificatesToNativeChain(ptr2, validator.Settings.TrustAnchors, &unitytls_errorstate);
						UnityTls.unitytls_x509list_ref unitytls_x509list_ref2 = UnityTls.NativeInterface.unitytls_x509list_get_ref(ptr2, &unitytls_errorstate);
						try
						{
							byte[] array;
							byte* ptr3;
							if ((array = bytes) == null || array.Length == 0)
							{
								ptr3 = null;
							}
							else
							{
								ptr3 = &array[0];
							}
							unitytls_x509verify_result = UnityTls.NativeInterface.unitytls_x509verify_explicit_ca(unitytls_x509list_ref, unitytls_x509list_ref2, ptr3, (IntPtr)bytes.Length, null, null, &unitytls_errorstate);
							goto IL_0200;
						}
						finally
						{
							byte[] array = null;
						}
					}
					finally
					{
						UnityTls.NativeInterface.unitytls_x509list_free(ptr2);
					}
				}
				try
				{
					byte[] array;
					byte* ptr4;
					if ((array = bytes) == null || array.Length == 0)
					{
						ptr4 = null;
					}
					else
					{
						ptr4 = &array[0];
					}
					unitytls_x509verify_result = UnityTls.NativeInterface.unitytls_x509verify_default_ca(unitytls_x509list_ref, ptr4, (IntPtr)bytes.Length, null, null, &unitytls_errorstate);
				}
				finally
				{
					byte[] array = null;
				}
			}
			finally
			{
				UnityTls.NativeInterface.unitytls_x509list_free(ptr);
			}
			IL_0200:
			errors = UnityTlsConversions.VerifyResultToPolicyErrror(unitytls_x509verify_result);
			if (x509ChainImplUnityTls != null)
			{
				x509ChainImplUnityTls.AddStatus(UnityTlsConversions.VerifyResultToChainStatus(unitytls_x509verify_result));
			}
			return unitytls_x509verify_result == UnityTls.unitytls_x509verify_result.UNITYTLS_X509VERIFY_SUCCESS && unitytls_errorstate.code == UnityTls.unitytls_error_code.UNITYTLS_SUCCESS;
		}
	}
}
