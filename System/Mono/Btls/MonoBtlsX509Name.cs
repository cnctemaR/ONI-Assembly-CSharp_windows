using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Mono.Btls
{
	internal class MonoBtlsX509Name : MonoBtlsObject
	{
		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_name_print_bio(IntPtr handle, IntPtr bio);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_name_print_string(IntPtr handle, IntPtr buffer, int size);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_name_get_raw_data(IntPtr handle, out IntPtr buffer, int use_canon_enc);

		[DllImport("libmono-btls-shared")]
		private static extern long mono_btls_x509_name_hash(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern long mono_btls_x509_name_hash_old(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_name_get_entry_count(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern MonoBtlsX509NameEntryType mono_btls_x509_name_get_entry_type(IntPtr name, int index);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_name_get_entry_oid(IntPtr name, int index, IntPtr buffer, int size);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_name_get_entry_oid_data(IntPtr name, int index, out IntPtr data);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_name_get_entry_value(IntPtr name, int index, out int tag, out IntPtr str);

		[DllImport("libmono-btls-shared")]
		private unsafe static extern IntPtr mono_btls_x509_name_from_data(void* data, int len, int use_canon_enc);

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_x509_name_free(IntPtr handle);

		internal new MonoBtlsX509Name.BoringX509NameHandle Handle
		{
			get
			{
				return (MonoBtlsX509Name.BoringX509NameHandle)base.Handle;
			}
		}

		internal MonoBtlsX509Name(MonoBtlsX509Name.BoringX509NameHandle handle)
			: base(handle)
		{
		}

		public string GetString()
		{
			IntPtr intPtr = Marshal.AllocHGlobal(4096);
			string text;
			try
			{
				int num = MonoBtlsX509Name.mono_btls_x509_name_print_string(this.Handle.DangerousGetHandle(), intPtr, 4096);
				base.CheckError(num, "GetString");
				text = Marshal.PtrToStringAnsi(intPtr);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return text;
		}

		public void PrintBio(MonoBtlsBio bio)
		{
			int num = MonoBtlsX509Name.mono_btls_x509_name_print_bio(this.Handle.DangerousGetHandle(), bio.Handle.DangerousGetHandle());
			base.CheckError(num, "PrintBio");
		}

		public byte[] GetRawData(bool use_canon_enc)
		{
			IntPtr intPtr;
			int num = MonoBtlsX509Name.mono_btls_x509_name_get_raw_data(this.Handle.DangerousGetHandle(), out intPtr, use_canon_enc ? 1 : 0);
			base.CheckError(num > 0, "GetRawData");
			byte[] array = new byte[num];
			Marshal.Copy(intPtr, array, 0, num);
			base.FreeDataPtr(intPtr);
			return array;
		}

		public long GetHash()
		{
			return MonoBtlsX509Name.mono_btls_x509_name_hash(this.Handle.DangerousGetHandle());
		}

		public long GetHashOld()
		{
			return MonoBtlsX509Name.mono_btls_x509_name_hash_old(this.Handle.DangerousGetHandle());
		}

		public int GetEntryCount()
		{
			return MonoBtlsX509Name.mono_btls_x509_name_get_entry_count(this.Handle.DangerousGetHandle());
		}

		public MonoBtlsX509NameEntryType GetEntryType(int index)
		{
			if (index >= this.GetEntryCount())
			{
				throw new ArgumentOutOfRangeException();
			}
			return MonoBtlsX509Name.mono_btls_x509_name_get_entry_type(this.Handle.DangerousGetHandle(), index);
		}

		public string GetEntryOid(int index)
		{
			if (index >= this.GetEntryCount())
			{
				throw new ArgumentOutOfRangeException();
			}
			IntPtr intPtr = Marshal.AllocHGlobal(4096);
			string text;
			try
			{
				int num = MonoBtlsX509Name.mono_btls_x509_name_get_entry_oid(this.Handle.DangerousGetHandle(), index, intPtr, 4096);
				base.CheckError(num > 0, "GetEntryOid");
				text = Marshal.PtrToStringAnsi(intPtr);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return text;
		}

		public byte[] GetEntryOidData(int index)
		{
			IntPtr intPtr;
			int num = MonoBtlsX509Name.mono_btls_x509_name_get_entry_oid_data(this.Handle.DangerousGetHandle(), index, out intPtr);
			base.CheckError(num > 0, "GetEntryOidData");
			byte[] array = new byte[num];
			Marshal.Copy(intPtr, array, 0, num);
			return array;
		}

		public unsafe string GetEntryValue(int index, out int tag)
		{
			if (index >= this.GetEntryCount())
			{
				throw new ArgumentOutOfRangeException();
			}
			IntPtr intPtr;
			int num = MonoBtlsX509Name.mono_btls_x509_name_get_entry_value(this.Handle.DangerousGetHandle(), index, out tag, out intPtr);
			if (num <= 0)
			{
				return null;
			}
			string @string;
			try
			{
				@string = new UTF8Encoding().GetString((byte*)(void*)intPtr, num);
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					base.FreeDataPtr(intPtr);
				}
			}
			return @string;
		}

		public unsafe static MonoBtlsX509Name CreateFromData(byte[] data, bool use_canon_enc)
		{
			void* ptr;
			if (data == null || data.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = (void*)(&data[0]);
			}
			IntPtr intPtr = MonoBtlsX509Name.mono_btls_x509_name_from_data(ptr, data.Length, use_canon_enc ? 1 : 0);
			if (intPtr == IntPtr.Zero)
			{
				throw new MonoBtlsException("mono_btls_x509_name_from_data() failed.");
			}
			return new MonoBtlsX509Name(new MonoBtlsX509Name.BoringX509NameHandle(intPtr, false));
		}

		internal class BoringX509NameHandle : MonoBtlsObject.MonoBtlsHandle
		{
			internal BoringX509NameHandle(IntPtr handle, bool ownsHandle)
				: base(handle, ownsHandle)
			{
				this.dontFree = !ownsHandle;
			}

			protected override bool ReleaseHandle()
			{
				if (!this.dontFree)
				{
					MonoBtlsX509Name.mono_btls_x509_name_free(this.handle);
				}
				return true;
			}

			private bool dontFree;
		}
	}
}
