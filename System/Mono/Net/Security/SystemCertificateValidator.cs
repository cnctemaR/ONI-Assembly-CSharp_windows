using System;
using System.Globalization;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Mono.Security.Interface;
using Mono.Security.X509;
using Mono.Security.X509.Extensions;

namespace Mono.Net.Security
{
	internal static class SystemCertificateValidator
	{
		static SystemCertificateValidator()
		{
			try
			{
				string environmentVariable = Environment.GetEnvironmentVariable("MONO_X509_REVOCATION_MODE");
				if (!string.IsNullOrEmpty(environmentVariable))
				{
					SystemCertificateValidator.revocation_mode = (X509RevocationMode)Enum.Parse(typeof(X509RevocationMode), environmentVariable, true);
				}
			}
			catch
			{
			}
		}

		public static global::System.Security.Cryptography.X509Certificates.X509Chain CreateX509Chain(global::System.Security.Cryptography.X509Certificates.X509CertificateCollection certs)
		{
			return new global::System.Security.Cryptography.X509Certificates.X509Chain
			{
				ChainPolicy = new X509ChainPolicy((global::System.Security.Cryptography.X509Certificates.X509CertificateCollection)certs),
				ChainPolicy = 
				{
					RevocationMode = SystemCertificateValidator.revocation_mode
				}
			};
		}

