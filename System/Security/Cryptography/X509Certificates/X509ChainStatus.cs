using System;

namespace System.Security.Cryptography.X509Certificates
{
	public struct X509ChainStatus
	{
		internal X509ChainStatus(X509ChainStatusFlags flag)
		{
			this.status = flag;
			this.info = X509ChainStatus.GetInformation(flag);
		}

		public X509ChainStatusFlags Status
		{
			get
			{
				return this.status;
			}
			set
			{
				this.status = value;
			}
		}

		public string StatusInformation
		{
			get
			{
				return this.info;
			}
			set
			{
				this.info = value;
			}
		}

		internal static string GetInformation(X509ChainStatusFlags flags)
		{
			if (flags <= X509ChainStatusFlags.InvalidNameConstraints)
			{
				if (flags <= X509ChainStatusFlags.RevocationStatusUnknown)
				{
					if (flags <= X509ChainStatusFlags.NotValidForUsage)
					{
						switch (flags)
						{
						case X509ChainStatusFlags.NoError:
						case X509ChainStatusFlags.NotTimeValid | X509ChainStatusFlags.NotTimeNested:
						case X509ChainStatusFlags.NotTimeValid | X509ChainStatusFlags.Revoked:
						case X509ChainStatusFlags.NotTimeNested | X509ChainStatusFlags.Revoked:
						case X509ChainStatusFlags.NotTimeValid | X509ChainStatusFlags.NotTimeNested | X509ChainStatusFlags.Revoked:
							goto IL_0125;
						case X509ChainStatusFlags.NotTimeValid:
						case X509ChainStatusFlags.NotTimeNested:
						case X509ChainStatusFlags.Revoked:
						case X509ChainStatusFlags.NotSignatureValid:
							break;
						default:
							if (flags != X509ChainStatusFlags.NotValidForUsage)
							{
								goto IL_0125;
							}
							break;
						}
					}
					else if (flags != X509ChainStatusFlags.UntrustedRoot && flags != X509ChainStatusFlags.RevocationStatusUnknown)
					{
						goto IL_0125;
					}
				}
				else if (flags <= X509ChainStatusFlags.InvalidExtension)
				{
					if (flags != X509ChainStatusFlags.Cyclic && flags != X509ChainStatusFlags.InvalidExtension)
					{
						goto IL_0125;
					}
				}
				else if (flags != X509ChainStatusFlags.InvalidPolicyConstraints && flags != X509ChainStatusFlags.InvalidBasicConstraints && flags != X509ChainStatusFlags.InvalidNameConstraints)
				{
					goto IL_0125;
				}
			}
			else if (flags <= X509ChainStatusFlags.PartialChain)
			{
				if (flags <= X509ChainStatusFlags.HasNotDefinedNameConstraint)
				{
					if (flags != X509ChainStatusFlags.HasNotSupportedNameConstraint && flags != X509ChainStatusFlags.HasNotDefinedNameConstraint)
					{
						goto IL_0125;
					}
				}
				else if (flags != X509ChainStatusFlags.HasNotPermittedNameConstraint && flags != X509ChainStatusFlags.HasExcludedNameConstraint && flags != X509ChainStatusFlags.PartialChain)
				{
					goto IL_0125;
				}
			}
			else if (flags <= X509ChainStatusFlags.CtlNotSignatureValid)
			{
				if (flags != X509ChainStatusFlags.CtlNotTimeValid && flags != X509ChainStatusFlags.CtlNotSignatureValid)
				{
					goto IL_0125;
				}
			}
			else if (flags != X509ChainStatusFlags.CtlNotValidForUsage && flags != X509ChainStatusFlags.OfflineRevocation && flags != X509ChainStatusFlags.NoIssuanceChainPolicy)
			{
				goto IL_0125;
			}
			return global::Locale.GetText(flags.ToString());
			IL_0125:
			return string.Empty;
		}

		private X509ChainStatusFlags status;

		private string info;
	}
}
