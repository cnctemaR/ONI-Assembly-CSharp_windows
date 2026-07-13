using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Unity.Burst.Intrinsics
{
	[DebuggerTypeProxy(typeof(V64DebugView))]
	[StructLayout(LayoutKind.Explicit)]
	public struct v64
	{
		public v64(byte b)
		{
			this = default(v64);
			this.Byte7 = b;
			this.Byte6 = b;
			this.Byte5 = b;
			this.Byte4 = b;
			this.Byte3 = b;
			this.Byte2 = b;
			this.Byte1 = b;
			this.Byte0 = b;
		}

		public v64(byte a, byte b, byte c, byte d, byte e, byte f, byte g, byte h)
		{
			this = default(v64);
			this.Byte0 = a;
			this.Byte1 = b;
			this.Byte2 = c;
			this.Byte3 = d;
			this.Byte4 = e;
			this.Byte5 = f;
			this.Byte6 = g;
			this.Byte7 = h;
		}

		public v64(sbyte b)
		{
			this = default(v64);
			this.SByte7 = b;
			this.SByte6 = b;
			this.SByte5 = b;
			this.SByte4 = b;
			this.SByte3 = b;
			this.SByte2 = b;
			this.SByte1 = b;
			this.SByte0 = b;
		}

		public v64(sbyte a, sbyte b, sbyte c, sbyte d, sbyte e, sbyte f, sbyte g, sbyte h)
		{
			this = default(v64);
			this.SByte0 = a;
			this.SByte1 = b;
			this.SByte2 = c;
			this.SByte3 = d;
			this.SByte4 = e;
			this.SByte5 = f;
			this.SByte6 = g;
			this.SByte7 = h;
		}

		public v64(short v)
		{
			this = default(v64);
			this.SShort3 = v;
			this.SShort2 = v;
			this.SShort1 = v;
			this.SShort0 = v;
		}

		public v64(short a, short b, short c, short d)
		{
			this = default(v64);
			this.SShort0 = a;
			this.SShort1 = b;
			this.SShort2 = c;
			this.SShort3 = d;
		}

		public v64(ushort v)
		{
			this = default(v64);
			this.UShort3 = v;
			this.UShort2 = v;
			this.UShort1 = v;
			this.UShort0 = v;
		}

		public v64(ushort a, ushort b, ushort c, ushort d)
		{
			this = default(v64);
			this.UShort0 = a;
			this.UShort1 = b;
			this.UShort2 = c;
			this.UShort3 = d;
		}

		public v64(int v)
		{
			this = default(v64);
			this.SInt1 = v;
			this.SInt0 = v;
		}

		public v64(int a, int b)
		{
			this = default(v64);
			this.SInt0 = a;
			this.SInt1 = b;
		}

		public v64(uint v)
		{
			this = default(v64);
			this.UInt1 = v;
			this.UInt0 = v;
		}

		public v64(uint a, uint b)
		{
			this = default(v64);
			this.UInt0 = a;
			this.UInt1 = b;
		}

		public v64(float f)
		{
			this = default(v64);
			this.Float1 = f;
			this.Float0 = f;
		}

		public v64(float a, float b)
		{
			this = default(v64);
			this.Float0 = a;
			this.Float1 = b;
		}

		public v64(double a)
		{
			this = default(v64);
			this.Double0 = a;
		}

		public v64(long a)
		{
			this = default(v64);
			this.SLong0 = a;
		}

		public v64(ulong a)
		{
			this = default(v64);
			this.ULong0 = a;
		}

		[FieldOffset(0)]
		public byte Byte0;

		[FieldOffset(1)]
		public byte Byte1;

		[FieldOffset(2)]
		public byte Byte2;

		[FieldOffset(3)]
		public byte Byte3;

		[FieldOffset(4)]
		public byte Byte4;

		[FieldOffset(5)]
		public byte Byte5;

		[FieldOffset(6)]
		public byte Byte6;

		[FieldOffset(7)]
		public byte Byte7;

		[FieldOffset(0)]
		public sbyte SByte0;

		[FieldOffset(1)]
		public sbyte SByte1;

		[FieldOffset(2)]
		public sbyte SByte2;

		[FieldOffset(3)]
		public sbyte SByte3;

		[FieldOffset(4)]
		public sbyte SByte4;

		[FieldOffset(5)]
		public sbyte SByte5;

		[FieldOffset(6)]
		public sbyte SByte6;

		[FieldOffset(7)]
		public sbyte SByte7;

		[FieldOffset(0)]
		public ushort UShort0;

		[FieldOffset(2)]
		public ushort UShort1;

		[FieldOffset(4)]
		public ushort UShort2;

		[FieldOffset(6)]
		public ushort UShort3;

		[FieldOffset(0)]
		public short SShort0;

		[FieldOffset(2)]
		public short SShort1;

		[FieldOffset(4)]
		public short SShort2;

		[FieldOffset(6)]
		public short SShort3;

		[FieldOffset(0)]
		public uint UInt0;

		[FieldOffset(4)]
		public uint UInt1;

		[FieldOffset(0)]
		public int SInt0;

		[FieldOffset(4)]
		public int SInt1;

		[FieldOffset(0)]
		public ulong ULong0;

		[FieldOffset(0)]
		public long SLong0;

		[FieldOffset(0)]
		public float Float0;

		[FieldOffset(4)]
		public float Float1;

		[FieldOffset(0)]
		public double Double0;
	}
}
