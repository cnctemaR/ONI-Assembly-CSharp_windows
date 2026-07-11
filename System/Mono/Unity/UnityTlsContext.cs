using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Mono.Net.Security;
using Mono.Security.Cryptography;
using Mono.Security.Interface;
using Mono.Util;

namespace Mono.Unity
{
	internal class UnityTlsContext : MobileTlsContext
	{
		public unsafe UnityTlsContext(MobileAuthenticatedStream parent, bool serverMode, string targetHost, SslProtocols enabledProtocols, X509Certificate serverCertificate, X509CertificateCollection clientCertificates, bool askForClientCert)
			: base(parent, serverMode, targetHost, enabledProtocols, serverCertificate, clientCertificates, askForClientCert)
		{
			this.handle = GCHandle.Alloc(this);
			UnityTls.unitytls_errorstate unitytls_errorstate = UnityTls.NativeInterface.unitytls_errorstate_create();
			UnityTls.unitytls_tlsctx_protocolrange unitytls_tlsctx_protocolrange = new UnityTls.unitytls_tlsctx_protocolrange
			{
				min = UnityTlsConversions.GetMinProtocol(enabledProtocols),
				max = UnityTlsConversions.GetMaxProtocol(enabledProtocols)
			};
			UnityTls.unitytls_tlsctx_callbacks unitytls_tlsctx_callbacks = new UnityTls.unitytls_tlsctx_callbacks
			{
				write = new UnityTls.unitytls_tlsctx_write_callback(UnityTlsContext.WriteCallback),
				read = new UnityTls.unitytls_tlsctx_read_callback(UnityTlsContext.ReadCallback),
				data = (void*)((IntPtr)this.handle)
			};
			if (serverMode)
			{
				UnityTls.unitytls_x509list* ptr;
				UnityTls.unitytls_key* ptr2;
				UnityTlsContext.ExtractNativeKeyAndChainFromManagedCertificate(serverCertificate, &unitytls_errorstate, out ptr, out ptr2);
				try
				{
					UnityTls.unitytls_x509list_ref unitytls_x509list_ref = UnityTls.NativeInterface.unitytls_x509list_get_ref(ptr, &unitytls_errorstate);
					UnityTls.unitytls_key_ref unitytls_key_ref = UnityTls.NativeInterface.unitytls_key_get_ref(ptr2, &unitytls_errorstate);
					Mono.Unity.Debug.CheckAndThrow(unitytls_errorstate, "Failed to parse server key/certificate", AlertDescription.InternalError);
					this.tlsContext = UnityTls.NativeInterface.unitytls_tlsctx_create_server(unitytls_tlsctx_protocolrange, unitytls_tlsctx_callbacks, unitytls_x509list_ref.handle, unitytls_key_ref.handle, &unitytls_errorstate);
					if (askForClientCert)
					{
						UnityTls.unitytls_x509list* ptr3 = null;
						try
						{
							ptr3 = UnityTls.NativeInterface.unitytls_x509list_create(&unitytls_errorstate);
							UnityTls.unitytls_x509list_ref unitytls_x509list_ref2 = UnityTls.NativeInterface.unitytls_x509list_get_ref(ptr3, &unitytls_errorstate);
							UnityTls.NativeInterface.unitytls_tlsctx_server_require_client_authentication(this.tlsContext, unitytls_x509list_ref2, &unitytls_errorstate);
						}
						finally
						{
							UnityTls.NativeInterface.unitytls_x509list_free(ptr3);
						}
					}
					goto IL_023A;
				}
				finally
				{
					UnityTls.NativeInterface.unitytls_x509list_free(ptr);
					UnityTls.NativeInterface.unitytls_key_free(ptr2);
				}
			}
			byte[] bytes = Encoding.UTF8.GetBytes(targetHost);
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
			this.tlsContext = UnityTls.NativeInterface.unitytls_tlsctx_create_client(unitytls_tlsctx_protocolrange, unitytls_tlsctx_callbacks, ptr4, bytes.Length, &unitytls_errorstate);
			array = null;
			UnityTls.NativeInterface.unitytls_tlsctx_set_certificate_callback(this.tlsContext, new UnityTls.unitytls_tlsctx_certificate_callback(UnityTlsContext.CertificateCallback), (void*)((IntPtr)this.handle), &unitytls_errorstate);
			IL_023A:
			UnityTls.NativeInterface.unitytls_tlsctx_set_x509verify_callback(this.tlsContext, new UnityTls.unitytls_tlsctx_x509verify_callback(UnityTlsContext.VerifyCallback), (void*)((IntPtr)this.handle), &unitytls_errorstate);
			Mono.Unity.Debug.CheckAndThrow(unitytls_errorstate, "Failed to create UnityTls context", AlertDescription.InternalError);
			this.hasContext = true;
		}

