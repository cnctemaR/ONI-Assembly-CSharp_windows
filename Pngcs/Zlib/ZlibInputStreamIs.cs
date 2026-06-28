using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace Hjg.Pngcs.Zlib
{
	internal class ZlibInputStreamIs : AZlibInputStream
	{
		public ZlibInputStreamIs(Stream st, bool leaveOpen)
			: base(st, leaveOpen)
		{
			this.ist = new InflaterInputStream(st);
			this.ist.IsStreamOwner = !leaveOpen;
		}

		public override int Read(byte[] array, int offset, int count)
		{
			return this.ist.Read(array, offset, count);
		}

		public override int ReadByte()
		{
			return this.ist.ReadByte();
		}

		public override void Close()
		{
			this.ist.Close();
		}

		public override void Flush()
		{
			this.ist.Flush();
		}

		public override string getImplementationId()
		{
			return "Zlib inflater: SharpZipLib";
		}

		private InflaterInputStream ist;
	}
}
