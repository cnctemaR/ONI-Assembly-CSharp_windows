using System;
using System.Runtime.InteropServices;

namespace System
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct Variant
	{
		public void SetValue(object obj)
		{
			this.vt = 0;
			if (obj == null)
			{
				return;
			}
			Type type = obj.GetType();
			if (type.IsEnum)
			{
				type = Enum.GetUnderlyingType(type);
			}
			if (type == typeof(sbyte))
			{
				this.vt = 16;
				this.cVal = (sbyte)obj;
				return;
			}
			if (type == typeof(byte))
			{
				this.vt = 17;
				this.bVal = (byte)obj;
				return;
			}
			if (type == typeof(short))
			{
				this.vt = 2;
				this.iVal = (short)obj;
				return;
			}
			if (type == typeof(ushort))
			{
				this.vt = 18;
				this.uiVal = (ushort)obj;
				return;
			}
			if (type == typeof(int))
			{
				this.vt = 3;
				this.lVal = (int)obj;
				return;
			}
			if (type == typeof(uint))
			{
				this.vt = 19;
				this.ulVal = (uint)obj;
				return;
			}
			if (type == typeof(long))
			{
				this.vt = 20;
				this.llVal = (long)obj;
				return;
			}
			if (type == typeof(ulong))
			{
				this.vt = 21;
				this.ullVal = (ulong)obj;
				return;
			}
			if (type == typeof(float))
			{
				this.vt = 4;
				this.fltVal = (float)obj;
				return;
			}
			if (type == typeof(double))
			{
				this.vt = 5;
				this.dblVal = (double)obj;
				return;
			}
			if (type == typeof(string))
			{
				this.vt = 8;
				this.bstrVal = Marshal.StringToBSTR((string)obj);
				return;
			}
			if (type == typeof(bool))
			{
				this.vt = 11;
				this.lVal = (((bool)obj) ? (-1) : 0);
				return;
			}
			if (type == typeof(BStrWrapper))
			{
				this.vt = 8;
				this.bstrVal = Marshal.StringToBSTR(((BStrWrapper)obj).WrappedObject);
				return;
			}
			if (type == typeof(UnknownWrapper))
			{
				this.vt = 13;
				this.pdispVal = Marshal.GetIUnknownForObject(((UnknownWrapper)obj).WrappedObject);
				return;
			}
			if (type == typeof(DispatchWrapper))
			{
				this.vt = 9;
				this.pdispVal = Marshal.GetIDispatchForObject(((DispatchWrapper)obj).WrappedObject);
				return;
			}
			try
			{
				this.pdispVal = Marshal.GetIDispatchForObject(obj);
				this.vt = 9;
				return;
			}
			catch
			{
			}
			try
			{
				this.vt = 13;
				this.pdispVal = Marshal.GetIUnknownForObject(obj);
			}
			catch (Exception ex)
			{
				throw new NotImplementedException(string.Format("Variant couldn't handle object of type {0}", obj.GetType()), ex);
			}
		}

		public static object GetValueAt(int vt, IntPtr addr)
		{
			object obj = null;
			switch (vt)
			{
			case 2:
				obj = Marshal.ReadInt16(addr);
				break;
			case 3:
				obj = Marshal.ReadInt32(addr);
				break;
			case 4:
				obj = Marshal.PtrToStructure(addr, typeof(float));
				break;
			case 5:
				obj = Marshal.PtrToStructure(addr, typeof(double));
				break;
			case 8:
				obj = Marshal.PtrToStringBSTR(Marshal.ReadIntPtr(addr));
				break;
			case 9:
			case 13:
			{
				IntPtr intPtr = Marshal.ReadIntPtr(addr);
				if (intPtr != IntPtr.Zero)
				{
					obj = Marshal.GetObjectForIUnknown(intPtr);
				}
				break;
			}
			case 11:
				obj = Marshal.ReadInt16(addr) != 0;
				break;
			case 16:
				obj = (sbyte)Marshal.ReadByte(addr);
				break;
			case 17:
				obj = Marshal.ReadByte(addr);
				break;
			case 18:
				obj = (ushort)Marshal.ReadInt16(addr);
				break;
			case 19:
				obj = (uint)Marshal.ReadInt32(addr);
				break;
			case 20:
				obj = Marshal.ReadInt64(addr);
				break;
			case 21:
				obj = (ulong)Marshal.ReadInt64(addr);
				break;
			}
			return obj;
		}

		public object GetValue()
		{
			object obj = null;
			switch (this.vt)
			{
			case 2:
				return this.iVal;
			case 3:
				return this.lVal;
			case 4:
				return this.fltVal;
			case 5:
				return this.dblVal;
			case 8:
				return Marshal.PtrToStringBSTR(this.bstrVal);
			case 9:
			case 13:
				if (this.pdispVal != IntPtr.Zero)
				{
					return Marshal.GetObjectForIUnknown(this.pdispVal);
				}
				return obj;
			case 11:
				return this.boolVal != 0;
			case 16:
				return this.cVal;
			case 17:
				return this.bVal;
			case 18:
				return this.uiVal;
			case 19:
				return this.ulVal;
			case 20:
				return this.llVal;
			case 21:
				return this.ullVal;
			}
			if ((this.vt & 16384) == 16384 && this.pdispVal != IntPtr.Zero)
			{
				obj = Variant.GetValueAt((int)(this.vt & -16385), this.pdispVal);
			}
			return obj;
		}

		public void Clear()
		{
			if (this.vt == 8)
			{
				Marshal.FreeBSTR(this.bstrVal);
				return;
			}
			if ((this.vt == 9 || this.vt == 13) && this.pdispVal != IntPtr.Zero)
			{
				Marshal.Release(this.pdispVal);
			}
		}

		[FieldOffset(0)]
		public short vt;

		[FieldOffset(2)]
		public ushort wReserved1;

		[FieldOffset(4)]
		public ushort wReserved2;

		[FieldOffset(6)]
		public ushort wReserved3;

		[FieldOffset(8)]
		public long llVal;

		[FieldOffset(8)]
		public int lVal;

		[FieldOffset(8)]
		public byte bVal;

		[FieldOffset(8)]
		public short iVal;

		[FieldOffset(8)]
		public float fltVal;

		[FieldOffset(8)]
		public double dblVal;

		[FieldOffset(8)]
		public short boolVal;

		[FieldOffset(8)]
		public IntPtr bstrVal;

		[FieldOffset(8)]
		public sbyte cVal;

		[FieldOffset(8)]
		public ushort uiVal;

		[FieldOffset(8)]
		public uint ulVal;

		[FieldOffset(8)]
		public ulong ullVal;

		[FieldOffset(8)]
		public int intVal;

		[FieldOffset(8)]
		public uint uintVal;

		[FieldOffset(8)]
		public IntPtr pdispVal;

		[FieldOffset(8)]
		public BRECORD bRecord;
	}
}
