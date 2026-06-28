using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
	public struct StringWrapper
	{
		public static implicit operator string(StringWrapper fstring)
		{
			string text;
			if (fstring.nativeUtf8Ptr == IntPtr.Zero)
			{
				text = "";
			}
			else
			{
				int num = 0;
				while (Marshal.ReadByte(fstring.nativeUtf8Ptr, num) != 0)
				{
					num++;
				}
				if (num > 0)
				{
					byte[] array = new byte[num];
					Marshal.Copy(fstring.nativeUtf8Ptr, array, 0, num);
					text = Encoding.UTF8.GetString(array, 0, num);
				}
				else
				{
					text = "";
				}
			}
			return text;
		}

		private IntPtr nativeUtf8Ptr;
	}
}
