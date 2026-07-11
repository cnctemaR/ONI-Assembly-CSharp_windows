using System;

namespace Mono.Security.Protocol.Tls
{
	internal class Alert
	{
		public AlertLevel Level
		{
			get
			{
				return this.level;
			}
		}

		public AlertDescription Description
		{
			get
			{
				return this.description;
			}
		}

		public string Message
		{
			get
			{
				return Alert.GetAlertMessage(this.description);
			}
		}

		public bool IsWarning
		{
			get
			{
				return this.level == AlertLevel.Warning;
			}
		}

		public bool IsCloseNotify
		{
			get
			{
				return this.IsWarning && this.description == AlertDescription.CloseNotify;
			}
		}

		public Alert(AlertDescription description)
		{
			this.description = description;
			this.level = Alert.inferAlertLevel(description);
		}

		public Alert(AlertLevel level, AlertDescription description)
		{
			this.level = level;
			this.description = description;
		}

		private static AlertLevel inferAlertLevel(AlertDescription description)
		{
			if (description <= AlertDescription.DecryptError)
			{
				if (description <= AlertDescription.UnexpectedMessage)
				{
					if (description != AlertDescription.CloseNotify)
					{
						if (description != AlertDescription.UnexpectedMessage)
						{
							return AlertLevel.Fatal;
						}
						return AlertLevel.Fatal;
					}
				}
				else
				{
					if (description - AlertDescription.BadRecordMAC <= 2)
					{
						return AlertLevel.Fatal;
					}
					switch (description)
					{
					case AlertDescription.DecompressionFailiure:
					case (AlertDescription)31:
					case (AlertDescription)32:
					case (AlertDescription)33:
					case (AlertDescription)34:
					case (AlertDescription)35:
					case (AlertDescription)36:
					case (AlertDescription)37:
					case (AlertDescription)38:
					case (AlertDescription)39:
					case AlertDescription.HandshakeFailiure:
					case AlertDescription.NoCertificate:
					case AlertDescription.BadCertificate:
					case AlertDescription.UnsupportedCertificate:
					case AlertDescription.CertificateRevoked:
					case AlertDescription.CertificateExpired:
					case AlertDescription.CertificateUnknown:
					case AlertDescription.IlegalParameter:
					case AlertDescription.UnknownCA:
					case AlertDescription.AccessDenied:
					case AlertDescription.DecodeError:
					case AlertDescription.DecryptError:
						return AlertLevel.Fatal;
					default:
						return AlertLevel.Fatal;
					}
				}
			}
			else if (description <= AlertDescription.InsuficientSecurity)
			{
				if (description != AlertDescription.ExportRestriction && description - AlertDescription.ProtocolVersion > 1)
				{
					return AlertLevel.Fatal;
				}
				return AlertLevel.Fatal;
			}
			else if (description == AlertDescription.InternalError || (description != AlertDescription.UserCancelled && description != AlertDescription.NoRenegotiation))
			{
				return AlertLevel.Fatal;
			}
			return AlertLevel.Warning;
		}

		public static string GetAlertMessage(AlertDescription description)
		{
			return "The authentication or decryption has failed.";
		}

		private AlertLevel level;

		private AlertDescription description;
	}
}
