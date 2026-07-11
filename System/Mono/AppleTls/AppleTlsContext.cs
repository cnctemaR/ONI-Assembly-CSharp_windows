using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Mono.Net;
using Mono.Net.Security;
using Mono.Security.Interface;
using Mono.Util;

namespace Mono.AppleTls
{
	internal class AppleTlsContext : MobileTlsContext
	{
		public AppleTlsContext(MobileAuthenticatedStream parent, bool serverMode, string targetHost, SslProtocols enabledProtocols, X509Certificate serverCertificate, X509CertificateCollection clientCertificates, bool askForClientCert)
			: base(parent, serverMode, targetHost, enabledProtocols, serverCertificate, clientCertificates, askForClientCert)
		{
			this.handle = GCHandle.Alloc(this, GCHandleType.Weak);
			this.readFunc = new SslReadFunc(AppleTlsContext.NativeReadCallback);
			this.writeFunc = new SslWriteFunc(AppleTlsContext.NativeWriteCallback);
			if (base.IsServer && serverCertificate == null)
			{
				throw new ArgumentNullException("serverCertificate");
			}
		}

		public IntPtr Handle
		{
			get
			{
				if (!this.HasContext)
				{
					throw new ObjectDisposedException("AppleTlsContext");
				}
				return this.context;
			}
		}

		public override bool HasContext
		{
			get
			{
				return !this.disposed && this.context != IntPtr.Zero;
			}
		}

		private void CheckStatusAndThrow(SslStatus status, params SslStatus[] acceptable)
		{
			Exception ex = Interlocked.Exchange<Exception>(ref this.lastException, null);
			if (ex != null)
			{
				throw ex;
			}
			if (status == SslStatus.Success || Array.IndexOf<SslStatus>(acceptable, status) > -1)
			{
				return;
			}
			switch (status)
			{
			case SslStatus.CertNotYetValid:
			case SslStatus.CertExpired:
				throw new TlsException(AlertDescription.CertificateExpired);
			case SslStatus.NoRootCert:
			case SslStatus.UnknownRootCert:
			case SslStatus.XCertChainInvalid:
				throw new TlsException(AlertDescription.CertificateUnknown, status.ToString());
			case SslStatus.ModuleAttach:
			case SslStatus.Internal:
			case SslStatus.Crypto:
				break;
			case SslStatus.BadCert:
				throw new TlsException(AlertDescription.BadCertificate);
			case SslStatus.ClosedAbort:
				throw new IOException("Connection closed.");
			default:
				if (status == SslStatus.Protocol)
				{
					throw new TlsException(AlertDescription.ProtocolVersion);
				}
				break;
			}
			throw new TlsException(AlertDescription.InternalError, "Unknown Secure Transport error `{0}'.", new object[] { status });
		}

		public override bool IsAuthenticated
		{
			get
			{
				return this.isAuthenticated;
			}
		}

		public override void StartHandshake()
		{
			if (Interlocked.CompareExchange(ref this.handshakeStarted, 1, 1) != 0)
			{
				throw new InvalidOperationException();
			}
			this.InitializeConnection();
			this.SetSessionOption(SslSessionOption.BreakOnCertRequested, true);
			this.SetSessionOption(SslSessionOption.BreakOnClientAuth, true);
			this.SetSessionOption(SslSessionOption.BreakOnServerAuth, true);
			if (base.IsServer)
			{
				SecCertificate[] array;
				this.serverIdentity = AppleCertificateHelper.GetIdentity(base.LocalServerCertificate, out array);
				if (this.serverIdentity == null)
				{
					throw new AuthenticationException("Unable to get server certificate from keychain.");
				}
				this.SetCertificate(this.serverIdentity, array);
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Dispose();
				}
			}
		}

		public override void FinishHandshake()
		{
			this.InitializeSession();
			this.isAuthenticated = true;
		}

		public override void Flush()
		{
		}

