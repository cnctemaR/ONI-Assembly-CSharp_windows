using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class MetaHeader
	{
		public MetaHeader()
		{
		}

		internal MetaHeader(WmfMetaHeader header)
		{
			this.wmf.file_type = header.file_type;
			this.wmf.header_size = header.header_size;
			this.wmf.version = header.version;
			this.wmf.file_size_low = header.file_size_low;
			this.wmf.file_size_high = header.file_size_high;
			this.wmf.num_of_objects = header.num_of_objects;
			this.wmf.max_record_size = header.max_record_size;
			this.wmf.num_of_params = header.num_of_params;
		}

		public short HeaderSize
		{
			get
			{
				return this.wmf.header_size;
			}
			set
			{
				this.wmf.header_size = value;
			}
		}

		public int MaxRecord
		{
			get
			{
				return this.wmf.max_record_size;
			}
			set
			{
				this.wmf.max_record_size = value;
			}
		}

		public short NoObjects
		{
			get
			{
				return this.wmf.num_of_objects;
			}
			set
			{
				this.wmf.num_of_objects = value;
			}
		}

		public short NoParameters
		{
			get
			{
				return this.wmf.num_of_params;
			}
			set
			{
				this.wmf.num_of_params = value;
			}
		}

		public int Size
		{
			get
			{
				if (BitConverter.IsLittleEndian)
				{
					return ((int)this.wmf.file_size_high << 16) | (int)this.wmf.file_size_low;
				}
				return ((int)this.wmf.file_size_low << 16) | (int)this.wmf.file_size_high;
			}
			set
			{
				if (BitConverter.IsLittleEndian)
				{
					this.wmf.file_size_high = (ushort)(value >> 16);
					this.wmf.file_size_low = (ushort)value;
				}
				else
				{
					this.wmf.file_size_high = (ushort)value;
					this.wmf.file_size_low = (ushort)(value >> 16);
				}
			}
		}

		public short Type
		{
			get
			{
				return this.wmf.file_type;
			}
			set
			{
				this.wmf.file_type = value;
			}
		}

		public short Version
		{
			get
			{
				return this.wmf.version;
			}
			set
			{
				this.wmf.version = value;
			}
		}

		private WmfMetaHeader wmf;
	}
}
