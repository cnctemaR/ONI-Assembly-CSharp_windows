using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Mono.Btls
{
	internal class X509ChainImplBtls : X509ChainImpl
	{
		internal X509ChainImplBtls(MonoBtlsX509Chain chain)
		{
			this.chain = chain.Copy();
			this.policy = new X509ChainPolicy();
		}

		internal X509ChainImplBtls(MonoBtlsX509StoreCtx storeCtx)
		{
			this.storeCtx = storeCtx.Copy();
			this.chain = storeCtx.GetChain();
			this.policy = new X509ChainPolicy();
			this.untrustedChain = storeCtx.GetUntrusted();
			if (this.untrustedChain != null)
			{
				this.untrusted = new X509Certificate2Collection();
				this.policy.ExtraStore = this.untrusted;
				for (int i = 0; i < this.untrustedChain.Count; i++)
				{
					using (X509CertificateImplBtls x509CertificateImplBtls = new X509CertificateImplBtls(this.untrustedChain.GetCertificate(i)))
					{
						this.untrusted.Add(new X509Certificate2(x509CertificateImplBtls));
					}
				}
			}
		}

		internal X509ChainImplBtls()
		{
			this.chain = new MonoBtlsX509Chain();
			this.elements = new X509ChainElementCollection();
			this.policy = new X509ChainPolicy();
		}

		public override bool IsValid
		{
			get
			{
				return this.chain != null && this.chain.IsValid;
			}
		}

		public override IntPtr Handle
		{
			get
			{
				return this.chain.Handle.DangerousGetHandle();
			}
		}

		internal MonoBtlsX509Chain Chain
		{
			get
			{
				base.ThrowIfContextInvalid();
				return this.chain;
			}
		}

		internal MonoBtlsX509StoreCtx StoreCtx
		{
			get
			{
				base.ThrowIfContextInvalid();
				return this.storeCtx;
			}
		}

		public override X509ChainElementCollection ChainElements
		{
			get
			{
				base.ThrowIfContextInvalid();
				if (this.elements != null)
				{
					return this.elements;
				}
				this.elements = new X509ChainElementCollection();
				this.certificates = new X509Certificate2[this.chain.Count];
				for (int i = 0; i < this.certificates.Length; i++)
				{
					using (X509CertificateImplBtls x509CertificateImplBtls = new X509CertificateImplBtls(this.chain.GetCertificate(i)))
					{
						this.certificates[i] = new X509Certificate2(x509CertificateImplBtls);
					}
					this.elements.Add(this.certificates[i]);
				}
				return this.elements;
			}
		}

		public override X509ChainPolicy ChainPolicy
		{
			get
			{
				return this.policy;
			}
			set
			{
				this.policy = value;
			}
		}

		public override X509ChainStatus[] ChainStatus
		{
			get
			{
				List<X509ChainStatus> list = this.chainStatusList;
				return ((list != null) ? list.ToArray() : null) ?? new X509ChainStatus[0];
			}
		}

		public override void AddStatus(X509ChainStatusFlags errorCode)
		{
			if (this.chainStatusList == null)
			{
				this.chainStatusList = new List<X509ChainStatus>();
			}
			this.chainStatusList.Add(new X509ChainStatus(errorCode));
		}

		public override bool Build(X509Certificate2 certificate)
		{
			return false;
		}

		public override void Reset()
		{
			if (this.certificates != null)
			{
				X509Certificate2[] array = this.certificates;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Dispose();
				}
				this.certificates = null;
			}
			if (this.elements != null)
			{
				this.elements.Clear();
				this.elements = null;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.chain != null)
				{
					this.chain.Dispose();
					this.chain = null;
				}
				if (this.storeCtx != null)
				{
					this.storeCtx.Dispose();
					this.storeCtx = null;
				}
				if (this.untrustedChain != null)
				{
					this.untrustedChain.Dispose();
					this.untrustedChain = null;
				}
				if (this.untrusted != null)
				{
					foreach (X509Certificate2 x509Certificate in this.untrusted)
					{
						x509Certificate.Dispose();
					}
					this.untrusted = null;
				}
				if (this.certificates != null)
				{
					X509Certificate2[] array = this.certificates;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].Dispose();
					}
					this.certificates = null;
				}
			}
			base.Dispose(disposing);
		}

		private MonoBtlsX509StoreCtx storeCtx;

		private MonoBtlsX509Chain chain;

		private MonoBtlsX509Chain untrustedChain;

		private X509ChainElementCollection elements;

		private X509Certificate2Collection untrusted;

		private X509Certificate2[] certificates;

		private X509ChainPolicy policy;

		private List<X509ChainStatus> chainStatusList;
	}
}
