using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal struct WmfMetaHeader
	{
		public short file_type;

		public short header_size;

		public short version;

		public ushort file_size_low;

		public ushort file_size_high;

		public short num_of_objects;

		public int max_record_size;

		public short num_of_params;
	}
}
