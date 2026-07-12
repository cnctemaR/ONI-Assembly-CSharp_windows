using System;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Authentication.ExtendedProtection;

namespace System.Net
{
	internal class NTAuthentication
	{
		internal bool IsCompleted
		{
			get
			{
				return this._isCompleted;
			}
		}

		internal bool IsValidContext
		{
			get
			{
				return this._securityContext != null && !this._securityContext.IsInvalid;
			}
		}

		internal string Package
		{
			get
			{
				return this._package;
			}
		}

		internal bool IsServer
		{
			get
			{
				return this._isServer;
			}
		}

		internal string ClientSpecifiedSpn
		{
			get
			{
				if (this._clientSpecifiedSpn == null)
				{
					this._clientSpecifiedSpn = this.GetClientSpecifiedSpn();
				}
				return this._clientSpecifiedSpn;
			}
		}

		internal string ProtocolName
		{
			get
			{
				if (this._protocolName == null)
				{
					string text = null;
					if (this.IsValidContext)
					{
						text = NegotiateStreamPal.QueryContextAuthenticationPackage(this._securityContext);
						if (this.IsCompleted)
						{
							this._protocolName = text;
						}
					}
					return text ?? string.Empty;
				}
				return this._protocolName;
			}
		}

		internal bool IsKerberos
		{
			get
			{
				if (this._lastProtocolName == null)
				{
					this._lastProtocolName = this.ProtocolName;
				}
				return this._lastProtocolName == "Kerberos";
			}
		}

		internal NTAuthentication(bool isServer, string package, NetworkCredential credential, string spn, ContextFlagsPal requestedContextFlags, ChannelBinding channelBinding)
		{
			this.Initialize(isServer, package, credential, spn, requestedContextFlags, channelBinding);
		}

