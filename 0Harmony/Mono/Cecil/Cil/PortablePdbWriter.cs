using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Mono.Cecil.Metadata;
using Mono.Cecil.PE;

namespace Mono.Cecil.Cil
{
	internal sealed class PortablePdbWriter : ISymbolWriter, IDisposable
	{
		private bool IsEmbedded
		{
			get
			{
				return this.writer == null;
			}
		}

		internal PortablePdbWriter(MetadataBuilder pdb_metadata, ModuleDefinition module)
		{
			this.pdb_metadata = pdb_metadata;
			this.module = module;
			this.module_metadata = module.metadata_builder;
			if (this.module_metadata != pdb_metadata)
			{
				this.pdb_metadata.metadata_builder = this.module_metadata;
			}
			pdb_metadata.AddCustomDebugInformations(module);
		}

		internal PortablePdbWriter(MetadataBuilder pdb_metadata, ModuleDefinition module, ImageWriter writer, Disposable<Stream> final_stream)
			: this(pdb_metadata, module)
		{
			this.writer = writer;
			this.final_stream = final_stream;
		}

		public ISymbolReaderProvider GetReaderProvider()
		{
			return new PortablePdbReaderProvider();
		}

		public void Write(MethodDebugInformation info)
		{
			this.CheckMethodDebugInformationTable();
			this.pdb_metadata.AddMethodDebugInformation(info);
		}

		public void Write()
		{
			if (this.IsEmbedded)
			{
				return;
			}
			this.WritePdbFile();
			if (this.final_stream.value != null)
			{
				this.writer.BaseStream.Seek(0L, SeekOrigin.Begin);
				byte[] array = new byte[8192];
				CryptoService.CopyStreamChunk(this.writer.BaseStream, this.final_stream.value, array, (int)this.writer.BaseStream.Length);
			}
		}

		public void Write(ICustomDebugInformationProvider provider)
		{
			this.pdb_metadata.AddCustomDebugInformations(provider);
		}

		public ImageDebugHeader GetDebugHeader()
		{
			if (this.IsEmbedded)
			{
				return new ImageDebugHeader();
			}
			ImageDebugDirectory imageDebugDirectory = new ImageDebugDirectory
			{
				MajorVersion = 256,
				MinorVersion = 20557,
				Type = ImageDebugType.CodeView,
				TimeDateStamp = (int)this.pdb_id_stamp
			};
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteUInt32(1396986706U);
			byteBuffer.WriteBytes(this.pdb_id_guid.ToByteArray());
			byteBuffer.WriteUInt32(1U);
			string text = this.writer.BaseStream.GetFileName();
			if (string.IsNullOrEmpty(text))
			{
				text = this.module.Assembly.Name.Name + ".pdb";
			}
			byteBuffer.WriteBytes(Encoding.UTF8.GetBytes(text));
			byteBuffer.WriteByte(0);
			byte[] array = new byte[byteBuffer.length];
			Buffer.BlockCopy(byteBuffer.buffer, 0, array, 0, byteBuffer.length);
			imageDebugDirectory.SizeOfData = array.Length;
			ImageDebugHeaderEntry imageDebugHeaderEntry = new ImageDebugHeaderEntry(imageDebugDirectory, array);
			ImageDebugDirectory imageDebugDirectory2 = new ImageDebugDirectory
			{
				MajorVersion = 1,
				MinorVersion = 0,
				Type = ImageDebugType.PdbChecksum,
				TimeDateStamp = 0
			};
			ByteBuffer byteBuffer2 = new ByteBuffer();
			byteBuffer2.WriteBytes(Encoding.UTF8.GetBytes("SHA256"));
			byteBuffer2.WriteByte(0);
			byteBuffer2.WriteBytes(this.pdb_checksum);
			byte[] array2 = new byte[byteBuffer2.length];
			Buffer.BlockCopy(byteBuffer2.buffer, 0, array2, 0, byteBuffer2.length);
			imageDebugDirectory2.SizeOfData = array2.Length;
			ImageDebugHeaderEntry imageDebugHeaderEntry2 = new ImageDebugHeaderEntry(imageDebugDirectory2, array2);
			return new ImageDebugHeader(new ImageDebugHeaderEntry[] { imageDebugHeaderEntry, imageDebugHeaderEntry2 });
		}

		private void CheckMethodDebugInformationTable()
		{
			MethodDebugInformationTable table = this.pdb_metadata.table_heap.GetTable<MethodDebugInformationTable>(Table.MethodDebugInformation);
			if (table.length > 0)
			{
				return;
			}
			table.rows = new Row<uint, uint>[this.module_metadata.method_rid - 1U];
			table.length = table.rows.Length;
		}

		public void Dispose()
		{
			this.writer.stream.Dispose();
			this.final_stream.Dispose();
		}

		private void WritePdbFile()
		{
			this.WritePdbHeap();
			this.WriteTableHeap();
			this.writer.BuildMetadataTextMap();
			this.writer.WriteMetadataHeader();
			this.writer.WriteMetadata();
			this.writer.Flush();
			this.ComputeChecksumAndPdbId();
			this.WritePdbId();
		}

		private void WritePdbHeap()
		{
			PdbHeapBuffer pdb_heap = this.pdb_metadata.pdb_heap;
			pdb_heap.WriteBytes(20);
			pdb_heap.WriteUInt32(this.module_metadata.entry_point.ToUInt32());
			MetadataTable[] tables = this.module_metadata.table_heap.tables;
			ulong num = 0UL;
			for (int i = 0; i < tables.Length; i++)
			{
				if (tables[i] != null && tables[i].Length != 0)
				{
					num |= 1UL << i;
				}
			}
			pdb_heap.WriteUInt64(num);
			for (int j = 0; j < tables.Length; j++)
			{
				if (tables[j] != null && tables[j].Length != 0)
				{
					pdb_heap.WriteUInt32((uint)tables[j].Length);
				}
			}
		}

		private void WriteTableHeap()
		{
			this.pdb_metadata.table_heap.string_offsets = this.pdb_metadata.string_heap.WriteStrings();
			this.pdb_metadata.table_heap.ComputeTableInformations();
			this.pdb_metadata.table_heap.WriteTableHeap();
		}

		private void ComputeChecksumAndPdbId()
		{
			byte[] array = new byte[8192];
			this.writer.BaseStream.Seek(0L, SeekOrigin.Begin);
			SHA256 sha = SHA256.Create();
			using (CryptoStream cryptoStream = new CryptoStream(Stream.Null, sha, CryptoStreamMode.Write))
			{
				CryptoService.CopyStreamChunk(this.writer.BaseStream, cryptoStream, array, (int)this.writer.BaseStream.Length);
			}
			this.pdb_checksum = sha.Hash;
			ByteBuffer byteBuffer = new ByteBuffer(this.pdb_checksum);
			this.pdb_id_guid = new Guid(byteBuffer.ReadBytes(16));
			this.pdb_id_stamp = byteBuffer.ReadUInt32();
		}

		private void WritePdbId()
		{
			this.writer.MoveToRVA(TextSegment.PdbHeap);
			this.writer.WriteBytes(this.pdb_id_guid.ToByteArray());
			this.writer.WriteUInt32(this.pdb_id_stamp);
		}

		private readonly MetadataBuilder pdb_metadata;

		private readonly ModuleDefinition module;

		private readonly ImageWriter writer;

		private readonly Disposable<Stream> final_stream;

		private MetadataBuilder module_metadata;

		internal byte[] pdb_checksum;

		internal Guid pdb_id_guid;

		internal uint pdb_id_stamp;
	}
}
