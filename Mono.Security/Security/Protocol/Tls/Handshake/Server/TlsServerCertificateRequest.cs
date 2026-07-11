using System;

namespace Mono.Security.Protocol.Tls.Handshake.Server
{
	internal class TlsServerCertificateRequest : HandshakeMessage
	{
		public TlsServerCertificateRequest(Context context)
			: base(context, HandshakeType.CertificateRequest)
		{
		}

		protected override void ProcessAsSsl3()
		{
			this.ProcessAsTls1();
		}

		protected override void ProcessAsTls1()
		{
			ServerContext serverContext = (ServerContext)base.Context;
			int num = serverContext.ServerSettings.CertificateTypes.Length;
			this.WriteByte(Convert.ToByte(num));
			for (int i = 0; i < num; i++)
			{
				this.WriteByte((byte)serverContext.ServerSettings.CertificateTypes[i]);
			}
			base.Write(0);
		}
	}
}
