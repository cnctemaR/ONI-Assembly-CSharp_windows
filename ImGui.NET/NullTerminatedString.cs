using System;
using System.Text;

namespace ImGuiNET
{
	public struct NullTerminatedString
	{
		public unsafe NullTerminatedString(byte* data)
		{
			this.Data = data;
		}

		public unsafe override string ToString()
		{
			int num = 0;
			byte* ptr = this.Data;
			while (*ptr != 0)
			{
				num++;
				ptr++;
			}
			return Encoding.ASCII.GetString(this.Data, num);
		}

		public static implicit operator string(NullTerminatedString nts)
		{
			return nts.ToString();
		}

		public unsafe readonly byte* Data;
	}
}
