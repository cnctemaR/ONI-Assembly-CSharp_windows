using System;
using System.Globalization;
using System.IO;
using Mono.Security.Protocol.Tls.Handshake;
using Mono.Security.Protocol.Tls.Handshake.Server;

namespace Mono.Security.Protocol.Tls
{
	internal class ServerRecordProtocol : RecordProtocol
	{
		public ServerRecordProtocol(Stream innerStream, ServerContext context)
			: base(innerStream, context)
		{
		}

		public override HandshakeMessage GetMessage(HandshakeType type)
		{
			return this.createServerHandshakeMessage(type);
		}

		protected override void ProcessHandshakeMessage(TlsStream handMsg)
		{
			HandshakeType handshakeType = (HandshakeType)handMsg.ReadByte();
			int num = handMsg.ReadInt24();
			byte[] array = new byte[num];
			handMsg.Read(array, 0, num);
			HandshakeMessage handshakeMessage = this.createClientHandshakeMessage(handshakeType, array);
			handshakeMessage.Process();
			base.Context.LastHandshakeMsg = handshakeType;
			if (handshakeMessage != null)
			{
				handshakeMessage.Update();
				base.Context.HandshakeMessages.WriteByte((byte)handshakeType);
				base.Context.HandshakeMessages.WriteInt24(num);
				base.Context.HandshakeMessages.Write(array, 0, array.Length);
			}
		}

		private HandshakeMessage createClientHandshakeMessage(HandshakeType type, byte[] buffer)
		{
			HandshakeType lastHandshakeMsg = this.context.LastHandshakeMsg;
			if (type <= HandshakeType.Certificate)
			{
				if (type == HandshakeType.ClientHello)
				{
					return new TlsClientHello(this.context, buffer);
				}
				if (type == HandshakeType.Certificate)
				{
					if (lastHandshakeMsg == HandshakeType.ClientHello)
					{
						this.cert = new TlsClientCertificate(this.context, buffer);
						return this.cert;
					}
					goto IL_0106;
				}
			}
			else if (type != HandshakeType.CertificateVerify)
			{
				if (type != HandshakeType.ClientKeyExchange)
				{
					if (type == HandshakeType.Finished)
					{
						if (((this.cert != null && this.cert.HasCertificate) ? (lastHandshakeMsg == HandshakeType.CertificateVerify) : (lastHandshakeMsg == HandshakeType.ClientKeyExchange)) && this.context.ChangeCipherSpecDone)
						{
							this.context.ChangeCipherSpecDone = false;
							return new TlsClientFinished(this.context, buffer);
						}
						goto IL_0106;
					}
				}
				else
				{
					if (lastHandshakeMsg == HandshakeType.ClientHello || lastHandshakeMsg == HandshakeType.Certificate)
					{
						return new TlsClientKeyExchange(this.context, buffer);
					}
					goto IL_0106;
				}
			}
			else
			{
				if (lastHandshakeMsg == HandshakeType.ClientKeyExchange && this.cert != null)
				{
					return new TlsClientCertificateVerify(this.context, buffer);
				}
				goto IL_0106;
			}
			throw new TlsException(AlertDescription.UnexpectedMessage, string.Format(CultureInfo.CurrentUICulture, "Unknown server handshake message received ({0})", type.ToString()));
			IL_0106:
			throw new TlsException(AlertDescription.HandshakeFailiure, string.Format("Protocol error, unexpected protocol transition from {0} to {1}", lastHandshakeMsg, type));
		}

		private HandshakeMessage createServerHandshakeMessage(HandshakeType type)
		{
			if (type <= HandshakeType.ServerHello)
			{
				if (type == HandshakeType.HelloRequest)
				{
					this.SendRecord(HandshakeType.ClientHello);
					return null;
				}
				if (type == HandshakeType.ServerHello)
				{
					return new TlsServerHello(this.context);
				}
			}
			else
			{
				switch (type)
				{
				case HandshakeType.Certificate:
					return new TlsServerCertificate(this.context);
				case HandshakeType.ServerKeyExchange:
					return new TlsServerKeyExchange(this.context);
				case HandshakeType.CertificateRequest:
					return new TlsServerCertificateRequest(this.context);
				case HandshakeType.ServerHelloDone:
					return new TlsServerHelloDone(this.context);
				default:
					if (type == HandshakeType.Finished)
					{
						return new TlsServerFinished(this.context);
					}
					break;
				}
			}
			throw new InvalidOperationException("Unknown server handshake message type: " + type.ToString());
		}

		private TlsClientCertificate cert;
	}
}
