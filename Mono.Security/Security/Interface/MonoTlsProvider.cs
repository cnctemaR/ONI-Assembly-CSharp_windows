using System;
using System.IO;
using System.Security.Authentication;

namespace Mono.Security.Interface
{
	public abstract class MonoTlsProvider
	{
		internal MonoTlsProvider()
		{
		}

		public abstract Guid ID { get; }

		public abstract string Name { get; }

		public abstract bool SupportsSslStream { get; }

		public abstract bool SupportsConnectionInfo { get; }

		public abstract bool SupportsMonoExtensions { get; }

		public abstract SslProtocols SupportedProtocols { get; }

		public abstract IMonoSslStream CreateSslStream(Stream innerStream, bool leaveInnerStreamOpen, MonoTlsSettings settings = null);

		internal virtual bool HasNativeCertificates
		{
			get
			{
				return false;
			}
		}

		internal abstract bool SupportsCleanShutdown { get; }
	}
}
