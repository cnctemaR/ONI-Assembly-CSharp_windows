using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class WmfPlaceableFileHeader
	{
		public int Key
		{
			get
			{
				return this._key;
			}
			set
			{
				this._key = value;
			}
		}

		public short Hmf
		{
			get
			{
				return this._hmf;
			}
			set
			{
				this._hmf = value;
			}
		}

		public short BboxLeft
		{
			get
			{
				return this._bboxLeft;
			}
			set
			{
				this._bboxLeft = value;
			}
		}

		public short BboxTop
		{
			get
			{
				return this._bboxTop;
			}
			set
			{
				this._bboxTop = value;
			}
		}

		public short BboxRight
		{
			get
			{
				return this._bboxRight;
			}
			set
			{
				this._bboxRight = value;
			}
		}

		public short BboxBottom
		{
			get
			{
				return this._bboxBottom;
			}
			set
			{
				this._bboxBottom = value;
			}
		}

		public short Inch
		{
			get
			{
				return this._inch;
			}
			set
			{
				this._inch = value;
			}
		}

		public int Reserved
		{
			get
			{
				return this._reserved;
			}
			set
			{
				this._reserved = value;
			}
		}

		public short Checksum
		{
			get
			{
				return this._checksum;
			}
			set
			{
				this._checksum = value;
			}
		}

		private int _key = -1698247209;

		private short _hmf;

		private short _bboxLeft;

		private short _bboxTop;

		private short _bboxRight;

		private short _bboxBottom;

		private short _inch;

		private int _reserved;

		private short _checksum;
	}
}