		private void Initialize(bool isServer, string package, NetworkCredential credential, string spn, ContextFlagsPal requestedContextFlags, ChannelBinding channelBinding)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Enter(this, package, spn, requestedContextFlags, "Initialize");
			}
			this._tokenSize = NegotiateStreamPal.QueryMaxTokenSize(package);
			this._isServer = isServer;
			this._spn = spn;
			this._securityContext = null;
			this._requestedContextFlags = requestedContextFlags;
			this._package = package;
			this._channelBinding = channelBinding;
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Info(this, FormattableStringFactory.Create("Peer SPN-> '{0}'", new object[] { this._spn }), "Initialize");
			}
			if (credential == CredentialCache.DefaultCredentials)
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Info(this, "using DefaultCredentials", "Initialize");
				}
				this._credentialsHandle = NegotiateStreamPal.AcquireDefaultCredential(package, this._isServer);
				return;
			}
			this._credentialsHandle = NegotiateStreamPal.AcquireCredentialsHandle(package, this._isServer, credential);
		}

		internal SafeDeleteContext GetContext(out SecurityStatusPal status)
		{
			status = new SecurityStatusPal(SecurityStatusPalErrorCode.OK, null);
			if (!this.IsCompleted || !this.IsValidContext)
			{
				NetEventSource.Fail(this, "Should be called only when completed with success, currently is not!", "GetContext");
			}
			if (!this.IsServer)
			{
				NetEventSource.Fail(this, "The method must not be called by the client side!", "GetContext");
			}
			if (!this.IsValidContext)
			{
				status = new SecurityStatusPal(SecurityStatusPalErrorCode.InvalidHandle, null);
				return null;
			}
			return this._securityContext;
		}

		internal void CloseContext()
		{
			if (this._securityContext != null && !this._securityContext.IsClosed)
			{
				this._securityContext.Dispose();
			}
		}

		internal int VerifySignature(byte[] buffer, int offset, int count)
		{
			return NegotiateStreamPal.VerifySignature(this._securityContext, buffer, offset, count);
		}

		internal int MakeSignature(byte[] buffer, int offset, int count, ref byte[] output)
		{
			return NegotiateStreamPal.MakeSignature(this._securityContext, buffer, offset, count, ref output);
		}

		internal string GetOutgoingBlob(string incomingBlob)
		{
			byte[] array = null;
			if (incomingBlob != null && incomingBlob.Length > 0)
			{
				array = Convert.FromBase64String(incomingBlob);
			}
			byte[] array2 = null;
			if ((this.IsValidContext || this.IsCompleted) && array == null)
			{
				this._isCompleted = true;
			}
			else
			{
				SecurityStatusPal securityStatusPal;
				array2 = this.GetOutgoingBlob(array, true, out securityStatusPal);
			}
			string text = null;
			if (array2 != null && array2.Length != 0)
			{
				text = Convert.ToBase64String(array2);
			}
			if (this.IsCompleted)
			{
				this.CloseContext();
			}
			return text;
		}

		internal byte[] GetOutgoingBlob(byte[] incomingBlob, bool thrownOnError)
		{
			SecurityStatusPal securityStatusPal;
			return this.GetOutgoingBlob(incomingBlob, thrownOnError, out securityStatusPal);
		}

		internal byte[] GetOutgoingBlob(byte[] incomingBlob, bool throwOnError, out SecurityStatusPal statusCode)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Enter(this, incomingBlob, "GetOutgoingBlob");
			}
			SecurityBuffer[] array = null;
			if (incomingBlob != null && this._channelBinding != null)
			{
				array = new SecurityBuffer[]
				{
					new SecurityBuffer(incomingBlob, SecurityBufferType.SECBUFFER_TOKEN),
					new SecurityBuffer(this._channelBinding)
				};
			}
			else if (incomingBlob != null)
			{
				array = new SecurityBuffer[]
				{
					new SecurityBuffer(incomingBlob, SecurityBufferType.SECBUFFER_TOKEN)
				};
			}
			else if (this._channelBinding != null)
			{
				array = new SecurityBuffer[]
				{
					new SecurityBuffer(this._channelBinding)
				};
			}
			SecurityBuffer securityBuffer = new SecurityBuffer(this._tokenSize, SecurityBufferType.SECBUFFER_TOKEN);
			bool flag = this._securityContext == null;
			try
			{
				if (!this._isServer)
				{
					statusCode = NegotiateStreamPal.InitializeSecurityContext(this._credentialsHandle, ref this._securityContext, this._spn, this._requestedContextFlags, array, securityBuffer, ref this._contextFlags);
					if (NetEventSource.IsEnabled)
					{
						NetEventSource.Info(this, FormattableStringFactory.Create("SSPIWrapper.InitializeSecurityContext() returns statusCode:0x{0:x8} ({1})", new object[]
						{
							(int)statusCode.ErrorCode,
							statusCode
						}), "GetOutgoingBlob");
					}
					if (statusCode.ErrorCode == SecurityStatusPalErrorCode.CompleteNeeded)
					{
						statusCode = NegotiateStreamPal.CompleteAuthToken(ref this._securityContext, new SecurityBuffer[] { securityBuffer });
						if (NetEventSource.IsEnabled)
						{
							NetEventSource.Info(this, FormattableStringFactory.Create("SSPIWrapper.CompleteAuthToken() returns statusCode:0x{0:x8} ({1})", new object[]
							{
								(int)statusCode.ErrorCode,
								statusCode
							}), "GetOutgoingBlob");
						}
						securityBuffer.token = null;
					}
				}
				else
				{
					statusCode = NegotiateStreamPal.AcceptSecurityContext(this._credentialsHandle, ref this._securityContext, this._requestedContextFlags, array, securityBuffer, ref this._contextFlags);
					if (NetEventSource.IsEnabled)
					{
						NetEventSource.Info(this, FormattableStringFactory.Create("SSPIWrapper.AcceptSecurityContext() returns statusCode:0x{0:x8} ({1})", new object[]
						{
							(int)statusCode.ErrorCode,
							statusCode
						}), "GetOutgoingBlob");
					}
				}
			}
			finally
			{
				if (flag && this._credentialsHandle != null)
				{
					this._credentialsHandle.Dispose();
				}
			}
			if (statusCode.ErrorCode < SecurityStatusPalErrorCode.OutOfMemory)
			{
				if (flag && this._credentialsHandle != null)
				{
					SSPIHandleCache.CacheCredential(this._credentialsHandle);
				}
				if (statusCode.ErrorCode == SecurityStatusPalErrorCode.OK)
				{
					this._isCompleted = true;
				}
				else if (NetEventSource.IsEnabled && NetEventSource.IsEnabled)
				{
					NetEventSource.Info(this, FormattableStringFactory.Create("need continue statusCode:0x{0:x8} ({1}) _securityContext:{2}", new object[]
					{
						(int)statusCode.ErrorCode,
						statusCode,
						this._securityContext
					}), "GetOutgoingBlob");
				}
				if (NetEventSource.IsEnabled && NetEventSource.IsEnabled)
				{
					NetEventSource.Exit(this, FormattableStringFactory.Create("IsCompleted: {0}", new object[] { this.IsCompleted }), "GetOutgoingBlob");
				}
				return securityBuffer.token;
			}
			this.CloseContext();
			this._isCompleted = true;
			if (throwOnError)
			{
				Exception ex = NegotiateStreamPal.CreateExceptionFromError(statusCode);
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Exit(this, ex, "GetOutgoingBlob");
				}
				throw ex;
			}
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Exit(this, FormattableStringFactory.Create("null statusCode:0x{0:x8} ({1})", new object[]
				{
					(int)statusCode.ErrorCode,
					statusCode
				}), "GetOutgoingBlob");
			}
			return null;
		}

		private string GetClientSpecifiedSpn()
		{
			if (!this.IsValidContext || !this.IsCompleted)
			{
				NetEventSource.Fail(this, "Trying to get the client SPN before handshaking is done!", "GetClientSpecifiedSpn");
			}
			string text = NegotiateStreamPal.QueryContextClientSpecifiedSpn(this._securityContext);
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Info(this, FormattableStringFactory.Create("The client specified SPN is [{0}]", new object[] { text }), "GetClientSpecifiedSpn");
			}
			return text;
		}

		private bool _isServer;

		private SafeFreeCredentials _credentialsHandle;

		private SafeDeleteContext _securityContext;

		private string _spn;

		private int _tokenSize;

		private ContextFlagsPal _requestedContextFlags;

		private ContextFlagsPal _contextFlags;

		private bool _isCompleted;

		private string _package;

		private string _lastProtocolName;

		private string _protocolName;

		private string _clientSpecifiedSpn;

		private ChannelBinding _channelBinding;
	}
}
