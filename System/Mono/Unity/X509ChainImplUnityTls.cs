using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Mono.Unity
{
	internal class X509ChainImplUnityTls : X509ChainImpl
	{
		internal X509ChainImplUnityTls(UnityTls.unitytls_x509list_ref nativeCertificateChain, bool reverseOrder = false)
		{
			this.elements = null;
			this.ownedList = null;
			this.nativeCertificateChain = nativeCertificateChain;
			this.reverseOrder = reverseOrder;
		}

		internal unsafe X509ChainImplUnityTls(UnityTls.unitytls_x509list* ownedList, UnityTls.unitytls_errorstate* errorState, bool reverseOrder = false)
		{
			this.elements = null;
			this.ownedList = ownedList;
			this.nativeCertificateChain = UnityTls.NativeInterface.unitytls_x509list_get_ref(ownedList, errorState);
			this.reverseOrder = reverseOrder;
		}

		public override bool IsValid
		{
			get
			{
				return this.nativeCertificateChain.handle != UnityTls.NativeInterface.UNITYTLS_INVALID_HANDLE;
			}
		}

		public override IntPtr Handle
		{
			get
			{
				return new IntPtr((long)this.nativeCertificateChain.handle);
			}
		}

		internal UnityTls.unitytls_x509list_ref NativeCertificateChain
		{
			get
			{
				return this.nativeCertificateChain;
			}
		}

		public unsafe override X509ChainElementCollection ChainElements
		{
			get
			{
				base.ThrowIfContextInvalid();
				if (this.elements != null)
				{
					return this.elements;
				}
				this.elements = new X509ChainElementCollection();
				UnityTls.unitytls_errorstate unitytls_errorstate = UnityTls.NativeInterface.unitytls_errorstate_create();
				UnityTls.unitytls_x509_ref unitytls_x509_ref = UnityTls.NativeInterface.unitytls_x509list_get_x509(this.nativeCertificateChain, (IntPtr)0, &unitytls_errorstate);
				int num = 1;
				while (unitytls_x509_ref.handle != UnityTls.NativeInterface.UNITYTLS_INVALID_HANDLE)
				{
					IntPtr intPtr = UnityTls.NativeInterface.unitytls_x509_export_der(unitytls_x509_ref, null, (IntPtr)0, &unitytls_errorstate);
					byte[] array = new byte[(int)intPtr];
					byte[] array2;
					byte* ptr;
					if ((array2 = array) == null || array2.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array2[0];
					}
					UnityTls.NativeInterface.unitytls_x509_export_der(unitytls_x509_ref, ptr, intPtr, &unitytls_errorstate);
					array2 = null;
					this.elements.Add(new X509Certificate2(array));
					unitytls_x509_ref = UnityTls.NativeInterface.unitytls_x509list_get_x509(this.nativeCertificateChain, (IntPtr)num, &unitytls_errorstate);
					num++;
				}
				if (this.reverseOrder)
				{
					X509ChainElementCollection x509ChainElementCollection = new X509ChainElementCollection();
					for (int i = this.elements.Count - 1; i >= 0; i--)
					{
						x509ChainElementCollection.Add(this.elements[i].Certificate);
					}
					this.elements = x509ChainElementCollection;
				}
				return this.elements;
			}
		}

		public override void AddStatus(X509ChainStatusFlags error)
		{
			if (this.chainStatusList == null)
			{
				this.chainStatusList = new List<X509ChainStatus>();
			}
			this.chainStatusList.Add(new X509ChainStatus(error));
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

		public override bool Build(X509Certificate2 certificate)
		{
			return false;
		}

		public override void Reset()
		{
			if (this.elements != null)
			{
				this.nativeCertificateChain.handle = UnityTls.NativeInterface.UNITYTLS_INVALID_HANDLE;
				this.elements.Clear();
				this.elements = null;
			}
			if (this.ownedList != null)
			{
				UnityTls.NativeInterface.unitytls_x509list_free(this.ownedList);
				this.ownedList = null;
			}
		}

		protected override void Dispose(bool disposing)
		{
			this.Reset();
			base.Dispose(disposing);
		}

		private X509ChainElementCollection elements;

		private unsafe UnityTls.unitytls_x509list* ownedList;

		private UnityTls.unitytls_x509list_ref nativeCertificateChain;

		private X509ChainPolicy policy = new X509ChainPolicy();

		private List<X509ChainStatus> chainStatusList;

		private bool reverseOrder;
	}
}
