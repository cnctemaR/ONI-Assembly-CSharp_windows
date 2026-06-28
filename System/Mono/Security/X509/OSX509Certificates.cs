using System;
using System.Runtime.InteropServices;

namespace Mono.Security.X509
{
	internal class OSX509Certificates
	{
		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern IntPtr SecCertificateCreateWithData(IntPtr allocator, IntPtr nsdataRef);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern int SecTrustCreateWithCertificates(IntPtr certOrCertArray, IntPtr policies, out IntPtr sectrustref);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern IntPtr SecPolicyCreateSSL(int server, IntPtr cfStringHostname);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern int SecTrustEvaluate(IntPtr secTrustRef, out OSX509Certificates.SecTrustResult secTrustResultTime);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private unsafe static extern IntPtr CFDataCreate(IntPtr allocator, byte* bytes, IntPtr length);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern void CFRelease(IntPtr handle);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFArrayCreate(IntPtr allocator, IntPtr values, IntPtr numValues, IntPtr callbacks);

		private unsafe static IntPtr MakeCFData(byte[] data)
		{
			int num = 0;
			return OSX509Certificates.CFDataCreate(IntPtr.Zero, &data[num], (IntPtr)data.Length);
		}

		private unsafe static IntPtr FromIntPtrs(IntPtr[] values)
		{
			fixed (IntPtr* ptr = (ref values != null && values.Length != 0 ? ref values[0] : ref *null))
			{
				return OSX509Certificates.CFArrayCreate(IntPtr.Zero, (IntPtr)((void*)ptr), (IntPtr)values.Length, IntPtr.Zero);
			}
		}

		public static OSX509Certificates.SecTrustResult TrustEvaluateSsl(X509CertificateCollection certificates)
		{
			OSX509Certificates.SecTrustResult secTrustResult;
			try
			{
				secTrustResult = OSX509Certificates._TrustEvaluateSsl(certificates);
			}
			catch
			{
				secTrustResult = OSX509Certificates.SecTrustResult.Deny;
			}
			return secTrustResult;
		}

		private static OSX509Certificates.SecTrustResult _TrustEvaluateSsl(X509CertificateCollection certificates)
		{
			if (certificates == null)
			{
				throw new ArgumentNullException("certificates");
			}
			int count = certificates.Count;
			IntPtr[] array = new IntPtr[count];
			IntPtr[] array2 = new IntPtr[count];
			IntPtr intPtr = IntPtr.Zero;
			OSX509Certificates.SecTrustResult secTrustResult2;
			try
			{
				for (int i = 0; i < count; i++)
				{
					array[i] = OSX509Certificates.MakeCFData(certificates[i].RawData);
				}
				for (int j = 0; j < count; j++)
				{
					array2[j] = OSX509Certificates.SecCertificateCreateWithData(IntPtr.Zero, array[j]);
					if (array2[j] == IntPtr.Zero)
					{
						return OSX509Certificates.SecTrustResult.Deny;
					}
				}
				intPtr = OSX509Certificates.FromIntPtrs(array2);
				IntPtr intPtr2;
				if (OSX509Certificates.SecTrustCreateWithCertificates(intPtr, OSX509Certificates.sslsecpolicy, out intPtr2) == 0)
				{
					OSX509Certificates.SecTrustResult secTrustResult;
					int num = OSX509Certificates.SecTrustEvaluate(intPtr2, out secTrustResult);
					if (num != 0)
					{
						secTrustResult2 = OSX509Certificates.SecTrustResult.Deny;
					}
					else
					{
						OSX509Certificates.CFRelease(intPtr2);
						secTrustResult2 = secTrustResult;
					}
				}
				else
				{
					secTrustResult2 = OSX509Certificates.SecTrustResult.Deny;
				}
			}
			finally
			{
				for (int k = 0; k < count; k++)
				{
					if (array[k] != IntPtr.Zero)
					{
						OSX509Certificates.CFRelease(array[k]);
					}
				}
				if (intPtr != IntPtr.Zero)
				{
					OSX509Certificates.CFRelease(intPtr);
				}
				else
				{
					for (int l = 0; l < count; l++)
					{
						if (array2[l] != IntPtr.Zero)
						{
							OSX509Certificates.CFRelease(array2[l]);
						}
					}
				}
			}
			return secTrustResult2;
		}

		public const string SecurityLibrary = "/System/Library/Frameworks/Security.framework/Security";

		public const string CoreFoundationLibrary = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

		private static IntPtr sslsecpolicy = OSX509Certificates.SecPolicyCreateSSL(0, IntPtr.Zero);

		public enum SecTrustResult
		{
			Invalid,
			Proceed,
			Confirm,
			Deny,
			Unspecified,
			RecoverableTrustFailure,
			FatalTrustFailure,
			ResultOtherError
		}
	}
}
