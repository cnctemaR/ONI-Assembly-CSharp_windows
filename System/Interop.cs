using System;
using System.IO;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using Microsoft.Win32.SafeHandles;

internal static class Interop
{
	internal static class Crypt32
	{
		internal static global::Interop.Crypt32.CRYPT_OID_INFO FindOidInfo(global::Interop.Crypt32.CryptOidInfoKeyType keyType, string key, OidGroup group, bool fallBackToAllGroups)
		{
			IntPtr intPtr = IntPtr.Zero;
			global::Interop.Crypt32.CRYPT_OID_INFO crypt_OID_INFO;
			try
			{
				if (keyType == global::Interop.Crypt32.CryptOidInfoKeyType.CRYPT_OID_INFO_OID_KEY)
				{
					intPtr = Marshal.StringToCoTaskMemAnsi(key);
				}
				else
				{
					if (keyType != global::Interop.Crypt32.CryptOidInfoKeyType.CRYPT_OID_INFO_NAME_KEY)
					{
						throw new NotSupportedException();
					}
					intPtr = Marshal.StringToCoTaskMemUni(key);
				}
				if (!global::Interop.Crypt32.OidGroupWillNotUseActiveDirectory(group))
				{
					OidGroup oidGroup = group | (OidGroup)(-2147483648);
					IntPtr intPtr2 = global::Interop.Crypt32.CryptFindOIDInfo(keyType, intPtr, oidGroup);
					if (intPtr2 != IntPtr.Zero)
					{
						return Marshal.PtrToStructure<global::Interop.Crypt32.CRYPT_OID_INFO>(intPtr2);
					}
				}
				IntPtr intPtr3 = global::Interop.Crypt32.CryptFindOIDInfo(keyType, intPtr, group);
				if (intPtr3 != IntPtr.Zero)
				{
					crypt_OID_INFO = Marshal.PtrToStructure<global::Interop.Crypt32.CRYPT_OID_INFO>(intPtr3);
				}
				else
				{
					if (fallBackToAllGroups && group != OidGroup.All)
					{
						IntPtr intPtr4 = global::Interop.Crypt32.CryptFindOIDInfo(keyType, intPtr, OidGroup.All);
						if (intPtr4 != IntPtr.Zero)
						{
							return Marshal.PtrToStructure<global::Interop.Crypt32.CRYPT_OID_INFO>(intPtr4);
						}
					}
					crypt_OID_INFO = new global::Interop.Crypt32.CRYPT_OID_INFO
					{
						AlgId = -1
					};
				}
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeCoTaskMem(intPtr);
				}
			}
			return crypt_OID_INFO;
		}

		private static bool OidGroupWillNotUseActiveDirectory(OidGroup group)
		{
			return group == OidGroup.HashAlgorithm || group == OidGroup.EncryptionAlgorithm || group == OidGroup.PublicKeyAlgorithm || group == OidGroup.SignatureAlgorithm || group == OidGroup.Attribute || group == OidGroup.ExtensionOrAttribute || group == OidGroup.KeyDerivationFunction;
		}

		[DllImport("crypt32.dll", CharSet = CharSet.Unicode)]
		private static extern IntPtr CryptFindOIDInfo(global::Interop.Crypt32.CryptOidInfoKeyType dwKeyType, IntPtr pvKey, OidGroup group);