		private unsafe static void ExtractNativeKeyAndChainFromManagedCertificate(X509Certificate cert, UnityTls.unitytls_errorstate* errorState, out UnityTls.unitytls_x509list* nativeCertChain, out UnityTls.unitytls_key* nativeKey)
		{
			if (cert == null)
			{
				throw new ArgumentNullException("cert");
			}
			X509Certificate2 x509Certificate = cert as X509Certificate2;
			if (x509Certificate == null || x509Certificate.PrivateKey == null)
			{
				throw new ArgumentException("Certificate does not have a private key", "cert");
			}
			nativeCertChain = (IntPtr)((UIntPtr)0);
			nativeKey = (IntPtr)((UIntPtr)0);
			try
			{
				nativeCertChain = UnityTls.NativeInterface.unitytls_x509list_create(errorState);
				CertHelper.AddCertificateToNativeChain(nativeCertChain, cert, errorState);
				byte[] array = Mono.Security.Cryptography.PKCS8.PrivateKeyInfo.Encode(x509Certificate.PrivateKey);
				try
				{
					byte[] array2;
					byte* ptr;
					if ((array2 = array) == null || array2.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array2[0];
					}
					nativeKey = UnityTls.NativeInterface.unitytls_key_parse_der(ptr, array.Length, null, 0, errorState);
				}
				finally
				{
					byte[] array2 = null;
				}
			}
			catch
			{
				UnityTls.NativeInterface.unitytls_x509list_free(nativeCertChain);
				UnityTls.NativeInterface.unitytls_key_free(nativeKey);
				throw;
			}
		}

		public override bool HasContext
		{
			get
			{
				return this.hasContext;
			}
		}

		public override bool IsAuthenticated
		{
			get
			{
				return this.isAuthenticated;
			}
		}

		public override MonoTlsConnectionInfo ConnectionInfo
		{
			get
			{
				return this.connectioninfo;
			}
		}

		internal override bool IsRemoteCertificateAvailable
		{
			get
			{
				return this.remoteCertificate != null;
			}
		}

		internal override X509Certificate LocalClientCertificate
		{
			get
			{
				return this.localClientCertificate;
			}
		}

		public override X509Certificate RemoteCertificate
		{
			get
			{
				return this.remoteCertificate;
			}
		}

		public override TlsProtocols NegotiatedProtocol
		{
			get
			{
				return this.ConnectionInfo.ProtocolVersion;
			}
		}

		public override void Flush()
		{
		}

