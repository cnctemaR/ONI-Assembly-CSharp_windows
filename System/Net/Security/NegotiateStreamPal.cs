using System;
using System.ComponentModel;
using System.Globalization;

namespace System.Net.Security
{
	internal static class NegotiateStreamPal
	{
		internal static int QueryMaxTokenSize(string package)
		{
			return SSPIWrapper.GetVerifyPackageInfo(GlobalSSPI.SSPIAuth, package, true).MaxToken;
		}

		internal static SafeFreeCredentials AcquireDefaultCredential(string package, bool isServer)
		{
			return SSPIWrapper.AcquireDefaultCredential(GlobalSSPI.SSPIAuth, package, isServer ? global::Interop.SspiCli.CredentialUse.SECPKG_CRED_INBOUND : global::Interop.SspiCli.CredentialUse.SECPKG_CRED_OUTBOUND);
		}

		internal static SafeFreeCredentials AcquireCredentialsHandle(string package, bool isServer, NetworkCredential credential)
		{
			SafeSspiAuthDataHandle safeSspiAuthDataHandle = null;
			SafeFreeCredentials safeFreeCredentials;
			try
			{
				global::Interop.SECURITY_STATUS security_STATUS = global::Interop.SspiCli.SspiEncodeStringsAsAuthIdentity(credential.UserName, credential.Domain, credential.Password, out safeSspiAuthDataHandle);
				if (security_STATUS != global::Interop.SECURITY_STATUS.OK)
				{
					if (NetEventSource.IsEnabled)
					{
						NetEventSource.Error(null, SR.Format("{0} failed with error {1}.", "SspiEncodeStringsAsAuthIdentity", string.Format("0x{0:X}", (int)security_STATUS)), "AcquireCredentialsHandle");
					}
					throw new Win32Exception((int)security_STATUS);
				}
				safeFreeCredentials = SSPIWrapper.AcquireCredentialsHandle(GlobalSSPI.SSPIAuth, package, isServer ? global::Interop.SspiCli.CredentialUse.SECPKG_CRED_INBOUND : global::Interop.SspiCli.CredentialUse.SECPKG_CRED_OUTBOUND, ref safeSspiAuthDataHandle);
			}
			finally
			{
				if (safeSspiAuthDataHandle != null)
				{
					safeSspiAuthDataHandle.Dispose();
				}
			}
			return safeFreeCredentials;
		}

		internal static string QueryContextClientSpecifiedSpn(SafeDeleteContext securityContext)
		{
			return SSPIWrapper.QueryContextAttributes(GlobalSSPI.SSPIAuth, securityContext, global::Interop.SspiCli.ContextAttribute.SECPKG_ATTR_CLIENT_SPECIFIED_TARGET) as string;
		}

		internal static string QueryContextAuthenticationPackage(SafeDeleteContext securityContext)
		{
			NegotiationInfoClass negotiationInfoClass = SSPIWrapper.QueryContextAttributes(GlobalSSPI.SSPIAuth, securityContext, global::Interop.SspiCli.ContextAttribute.SECPKG_ATTR_NEGOTIATION_INFO) as NegotiationInfoClass;
			if (negotiationInfoClass == null)
			{
				return null;
			}
			return negotiationInfoClass.AuthenticationPackage;
		}

		internal static SecurityStatusPal InitializeSecurityContext(SafeFreeCredentials credentialsHandle, ref SafeDeleteContext securityContext, string spn, ContextFlagsPal requestedContextFlags, SecurityBuffer[] inSecurityBufferArray, SecurityBuffer outSecurityBuffer, ref ContextFlagsPal contextFlags)
		{
			global::Interop.SspiCli.ContextFlags contextFlags2 = global::Interop.SspiCli.ContextFlags.Zero;
			global::Interop.SECURITY_STATUS security_STATUS = (global::Interop.SECURITY_STATUS)SSPIWrapper.InitializeSecurityContext(GlobalSSPI.SSPIAuth, credentialsHandle, ref securityContext, spn, ContextFlagsAdapterPal.GetInteropFromContextFlagsPal(requestedContextFlags), global::Interop.SspiCli.Endianness.SECURITY_NETWORK_DREP, inSecurityBufferArray, outSecurityBuffer, ref contextFlags2);
			contextFlags = ContextFlagsAdapterPal.GetContextFlagsPalFromInterop(contextFlags2);
			return SecurityStatusAdapterPal.GetSecurityStatusPalFromInterop(security_STATUS, false);
		}

