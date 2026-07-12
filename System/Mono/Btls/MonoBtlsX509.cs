using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Mono.Btls
{
	internal class MonoBtlsX509 : MonoBtlsObject
	{
		internal new MonoBtlsX509.BoringX509Handle Handle
		{
			get
			{
				return (MonoBtlsX509.BoringX509Handle)base.Handle;
			}
		}

		internal MonoBtlsX509(MonoBtlsX509.BoringX509Handle handle)
			: base(handle)
		{
		}

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_up_ref(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_from_data(IntPtr data, int len, MonoBtlsX509Format format);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_get_subject_name(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_get_issuer_name(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_get_subject_name_string(IntPtr handle, IntPtr buffer, int size);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_get_issuer_name_string(IntPtr handle, IntPtr buffer, int size);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_get_raw_data(IntPtr handle, IntPtr bio, MonoBtlsX509Format format);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_cmp(IntPtr a, IntPtr b);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_get_hash(IntPtr handle, out IntPtr data);

		[DllImport("libmono-btls-shared")]
		private static extern long mono_btls_x509_get_not_before(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern long mono_btls_x509_get_not_after(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_get_public_key(IntPtr handle, IntPtr bio);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_get_serial_number(IntPtr handle, IntPtr data, int size, int mono_style);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_get_version(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_get_signature_algorithm(IntPtr handle, IntPtr buffer, int size);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_get_public_key_asn1(IntPtr handle, IntPtr oid, int oid_size, out IntPtr data, out int size);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_get_public_key_parameters(IntPtr handle, IntPtr oid, int oid_size, out IntPtr data, out int size);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_get_pubkey(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_get_subject_key_identifier(IntPtr handle, out IntPtr data, out int size);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_print(IntPtr handle, IntPtr bio);

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_x509_free(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_dup(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_add_trust_object(IntPtr handle, MonoBtlsX509Purpose purpose);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_add_reject_object(IntPtr handle, MonoBtlsX509Purpose purpose);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_add_explicit_trust(IntPtr handle, MonoBtlsX509TrustKind kind);

		internal MonoBtlsX509 Copy()
		{
			IntPtr intPtr = MonoBtlsX509.mono_btls_x509_up_ref(this.Handle.DangerousGetHandle());
			base.CheckError(intPtr != IntPtr.Zero, "Copy");
			return new MonoBtlsX509(new MonoBtlsX509.BoringX509Handle(intPtr));
		}

		internal MonoBtlsX509 Duplicate()
		{
			IntPtr intPtr = MonoBtlsX509.mono_btls_x509_dup(this.Handle.DangerousGetHandle());
			base.CheckError(intPtr != IntPtr.Zero, "Duplicate");
			return new MonoBtlsX509(new MonoBtlsX509.BoringX509Handle(intPtr));
		}

		public static MonoBtlsX509 LoadFromData(byte[] buffer, MonoBtlsX509Format format)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(buffer.Length);
			if (intPtr == IntPtr.Zero)
			{
				throw new OutOfMemoryException();
			}
			MonoBtlsX509 monoBtlsX;
			try
			{
				Marshal.Copy(buffer, 0, intPtr, buffer.Length);
				IntPtr intPtr2 = MonoBtlsX509.mono_btls_x509_from_data(intPtr, buffer.Length, format);
				if (intPtr2 == IntPtr.Zero)
				{
					throw new MonoBtlsException("Failed to read certificate from data.");
				}
				monoBtlsX = new MonoBtlsX509(new MonoBtlsX509.BoringX509Handle(intPtr2));
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return monoBtlsX;
		}

		public MonoBtlsX509Name GetSubjectName()
		{
			IntPtr intPtr = MonoBtlsX509.mono_btls_x509_get_subject_name(this.Handle.DangerousGetHandle());
			base.CheckError(intPtr != IntPtr.Zero, "GetSubjectName");
			return new MonoBtlsX509Name(new MonoBtlsX509Name.BoringX509NameHandle(intPtr, false));
		}

		public string GetSubjectNameString()
		{
			IntPtr intPtr = Marshal.AllocHGlobal(4096);
			string text;
			try
			{
				int num = MonoBtlsX509.mono_btls_x509_get_subject_name_string(this.Handle.DangerousGetHandle(), intPtr, 4096);
				base.CheckError(num, "GetSubjectNameString");
				text = Marshal.PtrToStringAnsi(intPtr);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return text;
		}

		public long GetSubjectNameHash()
		{
			base.CheckThrow();
			long hash;
			using (MonoBtlsX509Name subjectName = this.GetSubjectName())
			{
				hash = subjectName.GetHash();
			}
			return hash;
		}

		public MonoBtlsX509Name GetIssuerName()
		{
			IntPtr intPtr = MonoBtlsX509.mono_btls_x509_get_issuer_name(this.Handle.DangerousGetHandle());
			base.CheckError(intPtr != IntPtr.Zero, "GetIssuerName");
			return new MonoBtlsX509Name(new MonoBtlsX509Name.BoringX509NameHandle(intPtr, false));
		}

		public string GetIssuerNameString()
		{
			IntPtr intPtr = Marshal.AllocHGlobal(4096);
			string text;
			try
			{
				int num = MonoBtlsX509.mono_btls_x509_get_issuer_name_string(this.Handle.DangerousGetHandle(), intPtr, 4096);
				base.CheckError(num, "GetIssuerNameString");
				text = Marshal.PtrToStringAnsi(intPtr);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return text;
		}

		public byte[] GetRawData(MonoBtlsX509Format format)
		{
			byte[] data;
			using (MonoBtlsBioMemory monoBtlsBioMemory = new MonoBtlsBioMemory())
			{
				int num = MonoBtlsX509.mono_btls_x509_get_raw_data(this.Handle.DangerousGetHandle(), monoBtlsBioMemory.Handle.DangerousGetHandle(), format);
				base.CheckError(num, "GetRawData");
				data = monoBtlsBioMemory.GetData();
			}
			return data;
		}

		public void GetRawData(MonoBtlsBio bio, MonoBtlsX509Format format)
		{
			base.CheckThrow();
			int num = MonoBtlsX509.mono_btls_x509_get_raw_data(this.Handle.DangerousGetHandle(), bio.Handle.DangerousGetHandle(), format);
			base.CheckError(num, "GetRawData");
		}

		public static int Compare(MonoBtlsX509 a, MonoBtlsX509 b)
		{
			return MonoBtlsX509.mono_btls_x509_cmp(a.Handle.DangerousGetHandle(), b.Handle.DangerousGetHandle());
		}

		public byte[] GetCertHash()
		{
			IntPtr intPtr;
			int num = MonoBtlsX509.mono_btls_x509_get_hash(this.Handle.DangerousGetHandle(), out intPtr);
			base.CheckError(num > 0, "GetCertHash");
			byte[] array = new byte[num];
			Marshal.Copy(intPtr, array, 0, num);
			return array;
		}

		public DateTime GetNotBefore()
		{
			long num = MonoBtlsX509.mono_btls_x509_get_not_before(this.Handle.DangerousGetHandle());
			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double)num);
		}

		public DateTime GetNotAfter()
		{
			long num = MonoBtlsX509.mono_btls_x509_get_not_after(this.Handle.DangerousGetHandle());
			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double)num);
		}

		public byte[] GetPublicKeyData()
		{
			byte[] data;
			using (MonoBtlsBioMemory monoBtlsBioMemory = new MonoBtlsBioMemory())
			{
				int num = MonoBtlsX509.mono_btls_x509_get_public_key(this.Handle.DangerousGetHandle(), monoBtlsBioMemory.Handle.DangerousGetHandle());
				base.CheckError(num > 0, "GetPublicKeyData");
				data = monoBtlsBioMemory.GetData();
			}
			return data;
		}

		public byte[] GetSerialNumber(bool mono_style)
		{
			int num = 256;
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			byte[] array2;
			try
			{
				int num2 = MonoBtlsX509.mono_btls_x509_get_serial_number(this.Handle.DangerousGetHandle(), intPtr, num, mono_style ? 1 : 0);
				base.CheckError(num2 > 0, "GetSerialNumber");
				byte[] array = new byte[num2];
				Marshal.Copy(intPtr, array, 0, num2);
				array2 = array;
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
			return array2;
		}

		public int GetVersion()
		{
			return MonoBtlsX509.mono_btls_x509_get_version(this.Handle.DangerousGetHandle());
		}

		public string GetSignatureAlgorithm()
		{
			int num = 256;
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			string text;
			try
			{
				int num2 = MonoBtlsX509.mono_btls_x509_get_signature_algorithm(this.Handle.DangerousGetHandle(), intPtr, num);
				base.CheckError(num2 > 0, "GetSignatureAlgorithm");
				text = Marshal.PtrToStringAnsi(intPtr);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return text;
		}

		public AsnEncodedData GetPublicKeyAsn1()
		{
			int num = 256;
			IntPtr intPtr = Marshal.AllocHGlobal(256);
			IntPtr intPtr2;
			int num3;
			string text;
			try
			{
				int num2 = MonoBtlsX509.mono_btls_x509_get_public_key_asn1(this.Handle.DangerousGetHandle(), intPtr, num, out intPtr2, out num3);
				base.CheckError(num2, "GetPublicKeyAsn1");
				text = Marshal.PtrToStringAnsi(intPtr);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			AsnEncodedData asnEncodedData;
			try
			{
				byte[] array = new byte[num3];
				Marshal.Copy(intPtr2, array, 0, num3);
				asnEncodedData = new AsnEncodedData(text.ToString(), array);
			}
			finally
			{
				if (intPtr2 != IntPtr.Zero)
				{
					base.FreeDataPtr(intPtr2);
				}
			}
			return asnEncodedData;
		}

		public AsnEncodedData GetPublicKeyParameters()
		{
			int num = 256;
			IntPtr intPtr = Marshal.AllocHGlobal(256);
			IntPtr intPtr2;
			int num3;
			string text;
			try
			{
				int num2 = MonoBtlsX509.mono_btls_x509_get_public_key_parameters(this.Handle.DangerousGetHandle(), intPtr, num, out intPtr2, out num3);
				base.CheckError(num2, "GetPublicKeyParameters");
				text = Marshal.PtrToStringAnsi(intPtr);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			AsnEncodedData asnEncodedData;
			try
			{
				byte[] array = new byte[num3];
				Marshal.Copy(intPtr2, array, 0, num3);
				asnEncodedData = new AsnEncodedData(text.ToString(), array);
			}
			finally
			{
				if (intPtr2 != IntPtr.Zero)
				{
					base.FreeDataPtr(intPtr2);
				}
			}
			return asnEncodedData;
		}

		public byte[] GetSubjectKeyIdentifier()
		{
			IntPtr zero = IntPtr.Zero;
			byte[] array2;
			try
			{
				int num2;
				int num = MonoBtlsX509.mono_btls_x509_get_subject_key_identifier(this.Handle.DangerousGetHandle(), out zero, out num2);
				base.CheckError(num, "GetSubjectKeyIdentifier");
				byte[] array = new byte[num2];
				Marshal.Copy(zero, array, 0, num2);
				array2 = array;
			}
			finally
			{
				if (zero != IntPtr.Zero)
				{
					base.FreeDataPtr(zero);
				}
			}
			return array2;
		}

		public MonoBtlsKey GetPublicKey()
		{
			IntPtr intPtr = MonoBtlsX509.mono_btls_x509_get_pubkey(this.Handle.DangerousGetHandle());
			base.CheckError(intPtr != IntPtr.Zero, "GetPublicKey");
			return new MonoBtlsKey(new MonoBtlsKey.BoringKeyHandle(intPtr));
		}

		public void Print(MonoBtlsBio bio)
		{
			int num = MonoBtlsX509.mono_btls_x509_print(this.Handle.DangerousGetHandle(), bio.Handle.DangerousGetHandle());
			base.CheckError(num, "Print");
		}

		public void ExportAsPEM(MonoBtlsBio bio, bool includeHumanReadableForm)
		{
			this.GetRawData(bio, MonoBtlsX509Format.PEM);
			if (!includeHumanReadableForm)
			{
				return;
			}
			this.Print(bio);
			byte[] certHash = this.GetCertHash();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SHA1 Fingerprint=");
			for (int i = 0; i < certHash.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(":");
				}
				stringBuilder.AppendFormat("{0:X2}", certHash[i]);
			}
			stringBuilder.AppendLine();
			byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
			bio.Write(bytes, 0, bytes.Length);
		}

		public void AddTrustObject(MonoBtlsX509Purpose purpose)
		{
			base.CheckThrow();
			int num = MonoBtlsX509.mono_btls_x509_add_trust_object(this.Handle.DangerousGetHandle(), purpose);
			base.CheckError(num, "AddTrustObject");
		}

		public void AddRejectObject(MonoBtlsX509Purpose purpose)
		{
			base.CheckThrow();
			int num = MonoBtlsX509.mono_btls_x509_add_reject_object(this.Handle.DangerousGetHandle(), purpose);
			base.CheckError(num, "AddRejectObject");
		}

		public void AddExplicitTrust(MonoBtlsX509TrustKind kind)
		{
			base.CheckThrow();
			int num = MonoBtlsX509.mono_btls_x509_add_explicit_trust(this.Handle.DangerousGetHandle(), kind);
			base.CheckError(num, "AddExplicitTrust");
		}

		internal class BoringX509Handle : MonoBtlsObject.MonoBtlsHandle
		{
			public BoringX509Handle(IntPtr handle)
				: base(handle, true)
			{
			}

			protected override bool ReleaseHandle()
			{
				if (this.handle != IntPtr.Zero)
				{
					MonoBtlsX509.mono_btls_x509_free(this.handle);
				}
				return true;
			}

			public IntPtr StealHandle()
			{
				return Interlocked.Exchange(ref this.handle, IntPtr.Zero);
			}
		}
	}
}