		[return: TupleElementNames(new string[] { "ret", "wantMore" })]
		public unsafe override ValueTuple<int, bool> Read(byte[] buffer, int offset, int count)
		{
			bool flag = false;
			this.lastException = null;
			UnityTls.unitytls_errorstate unitytls_errorstate = UnityTls.NativeInterface.unitytls_errorstate_create();
			int num;
			fixed (byte[] array = buffer)
			{
				byte* ptr;
				if (buffer == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				num = UnityTls.NativeInterface.unitytls_tlsctx_read(this.tlsContext, ptr + offset, count, &unitytls_errorstate);
			}
			if (this.lastException != null)
			{
				throw this.lastException;
			}
			if (unitytls_errorstate.code == UnityTls.unitytls_error_code.UNITYTLS_USER_WOULD_BLOCK || num < count)
			{
				flag = true;
			}
			else
			{
				if (unitytls_errorstate.code == UnityTls.unitytls_error_code.UNITYTLS_STREAM_CLOSED)
				{
					return new ValueTuple<int, bool>(0, false);
				}
				Mono.Unity.Debug.CheckAndThrow(unitytls_errorstate, "Failed to read data from TLS context", AlertDescription.InternalError);
			}
			return new ValueTuple<int, bool>(num, flag);
		}

		[return: TupleElementNames(new string[] { "ret", "wantMore" })]
		public unsafe override ValueTuple<int, bool> Write(byte[] buffer, int offset, int count)
		{
			bool flag = false;
			this.lastException = null;
			UnityTls.unitytls_errorstate unitytls_errorstate = UnityTls.NativeInterface.unitytls_errorstate_create();
			int num;
			fixed (byte[] array = buffer)
			{
				byte* ptr;
				if (buffer == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				num = UnityTls.NativeInterface.unitytls_tlsctx_write(this.tlsContext, ptr + offset, count, &unitytls_errorstate);
			}
			if (this.lastException != null)
			{
				throw this.lastException;
			}
			if (unitytls_errorstate.code == UnityTls.unitytls_error_code.UNITYTLS_USER_WOULD_BLOCK || num < count)
			{
				flag = true;
			}
			else
			{
				if (unitytls_errorstate.code == UnityTls.unitytls_error_code.UNITYTLS_STREAM_CLOSED)
				{
					return new ValueTuple<int, bool>(0, false);
				}
				Mono.Unity.Debug.CheckAndThrow(unitytls_errorstate, "Failed to write data to TLS context", AlertDescription.InternalError);
			}
			return new ValueTuple<int, bool>(num, flag);
		}

		public unsafe override void Shutdown()
		{
			if (base.Settings != null && base.Settings.SendCloseNotify)
			{
				UnityTls.unitytls_errorstate unitytls_errorstate = UnityTls.NativeInterface.unitytls_errorstate_create();
				UnityTls.NativeInterface.unitytls_tlsctx_notify_close(this.tlsContext, &unitytls_errorstate);
			}
			UnityTls.NativeInterface.unitytls_x509list_free(this.requestedClientCertChain);
			UnityTls.NativeInterface.unitytls_key_free(this.requestedClientKey);
			UnityTls.NativeInterface.unitytls_tlsctx_free(this.tlsContext);
			this.tlsContext = null;
			this.hasContext = false;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					this.Shutdown();
					this.localClientCertificate = null;
					this.remoteCertificate = null;
					if (this.localClientCertificate != null)
					{
						this.localClientCertificate.Dispose();
						this.localClientCertificate = null;
					}
					if (this.remoteCertificate != null)
					{
						this.remoteCertificate.Dispose();
						this.remoteCertificate = null;
					}
					this.connectioninfo = null;
					this.isAuthenticated = false;
					this.hasContext = false;
				}
				this.handle.Free();
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public unsafe override void StartHandshake()
		{
			if (base.Settings != null && base.Settings.EnabledCiphers != null)
			{
				UnityTls.unitytls_ciphersuite[] array = new UnityTls.unitytls_ciphersuite[base.Settings.EnabledCiphers.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (UnityTls.unitytls_ciphersuite)base.Settings.EnabledCiphers[i];
				}
				UnityTls.unitytls_errorstate unitytls_errorstate = UnityTls.NativeInterface.unitytls_errorstate_create();
				UnityTls.unitytls_ciphersuite[] array2;
				UnityTls.unitytls_ciphersuite* ptr;
				if ((array2 = array) == null || array2.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array2[0];
				}
				UnityTls.NativeInterface.unitytls_tlsctx_set_supported_ciphersuites(this.tlsContext, ptr, array.Length, &unitytls_errorstate);
				array2 = null;
				Mono.Unity.Debug.CheckAndThrow(unitytls_errorstate, "Failed to set list of supported ciphers", AlertDescription.HandshakeFailure);
			}
		}

		public unsafe override bool ProcessHandshake()
		{
			this.lastException = null;
			UnityTls.unitytls_errorstate unitytls_errorstate = UnityTls.NativeInterface.unitytls_errorstate_create();
			UnityTls.unitytls_x509verify_result unitytls_x509verify_result = UnityTls.NativeInterface.unitytls_tlsctx_process_handshake(this.tlsContext, &unitytls_errorstate);
			if (unitytls_errorstate.code == UnityTls.unitytls_error_code.UNITYTLS_USER_WOULD_BLOCK)
			{
				return false;
			}
			if (this.lastException != null)
			{
				throw this.lastException;
			}
			if (unitytls_x509verify_result == (UnityTls.unitytls_x509verify_result)2147483648U && base.IsServer && !base.AskForClientCertificate)
			{
				Mono.Unity.Debug.CheckAndThrow(unitytls_errorstate, "Handshake failed", AlertDescription.HandshakeFailure);
			}
			else
			{
				Mono.Unity.Debug.CheckAndThrow(unitytls_errorstate, unitytls_x509verify_result, "Handshake failed", AlertDescription.HandshakeFailure);
			}
			if (base.IsServer && !base.AskForClientCertificate && !base.ValidateCertificate(null, null))
			{
				throw new TlsException(AlertDescription.HandshakeFailure, "Verification failure during handshake");
			}
			return true;
		}

		public unsafe override void FinishHandshake()
		{
			UnityTls.unitytls_errorstate unitytls_errorstate = UnityTls.NativeInterface.unitytls_errorstate_create();
			UnityTls.unitytls_ciphersuite unitytls_ciphersuite = UnityTls.NativeInterface.unitytls_tlsctx_get_ciphersuite(this.tlsContext, &unitytls_errorstate);
			UnityTls.unitytls_protocol unitytls_protocol = UnityTls.NativeInterface.unitytls_tlsctx_get_protocol(this.tlsContext, &unitytls_errorstate);
			this.connectioninfo = new MonoTlsConnectionInfo
			{
				CipherSuiteCode = (CipherSuiteCode)unitytls_ciphersuite,
				ProtocolVersion = UnityTlsConversions.ConvertProtocolVersion(unitytls_protocol),
				PeerDomainName = base.ServerName
			};
			this.isAuthenticated = true;
		}

		[MonoPInvokeCallback(typeof(UnityTls.unitytls_tlsctx_write_callback))]
		private unsafe static size_t WriteCallback(void* userData, byte* data, size_t bufferLen, UnityTls.unitytls_errorstate* errorState)
		{
			return ((UnityTlsContext)((GCHandle)((IntPtr)userData)).Target).WriteCallback(data, bufferLen, errorState);
		}

		private unsafe size_t WriteCallback(byte* data, size_t bufferLen, UnityTls.unitytls_errorstate* errorState)
		{
			size_t size_t;
			try
			{
				if (this.writeBuffer == null || this.writeBuffer.Length < bufferLen)
				{
					this.writeBuffer = new byte[bufferLen];
				}
				Marshal.Copy((IntPtr)((void*)data), this.writeBuffer, 0, bufferLen);
				if (!base.Parent.InternalWrite(this.writeBuffer, 0, bufferLen))
				{
					UnityTls.NativeInterface.unitytls_errorstate_raise_error(errorState, UnityTls.unitytls_error_code.UNITYTLS_USER_WRITE_FAILED);
					size_t = 0;
				}
				else
				{
					size_t = bufferLen;
				}
			}
			catch (Exception ex)
			{
				UnityTls.NativeInterface.unitytls_errorstate_raise_error(errorState, UnityTls.unitytls_error_code.UNITYTLS_USER_UNKNOWN_ERROR);
				if (this.lastException == null)
				{
					this.lastException = ex;
				}
				size_t = 0;
			}
			return size_t;
		}

		[MonoPInvokeCallback(typeof(UnityTls.unitytls_tlsctx_read_callback))]
		private unsafe static size_t ReadCallback(void* userData, byte* buffer, size_t bufferLen, UnityTls.unitytls_errorstate* errorState)
		{
			return ((UnityTlsContext)((GCHandle)((IntPtr)userData)).Target).ReadCallback(buffer, bufferLen, errorState);
		}

		private unsafe size_t ReadCallback(byte* buffer, size_t bufferLen, UnityTls.unitytls_errorstate* errorState)
		{
			size_t size_t;
			try
			{
				if (this.readBuffer == null || this.readBuffer.Length < bufferLen)
				{
					this.readBuffer = new byte[bufferLen];
				}
				bool flag;
				int num = base.Parent.InternalRead(this.readBuffer, 0, bufferLen, out flag);
				if (flag)
				{
					UnityTls.NativeInterface.unitytls_errorstate_raise_error(errorState, UnityTls.unitytls_error_code.UNITYTLS_USER_WOULD_BLOCK);
					size_t = 0;
				}
				else if (num < 0)
				{
					UnityTls.NativeInterface.unitytls_errorstate_raise_error(errorState, UnityTls.unitytls_error_code.UNITYTLS_USER_READ_FAILED);
					size_t = 0;
				}
				else
				{
					Marshal.Copy(this.readBuffer, 0, (IntPtr)((void*)buffer), bufferLen);
					size_t = num;
				}
			}
			catch (Exception ex)
			{
				UnityTls.NativeInterface.unitytls_errorstate_raise_error(errorState, UnityTls.unitytls_error_code.UNITYTLS_USER_UNKNOWN_ERROR);
				if (this.lastException == null)
				{
					this.lastException = ex;
				}
				size_t = 0;
			}
			return size_t;
		}

		[MonoPInvokeCallback(typeof(UnityTls.unitytls_tlsctx_x509verify_callback))]
		private unsafe static UnityTls.unitytls_x509verify_result VerifyCallback(void* userData, UnityTls.unitytls_x509list_ref chain, UnityTls.unitytls_errorstate* errorState)
		{
			return ((UnityTlsContext)((GCHandle)((IntPtr)userData)).Target).VerifyCallback(chain, errorState);
		}

		private unsafe UnityTls.unitytls_x509verify_result VerifyCallback(UnityTls.unitytls_x509list_ref chain, UnityTls.unitytls_errorstate* errorState)
		{
			UnityTls.unitytls_x509verify_result unitytls_x509verify_result;
			try
			{
				X509CertificateCollection x509CertificateCollection = CertHelper.NativeChainToManagedCollection(chain, errorState);
				this.remoteCertificate = new X509Certificate(x509CertificateCollection[0]);
				if (base.ValidateCertificate(x509CertificateCollection))
				{
					unitytls_x509verify_result = UnityTls.unitytls_x509verify_result.UNITYTLS_X509VERIFY_SUCCESS;
				}
				else
				{
					unitytls_x509verify_result = UnityTls.unitytls_x509verify_result.UNITYTLS_X509VERIFY_FLAG_NOT_TRUSTED;
				}
			}
			catch (Exception ex)
			{
				if (this.lastException == null)
				{
					this.lastException = ex;
				}
				unitytls_x509verify_result = (UnityTls.unitytls_x509verify_result)4294967295U;
			}
			return unitytls_x509verify_result;
		}

		[MonoPInvokeCallback(typeof(UnityTls.unitytls_tlsctx_certificate_callback))]
		private unsafe static void CertificateCallback(void* userData, UnityTls.unitytls_tlsctx* ctx, byte* cn, size_t cnLen, UnityTls.unitytls_x509name* caList, size_t caListLen, UnityTls.unitytls_x509list_ref* chain, UnityTls.unitytls_key_ref* key, UnityTls.unitytls_errorstate* errorState)
		{
			((UnityTlsContext)((GCHandle)((IntPtr)userData)).Target).CertificateCallback(ctx, cn, cnLen, caList, caListLen, chain, key, errorState);
		}

		private unsafe void CertificateCallback(UnityTls.unitytls_tlsctx* ctx, byte* cn, size_t cnLen, UnityTls.unitytls_x509name* caList, size_t caListLen, UnityTls.unitytls_x509list_ref* chain, UnityTls.unitytls_key_ref* key, UnityTls.unitytls_errorstate* errorState)
		{
			try
			{
				if (this.remoteCertificate == null)
				{
					throw new TlsException(AlertDescription.InternalError, "Cannot request client certificate before receiving one from the server.");
				}
				this.localClientCertificate = base.SelectClientCertificate(this.remoteCertificate, null);
				if (this.localClientCertificate == null)
				{
					*chain = new UnityTls.unitytls_x509list_ref
					{
						handle = UnityTls.NativeInterface.UNITYTLS_INVALID_HANDLE
					};
					*key = new UnityTls.unitytls_key_ref
					{
						handle = UnityTls.NativeInterface.UNITYTLS_INVALID_HANDLE
					};
				}
				else
				{
					UnityTls.NativeInterface.unitytls_x509list_free(this.requestedClientCertChain);
					UnityTls.NativeInterface.unitytls_key_free(this.requestedClientKey);
					UnityTlsContext.ExtractNativeKeyAndChainFromManagedCertificate(this.localClientCertificate, errorState, out this.requestedClientCertChain, out this.requestedClientKey);
					*chain = UnityTls.NativeInterface.unitytls_x509list_get_ref(this.requestedClientCertChain, errorState);
					*key = UnityTls.NativeInterface.unitytls_key_get_ref(this.requestedClientKey, errorState);
				}
				Mono.Unity.Debug.CheckAndThrow(*errorState, "Failed to retrieve certificates on request.", AlertDescription.HandshakeFailure);
			}
			catch (Exception ex)
			{
				UnityTls.NativeInterface.unitytls_errorstate_raise_error(errorState, UnityTls.unitytls_error_code.UNITYTLS_USER_UNKNOWN_ERROR);
				if (this.lastException == null)
				{
					this.lastException = ex;
				}
			}
		}

		[MonoPInvokeCallback(typeof(UnityTls.unitytls_tlsctx_trace_callback))]
		private unsafe static void TraceCallback(void* userData, UnityTls.unitytls_tlsctx* ctx, byte* traceMessage, size_t traceMessageLen)
		{
			Console.Write(Encoding.UTF8.GetString(traceMessage, traceMessageLen));
		}

		private const bool ActivateTracing = false;

		private unsafe UnityTls.unitytls_tlsctx* tlsContext = null;

		private unsafe UnityTls.unitytls_x509list* requestedClientCertChain = null;

		private unsafe UnityTls.unitytls_key* requestedClientKey = null;

		private X509Certificate localClientCertificate;

		private X509Certificate remoteCertificate;

		private MonoTlsConnectionInfo connectioninfo;

		private bool isAuthenticated;

		private bool hasContext;

		private byte[] writeBuffer;

		private byte[] readBuffer;

		private GCHandle handle;

		private Exception lastException;
	}
}