		internal static SecurityStatusPal CompleteAuthToken(ref SafeDeleteContext securityContext, SecurityBuffer[] inSecurityBufferArray)
		{
			return SecurityStatusAdapterPal.GetSecurityStatusPalFromInterop((global::Interop.SECURITY_STATUS)SSPIWrapper.CompleteAuthToken(GlobalSSPI.SSPIAuth, ref securityContext, inSecurityBufferArray), false);
		}

		internal static SecurityStatusPal AcceptSecurityContext(SafeFreeCredentials credentialsHandle, ref SafeDeleteContext securityContext, ContextFlagsPal requestedContextFlags, SecurityBuffer[] inSecurityBufferArray, SecurityBuffer outSecurityBuffer, ref ContextFlagsPal contextFlags)
		{
			global::Interop.SspiCli.ContextFlags contextFlags2 = global::Interop.SspiCli.ContextFlags.Zero;
			global::Interop.SECURITY_STATUS security_STATUS = (global::Interop.SECURITY_STATUS)SSPIWrapper.AcceptSecurityContext(GlobalSSPI.SSPIAuth, credentialsHandle, ref securityContext, ContextFlagsAdapterPal.GetInteropFromContextFlagsPal(requestedContextFlags), global::Interop.SspiCli.Endianness.SECURITY_NETWORK_DREP, inSecurityBufferArray, outSecurityBuffer, ref contextFlags2);
			contextFlags = ContextFlagsAdapterPal.GetContextFlagsPalFromInterop(contextFlags2);
			return SecurityStatusAdapterPal.GetSecurityStatusPalFromInterop(security_STATUS, false);
		}

		internal static Win32Exception CreateExceptionFromError(SecurityStatusPal statusCode)
		{
			return new Win32Exception((int)SecurityStatusAdapterPal.GetInteropFromSecurityStatusPal(statusCode));
		}

		internal static int VerifySignature(SafeDeleteContext securityContext, byte[] buffer, int offset, int count)
		{
			if (offset < 0 || offset > ((buffer == null) ? 0 : buffer.Length))
			{
				NetEventSource.Info("Argument 'offset' out of range.", null, "VerifySignature");
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || count > ((buffer == null) ? 0 : (buffer.Length - offset)))
			{
				NetEventSource.Info("Argument 'count' out of range.", null, "VerifySignature");
				throw new ArgumentOutOfRangeException("count");
			}
			SecurityBuffer[] array = new SecurityBuffer[]
			{
				new SecurityBuffer(buffer, offset, count, SecurityBufferType.SECBUFFER_STREAM),
				new SecurityBuffer(0, SecurityBufferType.SECBUFFER_DATA)
			};
			int num = SSPIWrapper.VerifySignature(GlobalSSPI.SSPIAuth, securityContext, array, 0U);
			if (num != 0)
			{
				NetEventSource.Info("VerifySignature threw error: " + num.ToString("x", NumberFormatInfo.InvariantInfo), null, "VerifySignature");
				throw new Win32Exception(num);
			}
			if (array[1].type != SecurityBufferType.SECBUFFER_DATA)
			{
				throw new InternalException();
			}
			return array[1].size;
		}

		internal static int MakeSignature(SafeDeleteContext securityContext, byte[] buffer, int offset, int count, ref byte[] output)
		{
			SecPkgContext_Sizes secPkgContext_Sizes = SSPIWrapper.QueryContextAttributes(GlobalSSPI.SSPIAuth, securityContext, global::Interop.SspiCli.ContextAttribute.SECPKG_ATTR_SIZES) as SecPkgContext_Sizes;
			int num = count + secPkgContext_Sizes.cbMaxSignature;
			if (output == null || output.Length < num)
			{
				output = new byte[num];
			}
			Buffer.BlockCopy(buffer, offset, output, secPkgContext_Sizes.cbMaxSignature, count);
			SecurityBuffer[] array = new SecurityBuffer[]
			{
				new SecurityBuffer(output, 0, secPkgContext_Sizes.cbMaxSignature, SecurityBufferType.SECBUFFER_TOKEN),
				new SecurityBuffer(output, secPkgContext_Sizes.cbMaxSignature, count, SecurityBufferType.SECBUFFER_DATA)
			};
			int num2 = SSPIWrapper.MakeSignature(GlobalSSPI.SSPIAuth, securityContext, array, 0U);
			if (num2 != 0)
			{
				NetEventSource.Info("MakeSignature threw error: " + num2.ToString("x", NumberFormatInfo.InvariantInfo), null, "MakeSignature");
				throw new Win32Exception(num2);
			}
			return array[0].size + array[1].size;
		}
	}
}
