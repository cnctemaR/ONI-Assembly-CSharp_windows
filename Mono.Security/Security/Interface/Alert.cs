using System;

namespace Mono.Security.Interface
{
	public class Alert
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
			this.inferAlertLevel();
		}

		public Alert(AlertLevel level, AlertDescription description)
		{
			this.level = level;
			this.description = description;
		}

		private void inferAlertLevel()
		{
			AlertDescription alertDescription = this.description;
			if (alertDescription <= AlertDescription.ExportRestriction)
			{
				if (alertDescription <= AlertDescription.UnexpectedMessage)
				{
					if (alertDescription != AlertDescription.CloseNotify)
					{
						if (alertDescription != AlertDescription.UnexpectedMessage)
						{
							goto IL_00C5;
						}
						goto IL_00C5;
					}
				}
				else
				{
					if (alertDescription - AlertDescription.BadRecordMAC <= 2)
					{
						goto IL_00C5;
					}
					switch (alertDescription)
					{
					case AlertDescription.DecompressionFailure:
					case (AlertDescription)31:
					case (AlertDescription)32:
					case (AlertDescription)33:
					case (AlertDescription)34:
					case (AlertDescription)35:
					case (AlertDescription)36:
					case (AlertDescription)37:
					case (AlertDescription)38:
					case (AlertDescription)39:
					case AlertDescription.HandshakeFailure:
					case AlertDescription.NoCertificate_RESERVED:
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
						goto IL_00C5;
					default:
						if (alertDescription != AlertDescription.ExportRestriction)
						{
							goto IL_00C5;
						}
						goto IL_00C5;
					}
				}
			}
			else if (alertDescription <= AlertDescription.InternalError)
			{
				if (alertDescription - AlertDescription.ProtocolVersion > 1 && alertDescription != AlertDescription.InternalError)
				{
					goto IL_00C5;
				}
				goto IL_00C5;
			}
			else if (alertDescription != AlertDescription.UserCancelled && alertDescription != AlertDescription.NoRenegotiation)
			{
				if (alertDescription != AlertDescription.UnsupportedExtension)
				{
					goto IL_00C5;
				}
				goto IL_00C5;
			}
			this.level = AlertLevel.Warning;
			return;
			IL_00C5:
			this.level = AlertLevel.Fatal;
		}

		public override string ToString()
		{
			return string.Format("[Alert: {0}:{1}]", this.Level, this.Description);
		}

		public static string GetAlertMessage(AlertDescription description)
		{
			return "The authentication or decryption has failed.";
		}

		private AlertLevel level;

		private AlertDescription description;
	}
}
