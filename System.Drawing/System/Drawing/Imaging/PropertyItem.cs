using System;

namespace System.Drawing.Imaging
{
	public sealed class PropertyItem
	{
		internal PropertyItem()
		{
		}

		public int Id
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
			}
		}

		public int Len
		{
			get
			{
				return this._len;
			}
			set
			{
				this._len = value;
			}
		}

		public short Type
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
			}
		}

		public byte[] Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}

		private int _id;

		private int _len;

		private short _type;

		private byte[] _value;
	}
}
