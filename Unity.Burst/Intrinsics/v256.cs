using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Unity.Burst.Intrinsics
{
	[DebuggerTypeProxy(typeof(V256DebugView))]
	[StructLayout(LayoutKind.Explicit)]
	public struct v256
	{
		public v256(byte b)
		{
			this = default(v256);
			this.Byte31 = b;
			this.Byte30 = b;
			this.Byte29 = b;
			this.Byte28 = b;
			this.Byte27 = b;
			this.Byte26 = b;
			this.Byte25 = b;
			this.Byte24 = b;
			this.Byte23 = b;
			this.Byte22 = b;
			this.Byte21 = b;
			this.Byte20 = b;
			this.Byte19 = b;
			this.Byte18 = b;
			this.Byte17 = b;
			this.Byte16 = b;
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

		public v256(byte a, byte b, byte c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k, byte l, byte m, byte n, byte o, byte p, byte q, byte r, byte s, byte t, byte u, byte v, byte w, byte x, byte y, byte z, byte A, byte B, byte C, byte D, byte E, byte F)
		{
			this = default(v256);
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
			this.Byte16 = q;
			this.Byte17 = r;
			this.Byte18 = s;
			this.Byte19 = t;
			this.Byte20 = u;
			this.Byte21 = v;
			this.Byte22 = w;
			this.Byte23 = x;
			this.Byte24 = y;
			this.Byte25 = z;
			this.Byte26 = A;
			this.Byte27 = B;
			this.Byte28 = C;
			this.Byte29 = D;
			this.Byte30 = E;
			this.Byte31 = F;
		}

		public v256(sbyte b)
		{
			this = default(v256);
			this.SByte31 = b;
			this.SByte30 = b;
			this.SByte29 = b;
			this.SByte28 = b;
			this.SByte27 = b;
			this.SByte26 = b;
			this.SByte25 = b;
			this.SByte24 = b;
			this.SByte23 = b;
			this.SByte22 = b;
			this.SByte21 = b;
			this.SByte20 = b;
			this.SByte19 = b;
			this.SByte18 = b;
			this.SByte17 = b;
			this.SByte16 = b;
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

		public v256(sbyte a, sbyte b, sbyte c, sbyte d, sbyte e, sbyte f, sbyte g, sbyte h, sbyte i, sbyte j, sbyte k, sbyte l, sbyte m, sbyte n, sbyte o, sbyte p, sbyte q, sbyte r, sbyte s, sbyte t, sbyte u, sbyte v, sbyte w, sbyte x, sbyte y, sbyte z, sbyte A, sbyte B, sbyte C, sbyte D, sbyte E, sbyte F)
		{
			this = default(v256);
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
			this.SByte16 = q;
			this.SByte17 = r;
			this.SByte18 = s;
			this.SByte19 = t;
			this.SByte20 = u;
			this.SByte21 = v;
			this.SByte22 = w;
			this.SByte23 = x;
			this.SByte24 = y;
			this.SByte25 = z;
			this.SByte26 = A;
			this.SByte27 = B;
			this.SByte28 = C;
			this.SByte29 = D;
			this.SByte30 = E;
			this.SByte31 = F;
		}

		public v256(short v)
		{
			this = default(v256);
			this.SShort15 = v;
			this.SShort14 = v;
			this.SShort13 = v;
			this.SShort12 = v;
			this.SShort11 = v;
			this.SShort10 = v;
			this.SShort9 = v;
			this.SShort8 = v;
			this.SShort7 = v;
			this.SShort6 = v;
			this.SShort5 = v;
			this.SShort4 = v;
			this.SShort3 = v;
			this.SShort2 = v;
			this.SShort1 = v;
			this.SShort0 = v;
		}

		public v256(short a, short b, short c, short d, short e, short f, short g, short h, short i, short j, short k, short l, short m, short n, short o, short p)
		{
			this = default(v256);
			this.SShort0 = a;
			this.SShort1 = b;
			this.SShort2 = c;
			this.SShort3 = d;
			this.SShort4 = e;
			this.SShort5 = f;
			this.SShort6 = g;
			this.SShort7 = h;
			this.SShort8 = i;
			this.SShort9 = j;
			this.SShort10 = k;
			this.SShort11 = l;
			this.SShort12 = m;
			this.SShort13 = n;
			this.SShort14 = o;
			this.SShort15 = p;
		}

		public v256(ushort v)
		{
			this = default(v256);
			this.UShort15 = v;
			this.UShort14 = v;
			this.UShort13 = v;
			this.UShort12 = v;
			this.UShort11 = v;
			this.UShort10 = v;
			this.UShort9 = v;
			this.UShort8 = v;
			this.UShort7 = v;
			this.UShort6 = v;
			this.UShort5 = v;
			this.UShort4 = v;
			this.UShort3 = v;
			this.UShort2 = v;
			this.UShort1 = v;
			this.UShort0 = v;
		}

		public v256(ushort a, ushort b, ushort c, ushort d, ushort e, ushort f, ushort g, ushort h, ushort i, ushort j, ushort k, ushort l, ushort m, ushort n, ushort o, ushort p)
		{
			this = default(v256);
			this.UShort0 = a;
			this.UShort1 = b;
			this.UShort2 = c;
			this.UShort3 = d;
			this.UShort4 = e;
			this.UShort5 = f;
			this.UShort6 = g;
			this.UShort7 = h;
			this.UShort8 = i;
			this.UShort9 = j;
			this.UShort10 = k;
			this.UShort11 = l;
			this.UShort12 = m;
			this.UShort13 = n;
			this.UShort14 = o;
			this.UShort15 = p;
		}

		public v256(int v)
		{
			this = default(v256);
			this.SInt7 = v;
			this.SInt6 = v;
			this.SInt5 = v;
			this.SInt4 = v;
			this.SInt3 = v;
			this.SInt2 = v;
			this.SInt1 = v;
			this.SInt0 = v;
		}

		public v256(int a, int b, int c, int d, int e, int f, int g, int h)
		{
			this = default(v256);
			this.SInt0 = a;
			this.SInt1 = b;
			this.SInt2 = c;
			this.SInt3 = d;
			this.SInt4 = e;
			this.SInt5 = f;
			this.SInt6 = g;
			this.SInt7 = h;
		}

		public v256(uint v)
		{
			this = default(v256);
			this.UInt7 = v;
			this.UInt6 = v;
			this.UInt5 = v;
			this.UInt4 = v;
			this.UInt3 = v;
			this.UInt2 = v;
			this.UInt1 = v;
			this.UInt0 = v;
		}

		public v256(uint a, uint b, uint c, uint d, uint e, uint f, uint g, uint h)
		{
			this = default(v256);
			this.UInt0 = a;
			this.UInt1 = b;
			this.UInt2 = c;
			this.UInt3 = d;
			this.UInt4 = e;
			this.UInt5 = f;
			this.UInt6 = g;
			this.UInt7 = h;
		}

		public v256(float f)
		{
			this = default(v256);
			this.Float7 = f;
			this.Float6 = f;
			this.Float5 = f;
			this.Float4 = f;
			this.Float3 = f;
			this.Float2 = f;
			this.Float1 = f;
			this.Float0 = f;
		}

		public v256(float a, float b, float c, float d, float e, float f, float g, float h)
		{
			this = default(v256);
			this.Float0 = a;
			this.Float1 = b;
			this.Float2 = c;
			this.Float3 = d;
			this.Float4 = e;
			this.Float5 = f;
			this.Float6 = g;
			this.Float7 = h;
		}

		public v256(double f)
		{
			this = default(v256);
			this.Double3 = f;
			this.Double2 = f;
			this.Double1 = f;
			this.Double0 = f;
		}

		public v256(double a, double b, double c, double d)
		{
			this = default(v256);
			this.Double0 = a;
			this.Double1 = b;
			this.Double2 = c;
			this.Double3 = d;
		}

		public v256(long f)
		{
			this = default(v256);
			this.SLong3 = f;
			this.SLong2 = f;
			this.SLong1 = f;
			this.SLong0 = f;
		}

		public v256(long a, long b, long c, long d)
		{
			this = default(v256);
			this.SLong0 = a;
			this.SLong1 = b;
			this.SLong2 = c;
			this.SLong3 = d;
		}

		public v256(ulong f)
		{
			this = default(v256);
			this.ULong3 = f;
			this.ULong2 = f;
			this.ULong1 = f;
			this.ULong0 = f;
		}

		public v256(ulong a, ulong b, ulong c, ulong d)
		{
			this = default(v256);
			this.ULong0 = a;
			this.ULong1 = b;
			this.ULong2 = c;
			this.ULong3 = d;
		}

		public v256(v128 lo, v128 hi)
		{
			this = default(v256);
			this.Lo128 = lo;
			this.Hi128 = hi;
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

		[FieldOffset(16)]
		public byte Byte16;

		[FieldOffset(17)]
		public byte Byte17;

		[FieldOffset(18)]
		public byte Byte18;

		[FieldOffset(19)]
		public byte Byte19;

		[FieldOffset(20)]
		public byte Byte20;

		[FieldOffset(21)]
		public byte Byte21;

		[FieldOffset(22)]
		public byte Byte22;

		[FieldOffset(23)]
		public byte Byte23;

		[FieldOffset(24)]
		public byte Byte24;

		[FieldOffset(25)]
		public byte Byte25;

		[FieldOffset(26)]
		public byte Byte26;

		[FieldOffset(27)]
		public byte Byte27;

		[FieldOffset(28)]
		public byte Byte28;

		[FieldOffset(29)]
		public byte Byte29;

		[FieldOffset(30)]
		public byte Byte30;

		[FieldOffset(31)]
		public byte Byte31;

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

		[FieldOffset(16)]
		public sbyte SByte16;

		[FieldOffset(17)]
		public sbyte SByte17;

		[FieldOffset(18)]
		public sbyte SByte18;

		[FieldOffset(19)]
		public sbyte SByte19;

		[FieldOffset(20)]
		public sbyte SByte20;

		[FieldOffset(21)]
		public sbyte SByte21;

		[FieldOffset(22)]
		public sbyte SByte22;

		[FieldOffset(23)]
		public sbyte SByte23;

		[FieldOffset(24)]
		public sbyte SByte24;

		[FieldOffset(25)]
		public sbyte SByte25;

		[FieldOffset(26)]
		public sbyte SByte26;

		[FieldOffset(27)]
		public sbyte SByte27;

		[FieldOffset(28)]
		public sbyte SByte28;

		[FieldOffset(29)]
		public sbyte SByte29;

		[FieldOffset(30)]
		public sbyte SByte30;

		[FieldOffset(31)]
		public sbyte SByte31;

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

		[FieldOffset(16)]
		public ushort UShort8;

		[FieldOffset(18)]
		public ushort UShort9;

		[FieldOffset(20)]
		public ushort UShort10;

		[FieldOffset(22)]
		public ushort UShort11;

		[FieldOffset(24)]
		public ushort UShort12;

		[FieldOffset(26)]
		public ushort UShort13;

		[FieldOffset(28)]
		public ushort UShort14;

		[FieldOffset(30)]
		public ushort UShort15;

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

		[FieldOffset(16)]
		public short SShort8;

		[FieldOffset(18)]
		public short SShort9;

		[FieldOffset(20)]
		public short SShort10;

		[FieldOffset(22)]
		public short SShort11;

		[FieldOffset(24)]
		public short SShort12;

		[FieldOffset(26)]
		public short SShort13;

		[FieldOffset(28)]
		public short SShort14;

		[FieldOffset(30)]
		public short SShort15;

		[FieldOffset(0)]
		public uint UInt0;

		[FieldOffset(4)]
		public uint UInt1;

		[FieldOffset(8)]
		public uint UInt2;

		[FieldOffset(12)]
		public uint UInt3;

		[FieldOffset(16)]
		public uint UInt4;

		[FieldOffset(20)]
		public uint UInt5;

		[FieldOffset(24)]
		public uint UInt6;

		[FieldOffset(28)]
		public uint UInt7;

		[FieldOffset(0)]
		public int SInt0;

		[FieldOffset(4)]
		public int SInt1;

		[FieldOffset(8)]
		public int SInt2;

		[FieldOffset(12)]
		public int SInt3;

		[FieldOffset(16)]
		public int SInt4;

		[FieldOffset(20)]
		public int SInt5;

		[FieldOffset(24)]
		public int SInt6;

		[FieldOffset(28)]
		public int SInt7;

		[FieldOffset(0)]
		public ulong ULong0;

		[FieldOffset(8)]
		public ulong ULong1;

		[FieldOffset(16)]
		public ulong ULong2;

		[FieldOffset(24)]
		public ulong ULong3;

		[FieldOffset(0)]
		public long SLong0;

		[FieldOffset(8)]
		public long SLong1;

		[FieldOffset(16)]
		public long SLong2;

		[FieldOffset(24)]
		public long SLong3;

		[FieldOffset(0)]
		public float Float0;

		[FieldOffset(4)]
		public float Float1;

		[FieldOffset(8)]
		public float Float2;

		[FieldOffset(12)]
		public float Float3;

		[FieldOffset(16)]
		public float Float4;

		[FieldOffset(20)]
		public float Float5;

		[FieldOffset(24)]
		public float Float6;

		[FieldOffset(28)]
		public float Float7;

		[FieldOffset(0)]
		public double Double0;

		[FieldOffset(8)]
		public double Double1;

		[FieldOffset(16)]
		public double Double2;

		[FieldOffset(24)]
		public double Double3;

		[FieldOffset(0)]
		public v128 Lo128;

		[FieldOffset(16)]
		public v128 Hi128;
	}
}
