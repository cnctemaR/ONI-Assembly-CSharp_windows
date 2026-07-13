using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Unity.Burst.Intrinsics
{
	[DebuggerTypeProxy(typeof(V128DebugView))]
	[StructLayout(LayoutKind.Explicit)]
	public struct v128
	{
		public v128(byte b)
		{
			this = default(v128);
			this.Byte15 = b;
			this.Byte14 = b;
			this.Byte13 = b;
			this.Byte12 = b;
			this.Byte11 = b;
			this.Byte10 = b;
			this.Byte9 = b;
			this.Byte8 = b;
			this.Byte7 = b;
			this.Byte6 = b;
			this.Byte5 = b;
			this.Byte4 = b;
			this.Byte3 = b;
			this.Byte2 = b;
			this.Byte1 = b;
			this.Byte0 = b;
		}

		public v128(byte a, byte b, byte c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k, byte l, byte m, byte n, byte o, byte p)
		{
			this = default(v128);
			this.Byte0 = a;
			this.Byte1 = b;
			this.Byte2 = c;
			this.Byte3 = d;
			this.Byte4 = e;
			this.Byte5 = f;
			this.Byte6 = g;
			this.Byte7 = h;
			this.Byte8 = i;
			this.Byte9 = j;
			this.Byte10 = k;
			this.Byte11 = l;
			this.Byte12 = m;
			this.Byte13 = n;
			this.Byte14 = o;
			this.Byte15 = p;
		}

		public v128(sbyte b)
		{
			this = default(v128);
			this.SByte15 = b;
			this.SByte14 = b;
			this.SByte13 = b;
			this.SByte12 = b;
			this.SByte11 = b;
			this.SByte10 = b;
			this.SByte9 = b;
			this.SByte8 = b;
			this.SByte7 = b;
			this.SByte6 = b;
			this.SByte5 = b;
			this.SByte4 = b;
			this.SByte3 = b;
			this.SByte2 = b;
			this.SByte1 = b;
			this.SByte0 = b;
		}

		public v128(sbyte a, sbyte b, sbyte c, sbyte d, sbyte e, sbyte f, sbyte g, sbyte h, sbyte i, sbyte j, sbyte k, sbyte l, sbyte m, sbyte n, sbyte o, sbyte p)
		{
			this = default(v128);
			this.SByte0 = a;
			this.SByte1 = b;
			this.SByte2 = c;
			this.SByte3 = d;
			this.SByte4 = e;
			this.SByte5 = f;
			this.SByte6 = g;
			this.SByte7 = h;
			this.SByte8 = i;
			this.SByte9 = j;
			this.SByte10 = k;
			this.SByte11 = l;
			this.SByte12 = m;
			this.SByte13 = n;
			this.SByte14 = o;
			this.SByte15 = p;
		}

		public v128(short v)
		{
			this = default(v128);
			this.SShort7 = v;
			this.SShort6 = v;
			this.SShort5 = v;
			this.SShort4 = v;
			this.SShort3 = v;
			this.SShort2 = v;
			this.SShort1 = v;
			this.SShort0 = v;
		}

		public v128(short a, short b, short c, short d, short e, short f, short g, short h)
		{
			this = default(v128);
			this.SShort0 = a;
			this.SShort1 = b;
			this.SShort2 = c;
			this.SShort3 = d;
			this.SShort4 = e;
			this.SShort5 = f;
			this.SShort6 = g;
			this.SShort7 = h;
		}

		public v128(ushort v)
		{
			this = default(v128);
			this.UShort7 = v;
			this.UShort6 = v;
			this.UShort5 = v;
			this.UShort4 = v;
			this.UShort3 = v;
			this.UShort2 = v;
			this.UShort1 = v;
			this.UShort0 = v;
		}

		public v128(ushort a, ushort b, ushort c, ushort d, ushort e, ushort f, ushort g, ushort h)
		{
			this = default(v128);
			this.UShort0 = a;
			this.UShort1 = b;
			this.UShort2 = c;
			this.UShort3 = d;
			this.UShort4 = e;
			this.UShort5 = f;
			this.UShort6 = g;
			this.UShort7 = h;
		}

		public v128(int v)
		{
			this = default(v128);
			this.SInt3 = v;
			this.SInt2 = v;
			this.SInt1 = v;
			this.SInt0 = v;
		}

		public v128(int a, int b, int c, int d)
		{
			this = default(v128);
			this.SInt0 = a;
			this.SInt1 = b;
			this.SInt2 = c;
			this.SInt3 = d;
		}

		public v128(uint v)
		{
			this = default(v128);
			this.UInt3 = v;
			this.UInt2 = v;
			this.UInt1 = v;
			this.UInt0 = v;
		}

		public v128(uint a, uint b, uint c, uint d)
		{
			this = default(v128);
			this.UInt0 = a;
			this.UInt1 = b;
			this.UInt2 = c;
			this.UInt3 = d;
		}

		public v128(float f)
		{
			this = default(v128);
			this.Float3 = f;
			this.Float2 = f;
			this.Float1 = f;
			this.Float0 = f;
		}

		public v128(float a, float b, float c, float d)
		{
			this = default(v128);
			this.Float0 = a;
			this.Float1 = b;
			this.Float2 = c;
			this.Float3 = d;
		}

		public v128(double f)
		{
			this = default(v128);
			this.Double1 = f;
			this.Double0 = f;
		}

		public v128(double a, double b)
		{
			this = default(v128);
			this.Double0 = a;
			this.Double1 = b;
		}

		public v128(long f)
		{
			this = default(v128);
			this.SLong1 = f;
			this.SLong0 = f;
		}

		public v128(long a, long b)
		{
			this = default(v128);
			this.SLong0 = a;
			this.SLong1 = b;
		}

		public v128(ulong f)
		{
			this = default(v128);
			this.ULong1 = f;
			this.ULong0 = f;
		}

		public v128(ulong a, ulong b)
		{
			this = default(v128);
			this.ULong0 = a;
			this.ULong1 = b;
		}

		public v128(v64 lo, v64 hi)
		{
			this = default(v128);
			this.Lo64 = lo;
			this.Hi64 = hi;
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

		[FieldOffset(8)]
		public byte Byte8;

		[FieldOffset(9)]
		public byte Byte9;

		[FieldOffset(10)]
		public byte Byte10;

		[FieldOffset(11)]
		public byte Byte11;

		[FieldOffset(12)]
		public byte Byte12;

		[FieldOffset(13)]
		public byte Byte13;

		[FieldOffset(14)]
		public byte Byte14;

		[FieldOffset(15)]
		public byte Byte15;

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

		[FieldOffset(8)]
		public sbyte SByte8;

		[FieldOffset(9)]
		public sbyte SByte9;

		[FieldOffset(10)]
		public sbyte SByte10;

		[FieldOffset(11)]
		public sbyte SByte11;

		[FieldOffset(12)]
		public sbyte SByte12;

		[FieldOffset(13)]
		public sbyte SByte13;

		[FieldOffset(14)]
		public sbyte SByte14;

		[FieldOffset(15)]
		public sbyte SByte15;

		[FieldOffset(0)]
		public ushort UShort0;

		[FieldOffset(2)]
		public ushort UShort1;

		[FieldOffset(4)]
		public ushort UShort2;

		[FieldOffset(6)]
		public ushort UShort3;

		[FieldOffset(8)]
		public ushort UShort4;

		[FieldOffset(10)]
		public ushort UShort5;

		[FieldOffset(12)]
		public ushort UShort6;

		[FieldOffset(14)]
		public ushort UShort7;

		[FieldOffset(0)]
		public short SShort0;

		[FieldOffset(2)]
		public short SShort1;

		[FieldOffset(4)]
		public short SShort2;

		[FieldOffset(6)]
		public short SShort3;

		[FieldOffset(8)]
		public short SShort4;

		[FieldOffset(10)]
		public short SShort5;

		[FieldOffset(12)]
		public short SShort6;

		[FieldOffset(14)]
		public short SShort7;

		[FieldOffset(0)]
		public uint UInt0;

		[FieldOffset(4)]
		public uint UInt1;

		[FieldOffset(8)]
		public uint UInt2;

		[FieldOffset(12)]
		public uint UInt3;

		[FieldOffset(0)]
		public int SInt0;

		[FieldOffset(4)]
		public int SInt1;

		[FieldOffset(8)]
		public int SInt2;

		[FieldOffset(12)]
		public int SInt3;

		[FieldOffset(0)]
		public ulong ULong0;

		[FieldOffset(8)]
		public ulong ULong1;

		[FieldOffset(0)]
		public long SLong0;

		[FieldOffset(8)]
		public long SLong1;

		[FieldOffset(0)]
		public float Float0;

		[FieldOffset(4)]
		public float Float1;

		[FieldOffset(8)]
		public float Float2;

		[FieldOffset(12)]
		public float Float3;

		[FieldOffset(0)]
		public double Double0;

		[FieldOffset(8)]
		public double Double1;

		[FieldOffset(0)]
		public v64 Lo64;

		[FieldOffset(8)]
		public v64 Hi64;
	}
}
