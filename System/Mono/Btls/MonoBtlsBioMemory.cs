using System;
using System.Runtime.InteropServices;

namespace Mono.Btls
{
	internal class MonoBtlsBioMemory : MonoBtlsBio
	{
		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_bio_mem_new();

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_bio_mem_get_data(IntPtr handle, out IntPtr data);

		public MonoBtlsBioMemory()
			: base(new MonoBtlsBio.BoringBioHandle(MonoBtlsBioMemory.mono_btls_bio_mem_new()))
		{
		}

		public byte[] GetData()
		{
			bool flag = false;
			byte[] array2;
			try
			{
				base.Handle.DangerousAddRef(ref flag);
				IntPtr intPtr;
				int num = MonoBtlsBioMemory.mono_btls_bio_mem_get_data(base.Handle.DangerousGetHandle(), out intPtr);
				base.CheckError(num > 0, "GetData");
				byte[] array = new byte[num];
				Marshal.Copy(intPtr, array, 0, num);
				array2 = array;
			}
			finally
			{
				if (flag)
				{
					base.Handle.DangerousRelease();
				}
			}
			return array2;
		}
	}
}
