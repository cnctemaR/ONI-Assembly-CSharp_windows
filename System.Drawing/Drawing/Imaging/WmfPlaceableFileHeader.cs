using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	public sealed class WmfPlaceableFileHeader
	{
		public WmfPlaceableFileHeader()
		{
			this.key = -1698247209;
		}

		public short BboxBottom
		{
			get
			{
				return this.bottom;
			}
			set
			{
				this.bottom = value;
			}
		}

		public short BboxLeft
		{
			get
			{
				return this.left;
			}
			set
			{
				this.left = value;
			}
		}

		public short BboxRight
		{
			get
			{
				return this.right;
			}
			set
			{
				this.right = value;
			}
		}

		public short BboxTop
		{
			get
			{
				return this.top;
			}
			set
			{
				this.top = value;
			}
		}

		public short Checksum
		{
			get
			{
				return this.checksum;
			}
			set
			{
				this.checksum = value;
			}
		}

		public short Hmf
		{
			get
			{
				return this.handle;
			}
			set
			{
				this.handle = value;
			}
		}

		public short Inch
		{
			get
			{
				return this.inch;
			}
			set
			{
				this.inch = value;
			}
		}

		public int Key
		{
			get
			{
				return this.key;
			}
			set
			{
				this.key = value;
			}
		}

		public int Reserved
		{
			get
			{
				return this.reserved;
			}
			set
			{
				this.reserved = value;
			}
		}

		private int key;

		private short handle;

		private short left;

		private short top;

		private short right;

		private short bottom;

		private short inch;

		private int reserved;

		private short checksum;
	}
}
