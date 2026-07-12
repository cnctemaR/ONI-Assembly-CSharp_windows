using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Mono.Security.Cryptography;

namespace Mono.Btls
{
	internal class MonoBtlsKey : MonoBtlsObject
	{
		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_key_new();

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_key_free(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_key_up_ref(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_key_get_bytes(IntPtr handle, out IntPtr data, out int size, int include_private_bits);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_key_get_bits(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_key_is_rsa(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_key_assign_rsa_private_key(IntPtr handle, byte[] der, int der_length);

		internal new MonoBtlsKey.BoringKeyHandle Handle
		{
			get
			{
				return (MonoBtlsKey.BoringKeyHandle)base.Handle;
			}
		}

		internal MonoBtlsKey(MonoBtlsKey.BoringKeyHandle handle)
			: base(handle)
		{
		}

		public byte[] GetBytes(bool include_private_bits)
		{
			IntPtr intPtr;
			int num2;
			int num = MonoBtlsKey.mono_btls_key_get_bytes(this.Handle.DangerousGetHandle(), out intPtr, out num2, include_private_bits ? 1 : 0);
			base.CheckError(num, "GetBytes");
			byte[] array = new byte[num2];
			Marshal.Copy(intPtr, array, 0, num2);
			base.FreeDataPtr(intPtr);
			return array;
		}

		public bool IsRsa
		{
			get
			{
				return MonoBtlsKey.mono_btls_key_is_rsa(this.Handle.DangerousGetHandle()) != 0;
			}
		}

		public MonoBtlsKey Copy()
		{
			base.CheckThrow();
			IntPtr intPtr = MonoBtlsKey.mono_btls_key_up_ref(this.Handle.DangerousGetHandle());
			base.CheckError(intPtr != IntPtr.Zero, "Copy");
			return new MonoBtlsKey(new MonoBtlsKey.BoringKeyHandle(intPtr));
		}

		public static MonoBtlsKey CreateFromRSAPrivateKey(RSA privateKey)
		{
			byte[] array = PKCS8.PrivateKeyInfo.Encode(privateKey);
			MonoBtlsKey monoBtlsKey = new MonoBtlsKey(new MonoBtlsKey.BoringKeyHandle(MonoBtlsKey.mono_btls_key_new()));
			if (MonoBtlsKey.mono_btls_key_assign_rsa_private_key(monoBtlsKey.Handle.DangerousGetHandle(), array, array.Length) == 0)
			{
				throw new MonoBtlsException("Assigning private key failed.");
			}
			return monoBtlsKey;
		}

		internal class BoringKeyHandle : MonoBtlsObject.MonoBtlsHandle
		{
			internal BoringKeyHandle(IntPtr handle)
				: base(handle, true)
			{
			}

			protected override bool ReleaseHandle()
			{
				MonoBtlsKey.mono_btls_key_free(this.handle);
				return true;
			}
		}
	}
}