		private static bool BuildX509Chain(global::System.Security.Cryptography.X509Certificates.X509CertificateCollection certs, global::System.Security.Cryptography.X509Certificates.X509Chain chain, ref SslPolicyErrors errors, ref int status11)
		{
			if (SystemCertificateValidator.is_macosx)
			{
				return false;
			}
			X509Certificate2 x509Certificate = (X509Certificate2)certs[0];
			bool flag;
			try
			{
				flag = chain.Build(x509Certificate);
				if (!flag)
				{
					errors |= SystemCertificateValidator.GetErrorsFromChain(chain);
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("ERROR building certificate chain: {0}", ex);
				Console.Error.WriteLine("Please, report this problem to the Mono team");
				errors |= SslPolicyErrors.RemoteCertificateChainErrors;
				flag = false;
			}
			try
			{
				status11 = SystemCertificateValidator.GetStatusFromChain(chain);
			}
			catch
			{
				status11 = -2146762485;
			}
			return flag;
		}

		private static bool CheckUsage(global::System.Security.Cryptography.X509Certificates.X509CertificateCollection certs, string host, ref SslPolicyErrors errors, ref int status11)
		{
			X509Certificate2 x509Certificate = certs[0] as X509Certificate2;
			if (x509Certificate == null)
			{
				x509Certificate = new X509Certificate2(certs[0]);
			}
			if (!SystemCertificateValidator.is_macosx)
			{
				if (!SystemCertificateValidator.CheckCertificateUsage(x509Certificate))
				{
					errors |= SslPolicyErrors.RemoteCertificateChainErrors;
					status11 = -2146762490;
					return false;
				}
				if (!string.IsNullOrEmpty(host) && !SystemCertificateValidator.CheckServerIdentity(x509Certificate, host))
				{
					errors |= SslPolicyErrors.RemoteCertificateNameMismatch;
					status11 = -2146762481;
					return false;
				}
			}
			return true;
		}

		private static bool EvaluateSystem(global::System.Security.Cryptography.X509Certificates.X509CertificateCollection certs, global::System.Security.Cryptography.X509Certificates.X509CertificateCollection anchors, string host, global::System.Security.Cryptography.X509Certificates.X509Chain chain, ref SslPolicyErrors errors, ref int status11)
		{
			global::System.Security.Cryptography.X509Certificates.X509Certificate x509Certificate = certs[0];
			bool flag;
			if (SystemCertificateValidator.is_macosx)
			{
				OSX509Certificates.SecTrustResult secTrustResult = OSX509Certificates.SecTrustResult.Deny;
				try
				{
					secTrustResult = OSX509Certificates.TrustEvaluateSsl(certs, anchors, host);
					flag = secTrustResult == OSX509Certificates.SecTrustResult.Proceed || secTrustResult == OSX509Certificates.SecTrustResult.Unspecified;
				}
				catch
				{
					flag = false;
					errors |= SslPolicyErrors.RemoteCertificateChainErrors;
				}
				if (flag)
				{
					errors = SslPolicyErrors.None;
				}
				else
				{
					status11 = (int)secTrustResult;
					errors |= SslPolicyErrors.RemoteCertificateChainErrors;
				}
			}
			else
			{
				flag = SystemCertificateValidator.BuildX509Chain(certs, chain, ref errors, ref status11);
			}
			return flag;
		}

		public static bool Evaluate(MonoTlsSettings settings, string host, global::System.Security.Cryptography.X509Certificates.X509CertificateCollection certs, global::System.Security.Cryptography.X509Certificates.X509Chain chain, ref SslPolicyErrors errors, ref int status11)
		{
			if (!SystemCertificateValidator.CheckUsage(certs, host, ref errors, ref status11))
			{
				return false;
			}
			if (settings != null && settings.SkipSystemValidators)
			{
				return false;
			}
			global::System.Security.Cryptography.X509Certificates.X509CertificateCollection x509CertificateCollection = ((settings != null) ? settings.TrustAnchors : null);
			return SystemCertificateValidator.EvaluateSystem(certs, x509CertificateCollection, host, chain, ref errors, ref status11);
		}

		internal static bool NeedsChain(MonoTlsSettings settings)
		{
			return !SystemCertificateValidator.is_macosx || (CertificateValidationHelper.SupportsX509Chain && (settings == null || !settings.SkipSystemValidators || settings.CallbackNeedsCertificateChain));
		}

		private static int GetStatusFromChain(global::System.Security.Cryptography.X509Certificates.X509Chain chain)
		{
			long num = 0L;
			X509ChainStatus[] chainStatus = chain.ChainStatus;
			int i = 0;
			while (i < chainStatus.Length)
			{
				X509ChainStatus x509ChainStatus = chainStatus[i];
				global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags status = x509ChainStatus.Status;
				if (status != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
				{
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NotTimeValid) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146762495));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NotTimeNested) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146762494));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.Revoked) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146762484));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NotSignatureValid) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146869244));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NotValidForUsage) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146762480));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146762487));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.RevocationStatusUnknown) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146885614));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.Cyclic) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146762486));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.InvalidExtension) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146762485));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.InvalidPolicyConstraints) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146762483));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.InvalidBasicConstraints) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146869223));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.InvalidNameConstraints) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146762476));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.HasNotSupportedNameConstraint) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146762476));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.HasNotDefinedNameConstraint) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146762476));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.HasNotPermittedNameConstraint) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146762476));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.HasExcludedNameConstraint) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146762476));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.PartialChain) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146762486));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.CtlNotTimeValid) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146762495));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.CtlNotSignatureValid) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146869244));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.CtlNotValidForUsage) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146762480));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.OfflineRevocation) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146885614));
						break;
					}
					if ((status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoIssuanceChainPolicy) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (long)((ulong)(-2146762489));
						break;
					}
					num = (long)((ulong)(-2146762485));
					break;
				}
				else
				{
					i++;
				}
			}
			return (int)num;
		}

		private static SslPolicyErrors GetErrorsFromChain(global::System.Security.Cryptography.X509Certificates.X509Chain chain)
		{
			SslPolicyErrors sslPolicyErrors = SslPolicyErrors.None;
			foreach (X509ChainStatus x509ChainStatus in chain.ChainStatus)
			{
				if (x509ChainStatus.Status != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
				{
					sslPolicyErrors |= SslPolicyErrors.RemoteCertificateChainErrors;
					break;
				}
			}
			return sslPolicyErrors;
		}

		private static bool CheckCertificateUsage(X509Certificate2 cert)
		{
			bool flag;
			try
			{
				if (cert.Version < 3)
				{
					flag = true;
				}
				else
				{
					X509KeyUsageExtension x509KeyUsageExtension = cert.Extensions["2.5.29.15"] as X509KeyUsageExtension;
					X509EnhancedKeyUsageExtension x509EnhancedKeyUsageExtension = cert.Extensions["2.5.29.37"] as X509EnhancedKeyUsageExtension;
					if (x509KeyUsageExtension != null && x509EnhancedKeyUsageExtension != null)
					{
						if ((x509KeyUsageExtension.KeyUsages & SystemCertificateValidator.s_flags) == X509KeyUsageFlags.None)
						{
							flag = false;
						}
						else
						{
							flag = x509EnhancedKeyUsageExtension.EnhancedKeyUsages["1.3.6.1.5.5.7.3.1"] != null || x509EnhancedKeyUsageExtension.EnhancedKeyUsages["2.16.840.1.113730.4.1"] != null;
						}
					}
					else if (x509KeyUsageExtension != null)
					{
						flag = (x509KeyUsageExtension.KeyUsages & SystemCertificateValidator.s_flags) > X509KeyUsageFlags.None;
					}
					else if (x509EnhancedKeyUsageExtension != null)
					{
						flag = x509EnhancedKeyUsageExtension.EnhancedKeyUsages["1.3.6.1.5.5.7.3.1"] != null || x509EnhancedKeyUsageExtension.EnhancedKeyUsages["2.16.840.1.113730.4.1"] != null;
					}
					else
					{
						global::System.Security.Cryptography.X509Certificates.X509Extension x509Extension = cert.Extensions["2.16.840.1.113730.1.1"];
						if (x509Extension != null)
						{
							flag = x509Extension.NetscapeCertType(false).IndexOf("SSL Server Authentication", StringComparison.Ordinal) != -1;
						}
						else
						{
							flag = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("ERROR processing certificate: {0}", ex);
				Console.Error.WriteLine("Please, report this problem to the Mono team");
				flag = false;
			}
			return flag;
		}

		private static bool CheckServerIdentity(X509Certificate2 cert, string targetHost)
		{
			bool flag;
			try
			{
				Mono.Security.X509.X509Certificate x509Certificate = new Mono.Security.X509.X509Certificate(cert.RawData);
				Mono.Security.X509.X509Extension x509Extension = x509Certificate.Extensions["2.5.29.17"];
				if (x509Extension != null)
				{
					SubjectAltNameExtension subjectAltNameExtension = new SubjectAltNameExtension(x509Extension);
					foreach (string text in subjectAltNameExtension.DNSNames)
					{
						if (SystemCertificateValidator.Match(targetHost, text))
						{
							return true;
						}
					}
					string[] array = subjectAltNameExtension.IPAddresses;
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] == targetHost)
						{
							return true;
						}
					}
				}
				flag = SystemCertificateValidator.CheckDomainName(x509Certificate.SubjectName, targetHost);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("ERROR processing certificate: {0}", ex);
				Console.Error.WriteLine("Please, report this problem to the Mono team");
				flag = false;
			}
			return flag;
		}

		private static bool CheckDomainName(string subjectName, string targetHost)
		{
			string text = string.Empty;
			MatchCollection matchCollection = new Regex("CN\\s*=\\s*([^,]*)").Matches(subjectName);
			if (matchCollection.Count == 1 && matchCollection[0].Success)
			{
				text = matchCollection[0].Groups[1].Value.ToString();
			}
			return SystemCertificateValidator.Match(targetHost, text);
		}

		private static bool Match(string hostname, string pattern)
		{
			int num = pattern.IndexOf('*');
			if (num == -1)
			{
				return string.Compare(hostname, pattern, true, CultureInfo.InvariantCulture) == 0;
			}
			if (num != pattern.Length - 1 && pattern[num + 1] != '.')
			{
				return false;
			}
			if (pattern.IndexOf('*', num + 1) != -1)
			{
				return false;
			}
			string text = pattern.Substring(num + 1);
			int num2 = hostname.Length - text.Length;
			if (num2 <= 0)
			{
				return false;
			}
			if (string.Compare(hostname, num2, text, 0, text.Length, true, CultureInfo.InvariantCulture) != 0)
			{
				return false;
			}
			if (num == 0)
			{
				int num3 = hostname.IndexOf('.');
				return num3 == -1 || num3 >= hostname.Length - text.Length;
			}
			string text2 = pattern.Substring(0, num);
			return string.Compare(hostname, 0, text2, 0, text2.Length, true, CultureInfo.InvariantCulture) == 0;
		}

		private static bool is_macosx = Environment.OSVersion.Platform != PlatformID.Win32NT && File.Exists("/System/Library/Frameworks/Security.framework/Security");

		private static X509RevocationMode revocation_mode = X509RevocationMode.NoCheck;

		private static X509KeyUsageFlags s_flags = X509KeyUsageFlags.KeyAgreement | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature;
	}
}
