using System;
using Mono.Net;

namespace Mono.AppleTls
{
	internal static class SecClass
	{
		static SecClass()
		{
			IntPtr intPtr = CFObject.dlopen("/System/Library/Frameworks/Security.framework/Security", 0);
			if (intPtr == IntPtr.Zero)
			{
				return;
			}
			try
			{
				SecClass.Identity = CFObject.GetIntPtr(intPtr, "kSecClassIdentity");
				SecClass.Certificate = CFObject.GetIntPtr(intPtr, "kSecClassCertificate");
			}
			finally
			{
				CFObject.dlclose(intPtr);
			}
		}

		public static IntPtr FromSecKind(SecKind secKind)
		{
			if (secKind == SecKind.Identity)
			{
				return SecClass.Identity;
			}
			if (secKind != SecKind.Certificate)
			{
				throw new ArgumentException("secKind");
			}
			return SecClass.Certificate;
		}

		public static readonly IntPtr Identity;

		public static readonly IntPtr Certificate;
	}
}
