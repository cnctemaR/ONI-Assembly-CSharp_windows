using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[CLSCompliant(false)]
	[Serializable]
	public struct UIntPtr : ISerializable
	{
		public UIntPtr(ulong value)
		{
			if (value > (ulong)(-1) && UIntPtr.Size < 8)
			{
				throw new OverflowException(Locale.GetText("This isn't a 64bits machine."));
			}
			this._pointer = value;
		}

		public UIntPtr(uint value)
		{
			this._pointer = value;
		}

		[CLSCompliant(false)]
		public unsafe UIntPtr(void* value)
		{
			this._pointer = value;
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("pointer", this._pointer);
		}

		public override bool Equals(object obj)
		{
			if (obj is UIntPtr)
			{
				UIntPtr uintPtr = (UIntPtr)obj;
				return this._pointer == uintPtr._pointer;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this._pointer;
		}

		public uint ToUInt32()
		{
			return this._pointer;
		}

		public ulong ToUInt64()
		{
			return this._pointer;
		}

		[CLSCompliant(false)]
		public unsafe void* ToPointer()
		{
			return this._pointer;
		}

		public override string ToString()
		{
			return this._pointer.ToString();
		}

		public unsafe static int Size
		{
			get
			{
				return sizeof(void*);
			}
		}

		public static bool operator ==(UIntPtr value1, UIntPtr value2)
		{
			return value1._pointer == value2._pointer;
		}

		public static bool operator !=(UIntPtr value1, UIntPtr value2)
		{
			return value1._pointer != value2._pointer;
		}

		public static explicit operator ulong(UIntPtr value)
		{
			return value._pointer;
		}

		public static explicit operator uint(UIntPtr value)
		{
			return value._pointer;
		}

		public static explicit operator UIntPtr(ulong value)
		{
			return new UIntPtr(value);
		}

		[CLSCompliant(false)]
		public unsafe static explicit operator UIntPtr(void* value)
		{
			return new UIntPtr(value);
		}

		[CLSCompliant(false)]
		public unsafe static explicit operator void*(UIntPtr value)
		{
			return value.ToPointer();
		}

		public static explicit operator UIntPtr(uint value)
		{
			return new UIntPtr(value);
		}

		public static readonly UIntPtr Zero = new UIntPtr(0U);

		private unsafe void* _pointer;
	}
}
