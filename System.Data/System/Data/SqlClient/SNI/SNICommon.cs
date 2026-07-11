using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace System.Data.SqlClient.SNI
{
	internal class SNICommon
	{
		internal static bool ValidateSslServerCertificate(string targetServerName, object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
		{
			if (policyErrors == SslPolicyErrors.None)
			{
				return true;
			}
			if ((policyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) == SslPolicyErrors.None)
			{
				return false;
			}
			string text = cert.Subject.Substring(cert.Subject.IndexOf('=') + 1);
			if (targetServerName.Length > text.Length)
			{
				return false;
			}
			if (targetServerName.Length == text.Length)
			{
				if (!targetServerName.Equals(text, StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
			}
			else
			{
				if (string.Compare(targetServerName, 0, text, 0, targetServerName.Length, StringComparison.OrdinalIgnoreCase) != 0)
				{
					return false;
				}
				if (text[targetServerName.Length] != '.')
				{
					return false;
				}
			}
			return true;
		}

		internal static uint ReportSNIError(SNIProviders provider, uint nativeError, uint sniError, string errorMessage)
		{
			return SNICommon.ReportSNIError(new SNIError(provider, nativeError, sniError, errorMessage));
		}

		internal static uint ReportSNIError(SNIProviders provider, uint sniError, Exception sniException)
		{
			return SNICommon.ReportSNIError(new SNIError(provider, sniError, sniException));
		}

		internal static uint ReportSNIError(SNIError error)
		{
			SNILoadHandle.SingletonInstance.LastError = error;
			return 1U;
		}

		internal const int ConnTerminatedError = 2;

		internal const int InvalidParameterError = 5;

		internal const int ProtocolNotSupportedError = 8;

		internal const int ConnTimeoutError = 11;

		internal const int ConnNotUsableError = 19;

		internal const int InvalidConnStringError = 25;

		internal const int HandshakeFailureError = 31;

		internal const int InternalExceptionError = 35;

		internal const int ConnOpenFailedError = 40;

		internal const int ErrorSpnLookup = 44;

		internal const int LocalDBErrorCode = 50;

		internal const int MultiSubnetFailoverWithMoreThan64IPs = 47;

		internal const int MultiSubnetFailoverWithInstanceSpecified = 48;

		internal const int MultiSubnetFailoverWithNonTcpProtocol = 49;

		internal const int MaxErrorValue = 50157;

		internal const int LocalDBNoInstanceName = 51;

		internal const int LocalDBNoInstallation = 52;

		internal const int LocalDBInvalidConfig = 53;

		internal const int LocalDBNoSqlUserInstanceDllPath = 54;

		internal const int LocalDBInvalidSqlUserInstanceDllPath = 55;

		internal const int LocalDBFailedToLoadDll = 56;

		internal const int LocalDBBadRuntime = 57;
	}
}
