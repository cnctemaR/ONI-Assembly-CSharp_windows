using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Mono.Net;
using Mono.Security.Cryptography;

namespace Mono.AppleTls
{
	internal class SecImportExport
	{
		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SecStatusCode SecPKCS12Import(IntPtr pkcs12_data, IntPtr options, out IntPtr items);

		public static SecStatusCode ImportPkcs12(byte[] buffer, CFDictionary options, out CFDictionary[] array)
		{
			SecStatusCode secStatusCode;
			using (CFData cfdata = CFData.FromData(buffer))
			{
				secStatusCode = SecImportExport.ImportPkcs12(cfdata, options, out array);
			}
			return secStatusCode;
		}

		public static SecStatusCode ImportPkcs12(CFData data, CFDictionary options, out CFDictionary[] array)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			IntPtr intPtr;
			SecStatusCode secStatusCode = SecImportExport.SecPKCS12Import(data.Handle, options.Handle, out intPtr);
			array = CFArray.ArrayFromHandle<CFDictionary>(intPtr, (IntPtr h) => new CFDictionary(h, false));
			if (intPtr != IntPtr.Zero)
			{
				CFObject.CFRelease(intPtr);
			}
			return secStatusCode;
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SecStatusCode SecItemImport(IntPtr importedData, IntPtr fileNameOrExtension, ref SecImportExport.SecExternalFormat inputFormat, ref SecImportExport.SecExternalItemType itemType, SecImportExport.SecItemImportExportFlags flags, IntPtr keyParams, IntPtr importKeychain, out IntPtr outItems);

		public static CFArray ItemImport(byte[] buffer, string password)
		{
			CFArray cfarray;
			using (CFData cfdata = CFData.FromData(buffer))
			{
				using (CFString cfstring = CFString.Create(password))
				{
					cfarray = SecImportExport.ItemImport(cfdata, SecImportExport.SecExternalFormat.PKCS12, SecImportExport.SecExternalItemType.Aggregate, SecImportExport.SecItemImportExportFlags.None, new SecImportExport.SecItemImportExportKeyParameters?(new SecImportExport.SecItemImportExportKeyParameters
					{
						passphrase = cfstring.Handle
					}));
				}
			}
			return cfarray;
		}

		private static CFArray ItemImport(CFData data, SecImportExport.SecExternalFormat format, SecImportExport.SecExternalItemType itemType, SecImportExport.SecItemImportExportFlags flags = SecImportExport.SecItemImportExportFlags.None, SecImportExport.SecItemImportExportKeyParameters? keyParams = null)
		{
			return SecImportExport.ItemImport(data, ref format, ref itemType, flags, keyParams);
		}

		private static CFArray ItemImport(CFData data, ref SecImportExport.SecExternalFormat format, ref SecImportExport.SecExternalItemType itemType, SecImportExport.SecItemImportExportFlags flags = SecImportExport.SecItemImportExportFlags.None, SecImportExport.SecItemImportExportKeyParameters? keyParams = null)
		{
			IntPtr intPtr = IntPtr.Zero;
			if (keyParams != null)
			{
				intPtr = Marshal.AllocHGlobal(Marshal.SizeOf<SecImportExport.SecItemImportExportKeyParameters>(keyParams.Value));
				if (intPtr == IntPtr.Zero)
				{
					throw new OutOfMemoryException();
				}
				Marshal.StructureToPtr<SecImportExport.SecItemImportExportKeyParameters>(keyParams.Value, intPtr, false);
			}
			IntPtr intPtr2;
			SecStatusCode secStatusCode = SecImportExport.SecItemImport(data.Handle, IntPtr.Zero, ref format, ref itemType, flags, intPtr, IntPtr.Zero, out intPtr2);
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
			if (secStatusCode != SecStatusCode.Success)
			{
				throw new NotSupportedException(secStatusCode.ToString());
			}
			return new CFArray(intPtr2, true);
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern IntPtr SecIdentityCreate(IntPtr allocator, IntPtr certificate, IntPtr privateKey);

		public static SecIdentity ItemImport(X509Certificate2 certificate)
		{
			if (!certificate.HasPrivateKey)
			{
				throw new NotSupportedException();
			}
			SecIdentity secIdentity;
			using (SecKey secKey = SecImportExport.ImportPrivateKey(certificate))
			{
				using (SecCertificate secCertificate = new SecCertificate(certificate))
				{
					IntPtr intPtr = SecImportExport.SecIdentityCreate(IntPtr.Zero, secCertificate.Handle, secKey.Handle);
					if (CFType.GetTypeID(intPtr) != SecIdentity.GetTypeID())
					{
						throw new InvalidOperationException();
					}
					secIdentity = new SecIdentity(intPtr, true);
				}
			}
			return secIdentity;
		}

		private static byte[] ExportKey(RSA key)
		{
			return Mono.Security.Cryptography.PKCS8.PrivateKeyInfo.Encode(key);
		}

		private static SecKey ImportPrivateKey(X509Certificate2 certificate)
		{
			if (!certificate.HasPrivateKey)
			{
				throw new NotSupportedException();
			}
			CFArray cfarray;
			using (CFData cfdata = CFData.FromData(SecImportExport.ExportKey((RSA)certificate.PrivateKey)))
			{
				cfarray = SecImportExport.ItemImport(cfdata, SecImportExport.SecExternalFormat.OpenSSL, SecImportExport.SecExternalItemType.PrivateKey, SecImportExport.SecItemImportExportFlags.None, null);
			}
			SecKey secKey;
			try
			{
				if (cfarray.Count != 1)
				{
					throw new InvalidOperationException("Private key import failed.");
				}
				IntPtr intPtr = cfarray[0];
				if (CFType.GetTypeID(intPtr) != SecKey.GetTypeID())
				{
					throw new InvalidOperationException("Private key import doesn't return SecKey.");
				}
				secKey = new SecKey(intPtr, cfarray.Handle);
			}
			finally
			{
				cfarray.Dispose();
			}
			return secKey;
		}

		private const int SEC_KEY_IMPORT_EXPORT_PARAMS_VERSION = 0;

		private enum SecExternalFormat
		{
			Unknown,
			OpenSSL,
			X509Cert = 9,
			PEMSequence,
			PKCS7,
			PKCS12
		}

		private enum SecExternalItemType
		{
			Unknown,
			PrivateKey,
			PublicKey,
			SessionKey,
			Certificate,
			Aggregate
		}

		private enum SecItemImportExportFlags
		{
			None,
			PemArmour
		}

		private struct SecItemImportExportKeyParameters
		{
			public int version;

			public int flags;

			public IntPtr passphrase;

			private IntPtr alertTitle;

			private IntPtr alertPrompt;

			public IntPtr accessRef;

			private IntPtr keyUsage;

			private IntPtr keyAttributes;
		}
	}
}
