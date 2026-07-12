using System;
using System.Text;

namespace ImGuiNET
{
	public static class RangeAccessorExtensions
	{
		public unsafe static string GetStringASCII(this RangeAccessor<byte> stringAccessor)
		{
			return Encoding.ASCII.GetString((byte*)stringAccessor.Data, stringAccessor.Count);
		}
	}
}