		public override bool ProcessHandshake()
		{
			if (this.handshakeFinished)
			{
				throw new NotSupportedException("Handshake already finished.");
			}
			for (;;)
			{
				this.lastException = null;
				SslStatus sslStatus = AppleTlsContext.SSLHandshake(this.Handle);
				this.CheckStatusAndThrow(sslStatus, new SslStatus[]
				{
					SslStatus.WouldBlock,
					SslStatus.PeerAuthCompleted,
					SslStatus.PeerClientCertRequested
				});
				if (sslStatus == SslStatus.PeerAuthCompleted)
				{
					this.RequirePeerTrust();
				}
				else if (sslStatus == SslStatus.PeerClientCertRequested)
				{
					this.RequirePeerTrust();
					if (this.remoteCertificate == null)
					{
						break;
					}
					this.localClientCertificate = base.SelectClientCertificate(this.remoteCertificate, null);
					if (this.localClientCertificate != null)
					{
						this.clientIdentity = AppleCertificateHelper.GetIdentity(this.localClientCertificate);
						if (this.clientIdentity == null)
						{
							goto Block_6;
						}
						this.SetCertificate(this.clientIdentity, new SecCertificate[0]);
					}
				}
				else
				{
					if (sslStatus == SslStatus.WouldBlock)
					{
						return false;
					}
					if (sslStatus == SslStatus.Success)
					{
						goto Block_8;
					}
				}
			}
			throw new TlsException(AlertDescription.InternalError, "Cannot request client certificate before receiving one from the server.");
			Block_6:
			throw new TlsException(AlertDescription.CertificateUnknown);
			Block_8:
			this.handshakeFinished = true;
			return true;
		}

		private void RequirePeerTrust()
		{
			if (!this.havePeerTrust)
			{
				this.EvaluateTrust();
				this.havePeerTrust = true;
			}
		}

		private void EvaluateTrust()
		{
			this.InitializeSession();
			SecTrust secTrust = null;
			X509CertificateCollection x509CertificateCollection = null;
			bool flag;
			try
			{
				secTrust = this.GetPeerTrust(!base.IsServer);
				if (secTrust == null || secTrust.Count == 0)
				{
					this.remoteCertificate = null;
					if (!base.IsServer)
					{
						throw new TlsException(AlertDescription.CertificateUnknown);
					}
					x509CertificateCollection = null;
				}
				else
				{
					int count = secTrust.Count;
					x509CertificateCollection = new X509CertificateCollection();
					for (int i = 0; i < secTrust.Count; i++)
					{
						x509CertificateCollection.Add(secTrust.GetCertificate(i));
					}
					this.remoteCertificate = new X509Certificate(x509CertificateCollection[0]);
				}
				flag = base.ValidateCertificate(x509CertificateCollection);
			}
			catch (Exception)
			{
				throw new TlsException(AlertDescription.CertificateUnknown, "Certificate validation threw exception.");
			}
			finally
			{
				if (secTrust != null)
				{
					secTrust.Dispose();
				}
				if (x509CertificateCollection != null)
				{
					for (int j = 0; j < x509CertificateCollection.Count; j++)
					{
						x509CertificateCollection[j].Dispose();
					}
				}
			}
			if (!flag)
			{
				throw new TlsException(AlertDescription.CertificateUnknown);
			}
		}

		private void InitializeConnection()
		{
			this.context = AppleTlsContext.SSLCreateContext(IntPtr.Zero, base.IsServer ? SslProtocolSide.Server : SslProtocolSide.Client, SslConnectionType.Stream);
			SslStatus sslStatus = AppleTlsContext.SSLSetIOFuncs(this.Handle, this.readFunc, this.writeFunc);
			this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
			sslStatus = AppleTlsContext.SSLSetConnection(this.Handle, GCHandle.ToIntPtr(this.handle));
			this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
			if ((base.EnabledProtocols & SslProtocols.Tls) != SslProtocols.None)
			{
				this.MinProtocol = SslProtocol.Tls_1_0;
			}
			else if ((base.EnabledProtocols & SslProtocols.Tls11) != SslProtocols.None)
			{
				this.MinProtocol = SslProtocol.Tls_1_1;
			}
			else
			{
				this.MinProtocol = SslProtocol.Tls_1_2;
			}
			if ((base.EnabledProtocols & SslProtocols.Tls12) != SslProtocols.None)
			{
				this.MaxProtocol = SslProtocol.Tls_1_2;
			}
			else if ((base.EnabledProtocols & SslProtocols.Tls11) != SslProtocols.None)
			{
				this.MaxProtocol = SslProtocol.Tls_1_1;
			}
			else
			{
				this.MaxProtocol = SslProtocol.Tls_1_0;
			}
			if (base.Settings != null && base.Settings.EnabledCiphers != null)
			{
				SslCipherSuite[] array = new SslCipherSuite[base.Settings.EnabledCiphers.Length];
				for (int i = 0; i < base.Settings.EnabledCiphers.Length; i++)
				{
					array[i] = (SslCipherSuite)base.Settings.EnabledCiphers[i];
				}
				this.SetEnabledCiphers(array);
			}
			if (base.AskForClientCertificate)
			{
				this.SetClientSideAuthenticate(SslAuthenticate.Try);
			}
			IPAddress ipaddress;
			if (!base.IsServer && !string.IsNullOrEmpty(base.TargetHost) && !IPAddress.TryParse(base.TargetHost, out ipaddress))
			{
				this.PeerDomainName = base.ServerName;
			}
		}

