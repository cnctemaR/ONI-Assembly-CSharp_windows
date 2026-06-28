using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ParameterID
	{
		public ParameterID(string id)
		{
			this.bytes = Encoding.UTF8.GetBytes(id + '\0');
		}

		public byte[] bytes { get; private set; }
	}
}
