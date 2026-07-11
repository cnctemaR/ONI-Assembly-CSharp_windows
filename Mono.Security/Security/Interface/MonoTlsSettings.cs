using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Mono.Security.Interface
{
	public sealed class MonoTlsSettings
	{
		public MonoRemoteCertificateValidationCallback RemoteCertificateValidationCallback { get; set; }

		public MonoLocalCertificateSelectionCallback ClientCertificateSelectionCallback { get; set; }

		public bool CheckCertificateName
		{
			get
			{
				return this.checkCertName;
			}
			set
			{
				this.checkCertName = value;
			}
		}

		public bool CheckCertificateRevocationStatus
		{
			get
			{
				return this.checkCertRevocationStatus;
			}
			set
			{
				this.checkCertRevocationStatus = value;
			}
		}

		public bool? UseServicePointManagerCallback
		{
			get
			{
				return this.useServicePointManagerCallback;
			}
			set
			{
				this.useServicePointManagerCallback = value;
			}
		}

		public bool SkipSystemValidators
		{
			get
			{
				return this.skipSystemValidators;
			}
			set
			{
				this.skipSystemValidators = value;
			}
		}

		public bool CallbackNeedsCertificateChain
		{
			get
			{
				return this.callbackNeedsChain;
			}
			set
			{
				this.callbackNeedsChain = value;
			}
		}

		public DateTime? CertificateValidationTime { get; set; }

		public X509CertificateCollection TrustAnchors { get; set; }

		public object UserSettings { get; set; }

		internal string[] CertificateSearchPaths { get; set; }

		internal bool SendCloseNotify { get; set; }

		public TlsProtocols? EnabledProtocols { get; set; }

		[CLSCompliant(false)]
		public CipherSuiteCode[] EnabledCiphers { get; set; }

		public MonoTlsSettings()
		{
		}

		public static MonoTlsSettings DefaultSettings
		{
			get
			{
				if (MonoTlsSettings.defaultSettings == null)
				{
					Interlocked.CompareExchange<MonoTlsSettings>(ref MonoTlsSettings.defaultSettings, new MonoTlsSettings(), null);
				}
				return MonoTlsSettings.defaultSettings;
			}
			set
			{
				MonoTlsSettings.defaultSettings = value ?? new MonoTlsSettings();
			}
		}

		public static MonoTlsSettings CopyDefaultSettings()
		{
			return MonoTlsSettings.DefaultSettings.Clone();
		}

		[Obsolete("Do not use outside System.dll!")]
		public ICertificateValidator CertificateValidator
		{
			get
			{
				return this.certificateValidator;
			}
		}

		[Obsolete("Do not use outside System.dll!")]
		public MonoTlsSettings CloneWithValidator(ICertificateValidator validator)
		{
			if (this.cloned)
			{
				this.certificateValidator = validator;
				return this;
			}
			return new MonoTlsSettings(this)
			{
				certificateValidator = validator
			};
		}

		public MonoTlsSettings Clone()
		{
			return new MonoTlsSettings(this);
		}

		private MonoTlsSettings(MonoTlsSettings other)
		{
			this.RemoteCertificateValidationCallback = other.RemoteCertificateValidationCallback;
			this.ClientCertificateSelectionCallback = other.ClientCertificateSelectionCallback;
			this.checkCertName = other.checkCertName;
			this.checkCertRevocationStatus = other.checkCertRevocationStatus;
			this.UseServicePointManagerCallback = other.useServicePointManagerCallback;
			this.skipSystemValidators = other.skipSystemValidators;
			this.callbackNeedsChain = other.callbackNeedsChain;
			this.UserSettings = other.UserSettings;
			this.EnabledProtocols = other.EnabledProtocols;
			this.EnabledCiphers = other.EnabledCiphers;
			this.CertificateValidationTime = other.CertificateValidationTime;
			this.SendCloseNotify = other.SendCloseNotify;
			if (other.TrustAnchors != null)
			{
				this.TrustAnchors = new X509CertificateCollection(other.TrustAnchors);
			}
			if (other.CertificateSearchPaths != null)
			{
				this.CertificateSearchPaths = new string[other.CertificateSearchPaths.Length];
				other.CertificateSearchPaths.CopyTo(this.CertificateSearchPaths, 0);
			}
			this.cloned = true;
		}

		private bool cloned;

		private bool checkCertName = true;

		private bool checkCertRevocationStatus;

		private bool? useServicePointManagerCallback;

		private bool skipSystemValidators;

		private bool callbackNeedsChain = true;

		private ICertificateValidator certificateValidator;

		private static MonoTlsSettings defaultSettings;
	}
}
