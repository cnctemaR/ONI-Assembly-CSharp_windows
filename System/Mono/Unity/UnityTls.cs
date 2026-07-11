using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Unity
{
	internal static class UnityTls
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetUnityTlsInterface();

		public static bool IsSupported
		{
			get
			{
				return UnityTls.NativeInterface != null;
			}
		}

		public static UnityTls.unitytls_interface_struct NativeInterface
		{
			get
			{
				if (UnityTls.marshalledInterface == null)
				{
					IntPtr unityTlsInterface = UnityTls.GetUnityTlsInterface();
					if (unityTlsInterface == IntPtr.Zero)
					{
						return null;
					}
					UnityTls.marshalledInterface = Marshal.PtrToStructure<UnityTls.unitytls_interface_struct>(unityTlsInterface);
				}
				return UnityTls.marshalledInterface;
			}
		}

		private static UnityTls.unitytls_interface_struct marshalledInterface;

		public enum unitytls_error_code : uint
		{
			UNITYTLS_SUCCESS,
			UNITYTLS_INVALID_ARGUMENT,
			UNITYTLS_INVALID_FORMAT,
			UNITYTLS_INVALID_PASSWORD,
			UNITYTLS_INVALID_STATE,
			UNITYTLS_BUFFER_OVERFLOW,
			UNITYTLS_OUT_OF_MEMORY,
			UNITYTLS_INTERNAL_ERROR,
			UNITYTLS_NOT_SUPPORTED,
			UNITYTLS_ENTROPY_SOURCE_FAILED,
			UNITYTLS_STREAM_CLOSED,
			UNITYTLS_USER_CUSTOM_ERROR_START = 1048576U,
			UNITYTLS_USER_WOULD_BLOCK,
			UNITYTLS_USER_READ_FAILED,
			UNITYTLS_USER_WRITE_FAILED,
			UNITYTLS_USER_UNKNOWN_ERROR,
			UNITYTLS_USER_CUSTOM_ERROR_END = 2097152U
		}

		public struct unitytls_errorstate
		{
			private uint magic;

			public UnityTls.unitytls_error_code code;

			private ulong reserved;
		}

		public struct unitytls_key
		{
		}

		public struct unitytls_key_ref
		{
			public ulong handle;
		}

		public struct unitytls_x509
		{
		}

		public struct unitytls_x509_ref
		{
			public ulong handle;
		}

		public struct unitytls_x509list
		{
		}

		public struct unitytls_x509list_ref
		{
			public ulong handle;
		}

		[Flags]
		public enum unitytls_x509verify_result : uint
		{
			UNITYTLS_X509VERIFY_SUCCESS = 0U,
			UNITYTLS_X509VERIFY_NOT_DONE = 2147483648U,
			UNITYTLS_X509VERIFY_FATAL_ERROR = 4294967295U,
			UNITYTLS_X509VERIFY_FLAG_EXPIRED = 1U,
			UNITYTLS_X509VERIFY_FLAG_REVOKED = 2U,
			UNITYTLS_X509VERIFY_FLAG_CN_MISMATCH = 4U,
			UNITYTLS_X509VERIFY_FLAG_NOT_TRUSTED = 8U,
			UNITYTLS_X509VERIFY_FLAG_USER_ERROR1 = 65536U,
			UNITYTLS_X509VERIFY_FLAG_USER_ERROR2 = 131072U,
			UNITYTLS_X509VERIFY_FLAG_USER_ERROR3 = 262144U,
			UNITYTLS_X509VERIFY_FLAG_USER_ERROR4 = 524288U,
			UNITYTLS_X509VERIFY_FLAG_USER_ERROR5 = 1048576U,
			UNITYTLS_X509VERIFY_FLAG_USER_ERROR6 = 2097152U,
			UNITYTLS_X509VERIFY_FLAG_USER_ERROR7 = 4194304U,
			UNITYTLS_X509VERIFY_FLAG_USER_ERROR8 = 8388608U,
			UNITYTLS_X509VERIFY_FLAG_UNKNOWN_ERROR = 134217728U
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate UnityTls.unitytls_x509verify_result unitytls_x509verify_callback(void* userData, UnityTls.unitytls_x509_ref cert, UnityTls.unitytls_x509verify_result result, UnityTls.unitytls_errorstate* errorState);

		public struct unitytls_tlsctx
		{
		}

		public struct unitytls_tlsctx_ref
		{
			public ulong handle;
		}

		public struct unitytls_x509name
		{
		}

		public enum unitytls_ciphersuite : uint
		{
			UNITYTLS_CIPHERSUITE_INVALID = 16777215U
		}

		public enum unitytls_protocol : uint
		{
			UNITYTLS_PROTOCOL_TLS_1_0,
			UNITYTLS_PROTOCOL_TLS_1_1,
			UNITYTLS_PROTOCOL_TLS_1_2,
			UNITYTLS_PROTOCOL_INVALID
		}

		public struct unitytls_tlsctx_protocolrange
		{
			public UnityTls.unitytls_protocol min;

			public UnityTls.unitytls_protocol max;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate size_t unitytls_tlsctx_write_callback(void* userData, byte* data, size_t bufferLen, UnityTls.unitytls_errorstate* errorState);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate size_t unitytls_tlsctx_read_callback(void* userData, byte* buffer, size_t bufferLen, UnityTls.unitytls_errorstate* errorState);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate void unitytls_tlsctx_trace_callback(void* userData, UnityTls.unitytls_tlsctx* ctx, byte* traceMessage, size_t traceMessageLen);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate void unitytls_tlsctx_certificate_callback(void* userData, UnityTls.unitytls_tlsctx* ctx, byte* cn, size_t cnLen, UnityTls.unitytls_x509name* caList, size_t caListLen, UnityTls.unitytls_x509list_ref* chain, UnityTls.unitytls_key_ref* key, UnityTls.unitytls_errorstate* errorState);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate UnityTls.unitytls_x509verify_result unitytls_tlsctx_x509verify_callback(void* userData, UnityTls.unitytls_x509list_ref chain, UnityTls.unitytls_errorstate* errorState);

		public struct unitytls_tlsctx_callbacks
		{
			public UnityTls.unitytls_tlsctx_read_callback read;

			public UnityTls.unitytls_tlsctx_write_callback write;

			public unsafe void* data;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class unitytls_interface_struct
		{
			public readonly ulong UNITYTLS_INVALID_HANDLE;

			public readonly UnityTls.unitytls_tlsctx_protocolrange UNITYTLS_TLSCTX_PROTOCOLRANGE_DEFAULT;

			public UnityTls.unitytls_interface_struct.unitytls_errorstate_create_t unitytls_errorstate_create;

			public UnityTls.unitytls_interface_struct.unitytls_errorstate_raise_error_t unitytls_errorstate_raise_error;

			public UnityTls.unitytls_interface_struct.unitytls_key_get_ref_t unitytls_key_get_ref;

			public UnityTls.unitytls_interface_struct.unitytls_key_parse_der_t unitytls_key_parse_der;

			public UnityTls.unitytls_interface_struct.unitytls_key_parse_pem_t unitytls_key_parse_pem;

			public UnityTls.unitytls_interface_struct.unitytls_key_free_t unitytls_key_free;

			public UnityTls.unitytls_interface_struct.unitytls_x509_export_der_t unitytls_x509_export_der;

			public UnityTls.unitytls_interface_struct.unitytls_x509list_get_ref_t unitytls_x509list_get_ref;

			public UnityTls.unitytls_interface_struct.unitytls_x509list_get_x509_t unitytls_x509list_get_x509;

			public UnityTls.unitytls_interface_struct.unitytls_x509list_create_t unitytls_x509list_create;

			public UnityTls.unitytls_interface_struct.unitytls_x509list_append_t unitytls_x509list_append;

			public UnityTls.unitytls_interface_struct.unitytls_x509list_append_der_t unitytls_x509list_append_der;

			public UnityTls.unitytls_interface_struct.unitytls_x509list_append_der_t unitytls_x509list_append_pem;

			public UnityTls.unitytls_interface_struct.unitytls_x509list_free_t unitytls_x509list_free;

			public UnityTls.unitytls_interface_struct.unitytls_x509verify_default_ca_t unitytls_x509verify_default_ca;

			public UnityTls.unitytls_interface_struct.unitytls_x509verify_explicit_ca_t unitytls_x509verify_explicit_ca;

			public UnityTls.unitytls_interface_struct.unitytls_tlsctx_create_server_t unitytls_tlsctx_create_server;

			public UnityTls.unitytls_interface_struct.unitytls_tlsctx_create_client_t unitytls_tlsctx_create_client;

			public UnityTls.unitytls_interface_struct.unitytls_tlsctx_server_require_client_authentication_t unitytls_tlsctx_server_require_client_authentication;

			public UnityTls.unitytls_interface_struct.unitytls_tlsctx_set_certificate_callback_t unitytls_tlsctx_set_certificate_callback;

			public UnityTls.unitytls_interface_struct.unitytls_tlsctx_set_trace_callback_t unitytls_tlsctx_set_trace_callback;

			public UnityTls.unitytls_interface_struct.unitytls_tlsctx_set_x509verify_callback_t unitytls_tlsctx_set_x509verify_callback;

			public UnityTls.unitytls_interface_struct.unitytls_tlsctx_set_supported_ciphersuites_t unitytls_tlsctx_set_supported_ciphersuites;

			public UnityTls.unitytls_interface_struct.unitytls_tlsctx_get_ciphersuite_t unitytls_tlsctx_get_ciphersuite;

			public UnityTls.unitytls_interface_struct.unitytls_tlsctx_get_protocol_t unitytls_tlsctx_get_protocol;

			public UnityTls.unitytls_interface_struct.unitytls_tlsctx_process_handshake_t unitytls_tlsctx_process_handshake;

			public UnityTls.unitytls_interface_struct.unitytls_tlsctx_read_t unitytls_tlsctx_read;

			public UnityTls.unitytls_interface_struct.unitytls_tlsctx_write_t unitytls_tlsctx_write;

			public UnityTls.unitytls_interface_struct.unitytls_tlsctx_notify_close_t unitytls_tlsctx_notify_close;

			public UnityTls.unitytls_interface_struct.unitytls_tlsctx_free_t unitytls_tlsctx_free;

			public UnityTls.unitytls_interface_struct.unitytls_random_generate_bytes_t unitytls_random_generate_bytes;

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate UnityTls.unitytls_errorstate unitytls_errorstate_create_t();

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate void unitytls_errorstate_raise_error_t(UnityTls.unitytls_errorstate* errorState, UnityTls.unitytls_error_code errorCode);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate UnityTls.unitytls_key_ref unitytls_key_get_ref_t(UnityTls.unitytls_key* key, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate UnityTls.unitytls_key* unitytls_key_parse_der_t(byte* buffer, size_t bufferLen, byte* password, size_t passwordLen, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate UnityTls.unitytls_key* unitytls_key_parse_pem_t(byte* buffer, size_t bufferLen, byte* password, size_t passwordLen, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate void unitytls_key_free_t(UnityTls.unitytls_key* key);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate size_t unitytls_x509_export_der_t(UnityTls.unitytls_x509_ref cert, byte* buffer, size_t bufferLen, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate UnityTls.unitytls_x509list_ref unitytls_x509list_get_ref_t(UnityTls.unitytls_x509list* list, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate UnityTls.unitytls_x509_ref unitytls_x509list_get_x509_t(UnityTls.unitytls_x509list_ref list, size_t index, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate UnityTls.unitytls_x509list* unitytls_x509list_create_t(UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate void unitytls_x509list_append_t(UnityTls.unitytls_x509list* list, UnityTls.unitytls_x509_ref cert, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate void unitytls_x509list_append_der_t(UnityTls.unitytls_x509list* list, byte* buffer, size_t bufferLen, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate void unitytls_x509list_append_pem_t(UnityTls.unitytls_x509list* list, byte* buffer, size_t bufferLen, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate void unitytls_x509list_free_t(UnityTls.unitytls_x509list* list);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate UnityTls.unitytls_x509verify_result unitytls_x509verify_default_ca_t(UnityTls.unitytls_x509list_ref chain, byte* cn, size_t cnLen, UnityTls.unitytls_x509verify_callback cb, void* userData, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate UnityTls.unitytls_x509verify_result unitytls_x509verify_explicit_ca_t(UnityTls.unitytls_x509list_ref chain, UnityTls.unitytls_x509list_ref trustCA, byte* cn, size_t cnLen, UnityTls.unitytls_x509verify_callback cb, void* userData, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate UnityTls.unitytls_tlsctx* unitytls_tlsctx_create_server_t(UnityTls.unitytls_tlsctx_protocolrange supportedProtocols, UnityTls.unitytls_tlsctx_callbacks callbacks, ulong certChain, ulong leafCertificateKey, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate UnityTls.unitytls_tlsctx* unitytls_tlsctx_create_client_t(UnityTls.unitytls_tlsctx_protocolrange supportedProtocols, UnityTls.unitytls_tlsctx_callbacks callbacks, byte* cn, size_t cnLen, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate void unitytls_tlsctx_server_require_client_authentication_t(UnityTls.unitytls_tlsctx* ctx, UnityTls.unitytls_x509list_ref clientAuthCAList, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate void unitytls_tlsctx_set_certificate_callback_t(UnityTls.unitytls_tlsctx* ctx, UnityTls.unitytls_tlsctx_certificate_callback cb, void* userData, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate void unitytls_tlsctx_set_trace_callback_t(UnityTls.unitytls_tlsctx* ctx, UnityTls.unitytls_tlsctx_trace_callback cb, void* userData, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate void unitytls_tlsctx_set_x509verify_callback_t(UnityTls.unitytls_tlsctx* ctx, UnityTls.unitytls_tlsctx_x509verify_callback cb, void* userData, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate void unitytls_tlsctx_set_supported_ciphersuites_t(UnityTls.unitytls_tlsctx* ctx, UnityTls.unitytls_ciphersuite* supportedCiphersuites, size_t supportedCiphersuitesLen, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate UnityTls.unitytls_ciphersuite unitytls_tlsctx_get_ciphersuite_t(UnityTls.unitytls_tlsctx* ctx, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate UnityTls.unitytls_protocol unitytls_tlsctx_get_protocol_t(UnityTls.unitytls_tlsctx* ctx, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate UnityTls.unitytls_x509verify_result unitytls_tlsctx_process_handshake_t(UnityTls.unitytls_tlsctx* ctx, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate size_t unitytls_tlsctx_read_t(UnityTls.unitytls_tlsctx* ctx, byte* buffer, size_t bufferLen, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate size_t unitytls_tlsctx_write_t(UnityTls.unitytls_tlsctx* ctx, byte* data, size_t bufferLen, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate void unitytls_tlsctx_notify_close_t(UnityTls.unitytls_tlsctx* ctx, UnityTls.unitytls_errorstate* errorState);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate void unitytls_tlsctx_free_t(UnityTls.unitytls_tlsctx* ctx);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public unsafe delegate void unitytls_random_generate_bytes_t(byte* buffer, size_t bufferLen, UnityTls.unitytls_errorstate* errorState);
		}
	}
}
