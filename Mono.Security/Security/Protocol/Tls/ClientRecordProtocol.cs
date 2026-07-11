using System;
using System.Globalization;
using System.IO;
using Mono.Security.Protocol.Tls.Handshake;
using Mono.Security.Protocol.Tls.Handshake.Client;

namespace Mono.Security.Protocol.Tls
{
	internal class ClientRecordProtocol : RecordProtocol
	{
		public ClientRecordProtocol(Stream innerStream, ClientContext context)
			: base(innerStream, context)
		{
		}

		public override HandshakeMessage GetMessage(HandshakeType type)
		{
			return this.createClientHandshakeMessage(type);
		}

		protected override void ProcessHandshakeMessage(TlsStream handMsg)
		{
			HandshakeType handshakeType = (HandshakeType)handMsg.ReadByte();
			int num = handMsg.ReadInt24();
			byte[] array = null;
			if (num > 0)
			{
				array = new byte[num];
				handMsg.Read(array, 0, num);
			}
			HandshakeMessage handshakeMessage = this.createServerHandshakeMessage(handshakeType, array);
			if (handshakeMessage != null)
			{
				handshakeMessage.Process();
			}
			base.Context.LastHandshakeMsg = handshakeType;
			if (handshakeMessage != null)
			{
				handshakeMessage.Update();
				base.Context.HandshakeMessages.WriteByte((byte)handshakeType);
				base.Context.HandshakeMessages.WriteInt24(num);
				if (num > 0)
				{
					base.Context.HandshakeMessages.Write(array, 0, array.Length);
				}
			}
		}

		private HandshakeMessage createClientHandshakeMessage(HandshakeType type)
		{
			if (type <= HandshakeType.Certificate)
			{
				if (type == HandshakeType.ClientHello)
				{
					return new TlsClientHello(this.context);
				}
				if (type == HandshakeType.Certificate)
				{
					return new TlsClientCertificate(this.context);
				}
			}
			else
			{
				if (type == HandshakeType.CertificateVerify)
				{
					return new TlsClientCertificateVerify(this.context);
				}
				if (type == HandshakeType.ClientKeyExchange)
				{
					return new TlsClientKeyExchange(this.context);
				}
				if (type == HandshakeType.Finished)
				{
					return new TlsClientFinished(this.context);
				}
			}
			throw new InvalidOperationException("Unknown client handshake message type: " + type.ToString());
		}

		private HandshakeMessage createServerHandshakeMessage(HandshakeType type, byte[] buffer)
		{
			ClientContext clientContext = (ClientContext)this.context;
			HandshakeType lastHandshakeMsg = clientContext.LastHandshakeMsg;
			if (type <= HandshakeType.ServerHello)
			{
				if (type == HandshakeType.HelloRequest)
				{
					if (clientContext.HandshakeState != HandshakeState.Started)
					{
						clientContext.HandshakeState = HandshakeState.None;
					}
					else
					{
						base.SendAlert(AlertLevel.Warning, AlertDescription.NoRenegotiation);
					}
					return null;
				}
				if (type == HandshakeType.ServerHello)
				{
					if (lastHandshakeMsg == HandshakeType.HelloRequest)
					{
						return new TlsServerHello(this.context, buffer);
					}
					goto IL_0111;
				}
			}
			else
			{
				switch (type)
				{
				case HandshakeType.Certificate:
					if (lastHandshakeMsg == HandshakeType.ServerHello)
					{
						return new TlsServerCertificate(this.context, buffer);
					}
					goto IL_0111;
				case HandshakeType.ServerKeyExchange:
					break;
				case HandshakeType.CertificateRequest:
					if (lastHandshakeMsg == HandshakeType.ServerKeyExchange || lastHandshakeMsg == HandshakeType.Certificate)
					{
						return new TlsServerCertificateRequest(this.context, buffer);
					}
					goto IL_0111;
				case HandshakeType.ServerHelloDone:
					if (lastHandshakeMsg == HandshakeType.CertificateRequest || lastHandshakeMsg == HandshakeType.Certificate || lastHandshakeMsg == HandshakeType.ServerHello)
					{
						return new TlsServerHelloDone(this.context, buffer);
					}
					goto IL_0111;
				default:
					if (type == HandshakeType.Finished)
					{
						if ((clientContext.AbbreviatedHandshake ? (lastHandshakeMsg == HandshakeType.ServerHello) : (lastHandshakeMsg == HandshakeType.ServerHelloDone)) && clientContext.ChangeCipherSpecDone)
						{
							clientContext.ChangeCipherSpecDone = false;
							return new TlsServerFinished(this.context, buffer);
						}
						goto IL_0111;
					}
					break;
				}
			}
			throw new TlsException(AlertDescription.UnexpectedMessage, string.Format(CultureInfo.CurrentUICulture, "Unknown server handshake message received ({0})", type.ToString()));
			IL_0111:
			throw new TlsException(AlertDescription.HandshakeFailiure, string.Format("Protocol error, unexpected protocol transition from {0} to {1}", lastHandshakeMsg, type));
		}
	}
}
