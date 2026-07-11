using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Mono.Net;
using ObjCRuntimeInternal;

namespace Mono.AppleTls
{
	internal class SecCertificate : INativeObject, IDisposable
	{
		internal SecCertificate(IntPtr handle, bool owns = false)
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

		[DllImport("/System/Library/Frameworks/Security.framework/Security", EntryPoint = "SecCertificateGetTypeID")]
		public static extern IntPtr GetTypeID();

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern IntPtr SecCertificateCreateWithData(IntPtr allocator, IntPtr cfData);

		public SecCertificate(X509Certificate certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			this.handle = certificate.Impl.GetNativeAppleCertificate();
			if (this.handle != IntPtr.Zero)
			{
				CFObject.CFRetain(this.handle);
				return;
			}
			using (CFData cfdata = CFData.FromData(certificate.GetRawCertData()))
			{
				this.Initialize(cfdata);
			}
		}

		internal SecCertificate(X509CertificateImpl impl)
		{
			this.handle = impl.GetNativeAppleCertificate();
			if (this.handle != IntPtr.Zero)
			{
				CFObject.CFRetain(this.handle);
				return;
			}
			using (CFData cfdata = CFData.FromData(impl.GetRawCertData()))
			{
				this.Initialize(cfdata);
			}
		}

		private void Initialize(CFData data)
		{
			this.handle = SecCertificate.SecCertificateCreateWithData(IntPtr.Zero, data.Handle);
			if (this.handle == IntPtr.Zero)
			{
				throw new ArgumentException("Not a valid DER-encoded X.509 certificate");
			}
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern IntPtr SecCertificateCopySubjectSummary(IntPtr cert);

		public string SubjectSummary
		{
			get
			{
				if (this.handle == IntPtr.Zero)
				{
					throw new ObjectDisposedException("SecCertificate");
				}
				IntPtr intPtr = IntPtr.Zero;
				string text;
				try
				{
					intPtr = SecCertificate.SecCertificateCopySubjectSummary(this.handle);
					text = CFString.AsString(intPtr);
				}
				finally
				{
					if (intPtr != IntPtr.Zero)
					{
						CFObject.CFRelease(intPtr);
					}
				}
				return text;
			}
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern IntPtr SecCertificateCopyData(IntPtr cert);

		public CFData DerData
		{
			get
			{
				if (this.handle == IntPtr.Zero)
				{
					throw new ObjectDisposedException("SecCertificate");
				}
				IntPtr intPtr = SecCertificate.SecCertificateCopyData(this.handle);
				if (intPtr == IntPtr.Zero)
				{
					throw new ArgumentException("Not a valid certificate");
				}
				return new CFData(intPtr, true);
			}
		}

		public X509Certificate ToX509Certificate()
		{
			if (this.handle == IntPtr.Zero)
			{
				throw new ObjectDisposedException("SecCertificate");
			}
			return new X509Certificate(this.handle);
		}

		internal static bool Equals(SecCertificate first, SecCertificate second)
		{
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			if (second == null)
			{
				throw new ArgumentNullException("second");
			}
			if (first.Handle == second.Handle)
			{
				return true;
			}
			bool flag;
			using (CFData derData = first.DerData)
			{
				using (CFData derData2 = second.DerData)
				{
					if (derData.Handle == derData2.Handle)
					{
						flag = true;
					}
					else if (derData.Length != derData2.Length)
					{
						flag = false;
					}
					else
					{
						IntPtr length = derData.Length;
						for (long num = 0L; num < (long)length; num += 1L)
						{
							if (derData[num] != derData2[num])
							{
								return false;
							}
						}
						flag = true;
					}
				}
			}
			return flag;
		}

		~SecCertificate()
		{
			this.Dispose(false);
		}

		public IntPtr Handle
		{
			get
			{
				return this.handle;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.handle != IntPtr.Zero)
			{
				CFObject.CFRelease(this.handle);
				this.handle = IntPtr.Zero;
			}
		}

		internal IntPtr handle;
	}
}
