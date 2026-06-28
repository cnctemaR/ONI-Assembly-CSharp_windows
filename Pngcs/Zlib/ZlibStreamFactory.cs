using System;
using System.IO;

namespace Hjg.Pngcs.Zlib
{
	public class ZlibStreamFactory
	{
		public static AZlibInputStream createZlibInputStream(Stream st, bool leaveOpen)
		{
			return new ZlibInputStreamIs(st, leaveOpen);
		}

		public static AZlibInputStream createZlibInputStream(Stream st)
		{
			return ZlibStreamFactory.createZlibInputStream(st, false);
		}

		public static AZlibOutputStream createZlibOutputStream(Stream st, int compressLevel, EDeflateCompressStrategy strat, bool leaveOpen)
		{
			return new ZlibOutputStreamIs(st, compressLevel, strat, leaveOpen);
		}

		public static AZlibOutputStream createZlibOutputStream(Stream st)
		{
			return ZlibStreamFactory.createZlibOutputStream(st, false);
		}

		public static AZlibOutputStream createZlibOutputStream(Stream st, bool leaveOpen)
		{
			return ZlibStreamFactory.createZlibOutputStream(st, 6, EDeflateCompressStrategy.Default, leaveOpen);
		}
	}
}
