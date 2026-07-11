using System;
using System.ComponentModel;
using Microsoft.Win32.SafeHandles;

namespace System.Net.Security
{
	internal static class NegotiateStreamPal
	{
		internal static string QueryContextClientSpecifiedSpn(SafeDeleteContext securityContext)
		{
			throw new PlatformNotSupportedException("Server implementation is not supported.");
		}

		internal static string QueryContextAuthenticationPackage(SafeDeleteContext securityContext)
		{
			if (!((SafeDeleteNegoContext)securityContext).IsNtlmUsed)
			{
				return "Kerberos";
			}
			return "NTLM";
		}

		private static byte[] GssWrap(SafeGssContextHandle context, bool encrypt, byte[] buffer, int offset, int count)
		{
			Interop.NetSecurityNative.GssBuffer gssBuffer = default(Interop.NetSecurityNative.GssBuffer);
			byte[] array;
			try
			{
				Interop.NetSecurityNative.Status status2;
				Interop.NetSecurityNative.Status status = Interop.NetSecurityNative.WrapBuffer(out status2, context, encrypt, buffer, offset, count, ref gssBuffer);
				if (status != Interop.NetSecurityNative.Status.GSS_S_COMPLETE)
				{
					throw new Interop.NetSecurityNative.GssApiException(status, status2);
				}
				array = gssBuffer.ToByteArray();
			}
			finally
			{
				gssBuffer.Dispose();
			}
			return array;
		}

		private static int GssUnwrap(SafeGssContextHandle context, byte[] buffer, int offset, int count)
		{
			Interop.NetSecurityNative.GssBuffer gssBuffer = default(Interop.NetSecurityNative.GssBuffer);
			int num;
			try
			{
				Interop.NetSecurityNative.Status status2;
				Interop.NetSecurityNative.Status status = Interop.NetSecurityNative.UnwrapBuffer(out status2, context, buffer, offset, count, ref gssBuffer);
				if (status != Interop.NetSecurityNative.Status.GSS_S_COMPLETE)
				{
					throw new Interop.NetSecurityNative.GssApiException(status, status2);
				}
				num = gssBuffer.Copy(buffer, offset);
			}
			finally
			{
				gssBuffer.Dispose();
			}
			return num;
		}

		private static bool GssInitSecurityContext(ref SafeGssContextHandle context, SafeGssCredHandle credential, bool isNtlm, SafeGssNameHandle targetName, Interop.NetSecurityNative.GssFlags inFlags, byte[] buffer, out byte[] outputBuffer, out uint outFlags, out int isNtlmUsed)
		{
			outputBuffer = null;
			outFlags = 0U;
			if (context == null)
			{
				context = new SafeGssContextHandle();
			}
			Interop.NetSecurityNative.GssBuffer gssBuffer = default(Interop.NetSecurityNative.GssBuffer);
			Interop.NetSecurityNative.Status status;
			try
			{
				Interop.NetSecurityNative.Status status2;
				status = Interop.NetSecurityNative.InitSecContext(out status2, credential, ref context, isNtlm, targetName, (uint)inFlags, buffer, (buffer == null) ? 0 : buffer.Length, ref gssBuffer, out outFlags, out isNtlmUsed);
				if (status != Interop.NetSecurityNative.Status.GSS_S_COMPLETE && status != Interop.NetSecurityNative.Status.GSS_S_CONTINUE_NEEDED)
				{
					throw new Interop.NetSecurityNative.GssApiException(status, status2);
				}
				outputBuffer = gssBuffer.ToByteArray();
			}
			finally
			{
				gssBuffer.Dispose();
			}
			return status == Interop.NetSecurityNative.Status.GSS_S_COMPLETE;
		}

