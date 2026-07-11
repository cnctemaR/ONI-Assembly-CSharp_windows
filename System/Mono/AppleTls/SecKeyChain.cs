using System;
using System.Runtime.InteropServices;
using Mono.Net;
using ObjCRuntimeInternal;

namespace Mono.AppleTls
{
	internal class SecKeyChain : INativeObject, IDisposable
	{
		internal SecKeyChain(IntPtr handle, bool owns = false)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentException("Invalid handle");
			}
			this.handle = handle;
			if (!owns)
			{
				CFObject.CFRetain(handle);
			}
		}

		static SecKeyChain()
		{
			IntPtr intPtr = CFObject.dlopen("/System/Library/Frameworks/Security.framework/Security", 0);
			if (intPtr == IntPtr.Zero)
			{
				return;
			}
			try
			{
				SecKeyChain.MatchLimit = CFObject.GetIntPtr(intPtr, "kSecMatchLimit");
				SecKeyChain.MatchLimitAll = CFObject.GetIntPtr(intPtr, "kSecMatchLimitAll");
				SecKeyChain.MatchLimitOne = CFObject.GetIntPtr(intPtr, "kSecMatchLimitOne");
			}
			finally
			{
				CFObject.dlclose(intPtr);
			}
		}

		public static SecIdentity FindIdentity(SecCertificate certificate, bool throwOnError = false)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			SecIdentity secIdentity = SecKeyChain.FindIdentity((SecCertificate cert) => SecCertificate.Equals(certificate, cert));
			if (!throwOnError || secIdentity != null)
			{
				return secIdentity;
			}
			throw new InvalidOperationException(string.Format("Could not find SecIdentity for certificate '{0}' in keychain.", certificate.SubjectSummary));
		}

		private static SecIdentity FindIdentity(Predicate<SecCertificate> filter)
		{
			using (SecRecord secRecord = new SecRecord(SecKind.Identity))
			{
				SecStatusCode secStatusCode;
				INativeObject[] array = SecKeyChain.QueryAsReference(secRecord, -1, out secStatusCode);
				if (secStatusCode != SecStatusCode.Success || array == null)
				{
					return null;
				}
				foreach (SecIdentity secIdentity in array)
				{
					if (filter(secIdentity.Certificate))
					{
						return secIdentity;
					}
				}
			}
			return null;
		}

		private static INativeObject[] QueryAsReference(SecRecord query, int max, out SecStatusCode result)
		{
			if (query == null)
			{
				result = SecStatusCode.Param;
				return null;
			}
			INativeObject[] array;
			using (CFMutableDictionary cfmutableDictionary = query.QueryDict.MutableCopy())
			{
				cfmutableDictionary.SetValue(CFBoolean.True.Handle, SecItem.ReturnRef);
				SecKeyChain.SetLimit(cfmutableDictionary, max);
				array = SecKeyChain.QueryAsReference(cfmutableDictionary, out result);
			}
			return array;
		}

		private static INativeObject[] QueryAsReference(CFDictionary query, out SecStatusCode result)
		{
			if (query == null)
			{
				result = SecStatusCode.Param;
				return null;
			}
			IntPtr intPtr;
			result = SecItem.SecItemCopyMatching(query.Handle, out intPtr);
			if (result == SecStatusCode.Success && intPtr != IntPtr.Zero)
			{
				return CFArray.ArrayFromHandle<INativeObject>(intPtr, delegate(IntPtr p)
				{
					IntPtr typeID = CFType.GetTypeID(p);
					if (typeID == SecCertificate.GetTypeID())
					{
						return new SecCertificate(p, true);
					}
					if (typeID == SecKey.GetTypeID())
					{
						return new SecKey(p, true);
					}
					if (typeID == SecIdentity.GetTypeID())
					{
						return new SecIdentity(p, true);
					}
					throw new Exception(string.Format("Unexpected type: 0x{0:x}", typeID));
				});
			}
			return null;
		}

		internal static CFNumber SetLimit(CFMutableDictionary dict, int max)
		{
			CFNumber cfnumber = null;
			IntPtr intPtr;
			if (max == -1)
			{
				intPtr = SecKeyChain.MatchLimitAll;
			}
			else if (max == 1)
			{
				intPtr = SecKeyChain.MatchLimitOne;
			}
			else
			{
				cfnumber = CFNumber.FromInt32(max);
				intPtr = cfnumber.Handle;
			}
			dict.SetValue(intPtr, SecKeyChain.MatchLimit);
			return cfnumber;
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SecStatusCode SecKeychainCreate(IntPtr pathName, uint passwordLength, IntPtr password, bool promptUser, IntPtr initialAccess, out IntPtr keychain);

		internal static SecKeyChain Create(string pathName, string password)
		{
			IntPtr intPtr = Marshal.StringToHGlobalAnsi(pathName);
			IntPtr intPtr2 = Marshal.StringToHGlobalAnsi(password);
			IntPtr intPtr3;
			SecStatusCode secStatusCode = SecKeyChain.SecKeychainCreate(intPtr, (uint)password.Length, intPtr2, false, IntPtr.Zero, out intPtr3);
			if (secStatusCode != SecStatusCode.Success)
			{
				throw new InvalidOperationException(secStatusCode.ToString());
			}
			return new SecKeyChain(intPtr3, true);
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SecStatusCode SecKeychainOpen(IntPtr pathName, out IntPtr keychain);

		internal static SecKeyChain Open(string pathName)
		{
			IntPtr intPtr = IntPtr.Zero;
			SecKeyChain secKeyChain;
			try
			{
				intPtr = Marshal.StringToHGlobalAnsi(pathName);
				IntPtr intPtr2;
				SecStatusCode secStatusCode = SecKeyChain.SecKeychainOpen(intPtr, out intPtr2);
				if (secStatusCode != SecStatusCode.Success)
				{
					throw new InvalidOperationException(secStatusCode.ToString());
				}
				secKeyChain = new SecKeyChain(intPtr2, true);
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
			return secKeyChain;
		}

		internal static SecKeyChain OpenSystemRootCertificates()
		{
			return SecKeyChain.Open("/System/Library/Keychains/SystemRootCertificates.keychain");
		}

		~SecKeyChain()
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

		internal static readonly IntPtr MatchLimitAll;

		internal static readonly IntPtr MatchLimitOne;

		internal static readonly IntPtr MatchLimit;

		private IntPtr handle;
	}
}
