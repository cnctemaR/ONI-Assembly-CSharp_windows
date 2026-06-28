using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Mono.Security;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public struct Guid : IFormattable, IComparable, IComparable<Guid>, IEquatable<Guid>
	{
		public Guid(byte[] b)
		{
			Guid.CheckArray(b, 16);
			this._a = BitConverterLE.ToInt32(b, 0);
			this._b = BitConverterLE.ToInt16(b, 4);
			this._c = BitConverterLE.ToInt16(b, 6);
			this._d = b[8];
			this._e = b[9];
			this._f = b[10];
			this._g = b[11];
			this._h = b[12];
			this._i = b[13];
			this._j = b[14];
			this._k = b[15];
		}

		public Guid(string g)
		{
			Guid.CheckNull(g);
			g = g.Trim();
			Guid.GuidParser guidParser = new Guid.GuidParser(g);
			Guid guid = guidParser.Parse();
			this = guid;
		}

		public Guid(int a, short b, short c, byte[] d)
		{
			Guid.CheckArray(d, 8);
			this._a = a;
			this._b = b;
			this._c = c;
			this._d = d[0];
			this._e = d[1];
			this._f = d[2];
			this._g = d[3];
			this._h = d[4];
			this._i = d[5];
			this._j = d[6];
			this._k = d[7];
		}

		public Guid(int a, short b, short c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
		{
			this._a = a;
			this._b = b;
			this._c = c;
			this._d = d;
			this._e = e;
			this._f = f;
			this._g = g;
			this._h = h;
			this._i = i;
			this._j = j;
			this._k = k;
		}

		[CLSCompliant(false)]
		public Guid(uint a, ushort b, ushort c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
		{
			this = new Guid((int)a, (short)b, (short)c, d, e, f, g, h, i, j, k);
		}

		private static void CheckNull(object o)
		{
			if (o == null)
			{
				throw new ArgumentNullException(Locale.GetText("Value cannot be null."));
			}
		}

		private static void CheckLength(byte[] o, int l)
		{
			if (o.Length != l)
			{
				throw new ArgumentException(string.Format(Locale.GetText("Array should be exactly {0} bytes long."), l));
			}
		}

		private static void CheckArray(byte[] o, int l)
		{
			Guid.CheckNull(o);
			Guid.CheckLength(o, l);
		}

		private static int Compare(int x, int y)
		{
			if (x < y)
			{
				return -1;
			}
			return 1;
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is Guid))
			{
				throw new ArgumentException("value", Locale.GetText("Argument of System.Guid.CompareTo should be a Guid."));
			}
			return this.CompareTo((Guid)value);
		}

		public override bool Equals(object o)
		{
			return o is Guid && this.CompareTo((Guid)o) == 0;
		}

		public int CompareTo(Guid value)
		{
			if (this._a != value._a)
			{
				return Guid.Compare(this._a, value._a);
			}
			if (this._b != value._b)
			{
				return Guid.Compare((int)this._b, (int)value._b);
			}
			if (this._c != value._c)
			{
				return Guid.Compare((int)this._c, (int)value._c);
			}
			if (this._d != value._d)
			{
				return Guid.Compare((int)this._d, (int)value._d);
			}
			if (this._e != value._e)
			{
				return Guid.Compare((int)this._e, (int)value._e);
			}
			if (this._f != value._f)
			{
				return Guid.Compare((int)this._f, (int)value._f);
			}
			if (this._g != value._g)
			{
				return Guid.Compare((int)this._g, (int)value._g);
			}
			if (this._h != value._h)
			{
				return Guid.Compare((int)this._h, (int)value._h);
			}
			if (this._i != value._i)
			{
				return Guid.Compare((int)this._i, (int)value._i);
			}
			if (this._j != value._j)
			{
				return Guid.Compare((int)this._j, (int)value._j);
			}
			if (this._k != value._k)
			{
				return Guid.Compare((int)this._k, (int)value._k);
			}
			return 0;
		}

		public bool Equals(Guid g)
		{
			return this.CompareTo(g) == 0;
		}

		public override int GetHashCode()
		{
			int num = this._a;
			num ^= ((int)this._b << 16) | (int)this._c;
			num ^= (int)this._d << 24;
			num ^= (int)this._e << 16;
			num ^= (int)this._f << 8;
			num ^= (int)this._g;
			num ^= (int)this._h << 24;
			num ^= (int)this._i << 16;
			num ^= (int)this._j << 8;
			return num ^ (int)this._k;
		}

		private static char ToHex(int b)
		{
			return (char)((b >= 10) ? (97 + b - 10) : (48 + b));
		}

		public static Guid NewGuid()
		{
			byte[] array = new byte[16];
			object rngAccess = Guid._rngAccess;
			lock (rngAccess)
			{
				if (Guid._rng == null)
				{
					Guid._rng = RandomNumberGenerator.Create();
				}
				Guid._rng.GetBytes(array);
			}
			Guid guid = new Guid(array);
			guid._d = (guid._d & 63) | 128;
			guid._c = (short)(((long)guid._c & 4095L) | 16384L);
			return guid;
		}

		internal static byte[] FastNewGuidArray()
		{
			byte[] array = new byte[16];
			object rngAccess = Guid._rngAccess;
			lock (rngAccess)
			{
				if (Guid._rng != null)
				{
					Guid._fastRng = Guid._rng;
				}
				if (Guid._fastRng == null)
				{
					Guid._fastRng = new RNGCryptoServiceProvider();
				}
				Guid._fastRng.GetBytes(array);
			}
			array[8] = (array[8] & 63) | 128;
			array[7] = (array[7] & 15) | 64;
			return array;
		}

		public byte[] ToByteArray()
		{
			byte[] array = new byte[16];
			int num = 0;
			byte[] array2 = BitConverterLE.GetBytes(this._a);
			for (int i = 0; i < 4; i++)
			{
				array[num++] = array2[i];
			}
			array2 = BitConverterLE.GetBytes(this._b);
			for (int i = 0; i < 2; i++)
			{
				array[num++] = array2[i];
			}
			array2 = BitConverterLE.GetBytes(this._c);
			for (int i = 0; i < 2; i++)
			{
				array[num++] = array2[i];
			}
			array[8] = this._d;
			array[9] = this._e;
			array[10] = this._f;
			array[11] = this._g;
			array[12] = this._h;
			array[13] = this._i;
			array[14] = this._j;
			array[15] = this._k;
			return array;
		}

		private static void AppendInt(StringBuilder builder, int value)
		{
			builder.Append(Guid.ToHex((value >> 28) & 15));
			builder.Append(Guid.ToHex((value >> 24) & 15));
			builder.Append(Guid.ToHex((value >> 20) & 15));
			builder.Append(Guid.ToHex((value >> 16) & 15));
			builder.Append(Guid.ToHex((value >> 12) & 15));
			builder.Append(Guid.ToHex((value >> 8) & 15));
			builder.Append(Guid.ToHex((value >> 4) & 15));
			builder.Append(Guid.ToHex(value & 15));
		}

		private static void AppendShort(StringBuilder builder, short value)
		{
			builder.Append(Guid.ToHex((value >> 12) & 15));
			builder.Append(Guid.ToHex((value >> 8) & 15));
			builder.Append(Guid.ToHex((value >> 4) & 15));
			builder.Append(Guid.ToHex((int)(value & 15)));
		}

		private static void AppendByte(StringBuilder builder, byte value)
		{
			builder.Append(Guid.ToHex((value >> 4) & 15));
			builder.Append(Guid.ToHex((int)(value & 15)));
		}

		private string BaseToString(bool h, bool p, bool b)
		{
			StringBuilder stringBuilder = new StringBuilder(40);
			if (p)
			{
				stringBuilder.Append('(');
			}
			else if (b)
			{
				stringBuilder.Append('{');
			}
			Guid.AppendInt(stringBuilder, this._a);
			if (h)
			{
				stringBuilder.Append('-');
			}
			Guid.AppendShort(stringBuilder, this._b);
			if (h)
			{
				stringBuilder.Append('-');
			}
			Guid.AppendShort(stringBuilder, this._c);
			if (h)
			{
				stringBuilder.Append('-');
			}
			Guid.AppendByte(stringBuilder, this._d);
			Guid.AppendByte(stringBuilder, this._e);
			if (h)
			{
				stringBuilder.Append('-');
			}
			Guid.AppendByte(stringBuilder, this._f);
			Guid.AppendByte(stringBuilder, this._g);
			Guid.AppendByte(stringBuilder, this._h);
			Guid.AppendByte(stringBuilder, this._i);
			Guid.AppendByte(stringBuilder, this._j);
			Guid.AppendByte(stringBuilder, this._k);
			if (p)
			{
				stringBuilder.Append(')');
			}
			else if (b)
			{
				stringBuilder.Append('}');
			}
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			return this.BaseToString(true, false, false);
		}

		public string ToString(string format)
		{
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			if (format != null)
			{
				string text = format.ToLowerInvariant();
				if (text == "b")
				{
					flag3 = true;
				}
				else if (text == "p")
				{
					flag2 = true;
				}
				else if (text == "n")
				{
					flag = false;
				}
				else if (text != "d" && text != string.Empty)
				{
					throw new FormatException(Locale.GetText("Argument to Guid.ToString(string format) should be \"b\", \"B\", \"d\", \"D\", \"n\", \"N\", \"p\" or \"P\""));
				}
			}
			return this.BaseToString(flag, flag2, flag3);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return this.ToString(format);
		}

		public static bool operator ==(Guid a, Guid b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Guid a, Guid b)
		{
			return !a.Equals(b);
		}

		private int _a;

		private short _b;

		private short _c;

		private byte _d;

		private byte _e;

		private byte _f;

		private byte _g;

		private byte _h;

		private byte _i;

		private byte _j;

		private byte _k;

		public static readonly Guid Empty = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

		private static object _rngAccess = new object();

		private static RandomNumberGenerator _rng;

		private static RandomNumberGenerator _fastRng;

		internal class GuidParser
		{
			public GuidParser(string src)
			{
				this._src = src;
				this.Reset();
			}

			private void Reset()
			{
				this._cur = 0;
				this._length = this._src.Length;
			}

			private bool AtEnd()
			{
				return this._cur >= this._length;
			}

			private void ThrowFormatException()
			{
				throw new FormatException(Locale.GetText("Invalid format for Guid.Guid(string)."));
			}

			private ulong ParseHex(int length, bool strictLength)
			{
				ulong num = 0UL;
				bool flag = false;
				int num2 = 0;
				while (!flag && num2 < length)
				{
					if (this.AtEnd())
					{
						if (strictLength || num2 == 0)
						{
							this.ThrowFormatException();
						}
						else
						{
							flag = true;
						}
					}
					else
					{
						char c = char.ToLowerInvariant(this._src[this._cur]);
						if (char.IsDigit(c))
						{
							num = num * 16UL + (ulong)c - 48UL;
							this._cur++;
						}
						else if (c >= 'a' && c <= 'f')
						{
							num = num * 16UL + (ulong)c - 97UL + 10UL;
							this._cur++;
						}
						else if (strictLength || num2 == 0)
						{
							this.ThrowFormatException();
						}
						else
						{
							flag = true;
						}
					}
					num2++;
				}
				return num;
			}

			private bool ParseOptChar(char c)
			{
				if (!this.AtEnd() && this._src[this._cur] == c)
				{
					this._cur++;
					return true;
				}
				return false;
			}

			private void ParseChar(char c)
			{
				if (!this.ParseOptChar(c))
				{
					this.ThrowFormatException();
				}
			}

			private Guid ParseGuid1()
			{
				bool flag = true;
				char c = '}';
				byte[] array = new byte[8];
				bool flag2 = this.ParseOptChar('{');
				if (!flag2)
				{
					flag2 = this.ParseOptChar('(');
					if (flag2)
					{
						c = ')';
					}
				}
				int num = (int)this.ParseHex(8, true);
				if (flag2)
				{
					this.ParseChar('-');
				}
				else
				{
					flag = this.ParseOptChar('-');
				}
				short num2 = (short)this.ParseHex(4, true);
				if (flag)
				{
					this.ParseChar('-');
				}
				short num3 = (short)this.ParseHex(4, true);
				if (flag)
				{
					this.ParseChar('-');
				}
				for (int i = 0; i < 8; i++)
				{
					array[i] = (byte)this.ParseHex(2, true);
					if (i == 1 && flag)
					{
						this.ParseChar('-');
					}
				}
				if (flag2 && !this.ParseOptChar(c))
				{
					this.ThrowFormatException();
				}
				return new Guid(num, num2, num3, array);
			}

			private void ParseHexPrefix()
			{
				this.ParseChar('0');
				this.ParseChar('x');
			}

			private Guid ParseGuid2()
			{
				byte[] array = new byte[8];
				this.ParseChar('{');
				this.ParseHexPrefix();
				int num = (int)this.ParseHex(8, false);
				this.ParseChar(',');
				this.ParseHexPrefix();
				short num2 = (short)this.ParseHex(4, false);
				this.ParseChar(',');
				this.ParseHexPrefix();
				short num3 = (short)this.ParseHex(4, false);
				this.ParseChar(',');
				this.ParseChar('{');
				for (int i = 0; i < 8; i++)
				{
					this.ParseHexPrefix();
					array[i] = (byte)this.ParseHex(2, false);
					if (i != 7)
					{
						this.ParseChar(',');
					}
				}
				this.ParseChar('}');
				this.ParseChar('}');
				return new Guid(num, num2, num3, array);
			}

			public Guid Parse()
			{
				Guid guid;
				try
				{
					guid = this.ParseGuid1();
				}
				catch (FormatException)
				{
					this.Reset();
					guid = this.ParseGuid2();
				}
				if (!this.AtEnd())
				{
					this.ThrowFormatException();
				}
				return guid;
			}

			private string _src;

			private int _length;

			private int _cur;
		}
	}
}