		[DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CertFreeCertificateContext(IntPtr pCertContext);

		[DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CertVerifyCertificateChainPolicy(IntPtr pszPolicyOID, SafeX509ChainHandle pChainContext, [In] ref global::Interop.Crypt32.CERT_CHAIN_POLICY_PARA pPolicyPara, [In] [Out] ref global::Interop.Crypt32.CERT_CHAIN_POLICY_STATUS pPolicyStatus);

		internal struct CRYPT_OID_INFO
		{
			public string OID
			{
				get
				{
					return Marshal.PtrToStringAnsi(this.pszOID);
				}
			}

			public string Name
			{
				get
				{
					return Marshal.PtrToStringUni(this.pwszName);
				}
			}

			public int cbSize;

			public IntPtr pszOID;

			public IntPtr pwszName;

			public OidGroup dwGroupId;

			public int AlgId;

			public int cbData;

			public IntPtr pbData;
		}

		internal enum CryptOidInfoKeyType
		{
			CRYPT_OID_INFO_OID_KEY = 1,
			CRYPT_OID_INFO_NAME_KEY,
			CRYPT_OID_INFO_ALGID_KEY,
			CRYPT_OID_INFO_SIGN_KEY,
			CRYPT_OID_INFO_CNG_ALGID_KEY,
			CRYPT_OID_INFO_CNG_SIGN_KEY
		}

		internal static class AuthType
		{
			internal const uint AUTHTYPE_CLIENT = 1U;

			internal const uint AUTHTYPE_SERVER = 2U;
		}

		internal static class CertChainPolicyIgnoreFlags
		{
			internal const uint CERT_CHAIN_POLICY_IGNORE_NOT_TIME_VALID_FLAG = 1U;

			internal const uint CERT_CHAIN_POLICY_IGNORE_CTL_NOT_TIME_VALID_FLAG = 2U;

			internal const uint CERT_CHAIN_POLICY_IGNORE_NOT_TIME_NESTED_FLAG = 4U;

			internal const uint CERT_CHAIN_POLICY_IGNORE_INVALID_BASIC_CONSTRAINTS_FLAG = 8U;

			internal const uint CERT_CHAIN_POLICY_ALLOW_UNKNOWN_CA_FLAG = 16U;

			internal const uint CERT_CHAIN_POLICY_IGNORE_WRONG_USAGE_FLAG = 32U;

			internal const uint CERT_CHAIN_POLICY_IGNORE_INVALID_NAME_FLAG = 64U;

			internal const uint CERT_CHAIN_POLICY_IGNORE_INVALID_POLICY_FLAG = 128U;

			internal const uint CERT_CHAIN_POLICY_IGNORE_END_REV_UNKNOWN_FLAG = 256U;

			internal const uint CERT_CHAIN_POLICY_IGNORE_CTL_SIGNER_REV_UNKNOWN_FLAG = 512U;

			internal const uint CERT_CHAIN_POLICY_IGNORE_CA_REV_UNKNOWN_FLAG = 1024U;

			internal const uint CERT_CHAIN_POLICY_IGNORE_ROOT_REV_UNKNOWN_FLAG = 2048U;

			internal const uint CERT_CHAIN_POLICY_IGNORE_ALL = 4095U;
		}

		internal static class CertChainPolicy
		{
			internal const int CERT_CHAIN_POLICY_BASE = 1;

			internal const int CERT_CHAIN_POLICY_AUTHENTICODE = 2;

			internal const int CERT_CHAIN_POLICY_AUTHENTICODE_TS = 3;

			internal const int CERT_CHAIN_POLICY_SSL = 4;

			internal const int CERT_CHAIN_POLICY_BASIC_CONSTRAINTS = 5;

			internal const int CERT_CHAIN_POLICY_NT_AUTH = 6;

			internal const int CERT_CHAIN_POLICY_MICROSOFT_ROOT = 7;

			internal const int CERT_CHAIN_POLICY_EV = 8;
		}

		internal static class CertChainPolicyErrors
		{
			internal const uint TRUST_E_CERT_SIGNATURE = 2148098052U;

			internal const uint CRYPT_E_REVOKED = 2148081680U;

			internal const uint CERT_E_UNTRUSTEDROOT = 2148204809U;

			internal const uint CERT_E_UNTRUSTEDTESTROOT = 2148204813U;

			internal const uint CERT_E_CHAINING = 2148204810U;

			internal const uint CERT_E_WRONG_USAGE = 2148204816U;

			internal const uint CERT_E_EXPIRE = 2148204801U;

			internal const uint CERT_E_INVALID_NAME = 2148204820U;

			internal const uint CERT_E_INVALID_POLICY = 2148204819U;

			internal const uint TRUST_E_BASIC_CONSTRAINTS = 2148098073U;

			internal const uint CERT_E_CRITICAL = 2148204805U;

			internal const uint CERT_E_VALIDITYPERIODNESTING = 2148204802U;

			internal const uint CRYPT_E_NO_REVOCATION_CHECK = 2148081682U;

			internal const uint CRYPT_E_REVOCATION_OFFLINE = 2148081683U;

			internal const uint CERT_E_PURPOSE = 2148204806U;

			internal const uint CERT_E_REVOKED = 2148204812U;

			internal const uint CERT_E_REVOCATION_FAILURE = 2148204814U;

			internal const uint CERT_E_CN_NO_MATCH = 2148204815U;

			internal const uint CERT_E_ROLE = 2148204803U;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct CERT_CONTEXT
		{
			internal uint dwCertEncodingType;

			internal IntPtr pbCertEncoded;

			internal uint cbCertEncoded;

			internal IntPtr pCertInfo;

			internal IntPtr hCertStore;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct SSL_EXTRA_CERT_CHAIN_POLICY_PARA
		{
			internal uint cbSize;

			internal uint dwAuthType;

			internal uint fdwChecks;

			internal unsafe char* pwszServerName;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct CERT_CHAIN_POLICY_PARA
		{
			public uint cbSize;

			public uint dwFlags;

			public unsafe global::Interop.Crypt32.SSL_EXTRA_CERT_CHAIN_POLICY_PARA* pvExtraPolicyPara;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct CERT_CHAIN_POLICY_STATUS
		{
			public uint cbSize;

			public uint dwError;

			public int lChainIndex;

			public int lElementIndex;

			public unsafe void* pvExtraPolicyStatus;
		}
	}

	internal enum BOOL
	{
		FALSE,
		TRUE
	}

	internal static class Libraries
	{
		internal const string Advapi32 = "advapi32.dll";

		internal const string BCrypt = "BCrypt.dll";

		internal const string CoreComm_L1_1_1 = "api-ms-win-core-comm-l1-1-1.dll";

		internal const string Crypt32 = "crypt32.dll";

		internal const string Error_L1 = "api-ms-win-core-winrt-error-l1-1-0.dll";

		internal const string HttpApi = "httpapi.dll";

		internal const string IpHlpApi = "iphlpapi.dll";

		internal const string Kernel32 = "kernel32.dll";

		internal const string Memory_L1_3 = "api-ms-win-core-memory-l1-1-3.dll";

		internal const string Mswsock = "mswsock.dll";

		internal const string NCrypt = "ncrypt.dll";

		internal const string NtDll = "ntdll.dll";

		internal const string Odbc32 = "odbc32.dll";

		internal const string OleAut32 = "oleaut32.dll";

		internal const string PerfCounter = "perfcounter.dll";

		internal const string RoBuffer = "api-ms-win-core-winrt-robuffer-l1-1-0.dll";

		internal const string Secur32 = "secur32.dll";

		internal const string Shell32 = "shell32.dll";

		internal const string SspiCli = "sspicli.dll";

		internal const string User32 = "user32.dll";

		internal const string Version = "version.dll";

		internal const string WebSocket = "websocket.dll";

		internal const string WinHttp = "winhttp.dll";

		internal const string Ws2_32 = "ws2_32.dll";

		internal const string Wtsapi32 = "wtsapi32.dll";

		internal const string CompressionNative = "clrcompression.dll";
	}

	internal enum SECURITY_STATUS
	{
		OK,
		ContinueNeeded = 590610,
		CompleteNeeded,
		CompAndContinue,
		ContextExpired = 590615,
		CredentialsNeeded = 590624,
		Renegotiate,
		OutOfMemory = -2146893056,
		InvalidHandle,
		Unsupported,
		TargetUnknown,
		InternalError,
		PackageNotFound,
		NotOwner,
		CannotInstall,
		InvalidToken,
		CannotPack,
		QopNotSupported,
		NoImpersonation,
		LogonDenied,
		UnknownCredentials,
		NoCredentials,
		MessageAltered,
		OutOfSequence,
		NoAuthenticatingAuthority,
		IncompleteMessage = -2146893032,
		IncompleteCredentials = -2146893024,
		BufferNotEnough,
		WrongPrincipal,
		TimeSkew = -2146893020,
		UntrustedRoot,
		IllegalMessage,
		CertUnknown,
		CertExpired,
		AlgorithmMismatch = -2146893007,
		SecurityQosFailed,
		SmartcardLogonRequired = -2146892994,
		UnsupportedPreauth = -2146892989,
		BadBinding = -2146892986,
		DowngradeDetected = -2146892976,
		ApplicationProtocolMismatch = -2146892953
	}

	internal enum ApplicationProtocolNegotiationStatus
	{
		None,
		Success,
		SelectedClientOnly
	}

	internal enum ApplicationProtocolNegotiationExt
	{
		None,
		NPN,
		ALPN
	}

	[StructLayout(LayoutKind.Sequential)]
	internal class SecPkgContext_ApplicationProtocol
	{
		public byte[] Protocol
		{
			get
			{
				return new Span<byte>(this.ProtocolId, 0, (int)this.ProtocolIdSize).ToArray();
			}
		}

		private const int MaxProtocolIdSize = 255;

		public global::Interop.ApplicationProtocolNegotiationStatus ProtoNegoStatus;

		public global::Interop.ApplicationProtocolNegotiationExt ProtoNegoExt;

		public byte ProtocolIdSize;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
		public byte[] ProtocolId;
	}

	internal class Kernel32
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CloseHandle(IntPtr handle);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "CreateFileW", ExactSpelling = true, SetLastError = true)]
		private unsafe static extern IntPtr CreateFilePrivate(string lpFileName, int dwDesiredAccess, FileShare dwShareMode, global::Interop.Kernel32.SECURITY_ATTRIBUTES* securityAttrs, FileMode dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

		internal unsafe static SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, FileShare dwShareMode, ref global::Interop.Kernel32.SECURITY_ATTRIBUTES securityAttrs, FileMode dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile)
		{
			lpFileName = global::System.IO.PathInternal.EnsureExtendedPrefixIfNeeded(lpFileName);
			fixed (global::Interop.Kernel32.SECURITY_ATTRIBUTES* ptr = &securityAttrs)
			{
				global::Interop.Kernel32.SECURITY_ATTRIBUTES* ptr2 = ptr;
				IntPtr intPtr = global::Interop.Kernel32.CreateFilePrivate(lpFileName, dwDesiredAccess, dwShareMode, ptr2, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);
				SafeFileHandle safeFileHandle;
				try
				{
					safeFileHandle = new SafeFileHandle(intPtr, true);
				}
				catch
				{
					global::Interop.Kernel32.CloseHandle(intPtr);
					throw;
				}
				return safeFileHandle;
			}
		}

		internal static SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, FileShare dwShareMode, FileMode dwCreationDisposition, int dwFlagsAndAttributes)
		{
			IntPtr intPtr = global::Interop.Kernel32.CreateFile_IntPtr(lpFileName, dwDesiredAccess, dwShareMode, dwCreationDisposition, dwFlagsAndAttributes);
			SafeFileHandle safeFileHandle;
			try
			{
				safeFileHandle = new SafeFileHandle(intPtr, true);
			}
			catch
			{
				global::Interop.Kernel32.CloseHandle(intPtr);
				throw;
			}
			return safeFileHandle;
		}

		internal static IntPtr CreateFile_IntPtr(string lpFileName, int dwDesiredAccess, FileShare dwShareMode, FileMode dwCreationDisposition, int dwFlagsAndAttributes)
		{
			lpFileName = global::System.IO.PathInternal.EnsureExtendedPrefixIfNeeded(lpFileName);
			return global::Interop.Kernel32.CreateFilePrivate(lpFileName, dwDesiredAccess, dwShareMode, null, dwCreationDisposition, dwFlagsAndAttributes, IntPtr.Zero);
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal unsafe static extern bool ReadDirectoryChangesW(SafeFileHandle hDirectory, byte[] lpBuffer, uint nBufferLength, [MarshalAs(UnmanagedType.Bool)] bool bWatchSubtree, int dwNotifyFilter, out int lpBytesReturned, NativeOverlapped* lpOverlapped, IntPtr lpCompletionRoutine);

		internal const uint SEM_FAILCRITICALERRORS = 1U;

		internal class IOReparseOptions
		{
			internal const uint IO_REPARSE_TAG_FILE_PLACEHOLDER = 2147483669U;

			internal const uint IO_REPARSE_TAG_MOUNT_POINT = 2684354563U;
		}

		internal class FileOperations
		{
			internal const int OPEN_EXISTING = 3;

			internal const int COPY_FILE_FAIL_IF_EXISTS = 1;

			internal const int FILE_ACTION_ADDED = 1;

			internal const int FILE_ACTION_REMOVED = 2;

			internal const int FILE_ACTION_MODIFIED = 3;

			internal const int FILE_ACTION_RENAMED_OLD_NAME = 4;

			internal const int FILE_ACTION_RENAMED_NEW_NAME = 5;

			internal const int FILE_FLAG_BACKUP_SEMANTICS = 33554432;

			internal const int FILE_FLAG_FIRST_PIPE_INSTANCE = 524288;

			internal const int FILE_FLAG_OVERLAPPED = 1073741824;

			internal const int FILE_LIST_DIRECTORY = 1;
		}

		internal struct SECURITY_ATTRIBUTES
		{
			internal uint nLength;

			internal IntPtr lpSecurityDescriptor;

			internal global::Interop.BOOL bInheritHandle;
		}
	}

	internal static class SspiCli
	{
		[DllImport("sspicli.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern int EncryptMessage(ref global::Interop.SspiCli.CredHandle contextHandle, [In] uint qualityOfProtection, [In] [Out] ref global::Interop.SspiCli.SecBufferDesc inputOutput, [In] uint sequenceNumber);

		[DllImport("sspicli.dll", ExactSpelling = true, SetLastError = true)]
		internal unsafe static extern int DecryptMessage([In] ref global::Interop.SspiCli.CredHandle contextHandle, [In] [Out] ref global::Interop.SspiCli.SecBufferDesc inputOutput, [In] uint sequenceNumber, uint* qualityOfProtection);

		[DllImport("sspicli.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern int QuerySecurityContextToken(ref global::Interop.SspiCli.CredHandle phContext, out SecurityContextTokenHandle handle);

		[DllImport("sspicli.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern int FreeContextBuffer([In] IntPtr contextBuffer);

		[DllImport("sspicli.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern int FreeCredentialsHandle(ref global::Interop.SspiCli.CredHandle handlePtr);

		[DllImport("sspicli.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern int DeleteSecurityContext(ref global::Interop.SspiCli.CredHandle handlePtr);

		[DllImport("sspicli.dll", ExactSpelling = true, SetLastError = true)]
		internal unsafe static extern int AcceptSecurityContext(ref global::Interop.SspiCli.CredHandle credentialHandle, [In] void* inContextPtr, [In] global::Interop.SspiCli.SecBufferDesc* inputBuffer, [In] global::Interop.SspiCli.ContextFlags inFlags, [In] global::Interop.SspiCli.Endianness endianness, ref global::Interop.SspiCli.CredHandle outContextPtr, [In] [Out] ref global::Interop.SspiCli.SecBufferDesc outputBuffer, [In] [Out] ref global::Interop.SspiCli.ContextFlags attributes, out long timeStamp);

		[DllImport("sspicli.dll", ExactSpelling = true, SetLastError = true)]
		internal unsafe static extern int QueryContextAttributesW(ref global::Interop.SspiCli.CredHandle contextHandle, [In] global::Interop.SspiCli.ContextAttribute attribute, [In] void* buffer);

		[DllImport("sspicli.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern int SetContextAttributesW(ref global::Interop.SspiCli.CredHandle contextHandle, [In] global::Interop.SspiCli.ContextAttribute attribute, [In] byte[] buffer, [In] int bufferSize);

		[DllImport("sspicli.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern int EnumerateSecurityPackagesW(out int pkgnum, out SafeFreeContextBuffer_SECURITY handle);

		[DllImport("sspicli.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		internal unsafe static extern int AcquireCredentialsHandleW([In] string principal, [In] string moduleName, [In] int usage, [In] void* logonID, [In] ref global::Interop.SspiCli.SEC_WINNT_AUTH_IDENTITY_W authdata, [In] void* keyCallback, [In] void* keyArgument, ref global::Interop.SspiCli.CredHandle handlePtr, out long timeStamp);

		[DllImport("sspicli.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		internal unsafe static extern int AcquireCredentialsHandleW([In] string principal, [In] string moduleName, [In] int usage, [In] void* logonID, [In] IntPtr zero, [In] void* keyCallback, [In] void* keyArgument, ref global::Interop.SspiCli.CredHandle handlePtr, out long timeStamp);

		[DllImport("sspicli.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		internal unsafe static extern int AcquireCredentialsHandleW([In] string principal, [In] string moduleName, [In] int usage, [In] void* logonID, [In] SafeSspiAuthDataHandle authdata, [In] void* keyCallback, [In] void* keyArgument, ref global::Interop.SspiCli.CredHandle handlePtr, out long timeStamp);

		[DllImport("sspicli.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		internal unsafe static extern int AcquireCredentialsHandleW([In] string principal, [In] string moduleName, [In] int usage, [In] void* logonID, [In] ref global::Interop.SspiCli.SCHANNEL_CRED authData, [In] void* keyCallback, [In] void* keyArgument, ref global::Interop.SspiCli.CredHandle handlePtr, out long timeStamp);

		[DllImport("sspicli.dll", ExactSpelling = true, SetLastError = true)]
		internal unsafe static extern int InitializeSecurityContextW(ref global::Interop.SspiCli.CredHandle credentialHandle, [In] void* inContextPtr, [In] byte* targetName, [In] global::Interop.SspiCli.ContextFlags inFlags, [In] int reservedI, [In] global::Interop.SspiCli.Endianness endianness, [In] global::Interop.SspiCli.SecBufferDesc* inputBuffer, [In] int reservedII, ref global::Interop.SspiCli.CredHandle outContextPtr, [In] [Out] ref global::Interop.SspiCli.SecBufferDesc outputBuffer, [In] [Out] ref global::Interop.SspiCli.ContextFlags attributes, out long timeStamp);

		[DllImport("sspicli.dll", ExactSpelling = true, SetLastError = true)]
		internal unsafe static extern int CompleteAuthToken([In] void* inContextPtr, [In] [Out] ref global::Interop.SspiCli.SecBufferDesc inputBuffers);

		[DllImport("sspicli.dll", ExactSpelling = true, SetLastError = true)]
		internal unsafe static extern int ApplyControlToken([In] void* inContextPtr, [In] [Out] ref global::Interop.SspiCli.SecBufferDesc inputBuffers);

		[DllImport("sspicli.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern global::Interop.SECURITY_STATUS SspiFreeAuthIdentity([In] IntPtr authData);

		[DllImport("sspicli.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		internal static extern global::Interop.SECURITY_STATUS SspiEncodeStringsAsAuthIdentity([In] string userName, [In] string domainName, [In] string password, out SafeSspiAuthDataHandle authData);

		internal const uint SECQOP_WRAP_NO_ENCRYPT = 2147483649U;

		internal const int SEC_I_RENEGOTIATE = 590625;

		internal const int SECPKG_NEGOTIATION_COMPLETE = 0;

		internal const int SECPKG_NEGOTIATION_OPTIMISTIC = 1;

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct CredHandle
		{
			public bool IsZero
			{
				get
				{
					return this.dwLower == IntPtr.Zero && this.dwUpper == IntPtr.Zero;
				}
			}

			internal void SetToInvalid()
			{
				this.dwLower = IntPtr.Zero;
				this.dwUpper = IntPtr.Zero;
			}

			public override string ToString()
			{
				return this.dwLower.ToString("x") + ":" + this.dwUpper.ToString("x");
			}

			private IntPtr dwLower;

			private IntPtr dwUpper;
		}

		internal enum ContextAttribute
		{
			SECPKG_ATTR_SIZES,
			SECPKG_ATTR_NAMES,
			SECPKG_ATTR_LIFESPAN,
			SECPKG_ATTR_DCE_INFO,
			SECPKG_ATTR_STREAM_SIZES,
			SECPKG_ATTR_AUTHORITY = 6,
			SECPKG_ATTR_PACKAGE_INFO = 10,
			SECPKG_ATTR_NEGOTIATION_INFO = 12,
			SECPKG_ATTR_UNIQUE_BINDINGS = 25,
			SECPKG_ATTR_ENDPOINT_BINDINGS,
			SECPKG_ATTR_CLIENT_SPECIFIED_TARGET,
			SECPKG_ATTR_APPLICATION_PROTOCOL = 35,
			SECPKG_ATTR_REMOTE_CERT_CONTEXT = 83,
			SECPKG_ATTR_LOCAL_CERT_CONTEXT,
			SECPKG_ATTR_ROOT_STORE,
			SECPKG_ATTR_ISSUER_LIST_EX = 89,
			SECPKG_ATTR_CONNECTION_INFO,
			SECPKG_ATTR_UI_INFO = 104
		}

		[Flags]
		internal enum ContextFlags
		{
			Zero = 0,
			Delegate = 1,
			MutualAuth = 2,
			ReplayDetect = 4,
			SequenceDetect = 8,
			Confidentiality = 16,
			UseSessionKey = 32,
			AllocateMemory = 256,
			Connection = 2048,
			InitExtendedError = 16384,
			AcceptExtendedError = 32768,
			InitStream = 32768,
			AcceptStream = 65536,
			InitIntegrity = 65536,
			AcceptIntegrity = 131072,
			InitManualCredValidation = 524288,
			InitUseSuppliedCreds = 128,
			InitIdentify = 131072,
			AcceptIdentify = 524288,
			ProxyBindings = 67108864,
			AllowMissingBindings = 268435456,
			UnverifiedTargetName = 536870912
		}

		internal enum Endianness
		{
			SECURITY_NETWORK_DREP,
			SECURITY_NATIVE_DREP = 16
		}

		internal enum CredentialUse
		{
			SECPKG_CRED_INBOUND = 1,
			SECPKG_CRED_OUTBOUND,
			SECPKG_CRED_BOTH
		}

		internal struct CERT_CHAIN_ELEMENT
		{
			public uint cbSize;

			public IntPtr pCertContext;
		}

		internal struct SecPkgContext_IssuerListInfoEx
		{
			public unsafe SecPkgContext_IssuerListInfoEx(SafeHandle handle, byte[] nativeBuffer)
			{
				this.aIssuers = handle;
				fixed (byte[] array = nativeBuffer)
				{
					byte* ptr;
					if (nativeBuffer == null || array.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array[0];
					}
					this.cIssuers = *(uint*)(ptr + IntPtr.Size);
				}
			}

			public SafeHandle aIssuers;

			public uint cIssuers;
		}

		internal struct SCHANNEL_CRED
		{
			public const int CurrentVersion = 4;

			public int dwVersion;

			public int cCreds;

			public IntPtr paCred;

			public IntPtr hRootStore;

			public int cMappers;

			public IntPtr aphMappers;

			public int cSupportedAlgs;

			public IntPtr palgSupportedAlgs;

			public int grbitEnabledProtocols;

			public int dwMinimumCipherStrength;

			public int dwMaximumCipherStrength;

			public int dwSessionLifespan;

			public global::Interop.SspiCli.SCHANNEL_CRED.Flags dwFlags;

			public int reserved;

			[Flags]
			public enum Flags
			{
				Zero = 0,
				SCH_CRED_NO_SYSTEM_MAPPER = 2,
				SCH_CRED_NO_SERVERNAME_CHECK = 4,
				SCH_CRED_MANUAL_CRED_VALIDATION = 8,
				SCH_CRED_NO_DEFAULT_CREDS = 16,
				SCH_CRED_AUTO_CRED_VALIDATION = 32,
				SCH_SEND_AUX_RECORD = 2097152,
				SCH_USE_STRONG_CRYPTO = 4194304
			}
		}

		internal struct SecBuffer
		{
			public int cbBuffer;

			public SecurityBufferType BufferType;

			public IntPtr pvBuffer;

			public static readonly int Size = sizeof(global::Interop.SspiCli.SecBuffer);
		}

		internal struct SecBufferDesc
		{
			public SecBufferDesc(int count)
			{
				this.ulVersion = 0;
				this.cBuffers = count;
				this.pBuffers = null;
			}

			public readonly int ulVersion;

			public readonly int cBuffers;

			public unsafe void* pBuffers;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct SEC_WINNT_AUTH_IDENTITY_W
		{
			internal string User;

			internal int UserLength;

			internal string Domain;

			internal int DomainLength;

			internal string Password;

			internal int PasswordLength;

			internal int Flags;
		}
	}
}