		private static SecurityStatusPal EstablishSecurityContext(SafeFreeNegoCredentials credential, ref SafeDeleteContext context, string targetName, ContextFlagsPal inFlags, SecurityBuffer inputBuffer, SecurityBuffer outputBuffer, ref ContextFlagsPal outFlags)
		{
			bool isNtlmOnly = credential.IsNtlmOnly;
			if (context == null)
			{
				context = (isNtlmOnly ? new SafeDeleteNegoContext(credential, credential.UserName) : new SafeDeleteNegoContext(credential, targetName));
			}
			SafeDeleteNegoContext safeDeleteNegoContext = (SafeDeleteNegoContext)context;
			SecurityStatusPal securityStatusPal;
			try
			{
				Interop.NetSecurityNative.GssFlags interopFromContextFlagsPal = ContextFlagsAdapterPal.GetInteropFromContextFlagsPal(inFlags, false);
				SafeGssContextHandle gssContext = safeDeleteNegoContext.GssContext;
				uint num;
				int num2;
				bool flag = NegotiateStreamPal.GssInitSecurityContext(ref gssContext, credential.GssCredential, isNtlmOnly, safeDeleteNegoContext.TargetName, interopFromContextFlagsPal, (inputBuffer != null) ? inputBuffer.token : null, out outputBuffer.token, out num, out num2);
				outputBuffer.size = outputBuffer.token.Length;
				outputBuffer.offset = 0;
				outFlags = ContextFlagsAdapterPal.GetContextFlagsPalFromInterop((Interop.NetSecurityNative.GssFlags)num, false);
				if (safeDeleteNegoContext.GssContext == null)
				{
					safeDeleteNegoContext.SetGssContext(gssContext);
				}
				if (flag)
				{
					safeDeleteNegoContext.SetAuthenticationPackage(Convert.ToBoolean(num2));
				}
				securityStatusPal = new SecurityStatusPal(flag ? ((safeDeleteNegoContext.IsNtlmUsed && outputBuffer.size > 0) ? SecurityStatusPalErrorCode.OK : SecurityStatusPalErrorCode.CompleteNeeded) : SecurityStatusPalErrorCode.ContinueNeeded, null);
			}
			catch (Exception ex)
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Error(null, ex, "EstablishSecurityContext");
				}
				securityStatusPal = new SecurityStatusPal(SecurityStatusPalErrorCode.InternalError, ex);
			}
			return securityStatusPal;
		}

		internal static SecurityStatusPal InitializeSecurityContext(SafeFreeCredentials credentialsHandle, ref SafeDeleteContext securityContext, string spn, ContextFlagsPal requestedContextFlags, SecurityBuffer[] inSecurityBufferArray, SecurityBuffer outSecurityBuffer, ref ContextFlagsPal contextFlags)
		{
			if (inSecurityBufferArray != null && inSecurityBufferArray.Length > 1)
			{
				throw new PlatformNotSupportedException("No support for channel binding on operating systems other than Windows.");
			}
			SafeFreeNegoCredentials safeFreeNegoCredentials = (SafeFreeNegoCredentials)credentialsHandle;
			if (safeFreeNegoCredentials.IsDefault && string.IsNullOrEmpty(spn))
			{
				throw new PlatformNotSupportedException("Target name should be non-empty if default credentials are passed.");
			}
			SecurityStatusPal securityStatusPal = NegotiateStreamPal.EstablishSecurityContext(safeFreeNegoCredentials, ref securityContext, spn, requestedContextFlags, (inSecurityBufferArray != null && inSecurityBufferArray.Length != 0) ? inSecurityBufferArray[0] : null, outSecurityBuffer, ref contextFlags);
			if (securityStatusPal.ErrorCode == SecurityStatusPalErrorCode.CompleteNeeded)
			{
				ContextFlagsPal contextFlagsPal = ContextFlagsPal.Confidentiality;
				if ((requestedContextFlags & contextFlagsPal) != (contextFlags & contextFlagsPal))
				{
					throw new PlatformNotSupportedException("Requested protection level is not supported with the GSSAPI implementation currently installed.");
				}
			}
			return securityStatusPal;
		}

		internal static SecurityStatusPal AcceptSecurityContext(SafeFreeCredentials credentialsHandle, ref SafeDeleteContext securityContext, ContextFlagsPal requestedContextFlags, SecurityBuffer[] inSecurityBufferArray, SecurityBuffer outSecurityBuffer, ref ContextFlagsPal contextFlags)
		{
			throw new PlatformNotSupportedException("Server implementation is not supported.");
		}

		internal static Win32Exception CreateExceptionFromError(SecurityStatusPal statusCode)
		{
			return new Win32Exception(-2146893792, (statusCode.Exception != null) ? statusCode.Exception.Message : statusCode.ErrorCode.ToString());
		}

		internal static int QueryMaxTokenSize(string package)
		{
			return 0;
		}

		internal static SafeFreeCredentials AcquireDefaultCredential(string package, bool isServer)
		{
			return NegotiateStreamPal.AcquireCredentialsHandle(package, isServer, new NetworkCredential(string.Empty, string.Empty, string.Empty));
		}

		internal static SafeFreeCredentials AcquireCredentialsHandle(string package, bool isServer, NetworkCredential credential)
		{
			if (isServer)
			{
				throw new PlatformNotSupportedException("Server implementation is not supported.");
			}
			bool flag = string.IsNullOrWhiteSpace(credential.UserName) || string.IsNullOrWhiteSpace(credential.Password);
			bool flag2 = string.Equals(package, "NTLM", StringComparison.OrdinalIgnoreCase);
			if (flag2 && flag)
			{
				throw new PlatformNotSupportedException("NTLM authentication is not possible with default credentials on this platform.");
			}
			SafeFreeCredentials safeFreeCredentials;
			try
			{
				safeFreeCredentials = (flag ? new SafeFreeNegoCredentials(false, string.Empty, string.Empty, string.Empty) : new SafeFreeNegoCredentials(flag2, credential.UserName, credential.Password, credential.Domain));
			}
			catch (Exception ex)
			{
				throw new Win32Exception(-2146893792, ex.Message);
			}
			return safeFreeCredentials;
		}

		internal static SecurityStatusPal CompleteAuthToken(ref SafeDeleteContext securityContext, SecurityBuffer[] inSecurityBufferArray)
		{
			return new SecurityStatusPal(SecurityStatusPalErrorCode.OK, null);
		}

		internal static int Encrypt(SafeDeleteContext securityContext, byte[] buffer, int offset, int count, bool isConfidential, bool isNtlm, ref byte[] output, uint sequenceNumber)
		{
			byte[] array = NegotiateStreamPal.GssWrap(((SafeDeleteNegoContext)securityContext).GssContext, isConfidential, buffer, offset, count);
			output = new byte[array.Length + 4];
			Array.Copy(array, 0, output, 4, array.Length);
			int num = array.Length;
			output[0] = (byte)(num & 255);
			output[1] = (byte)((num >> 8) & 255);
			output[2] = (byte)((num >> 16) & 255);
			output[3] = (byte)((num >> 24) & 255);
			return num + 4;
		}

		internal static int Decrypt(SafeDeleteContext securityContext, byte[] buffer, int offset, int count, bool isConfidential, bool isNtlm, out int newOffset, uint sequenceNumber)
		{
			if (offset < 0 || offset > ((buffer == null) ? 0 : buffer.Length))
			{
				NetEventSource.Fail(securityContext, "Argument 'offset' out of range", "Decrypt");
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || count > ((buffer == null) ? 0 : (buffer.Length - offset)))
			{
				NetEventSource.Fail(securityContext, "Argument 'count' out of range.", "Decrypt");
				throw new ArgumentOutOfRangeException("count");
			}
			newOffset = offset;
			return NegotiateStreamPal.GssUnwrap(((SafeDeleteNegoContext)securityContext).GssContext, buffer, offset, count);
		}

		internal static int VerifySignature(SafeDeleteContext securityContext, byte[] buffer, int offset, int count)
		{
			if (offset < 0 || offset > ((buffer == null) ? 0 : buffer.Length))
			{
				NetEventSource.Fail(securityContext, "Argument 'offset' out of range", "VerifySignature");
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || count > ((buffer == null) ? 0 : (buffer.Length - offset)))
			{
				NetEventSource.Fail(securityContext, "Argument 'count' out of range.", "VerifySignature");
				throw new ArgumentOutOfRangeException("count");
			}
			return NegotiateStreamPal.GssUnwrap(((SafeDeleteNegoContext)securityContext).GssContext, buffer, offset, count);
		}

		internal static int MakeSignature(SafeDeleteContext securityContext, byte[] buffer, int offset, int count, ref byte[] output)
		{
			byte[] array = NegotiateStreamPal.GssWrap(((SafeDeleteNegoContext)securityContext).GssContext, false, buffer, offset, count);
			output = new byte[array.Length + 4];
			Array.Copy(array, 0, output, 4, array.Length);
			int num = array.Length;
			output[0] = (byte)(num & 255);
			output[1] = (byte)((num >> 8) & 255);
			output[2] = (byte)((num >> 16) & 255);
			output[3] = (byte)((num >> 24) & 255);
			return num + 4;
		}

		private const int NTE_FAIL = -2146893792;
	}
}
