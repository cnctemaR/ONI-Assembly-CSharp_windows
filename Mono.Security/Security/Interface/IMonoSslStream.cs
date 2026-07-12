using System;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Mono.Security.Interface
{
	public interface IMonoSslStream : IDisposable
	{
		SslStream SslStream { get; }

		Task AuthenticateAsClientAsync(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation);

		Task AuthenticateAsServerAsync(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation);

		Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);

		Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);

		Task ShutdownAsync();

		TransportContext TransportContext { get; }

		bool IsAuthenticated { get; }

		bool IsMutuallyAuthenticated { get; }

		bool IsEncrypted { get; }

		bool IsSigned { get; }

		bool IsServer { get; }

		CipherAlgorithmType CipherAlgorithm { get; }

		int CipherStrength { get; }

		HashAlgorithmType HashAlgorithm { get; }

		int HashStrength { get; }

		ExchangeAlgorithmType KeyExchangeAlgorithm { get; }

		int KeyExchangeStrength { get; }

		bool CanRead { get; }

		bool CanTimeout { get; }

		bool CanWrite { get; }

		long Length { get; }

		long Position { get; }

		void SetLength(long value);

		AuthenticatedStream AuthenticatedStream { get; }

		int ReadTimeout { get; set; }

		int WriteTimeout { get; set; }

		bool CheckCertRevocationStatus { get; }

		X509Certificate InternalLocalCertificate { get; }

		X509Certificate LocalCertificate { get; }

		X509Certificate RemoteCertificate { get; }

		SslProtocols SslProtocol { get; }

		MonoTlsProvider Provider { get; }

		MonoTlsConnectionInfo GetConnectionInfo();

		bool CanRenegotiate { get; }

		Task RenegotiateAsync(CancellationToken cancellationToken);
	}
}
