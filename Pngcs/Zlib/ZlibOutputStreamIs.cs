using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace Hjg.Pngcs.Zlib
{
	internal class ZlibOutputStreamIs : AZlibOutputStream
	{
		public ZlibOutputStreamIs(Stream st, int compressLevel, EDeflateCompressStrategy strat, bool leaveOpen)
			: base(st, compressLevel, strat, leaveOpen)
		{
			this.deflater = new Deflater(compressLevel);
			this.setStrat(strat);
			this.ost = new DeflaterOutputStream(st, this.deflater);
			this.ost.IsStreamOwner = !leaveOpen;
		}

		public void setStrat(EDeflateCompressStrategy strat)
		{
			if (strat == EDeflateCompressStrategy.Filtered)
			{
				this.deflater.SetStrategy(1);
				return;
			}
			if (strat == EDeflateCompressStrategy.Huffman)
			{
				this.deflater.SetStrategy(2);
				return;
			}
			this.deflater.SetStrategy(0);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.ost.Write(buffer, offset, count);
		}

		public override void WriteByte(byte value)
		{
			this.ost.WriteByte(value);
		}

		public override void Close()
		{
			this.ost.Close();
		}

		public override void Flush()
		{
			this.ost.Flush();
		}

		public override string getImplementationId()
		{
			return "Zlib deflater: SharpZipLib";
		}

		private DeflaterOutputStream ost;

		private Deflater deflater;
	}
}
