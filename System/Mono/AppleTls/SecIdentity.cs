using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Mono.Net;
using ObjCRuntimeInternal;

namespace Mono.AppleTls
{
	internal class SecIdentity : INativeObject, IDisposable
	{
		static SecIdentity()
		{
			IntPtr intPtr = CFObject.dlopen("/System/Library/Frameworks/Security.framework/Security", 0);
			if (intPtr == IntPtr.Zero)
			{
				return;
			}
			try
			{
				SecIdentity.ImportExportPassphase = CFObject.GetStringConstant(intPtr, "kSecImportExportPassphrase");
				SecIdentity.ImportItemIdentity = CFObject.GetStringConstant(intPtr, "kSecImportItemIdentity");
				SecIdentity.ImportExportAccess = CFObject.GetStringConstant(intPtr, "kSecImportExportAccess");
				SecIdentity.ImportExportKeychain = CFObject.GetStringConstant(intPtr, "kSecImportExportKeychain");
			}
			finally
			{
				CFObject.dlclose(intPtr);
			}
		}

		internal SecIdentity(IntPtr handle, bool owns = false)
		{
			this.handle = handle;
			if (!owns)
			{
				CFObject.CFRetain(handle);
			}
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security", EntryPoint = "SecIdentityGetTypeID")]
		public static extern IntPtr GetTypeID();

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern SecStatusCode SecIdentityCopyCertificate(IntPtr identityRef, out IntPtr certificateRef);

		public SecCertificate Certificate
		{
			get
			{
				if (this.handle == IntPtr.Zero)
				{
					throw new ObjectDisposedException("SecIdentity");
				}
				IntPtr intPtr;
				SecStatusCode secStatusCode = SecIdentity.SecIdentityCopyCertificate(this.handle, out intPtr);
				if (secStatusCode != SecStatusCode.Success)
				{
					throw new InvalidOperationException(secStatusCode.ToString());
				}
				return new SecCertificate(intPtr, true);
			}
		}

		private static CFDictionary CreateImportOptions(CFString password, SecIdentity.ImportOptions options = null)
		{
			if (options == null)
			{
				return CFDictionary.FromObjectAndKey(password.Handle, SecIdentity.ImportExportPassphase.Handle);
			}
			List<Tuple<IntPtr, IntPtr>> list = new List<Tuple<IntPtr, IntPtr>>();
			list.Add(new Tuple<IntPtr, IntPtr>(SecIdentity.ImportExportPassphase.Handle, password.Handle));
			if (options.KeyChain != null)
			{
				list.Add(new Tuple<IntPtr, IntPtr>(SecIdentity.ImportExportKeychain.Handle, options.KeyChain.Handle));
			}
			if (options.Access != null)
			{
				list.Add(new Tuple<IntPtr, IntPtr>(SecIdentity.ImportExportAccess.Handle, options.Access.Handle));
			}
			return CFDictionary.FromKeysAndObjects(list);
		}

		public static SecIdentity Import(byte[] data, string password, SecIdentity.ImportOptions options = null)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (string.IsNullOrEmpty(password))
			{
				throw new ArgumentException("password");
			}
			SecIdentity secIdentity;
			using (CFString cfstring = CFString.Create(password))
			{
				using (CFDictionary cfdictionary = SecIdentity.CreateImportOptions(cfstring, options))
				{
					CFDictionary[] array;
					SecStatusCode secStatusCode = SecImportExport.ImportPkcs12(data, cfdictionary, out array);
					if (secStatusCode != SecStatusCode.Success)
					{
						throw new InvalidOperationException(secStatusCode.ToString());
					}
					secIdentity = new SecIdentity(array[0].GetValue(SecIdentity.ImportItemIdentity.Handle), false);
				}
			}
			return secIdentity;
		}

		public static SecIdentity Import(X509Certificate2 certificate, SecIdentity.ImportOptions options = null)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			if (!certificate.HasPrivateKey)
			{
				throw new InvalidOperationException("Need X509Certificate2 with a private key.");
			}
			string text = Guid.NewGuid().ToString();
			return SecIdentity.Import(certificate.Export(X509ContentType.Pfx, text), text, options);
		}

		~SecIdentity()
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

		private static readonly CFString ImportExportPassphase;

		private static readonly CFString ImportItemIdentity;

		private static readonly CFString ImportExportAccess;

		private static readonly CFString ImportExportKeychain;

		internal IntPtr handle;

		internal class ImportOptions
		{
			public SecAccess Access { get; set; }

			public SecKeyChain KeyChain { get; set; }
		}
	}
}
