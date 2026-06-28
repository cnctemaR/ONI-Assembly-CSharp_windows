using System;
using System.IO;
using System.Text;
using Hjg.Pngcs.Zlib;

namespace Hjg.Pngcs
{
	public class PngHelperInternal
	{
		public static CRC32 GetCRC()
		{
			if (PngHelperInternal.crc32Engine == null)
			{
				PngHelperInternal.crc32Engine = new CRC32();
			}
			return PngHelperInternal.crc32Engine;
		}

		public static int DoubleToInt100000(double d)
		{
			return (int)(d * 100000.0 + 0.5);
		}

		public static double IntToDouble100000(int i)
		{
			return (double)i / 100000.0;
		}

		public static void WriteInt2(Stream os, int n)
		{
			byte[] array = new byte[]
			{
				(byte)((n >> 8) & 255),
				(byte)(n & 255)
			};
			PngHelperInternal.WriteBytes(os, array);
		}

		public static int ReadInt2(Stream mask0)
		{
			int num3;
			try
			{
				int num = mask0.ReadByte();
				int num2 = mask0.ReadByte();
				if (num == -1 || num2 == -1)
				{
					num3 = -1;
				}
				else
				{
					num3 = (num << 8) + num2;
				}
			}
			catch (IOException ex)
			{
				throw new PngjInputException("error reading readInt2", ex);
			}
			return num3;
		}

		public static int ReadInt4(Stream mask0)
		{
			int num5;
			try
			{
				int num = mask0.ReadByte();
				int num2 = mask0.ReadByte();
				int num3 = mask0.ReadByte();
				int num4 = mask0.ReadByte();
				if (num == -1 || num2 == -1 || num3 == -1 || num4 == -1)
				{
					num5 = -1;
				}
				else
				{
					num5 = (num << 24) + (num2 << 16) + (num3 << 8) + num4;
				}
			}
			catch (IOException ex)
			{
				throw new PngjInputException("error reading readInt4", ex);
			}
			return num5;
		}

		public static int ReadInt1fromByte(byte[] b, int offset)
		{
			return (int)(b[offset] & byte.MaxValue);
		}

		public static int ReadInt2fromBytes(byte[] b, int offset)
		{
			return ((int)(b[offset] & byte.MaxValue) << 16) | (int)(b[offset + 1] & byte.MaxValue);
		}

		public static int ReadInt4fromBytes(byte[] b, int offset)
		{
			return ((int)(b[offset] & byte.MaxValue) << 24) | ((int)(b[offset + 1] & byte.MaxValue) << 16) | ((int)(b[offset + 2] & byte.MaxValue) << 8) | (int)(b[offset + 3] & byte.MaxValue);
		}

		public static void WriteInt2tobytes(int n, byte[] b, int offset)
		{
			b[offset] = (byte)((n >> 8) & 255);
			b[offset + 1] = (byte)(n & 255);
		}

		public static void WriteInt4tobytes(int n, byte[] b, int offset)
		{
			b[offset] = (byte)((n >> 24) & 255);
			b[offset + 1] = (byte)((n >> 16) & 255);
			b[offset + 2] = (byte)((n >> 8) & 255);
			b[offset + 3] = (byte)(n & 255);
		}

		public static void WriteInt4(Stream os, int n)
		{
			byte[] array = new byte[4];
			PngHelperInternal.WriteInt4tobytes(n, array, 0);
			PngHelperInternal.WriteBytes(os, array);
		}

		public static void ReadBytes(Stream mask0, byte[] b, int offset, int len)
		{
			if (len == 0)
			{
				return;
			}
			try
			{
				int num;
				for (int i = 0; i < len; i += num)
				{
					num = mask0.Read(b, offset + i, len - i);
					if (num < 1)
					{
						throw new Exception(string.Concat(new object[] { "error reading, ", num, " !=", len }));
					}
				}
			}
			catch (IOException ex)
			{
				throw new PngjInputException("error reading", ex);
			}
		}

		public static void SkipBytes(Stream ist, int len)
		{
			byte[] array = new byte[32768];
			int i = len;
			try
			{
				while (i > 0)
				{
					int num = ist.Read(array, 0, (i > array.Length) ? array.Length : i);
					if (num < 0)
					{
						throw new PngjInputException("error reading (skipping) : EOF");
					}
					i -= num;
				}
			}
			catch (IOException ex)
			{
				throw new PngjInputException("error reading (skipping)", ex);
			}
		}

		public static void WriteBytes(Stream os, byte[] b)
		{
			try
			{
				os.Write(b, 0, b.Length);
			}
			catch (IOException ex)
			{
				throw new PngjOutputException(ex);
			}
		}

		public static void WriteBytes(Stream os, byte[] b, int offset, int n)
		{
			try
			{
				os.Write(b, offset, n);
			}
			catch (IOException ex)
			{
				throw new PngjOutputException(ex);
			}
		}

		public static int ReadByte(Stream mask0)
		{
			int num;
			try
			{
				num = mask0.ReadByte();
			}
			catch (IOException ex)
			{
				throw new PngjOutputException(ex);
			}
			return num;
		}

		public static void WriteByte(Stream os, byte b)
		{
			try
			{
				os.WriteByte(b);
			}
			catch (IOException ex)
			{
				throw new PngjOutputException(ex);
			}
		}

		public static int UnfilterRowPaeth(int r, int a, int b, int c)
		{
			return (r + PngHelperInternal.FilterPaethPredictor(a, b, c)) & 255;
		}

		public static int FilterPaethPredictor(int a, int b, int c)
		{
			int num = a + b - c;
			int num2 = ((num >= a) ? (num - a) : (a - num));
			int num3 = ((num >= b) ? (num - b) : (b - num));
			int num4 = ((num >= c) ? (num - c) : (c - num));
			if (num2 <= num3 && num2 <= num4)
			{
				return a;
			}
			if (num3 <= num4)
			{
				return b;
			}
			return c;
		}

		public static void Logdebug(string msg)
		{
			if (PngHelperInternal.DEBUG)
			{
				Console.Out.WriteLine(msg);
			}
		}

		public static void InitCrcForTests(PngReader pngr)
		{
			pngr.InitCrctest();
		}

		public static long GetCrctestVal(PngReader pngr)
		{
			return pngr.GetCrctestVal();
		}

		[ThreadStatic]
		private static CRC32 crc32Engine = null;

		public static readonly byte[] PNG_ID_SIGNATURE = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };

		public static Encoding charsetLatin1 = Encoding.GetEncoding("ISO-8859-1");

		public static Encoding charsetUtf8 = Encoding.GetEncoding("UTF-8");

		public static bool DEBUG = false;
	}
}
