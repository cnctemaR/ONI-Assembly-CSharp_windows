using System;
using System.IO;
using System.IO.Compression;
using Mono.Cecil.PE;

namespace Mono.Cecil.Cil
{
	internal sealed class EmbeddedPortablePdbWriter : ISymbolWriter, IDisposable
	{
		internal EmbeddedPortablePdbWriter(Stream stream, PortablePdbWriter writer)
		{
			this.stream = stream;
			this.writer = writer;
		}

		public ISymbolReaderProvider GetReaderProvider()
		{
			return new EmbeddedPortablePdbReaderProvider();
		}

		public void Write(MethodDebugInformation info)
		{
			this.writer.Write(info);
		}

		public void Write(ICustomDebugInformationProvider provider)
		{
			this.writer.Write(provider);
		}

		public ImageDebugHeader GetDebugHeader()
		{
			ImageDebugHeader debugHeader = this.writer.GetDebugHeader();
			ImageDebugDirectory imageDebugDirectory = new ImageDebugDirectory
			{
				Type = ImageDebugType.EmbeddedPortablePdb,
				MajorVersion = 256,
				MinorVersion = 256
			};
			MemoryStream memoryStream = new MemoryStream();
			BinaryStreamWriter binaryStreamWriter = new BinaryStreamWriter(memoryStream);
			binaryStreamWriter.WriteByte(77);
			binaryStreamWriter.WriteByte(80);
			binaryStreamWriter.WriteByte(68);
			binaryStreamWriter.WriteByte(66);
			binaryStreamWriter.WriteInt32((int)this.stream.Length);
			this.stream.Position = 0L;
			using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, true))
			{
				this.stream.CopyTo(deflateStream);
			}
			imageDebugDirectory.SizeOfData = (int)memoryStream.Length;
			ImageDebugHeaderEntry[] array = new ImageDebugHeaderEntry[debugHeader.Entries.Length + 1];
			for (int i = 0; i < debugHeader.Entries.Length; i++)
			{
				array[i] = debugHeader.Entries[i];
			}
			array[array.Length - 1] = new ImageDebugHeaderEntry(imageDebugDirectory, memoryStream.ToArray());
			return new ImageDebugHeader(array);
		}

		public void Write()
		{
			this.writer.Write();
		}

		public void Dispose()
		{
			this.writer.Dispose();
		}

		private readonly Stream stream;

		private readonly PortablePdbWriter writer;
	}
}
