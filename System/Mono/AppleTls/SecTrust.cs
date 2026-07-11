using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Mono.Net;
using ObjCRuntimeInternal;

namespace Mono.AppleTls
{
	internal class SecTrust : INativeObject, IDisposable
	{
		internal SecTrust(IntPtr handle, bool owns = false)
		{
			if (handle == IntPtr.Zero)
			{
				throw new Exception("Invalid handle");
			}
			this.handle = handle;
			if (!owns)
			{
				CFObject.CFRetain(handle);
			}
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SecStatusCode SecTrustCreateWithCertificates(IntPtr certOrCertArray, IntPtr policies, out IntPtr sectrustref);

		public SecTrust(X509CertificateCollection certificates, SecPolicy policy)
		{
			if (certificates == null)
			{
				throw new ArgumentNullException("certificates");
			}
			SecCertificate[] array = new SecCertificate[certificates.Count];
			int i = 0;
			foreach (X509Certificate x509Certificate in certificates)
			{
				array[i++] = new SecCertificate(x509Certificate);
			}
			this.Initialize(array, policy);
			for (i = 0; i < array.Length; i++)
			{
				array[i].Dispose();
			}
		}

		private void Initialize(SecCertificate[] array, SecPolicy policy)
		{
			using (CFArray cfarray = CFArray.CreateArray(array))
			{
				this.Initialize(cfarray.Handle, policy);
			}
		}

		private void Initialize(IntPtr certHandle, SecPolicy policy)
		{
			SecStatusCode secStatusCode = SecTrust.SecTrustCreateWithCertificates(certHandle, (policy == null) ? IntPtr.Zero : policy.Handle, out this.handle);
			if (secStatusCode != SecStatusCode.Success)
			{
				throw new ArgumentException(secStatusCode.ToString());
			}
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SecStatusCode SecTrustEvaluate(IntPtr trust, out SecTrustResult result);

		public SecTrustResult Evaluate()
		{
			if (this.handle == IntPtr.Zero)
			{
				throw new ObjectDisposedException("SecTrust");
			}
			SecTrustResult secTrustResult;
			SecStatusCode secStatusCode = SecTrust.SecTrustEvaluate(this.handle, out secTrustResult);
			if (secStatusCode != SecStatusCode.Success)
			{
				throw new InvalidOperationException(secStatusCode.ToString());
			}
			return secTrustResult;
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern IntPtr SecTrustGetCertificateCount(IntPtr trust);

		public int Count
		{
			get
			{
				if (this.handle == IntPtr.Zero)
				{
					return 0;
				}
				return (int)SecTrust.SecTrustGetCertificateCount(this.handle);
			}
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern IntPtr SecTrustGetCertificateAtIndex(IntPtr trust, IntPtr ix);

		public SecCertificate this[IntPtr index]
		{
			get
			{
				if (this.handle == IntPtr.Zero)
				{
					throw new ObjectDisposedException("SecTrust");
				}
				if ((long)index < 0L || (long)index >= (long)this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return new SecCertificate(SecTrust.SecTrustGetCertificateAtIndex(this.handle, index), false);
			}
		}

		internal X509Certificate GetCertificate(int index)
		{
			if (this.handle == IntPtr.Zero)
			{
				throw new ObjectDisposedException("SecTrust");
			}
			if (index < 0 || index >= this.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return new X509Certificate(SecTrust.SecTrustGetCertificateAtIndex(this.handle, (IntPtr)index));
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SecStatusCode SecTrustSetAnchorCertificates(IntPtr trust, IntPtr anchorCertificates);

		public SecStatusCode SetAnchorCertificates(X509CertificateCollection certificates)
		{
			if (this.handle == IntPtr.Zero)
			{
				throw new ObjectDisposedException("SecTrust");
			}
			if (certificates == null)
			{
				return SecTrust.SecTrustSetAnchorCertificates(this.handle, IntPtr.Zero);
			}
			SecCertificate[] array = new SecCertificate[certificates.Count];
			int num = 0;
			foreach (X509Certificate x509Certificate in certificates)
			{
				array[num++] = new SecCertificate(x509Certificate);
			}
			return this.SetAnchorCertificates(array);
		}

		public SecStatusCode SetAnchorCertificates(SecCertificate[] array)
		{
			if (array == null)
			{
				return SecTrust.SecTrustSetAnchorCertificates(this.handle, IntPtr.Zero);
			}
			SecStatusCode secStatusCode;
			using (CFArray cfarray = CFArray.FromNativeObjects(array))
			{
				secStatusCode = SecTrust.SecTrustSetAnchorCertificates(this.handle, cfarray.Handle);
			}
			return secStatusCode;
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SecStatusCode SecTrustSetAnchorCertificatesOnly(IntPtr trust, bool anchorCertificatesOnly);

		public SecStatusCode SetAnchorCertificatesOnly(bool anchorCertificatesOnly)
		{
			if (this.handle == IntPtr.Zero)
			{
				throw new ObjectDisposedException("SecTrust");
			}
			return SecTrust.SecTrustSetAnchorCertificatesOnly(this.handle, anchorCertificatesOnly);
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SecStatusCode SecTrustSetVerifyDate(IntPtr trust, IntPtr date);

		public SecStatusCode SetVerifyDate(DateTime date)
		{
			SecStatusCode secStatusCode;
			using (CFDate cfdate = CFDate.Create(date))
			{
				secStatusCode = SecTrust.SecTrustSetVerifyDate(this.handle, cfdate.Handle);
			}
			return secStatusCode;
		}

		~SecTrust()
		{
			this.Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.handle != IntPtr.Zero)
			{
				CFObject.CFRelease(this.handle);
				this.handle = IntPtr.Zero;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public IntPtr Handle
		{
			get
			{
				return this.handle;
			}
		}

		private IntPtr handle;
	}
}
