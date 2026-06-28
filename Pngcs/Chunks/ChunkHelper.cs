using System;
using System.Collections.Generic;
using System.IO;
using Hjg.Pngcs.Zlib;

namespace Hjg.Pngcs.Chunks
{
	public class ChunkHelper
	{
		public static byte[] ToBytes(string x)
		{
			return PngHelperInternal.charsetLatin1.GetBytes(x);
		}

		public static string ToString(byte[] x)
		{
			return PngHelperInternal.charsetLatin1.GetString(x);
		}

		public static string ToString(byte[] x, int offset, int len)
		{
			return PngHelperInternal.charsetLatin1.GetString(x, offset, len);
		}

		public static byte[] ToBytesUTF8(string x)
		{
			return PngHelperInternal.charsetUtf8.GetBytes(x);
		}

		public static string ToStringUTF8(byte[] x)
		{
			return PngHelperInternal.charsetUtf8.GetString(x);
		}

		public static string ToStringUTF8(byte[] x, int offset, int len)
		{
			return PngHelperInternal.charsetUtf8.GetString(x, offset, len);
		}

		public static void WriteBytesToStream(Stream stream, byte[] bytes)
		{
			stream.Write(bytes, 0, bytes.Length);
		}

		public static bool IsCritical(string id)
		{
			return char.IsUpper(id[0]);
		}

		public static bool IsPublic(string id)
		{
			return char.IsUpper(id[1]);
		}

		public static bool IsSafeToCopy(string id)
		{
			return !char.IsUpper(id[3]);
		}

		public static bool IsUnknown(PngChunk chunk)
		{
			return chunk is PngChunkUNKNOWN;
		}

		public static int PosNullByte(byte[] bytes)
		{
			for (int i = 0; i < bytes.Length; i++)
			{
				if (bytes[i] == 0)
				{
					return i;
				}
			}
			return -1;
		}

		public static bool ShouldLoad(string id, ChunkLoadBehaviour behav)
		{
			if (ChunkHelper.IsCritical(id))
			{
				return true;
			}
			bool flag = PngChunk.isKnown(id);
			switch (behav)
			{
			case ChunkLoadBehaviour.LOAD_CHUNK_NEVER:
				return false;
			case ChunkLoadBehaviour.LOAD_CHUNK_KNOWN:
				return flag;
			case ChunkLoadBehaviour.LOAD_CHUNK_IF_SAFE:
				return flag || ChunkHelper.IsSafeToCopy(id);
			case ChunkLoadBehaviour.LOAD_CHUNK_ALWAYS:
				return true;
			default:
				return false;
			}
		}

		internal static byte[] compressBytes(byte[] ori, bool compress)
		{
			return ChunkHelper.compressBytes(ori, 0, ori.Length, compress);
		}

		internal static byte[] compressBytes(byte[] ori, int offset, int len, bool compress)
		{
			byte[] array2;
			try
			{
				MemoryStream memoryStream = new MemoryStream(ori, offset, len);
				Stream stream = memoryStream;
				if (!compress)
				{
					stream = ZlibStreamFactory.createZlibInputStream(memoryStream);
				}
				MemoryStream memoryStream2 = new MemoryStream();
				Stream stream2 = memoryStream2;
				if (compress)
				{
					stream2 = ZlibStreamFactory.createZlibOutputStream(memoryStream2);
				}
				ChunkHelper.shovelInToOut(stream, stream2);
				stream.Close();
				stream2.Close();
				byte[] array = memoryStream2.ToArray();
				array2 = array;
			}
			catch (Exception ex)
			{
				throw new PngjException(ex);
			}
			return array2;
		}

		private static void shovelInToOut(Stream inx, Stream outx)
		{
			byte[] array = new byte[1024];
			int num;
			while ((num = inx.Read(array, 0, 1024)) > 0)
			{
				outx.Write(array, 0, num);
			}
		}

		internal static bool maskMatch(int v, int mask)
		{
			return (v & mask) != 0;
		}

		public static List<PngChunk> FilterList(List<PngChunk> list, ChunkPredicate predicateKeep)
		{
			List<PngChunk> list2 = new List<PngChunk>();
			foreach (PngChunk pngChunk in list)
			{
				if (predicateKeep.Matches(pngChunk))
				{
					list2.Add(pngChunk);
				}
			}
			return list2;
		}

		public static int TrimList(List<PngChunk> list, ChunkPredicate predicateRemove)
		{
			int num = 0;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (predicateRemove.Matches(list[i]))
				{
					list.RemoveAt(i);
					num++;
				}
			}
			return num;
		}

		public static bool Equivalent(PngChunk c1, PngChunk c2)
		{
			if (c1 == c2)
			{
				return true;
			}
			if (c1 == null || c2 == null || !c1.Id.Equals(c2.Id))
			{
				return false;
			}
			if (c1.GetType() != c2.GetType())
			{
				return false;
			}
			if (!c2.AllowsMultiple())
			{
				return true;
			}
			if (c1 is PngChunkTextVar)
			{
				return ((PngChunkTextVar)c1).GetKey().Equals(((PngChunkTextVar)c2).GetKey());
			}
			return c1 is PngChunkSPLT && ((PngChunkSPLT)c1).PalName.Equals(((PngChunkSPLT)c2).PalName);
		}

		public static bool IsText(PngChunk c)
		{
			return c is PngChunkTextVar;
		}

		internal const string IHDR = "IHDR";

		internal const string PLTE = "PLTE";

		internal const string IDAT = "IDAT";

		internal const string IEND = "IEND";

		internal const string cHRM = "cHRM";

		internal const string gAMA = "gAMA";

		internal const string iCCP = "iCCP";

		internal const string sBIT = "sBIT";

		internal const string sRGB = "sRGB";

		internal const string bKGD = "bKGD";

		internal const string hIST = "hIST";

		internal const string tRNS = "tRNS";

		internal const string pHYs = "pHYs";

		internal const string sPLT = "sPLT";

		internal const string tIME = "tIME";

		internal const string iTXt = "iTXt";

		internal const string tEXt = "tEXt";

		internal const string zTXt = "zTXt";

		internal static readonly byte[] b_IHDR = ChunkHelper.ToBytes("IHDR");

		internal static readonly byte[] b_PLTE = ChunkHelper.ToBytes("PLTE");

		internal static readonly byte[] b_IDAT = ChunkHelper.ToBytes("IDAT");

		internal static readonly byte[] b_IEND = ChunkHelper.ToBytes("IEND");
	}
}