		private void InitializeSession()
		{
			if (this.connectionInfo != null)
			{
				return;
			}
			SslCipherSuite negotiatedCipher = this.NegotiatedCipher;
			SslProtocol negotiatedProtocolVersion = this.GetNegotiatedProtocolVersion();
			this.connectionInfo = new MonoTlsConnectionInfo
			{
				CipherSuiteCode = (CipherSuiteCode)negotiatedCipher,
				ProtocolVersion = AppleTlsContext.GetProtocol(negotiatedProtocolVersion),
				PeerDomainName = this.PeerDomainName
			};
		}

		private static TlsProtocols GetProtocol(SslProtocol protocol)
		{
			switch (protocol)
			{
			case SslProtocol.Tls_1_0:
				return TlsProtocols.Tls10;
			case SslProtocol.Tls_1_1:
				return TlsProtocols.Tls11;
			case SslProtocol.Tls_1_2:
				return TlsProtocols.Tls12;
			}
			throw new NotSupportedException();
		}

		public override MonoTlsConnectionInfo ConnectionInfo
		{
			get
			{
				return this.connectionInfo;
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
				return this.connectionInfo.ProtocolVersion;
			}
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLGetProtocolVersionMax(IntPtr context, out SslProtocol maxVersion);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLSetProtocolVersionMax(IntPtr context, SslProtocol maxVersion);

		public SslProtocol MaxProtocol
		{
			get
			{
				SslProtocol sslProtocol;
				SslStatus sslStatus = AppleTlsContext.SSLGetProtocolVersionMax(this.Handle, out sslProtocol);
				this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
				return sslProtocol;
			}
			set
			{
				SslStatus sslStatus = AppleTlsContext.SSLSetProtocolVersionMax(this.Handle, value);
				this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
			}
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLGetProtocolVersionMin(IntPtr context, out SslProtocol minVersion);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLSetProtocolVersionMin(IntPtr context, SslProtocol minVersion);

		public SslProtocol MinProtocol
		{
			get
			{
				SslProtocol sslProtocol;
				SslStatus sslStatus = AppleTlsContext.SSLGetProtocolVersionMin(this.Handle, out sslProtocol);
				this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
				return sslProtocol;
			}
			set
			{
				SslStatus sslStatus = AppleTlsContext.SSLSetProtocolVersionMin(this.Handle, value);
				this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
			}
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLGetNegotiatedProtocolVersion(IntPtr context, out SslProtocol protocol);

		public SslProtocol GetNegotiatedProtocolVersion()
		{
			SslProtocol sslProtocol;
			SslStatus sslStatus = AppleTlsContext.SSLGetNegotiatedProtocolVersion(this.Handle, out sslProtocol);
			this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
			return sslProtocol;
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLGetSessionOption(IntPtr context, SslSessionOption option, out bool value);

		public bool GetSessionOption(SslSessionOption option)
		{
			bool flag;
			SslStatus sslStatus = AppleTlsContext.SSLGetSessionOption(this.Handle, option, out flag);
			this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
			return flag;
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLSetSessionOption(IntPtr context, SslSessionOption option, bool value);

		public void SetSessionOption(SslSessionOption option, bool value)
		{
			SslStatus sslStatus = AppleTlsContext.SSLSetSessionOption(this.Handle, option, value);
			this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLSetClientSideAuthenticate(IntPtr context, SslAuthenticate auth);

		public void SetClientSideAuthenticate(SslAuthenticate auth)
		{
			SslStatus sslStatus = AppleTlsContext.SSLSetClientSideAuthenticate(this.Handle, auth);
			this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLHandshake(IntPtr context);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLGetSessionState(IntPtr context, ref SslSessionState state);

		public SslSessionState SessionState
		{
			get
			{
				SslSessionState sslSessionState = SslSessionState.Invalid;
				SslStatus sslStatus = AppleTlsContext.SSLGetSessionState(this.Handle, ref sslSessionState);
				this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
				return sslSessionState;
			}
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLGetPeerID(IntPtr context, out IntPtr peerID, out IntPtr peerIDLen);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private unsafe static extern SslStatus SSLSetPeerID(IntPtr context, byte* peerID, IntPtr peerIDLen);

		public unsafe byte[] PeerId
		{
			get
			{
				IntPtr intPtr;
				IntPtr intPtr2;
				SslStatus sslStatus = AppleTlsContext.SSLGetPeerID(this.Handle, out intPtr, out intPtr2);
				this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
				if (sslStatus != SslStatus.Success || (int)intPtr2 == 0)
				{
					return null;
				}
				byte[] array = new byte[(int)intPtr2];
				Marshal.Copy(intPtr, array, 0, (int)intPtr2);
				return array;
			}
			set
			{
				IntPtr intPtr = ((value == null) ? IntPtr.Zero : ((IntPtr)value.Length));
				SslStatus sslStatus;
				fixed (byte[] array = value)
				{
					byte* ptr;
					if (value == null || array.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array[0];
					}
					sslStatus = AppleTlsContext.SSLSetPeerID(this.Handle, ptr, intPtr);
				}
				this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
			}
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLGetBufferedReadSize(IntPtr context, out IntPtr bufSize);

		public IntPtr BufferedReadSize
		{
			get
			{
				IntPtr intPtr;
				SslStatus sslStatus = AppleTlsContext.SSLGetBufferedReadSize(this.Handle, out intPtr);
				this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
				return intPtr;
			}
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLGetNumberSupportedCiphers(IntPtr context, out IntPtr numCiphers);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private unsafe static extern SslStatus SSLGetSupportedCiphers(IntPtr context, SslCipherSuite* ciphers, ref IntPtr numCiphers);

		public unsafe IList<SslCipherSuite> GetSupportedCiphers()
		{
			IntPtr intPtr;
			SslStatus sslStatus = AppleTlsContext.SSLGetNumberSupportedCiphers(this.Handle, out intPtr);
			this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
			if (sslStatus != SslStatus.Success || (int)intPtr <= 0)
			{
				return null;
			}
			SslCipherSuite[] array2;
			SslCipherSuite[] array = (array2 = new SslCipherSuite[(int)intPtr]);
			SslCipherSuite* ptr;
			if (array == null || array2.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array2[0];
			}
			sslStatus = AppleTlsContext.SSLGetSupportedCiphers(this.Handle, ptr, ref intPtr);
			array2 = null;
			this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
			return array;
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLGetNumberEnabledCiphers(IntPtr context, out IntPtr numCiphers);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private unsafe static extern SslStatus SSLGetEnabledCiphers(IntPtr context, SslCipherSuite* ciphers, ref IntPtr numCiphers);

		public unsafe IList<SslCipherSuite> GetEnabledCiphers()
		{
			IntPtr intPtr;
			SslStatus sslStatus = AppleTlsContext.SSLGetNumberEnabledCiphers(this.Handle, out intPtr);
			this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
			if (sslStatus != SslStatus.Success || (int)intPtr <= 0)
			{
				return null;
			}
			SslCipherSuite[] array2;
			SslCipherSuite[] array = (array2 = new SslCipherSuite[(int)intPtr]);
			SslCipherSuite* ptr;
			if (array == null || array2.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array2[0];
			}
			sslStatus = AppleTlsContext.SSLGetEnabledCiphers(this.Handle, ptr, ref intPtr);
			array2 = null;
			this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
			return array;
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private unsafe static extern SslStatus SSLSetEnabledCiphers(IntPtr context, SslCipherSuite* ciphers, IntPtr numCiphers);

		public unsafe void SetEnabledCiphers(SslCipherSuite[] ciphers)
		{
			if (ciphers == null)
			{
				throw new ArgumentNullException("ciphers");
			}
			SslStatus sslStatus;
			fixed (SslCipherSuite[] array = ciphers)
			{
				SslCipherSuite* ptr;
				if (ciphers == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				sslStatus = AppleTlsContext.SSLSetEnabledCiphers(this.Handle, ptr, (IntPtr)ciphers.Length);
			}
			this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLGetNegotiatedCipher(IntPtr context, out SslCipherSuite cipherSuite);

		public SslCipherSuite NegotiatedCipher
		{
			get
			{
				SslCipherSuite sslCipherSuite;
				SslStatus sslStatus = AppleTlsContext.SSLGetNegotiatedCipher(this.Handle, out sslCipherSuite);
				this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
				return sslCipherSuite;
			}
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLGetPeerDomainNameLength(IntPtr context, out IntPtr peerNameLen);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLGetPeerDomainName(IntPtr context, byte[] peerName, ref IntPtr peerNameLen);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLSetPeerDomainName(IntPtr context, byte[] peerName, IntPtr peerNameLen);

		public string PeerDomainName
		{
			get
			{
				IntPtr intPtr;
				SslStatus sslStatus = AppleTlsContext.SSLGetPeerDomainNameLength(this.Handle, out intPtr);
				this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
				if (sslStatus != SslStatus.Success || (int)intPtr == 0)
				{
					return string.Empty;
				}
				byte[] array = new byte[(int)intPtr];
				sslStatus = AppleTlsContext.SSLGetPeerDomainName(this.Handle, array, ref intPtr);
				this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
				int num = (int)intPtr;
				if (sslStatus != SslStatus.Success)
				{
					return string.Empty;
				}
				if (num > 0 && array[num - 1] == 0)
				{
					num--;
				}
				return Encoding.UTF8.GetString(array, 0, num);
			}
			set
			{
				SslStatus sslStatus;
				if (value == null)
				{
					sslStatus = AppleTlsContext.SSLSetPeerDomainName(this.Handle, null, (IntPtr)0);
				}
				else
				{
					byte[] bytes = Encoding.UTF8.GetBytes(value);
					sslStatus = AppleTlsContext.SSLSetPeerDomainName(this.Handle, bytes, (IntPtr)bytes.Length);
				}
				this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
			}
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLSetCertificate(IntPtr context, IntPtr certRefs);

		private CFArray Bundle(SecIdentity identity, IEnumerable<SecCertificate> certificates)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			int num = 0;
			int num2 = 0;
			if (certificates != null)
			{
				foreach (SecCertificate secCertificate in certificates)
				{
					num2++;
				}
			}
			IntPtr[] array = new IntPtr[num2 + 1];
			array[0] = identity.Handle;
			foreach (SecCertificate secCertificate2 in certificates)
			{
				array[++num] = secCertificate2.Handle;
			}
			return CFArray.CreateArray(array);
		}

		public void SetCertificate(SecIdentity identify, IEnumerable<SecCertificate> certificates)
		{
			using (CFArray cfarray = this.Bundle(identify, certificates))
			{
				SslStatus sslStatus = AppleTlsContext.SSLSetCertificate(this.Handle, cfarray.Handle);
				this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
			}
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLGetClientCertificateState(IntPtr context, out SslClientCertificateState clientState);

		public SslClientCertificateState ClientCertificateState
		{
			get
			{
				SslClientCertificateState sslClientCertificateState;
				SslStatus sslStatus = AppleTlsContext.SSLGetClientCertificateState(this.Handle, out sslClientCertificateState);
				this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
				return sslClientCertificateState;
			}
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLCopyPeerTrust(IntPtr context, out IntPtr trust);

		public SecTrust GetPeerTrust(bool requireTrust)
		{
			IntPtr intPtr;
			SslStatus sslStatus = AppleTlsContext.SSLCopyPeerTrust(this.Handle, out intPtr);
			if (requireTrust)
			{
				this.CheckStatusAndThrow(sslStatus, Array.Empty<SslStatus>());
				if (intPtr == IntPtr.Zero)
				{
					throw new TlsException(AlertDescription.CertificateUnknown);
				}
			}
			if (!(intPtr == IntPtr.Zero))
			{
				return new SecTrust(intPtr, true);
			}
			return null;
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern IntPtr SSLCreateContext(IntPtr alloc, SslProtocolSide protocolSide, SslConnectionType connectionType);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLSetConnection(IntPtr context, IntPtr connection);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLSetIOFuncs(IntPtr context, SslReadFunc readFunc, SslWriteFunc writeFunc);

		[MonoPInvokeCallback(typeof(SslReadFunc))]
		private static SslStatus NativeReadCallback(IntPtr ptr, IntPtr data, ref IntPtr dataLength)
		{
			AppleTlsContext appleTlsContext = null;
			SslStatus sslStatus;
			try
			{
				GCHandle gchandle = GCHandle.FromIntPtr(ptr);
				if (!gchandle.IsAllocated)
				{
					sslStatus = SslStatus.Internal;
				}
				else
				{
					appleTlsContext = (AppleTlsContext)gchandle.Target;
					if (appleTlsContext == null || appleTlsContext.disposed)
					{
						sslStatus = SslStatus.ClosedAbort;
					}
					else
					{
						sslStatus = appleTlsContext.NativeReadCallback(data, ref dataLength);
					}
				}
			}
			catch (Exception ex)
			{
				if (appleTlsContext != null && appleTlsContext.lastException == null)
				{
					appleTlsContext.lastException = ex;
				}
				sslStatus = SslStatus.Internal;
			}
			return sslStatus;
		}

		[MonoPInvokeCallback(typeof(SslWriteFunc))]
		private static SslStatus NativeWriteCallback(IntPtr ptr, IntPtr data, ref IntPtr dataLength)
		{
			AppleTlsContext appleTlsContext = null;
			SslStatus sslStatus;
			try
			{
				GCHandle gchandle = GCHandle.FromIntPtr(ptr);
				if (!gchandle.IsAllocated)
				{
					sslStatus = SslStatus.Internal;
				}
				else
				{
					appleTlsContext = (AppleTlsContext)gchandle.Target;
					if (appleTlsContext == null || appleTlsContext.disposed)
					{
						sslStatus = SslStatus.ClosedAbort;
					}
					else
					{
						sslStatus = appleTlsContext.NativeWriteCallback(data, ref dataLength);
					}
				}
			}
			catch (Exception ex)
			{
				if (appleTlsContext != null && appleTlsContext.lastException == null)
				{
					appleTlsContext.lastException = ex;
				}
				sslStatus = SslStatus.Internal;
			}
			return sslStatus;
		}

		private SslStatus NativeReadCallback(IntPtr data, ref IntPtr dataLength)
		{
			if (this.closed || this.disposed || base.Parent == null)
			{
				return SslStatus.ClosedAbort;
			}
			int num = (int)dataLength;
			byte[] array = new byte[num];
			bool flag;
			int num2 = base.Parent.InternalRead(array, 0, num, out flag);
			dataLength = (IntPtr)num2;
			if (num2 < 0)
			{
				return SslStatus.ClosedAbort;
			}
			Marshal.Copy(array, 0, data, num2);
			if (num2 > 0)
			{
				return SslStatus.Success;
			}
			if (flag)
			{
				return SslStatus.WouldBlock;
			}
			if (num2 == 0)
			{
				this.closedGraceful = true;
				return SslStatus.ClosedGraceful;
			}
			return SslStatus.Success;
		}

		private SslStatus NativeWriteCallback(IntPtr data, ref IntPtr dataLength)
		{
			if (this.closed || this.disposed || base.Parent == null)
			{
				return SslStatus.ClosedAbort;
			}
			int num = (int)dataLength;
			byte[] array = new byte[num];
			Marshal.Copy(data, array, 0, num);
			if (!base.Parent.InternalWrite(array, 0, num))
			{
				return SslStatus.ClosedAbort;
			}
			return SslStatus.Success;
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private unsafe static extern SslStatus SSLRead(IntPtr context, byte* data, IntPtr dataLength, out IntPtr processed);

		[return: TupleElementNames(new string[] { "ret", "wantMore" })]
		public unsafe override ValueTuple<int, bool> Read(byte[] buffer, int offset, int count)
		{
			if (Interlocked.Exchange(ref this.pendingIO, 1) == 1)
			{
				throw new InvalidOperationException();
			}
			this.lastException = null;
			ValueTuple<int, bool> valueTuple;
			try
			{
				IntPtr intPtr;
				SslStatus sslStatus;
				try
				{
					fixed (byte* ptr = &buffer[offset])
					{
						byte* ptr2 = ptr;
						sslStatus = AppleTlsContext.SSLRead(this.Handle, ptr2, (IntPtr)count, out intPtr);
					}
				}
				finally
				{
					byte* ptr = null;
				}
				if (this.closedGraceful && (sslStatus == SslStatus.ClosedAbort || sslStatus == SslStatus.ClosedGraceful))
				{
					valueTuple = new ValueTuple<int, bool>(0, false);
				}
				else
				{
					this.CheckStatusAndThrow(sslStatus, new SslStatus[]
					{
						SslStatus.WouldBlock,
						SslStatus.ClosedGraceful
					});
					bool flag = sslStatus == SslStatus.WouldBlock;
					valueTuple = new ValueTuple<int, bool>((int)intPtr, flag);
				}
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				this.pendingIO = 0;
			}
			return valueTuple;
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private unsafe static extern SslStatus SSLWrite(IntPtr context, byte* data, IntPtr dataLength, out IntPtr processed);

		[return: TupleElementNames(new string[] { "ret", "wantMore" })]
		public unsafe override ValueTuple<int, bool> Write(byte[] buffer, int offset, int count)
		{
			if (Interlocked.Exchange(ref this.pendingIO, 1) == 1)
			{
				throw new InvalidOperationException();
			}
			this.lastException = null;
			ValueTuple<int, bool> valueTuple;
			try
			{
				SslStatus sslStatus = SslStatus.ClosedAbort;
				IntPtr intPtr = (IntPtr)(-1);
				try
				{
					fixed (byte* ptr = &buffer[offset])
					{
						byte* ptr2 = ptr;
						sslStatus = AppleTlsContext.SSLWrite(this.Handle, ptr2, (IntPtr)count, out intPtr);
					}
				}
				finally
				{
					byte* ptr = null;
				}
				this.CheckStatusAndThrow(sslStatus, new SslStatus[] { SslStatus.WouldBlock });
				bool flag = sslStatus == SslStatus.WouldBlock;
				valueTuple = new ValueTuple<int, bool>((int)intPtr, flag);
			}
			finally
			{
				this.pendingIO = 0;
			}
			return valueTuple;
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SslStatus SSLClose(IntPtr context);

		public override void Shutdown()
		{
			this.closed = true;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!this.disposed)
				{
					if (disposing)
					{
						this.disposed = true;
						if (this.serverIdentity != null)
						{
							this.serverIdentity.Dispose();
							this.serverIdentity = null;
						}
						if (this.clientIdentity != null)
						{
							this.clientIdentity.Dispose();
							this.clientIdentity = null;
						}
						if (this.remoteCertificate != null)
						{
							this.remoteCertificate.Dispose();
							this.remoteCertificate = null;
						}
					}
				}
			}
			finally
			{
				this.disposed = true;
				if (this.context != IntPtr.Zero)
				{
					CFObject.CFRelease(this.context);
					this.context = IntPtr.Zero;
				}
				base.Dispose(disposing);
			}
		}

		public const string SecurityLibrary = "/System/Library/Frameworks/Security.framework/Security";

		private GCHandle handle;

		private IntPtr context;

		private SslReadFunc readFunc;

		private SslWriteFunc writeFunc;

		private SecIdentity serverIdentity;

		private SecIdentity clientIdentity;

		private X509Certificate remoteCertificate;

		private X509Certificate localClientCertificate;

		private MonoTlsConnectionInfo connectionInfo;

		private bool havePeerTrust;

		private bool isAuthenticated;

		private bool handshakeFinished;

		private int handshakeStarted;

		private bool closed;

		private bool disposed;

		private bool closedGraceful;

		private int pendingIO;

		private Exception lastException;
	}
}
