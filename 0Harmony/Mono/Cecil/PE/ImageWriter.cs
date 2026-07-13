using System;
using System.IO;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;

namespace Mono.Cecil.PE
{
	internal sealed class ImageWriter : BinaryStreamWriter
	{
		private ImageWriter(ModuleDefinition module, string runtime_version, MetadataBuilder metadata, Disposable<Stream> stream, bool metadataOnly = false)
			: base(stream.value)
		{
			this.module = module;
			this.runtime_version = runtime_version;
			this.text_map = metadata.text_map;
			this.stream = stream;
			this.metadata = metadata;
			if (metadataOnly)
			{
				return;
			}
			this.pe64 = module.Architecture == TargetArchitecture.AMD64 || module.Architecture == TargetArchitecture.IA64 || module.Architecture == TargetArchitecture.ARM64;
			this.has_reloc = module.Architecture == TargetArchitecture.I386;
			this.GetDebugHeader();
			this.GetWin32Resources();
			this.BuildTextMap();
			this.sections = (this.has_reloc ? 2 : 1);
		}

		private void GetDebugHeader()
		{
			ISymbolWriter symbol_writer = this.metadata.symbol_writer;
			if (symbol_writer != null)
			{
				this.debug_header = symbol_writer.GetDebugHeader();
			}
			if (this.module.HasDebugHeader)
			{
				if (this.module.GetDebugHeader().GetDeterministicEntry() == null)
				{
					return;
				}
				this.debug_header = this.debug_header.AddDeterministicEntry();
			}
		}

		private void GetWin32Resources()
		{
			if (!this.module.HasImage)
			{
				return;
			}
			DataDirectory win32Resources = this.module.Image.Win32Resources;
			uint size = win32Resources.Size;
			if (size > 0U)
			{
				this.win32_resources = this.module.Image.GetReaderAt<uint, ByteBuffer>(win32Resources.VirtualAddress, size, (uint s, BinaryStreamReader reader) => new ByteBuffer(reader.ReadBytes((int)s)));
			}
		}

		public static ImageWriter CreateWriter(ModuleDefinition module, MetadataBuilder metadata, Disposable<Stream> stream)
		{
			ImageWriter imageWriter = new ImageWriter(module, module.runtime_version, metadata, stream, false);
			imageWriter.BuildSections();
			return imageWriter;
		}

		public static ImageWriter CreateDebugWriter(ModuleDefinition module, MetadataBuilder metadata, Disposable<Stream> stream)
		{
			ImageWriter imageWriter = new ImageWriter(module, "PDB v1.0", metadata, stream, true);
			uint length = metadata.text_map.GetLength();
			imageWriter.text = new Section
			{
				SizeOfRawData = length,
				VirtualSize = length
			};
			return imageWriter;
		}

		private void BuildSections()
		{
			bool flag = this.win32_resources != null;
			if (flag)
			{
				this.sections += 1;
			}
			this.text = this.CreateSection(".text", this.text_map.GetLength(), null);
			Section section = this.text;
			if (flag)
			{
				this.rsrc = this.CreateSection(".rsrc", (uint)this.win32_resources.length, section);
				this.PatchWin32Resources(this.win32_resources);
				section = this.rsrc;
			}
			if (this.has_reloc)
			{
				this.reloc = this.CreateSection(".reloc", 12U, section);
			}
		}

		private Section CreateSection(string name, uint size, Section previous)
		{
			return new Section
			{
				Name = name,
				VirtualAddress = ((previous != null) ? (previous.VirtualAddress + ImageWriter.Align(previous.VirtualSize, 8192U)) : 8192U),
				VirtualSize = size,
				PointerToRawData = ((previous != null) ? (previous.PointerToRawData + previous.SizeOfRawData) : ImageWriter.Align(this.GetHeaderSize(), 512U)),
				SizeOfRawData = ImageWriter.Align(size, 512U)
			};
		}

		private static uint Align(uint value, uint align)
		{
			align -= 1U;
			return (value + align) & ~align;
		}

		private void WriteDOSHeader()
		{
			this.Write(new byte[]
			{
				77, 90, 144, 0, 3, 0, 0, 0, 4, 0,
				0, 0, byte.MaxValue, byte.MaxValue, 0, 0, 184, 0, 0, 0,
				0, 0, 0, 0, 64, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				128, 0, 0, 0, 14, 31, 186, 14, 0, 180,
				9, 205, 33, 184, 1, 76, 205, 33, 84, 104,
				105, 115, 32, 112, 114, 111, 103, 114, 97, 109,
				32, 99, 97, 110, 110, 111, 116, 32, 98, 101,
				32, 114, 117, 110, 32, 105, 110, 32, 68, 79,
				83, 32, 109, 111, 100, 101, 46, 13, 13, 10,
				36, 0, 0, 0, 0, 0, 0, 0
			});
		}

		private ushort SizeOfOptionalHeader()
		{
			return (!this.pe64) ? 224 : 240;
		}

		private void WritePEFileHeader()
		{
			base.WriteUInt32(17744U);
			base.WriteUInt16((ushort)this.module.Architecture);
			base.WriteUInt16(this.sections);
			base.WriteUInt32(this.metadata.timestamp);
			base.WriteUInt32(0U);
			base.WriteUInt32(0U);
			base.WriteUInt16(this.SizeOfOptionalHeader());
			ushort num = (ushort)(2 | ((!this.pe64) ? 256 : 32));
			if (this.module.Kind == ModuleKind.Dll || this.module.Kind == ModuleKind.NetModule)
			{
				num |= 8192;
			}
			if (this.module.Image != null && (this.module.Image.Characteristics & 32U) != 0U)
			{
				num |= 32;
			}
			base.WriteUInt16(num);
		}

		private Section LastSection()
		{
			if (this.reloc != null)
			{
				return this.reloc;
			}
			if (this.rsrc != null)
			{
				return this.rsrc;
			}
			return this.text;
		}

		private void WriteOptionalHeaders()
		{
			base.WriteUInt16((!this.pe64) ? 267 : 523);
			base.WriteUInt16(this.module.linker_version);
			base.WriteUInt32(this.text.SizeOfRawData);
			base.WriteUInt32(((this.reloc != null) ? this.reloc.SizeOfRawData : 0U) + ((this.rsrc != null) ? this.rsrc.SizeOfRawData : 0U));
			base.WriteUInt32(0U);
			Range range = this.text_map.GetRange(TextSegment.StartupStub);
			base.WriteUInt32((range.Length > 0U) ? range.Start : 0U);
			base.WriteUInt32(8192U);
			if (!this.pe64)
			{
				base.WriteUInt32(0U);
				base.WriteUInt32(4194304U);
			}
			else
			{
				base.WriteUInt64(4194304UL);
			}
			base.WriteUInt32(8192U);
			base.WriteUInt32(512U);
			base.WriteUInt16(4);
			base.WriteUInt16(0);
			base.WriteUInt16(0);
			base.WriteUInt16(0);
			base.WriteUInt16(this.module.subsystem_major);
			base.WriteUInt16(this.module.subsystem_minor);
			base.WriteUInt32(0U);
			Section section = this.LastSection();
			base.WriteUInt32(section.VirtualAddress + ImageWriter.Align(section.VirtualSize, 8192U));
			base.WriteUInt32(this.text.PointerToRawData);
			base.WriteUInt32(0U);
			base.WriteUInt16(this.GetSubSystem());
			base.WriteUInt16((ushort)this.module.Characteristics);
			if (!this.pe64)
			{
				base.WriteUInt32(1048576U);
				base.WriteUInt32(4096U);
				base.WriteUInt32(1048576U);
				base.WriteUInt32(4096U);
			}
			else
			{
				base.WriteUInt64(4194304UL);
				base.WriteUInt64(16384UL);
				base.WriteUInt64(1048576UL);
				base.WriteUInt64(8192UL);
			}
			base.WriteUInt32(0U);
			base.WriteUInt32(16U);
			this.WriteZeroDataDirectory();
			base.WriteDataDirectory(this.text_map.GetDataDirectory(TextSegment.ImportDirectory));
			if (this.rsrc != null)
			{
				base.WriteUInt32(this.rsrc.VirtualAddress);
				base.WriteUInt32(this.rsrc.VirtualSize);
			}
			else
			{
				this.WriteZeroDataDirectory();
			}
			this.WriteZeroDataDirectory();
			this.WriteZeroDataDirectory();
			base.WriteUInt32((this.reloc != null) ? this.reloc.VirtualAddress : 0U);
			base.WriteUInt32((this.reloc != null) ? this.reloc.VirtualSize : 0U);
			if (this.text_map.GetLength(TextSegment.DebugDirectory) > 0)
			{
				base.WriteUInt32(this.text_map.GetRVA(TextSegment.DebugDirectory));
				base.WriteUInt32((uint)(this.debug_header.Entries.Length * 28));
			}
			else
			{
				this.WriteZeroDataDirectory();
			}
			this.WriteZeroDataDirectory();
			this.WriteZeroDataDirectory();
			this.WriteZeroDataDirectory();
			this.WriteZeroDataDirectory();
			this.WriteZeroDataDirectory();
			base.WriteDataDirectory(this.text_map.GetDataDirectory(TextSegment.ImportAddressTable));
			this.WriteZeroDataDirectory();
			base.WriteDataDirectory(this.text_map.GetDataDirectory(TextSegment.CLIHeader));
			this.WriteZeroDataDirectory();
		}

		private void WriteZeroDataDirectory()
		{
			base.WriteUInt32(0U);
			base.WriteUInt32(0U);
		}

		private ushort GetSubSystem()
		{
			switch (this.module.Kind)
			{
			case ModuleKind.Dll:
			case ModuleKind.Console:
			case ModuleKind.NetModule:
				return 3;
			case ModuleKind.Windows:
				return 2;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void WriteSectionHeaders()
		{
			this.WriteSection(this.text, 1610612768U);
			if (this.rsrc != null)
			{
				this.WriteSection(this.rsrc, 1073741888U);
			}
			if (this.reloc != null)
			{
				this.WriteSection(this.reloc, 1107296320U);
			}
		}

		private void WriteSection(Section section, uint characteristics)
		{
			byte[] array = new byte[8];
			string name = section.Name;
			for (int i = 0; i < name.Length; i++)
			{
				array[i] = (byte)name[i];
			}
			base.WriteBytes(array);
			base.WriteUInt32(section.VirtualSize);
			base.WriteUInt32(section.VirtualAddress);
			base.WriteUInt32(section.SizeOfRawData);
			base.WriteUInt32(section.PointerToRawData);
			base.WriteUInt32(0U);
			base.WriteUInt32(0U);
			base.WriteUInt16(0);
			base.WriteUInt16(0);
			base.WriteUInt32(characteristics);
		}

		private uint GetRVAFileOffset(Section section, uint rva)
		{
			return section.PointerToRawData + rva - section.VirtualAddress;
		}

		private void MoveTo(uint pointer)
		{
			this.BaseStream.Seek((long)((ulong)pointer), SeekOrigin.Begin);
		}

		private void MoveToRVA(Section section, uint rva)
		{
			this.BaseStream.Seek((long)((ulong)this.GetRVAFileOffset(section, rva)), SeekOrigin.Begin);
		}

		internal void MoveToRVA(TextSegment segment)
		{
			this.MoveToRVA(this.text, this.text_map.GetRVA(segment));
		}

		private void WriteRVA(uint rva)
		{
			if (!this.pe64)
			{
				base.WriteUInt32(rva);
				return;
			}
			base.WriteUInt64((ulong)rva);
		}

		private void PrepareSection(Section section)
		{
			this.MoveTo(section.PointerToRawData);
			if (section.SizeOfRawData <= 4096U)
			{
				this.Write(new byte[section.SizeOfRawData]);
				this.MoveTo(section.PointerToRawData);
				return;
			}
			int num = 0;
			byte[] array = new byte[4096];
			while ((long)num != (long)((ulong)section.SizeOfRawData))
			{
				int num2 = Math.Min((int)(section.SizeOfRawData - (uint)num), 4096);
				this.Write(array, 0, num2);
				num += num2;
			}
			this.MoveTo(section.PointerToRawData);
		}

		private void WriteText()
		{
			this.PrepareSection(this.text);
			if (this.has_reloc)
			{
				this.WriteRVA(this.text_map.GetRVA(TextSegment.ImportHintNameTable));
				this.WriteRVA(0U);
			}
			base.WriteUInt32(72U);
			base.WriteUInt16(2);
			base.WriteUInt16((this.module.Runtime <= TargetRuntime.Net_1_1) ? 0 : 5);
			base.WriteUInt32(this.text_map.GetRVA(TextSegment.MetadataHeader));
			base.WriteUInt32(this.GetMetadataLength());
			base.WriteUInt32((uint)this.module.Attributes);
			base.WriteUInt32(this.metadata.entry_point.ToUInt32());
			base.WriteDataDirectory(this.text_map.GetDataDirectory(TextSegment.Resources));
			base.WriteDataDirectory(this.text_map.GetDataDirectory(TextSegment.StrongNameSignature));
			this.WriteZeroDataDirectory();
			this.WriteZeroDataDirectory();
			this.WriteZeroDataDirectory();
			this.WriteZeroDataDirectory();
			this.MoveToRVA(TextSegment.Code);
			base.WriteBuffer(this.metadata.code);
			this.MoveToRVA(TextSegment.Resources);
			base.WriteBuffer(this.metadata.resources);
			if (this.metadata.data.length > 0)
			{
				this.MoveToRVA(TextSegment.Data);
				base.WriteBuffer(this.metadata.data);
			}
			this.MoveToRVA(TextSegment.MetadataHeader);
			this.WriteMetadataHeader();
			this.WriteMetadata();
			if (this.text_map.GetLength(TextSegment.DebugDirectory) > 0)
			{
				this.MoveToRVA(TextSegment.DebugDirectory);
				this.WriteDebugDirectory();
			}
			if (!this.has_reloc)
			{
				return;
			}
			this.MoveToRVA(TextSegment.ImportDirectory);
			this.WriteImportDirectory();
			this.MoveToRVA(TextSegment.StartupStub);
			this.WriteStartupStub();
		}

		private uint GetMetadataLength()
		{
			return this.text_map.GetRVA(TextSegment.DebugDirectory) - this.text_map.GetRVA(TextSegment.MetadataHeader);
		}

		public void WriteMetadataHeader()
		{
			base.WriteUInt32(1112167234U);
			base.WriteUInt16(1);
			base.WriteUInt16(1);
			base.WriteUInt32(0U);
			byte[] zeroTerminatedString = ImageWriter.GetZeroTerminatedString(this.runtime_version);
			base.WriteUInt32((uint)zeroTerminatedString.Length);
			base.WriteBytes(zeroTerminatedString);
			base.WriteUInt16(0);
			base.WriteUInt16(this.GetStreamCount());
			uint num = this.text_map.GetRVA(TextSegment.TableHeap) - this.text_map.GetRVA(TextSegment.MetadataHeader);
			this.WriteStreamHeader(ref num, TextSegment.TableHeap, "#~");
			this.WriteStreamHeader(ref num, TextSegment.StringHeap, "#Strings");
			this.WriteStreamHeader(ref num, TextSegment.UserStringHeap, "#US");
			this.WriteStreamHeader(ref num, TextSegment.GuidHeap, "#GUID");
			this.WriteStreamHeader(ref num, TextSegment.BlobHeap, "#Blob");
			this.WriteStreamHeader(ref num, TextSegment.PdbHeap, "#Pdb");
		}

		private ushort GetStreamCount()
		{
			return (ushort)(2 + ((!this.metadata.user_string_heap.IsEmpty) ? 1 : 0) + ((!this.metadata.guid_heap.IsEmpty) ? 1 : 0) + ((!this.metadata.blob_heap.IsEmpty) ? 1 : 0) + ((this.metadata.pdb_heap != null) ? 1 : 0));
		}

		private void WriteStreamHeader(ref uint offset, TextSegment heap, string name)
		{
			uint length = (uint)this.text_map.GetLength(heap);
			if (length == 0U)
			{
				return;
			}
			base.WriteUInt32(offset);
			base.WriteUInt32(length);
			base.WriteBytes(ImageWriter.GetZeroTerminatedString(name));
			offset += length;
		}

		private static int GetZeroTerminatedStringLength(string @string)
		{
			return (@string.Length + 1 + 3) & -4;
		}

		private static byte[] GetZeroTerminatedString(string @string)
		{
			return ImageWriter.GetString(@string, ImageWriter.GetZeroTerminatedStringLength(@string));
		}

		private static byte[] GetSimpleString(string @string)
		{
			return ImageWriter.GetString(@string, @string.Length);
		}

		private static byte[] GetString(string @string, int length)
		{
			byte[] array = new byte[length];
			for (int i = 0; i < @string.Length; i++)
			{
				array[i] = (byte)@string[i];
			}
			return array;
		}

		public void WriteMetadata()
		{
			this.WriteHeap(TextSegment.TableHeap, this.metadata.table_heap);
			this.WriteHeap(TextSegment.StringHeap, this.metadata.string_heap);
			this.WriteHeap(TextSegment.UserStringHeap, this.metadata.user_string_heap);
			this.WriteHeap(TextSegment.GuidHeap, this.metadata.guid_heap);
			this.WriteHeap(TextSegment.BlobHeap, this.metadata.blob_heap);
			this.WriteHeap(TextSegment.PdbHeap, this.metadata.pdb_heap);
		}

		private void WriteHeap(TextSegment heap, HeapBuffer buffer)
		{
			if (buffer == null || buffer.IsEmpty)
			{
				return;
			}
			this.MoveToRVA(heap);
			base.WriteBuffer(buffer);
		}

		private void WriteDebugDirectory()
		{
			int num = (int)this.BaseStream.Position + this.debug_header.Entries.Length * 28;
			for (int i = 0; i < this.debug_header.Entries.Length; i++)
			{
				ImageDebugHeaderEntry imageDebugHeaderEntry = this.debug_header.Entries[i];
				ImageDebugDirectory directory = imageDebugHeaderEntry.Directory;
				base.WriteInt32(directory.Characteristics);
				base.WriteInt32(directory.TimeDateStamp);
				base.WriteInt16(directory.MajorVersion);
				base.WriteInt16(directory.MinorVersion);
				base.WriteInt32((int)directory.Type);
				base.WriteInt32(directory.SizeOfData);
				base.WriteInt32(directory.AddressOfRawData);
				base.WriteInt32(num);
				num += imageDebugHeaderEntry.Data.Length;
			}
			this.debug_header_entries_position = this.BaseStream.Position;
			for (int j = 0; j < this.debug_header.Entries.Length; j++)
			{
				ImageDebugHeaderEntry imageDebugHeaderEntry2 = this.debug_header.Entries[j];
				base.WriteBytes(imageDebugHeaderEntry2.Data);
			}
		}

		private void WriteImportDirectory()
		{
			base.WriteUInt32(this.text_map.GetRVA(TextSegment.ImportDirectory) + 40U);
			base.WriteUInt32(0U);
			base.WriteUInt32(0U);
			base.WriteUInt32(this.text_map.GetRVA(TextSegment.ImportHintNameTable) + 14U);
			base.WriteUInt32(this.text_map.GetRVA(TextSegment.ImportAddressTable));
			base.Advance(20);
			base.WriteUInt32(this.text_map.GetRVA(TextSegment.ImportHintNameTable));
			this.MoveToRVA(TextSegment.ImportHintNameTable);
			base.WriteUInt16(0);
			base.WriteBytes(this.GetRuntimeMain());
			base.WriteByte(0);
			base.WriteBytes(ImageWriter.GetSimpleString("mscoree.dll"));
			base.WriteUInt16(0);
		}

		private byte[] GetRuntimeMain()
		{
			if (this.module.Kind != ModuleKind.Dll && this.module.Kind != ModuleKind.NetModule)
			{
				return ImageWriter.GetSimpleString("_CorExeMain");
			}
			return ImageWriter.GetSimpleString("_CorDllMain");
		}

		private void WriteStartupStub()
		{
			if (this.module.Architecture == TargetArchitecture.I386)
			{
				base.WriteUInt16(9727);
				base.WriteUInt32(4194304U + this.text_map.GetRVA(TextSegment.ImportAddressTable));
				return;
			}
			throw new NotSupportedException();
		}

		private void WriteRsrc()
		{
			this.PrepareSection(this.rsrc);
			base.WriteBuffer(this.win32_resources);
		}

		private void WriteReloc()
		{
			this.PrepareSection(this.reloc);
			uint num = this.text_map.GetRVA(TextSegment.StartupStub);
			num += ((this.module.Architecture == TargetArchitecture.IA64) ? 32U : 2U);
			uint num2 = num & 4294963200U;
			base.WriteUInt32(num2);
			base.WriteUInt32(12U);
			if (this.module.Architecture == TargetArchitecture.I386)
			{
				base.WriteUInt32(12288U + num - num2);
				return;
			}
			throw new NotSupportedException();
		}

		public void WriteImage()
		{
			this.WriteDOSHeader();
			this.WritePEFileHeader();
			this.WriteOptionalHeaders();
			this.WriteSectionHeaders();
			this.WriteText();
			if (this.rsrc != null)
			{
				this.WriteRsrc();
			}
			if (this.reloc != null)
			{
				this.WriteReloc();
			}
			this.Flush();
		}

		private void BuildTextMap()
		{
			TextMap textMap = this.text_map;
			textMap.AddMap(TextSegment.Code, this.metadata.code.length, (!this.pe64) ? 4 : 16);
			textMap.AddMap(TextSegment.Resources, this.metadata.resources.length, 8);
			textMap.AddMap(TextSegment.Data, this.metadata.data.length, this.metadata.data.BufferAlign);
			if (this.metadata.data.length > 0)
			{
				this.metadata.table_heap.FixupData(textMap.GetRVA(TextSegment.Data));
			}
			textMap.AddMap(TextSegment.StrongNameSignature, this.GetStrongNameLength(), 4);
			this.BuildMetadataTextMap();
			int num = 0;
			if (this.debug_header != null && this.debug_header.HasEntries)
			{
				int num2 = this.debug_header.Entries.Length * 28;
				int num3 = (int)(textMap.GetNextRVA(TextSegment.BlobHeap) + (uint)num2);
				int num4 = 0;
				for (int i = 0; i < this.debug_header.Entries.Length; i++)
				{
					ImageDebugHeaderEntry imageDebugHeaderEntry = this.debug_header.Entries[i];
					ImageDebugDirectory directory = imageDebugHeaderEntry.Directory;
					directory.AddressOfRawData = ((imageDebugHeaderEntry.Data.Length == 0) ? 0 : num3);
					imageDebugHeaderEntry.Directory = directory;
					num4 += imageDebugHeaderEntry.Data.Length;
					num3 += imageDebugHeaderEntry.Data.Length;
				}
				num = num2 + num4;
			}
			textMap.AddMap(TextSegment.DebugDirectory, num, 4);
			if (!this.has_reloc)
			{
				uint nextRVA = textMap.GetNextRVA(TextSegment.DebugDirectory);
				textMap.AddMap(TextSegment.ImportDirectory, new Range(nextRVA, 0U));
				textMap.AddMap(TextSegment.ImportHintNameTable, new Range(nextRVA, 0U));
				textMap.AddMap(TextSegment.StartupStub, new Range(nextRVA, 0U));
				return;
			}
			uint nextRVA2 = textMap.GetNextRVA(TextSegment.DebugDirectory);
			uint num5 = nextRVA2 + 48U;
			num5 = (num5 + 15U) & 4294967280U;
			uint num6 = num5 - nextRVA2 + 27U;
			uint num7 = nextRVA2 + num6;
			num7 = ((this.module.Architecture == TargetArchitecture.IA64) ? ((num7 + 15U) & 4294967280U) : (2U + ((num7 + 3U) & 4294967292U)));
			textMap.AddMap(TextSegment.ImportDirectory, new Range(nextRVA2, num6));
			textMap.AddMap(TextSegment.ImportHintNameTable, new Range(num5, 0U));
			textMap.AddMap(TextSegment.StartupStub, new Range(num7, this.GetStartupStubLength()));
		}

		public void BuildMetadataTextMap()
		{
			TextMap textMap = this.text_map;
			textMap.AddMap(TextSegment.MetadataHeader, this.GetMetadataHeaderLength(this.module.RuntimeVersion));
			textMap.AddMap(TextSegment.TableHeap, this.metadata.table_heap.length, 4);
			textMap.AddMap(TextSegment.StringHeap, this.metadata.string_heap.length, 4);
			textMap.AddMap(TextSegment.UserStringHeap, this.metadata.user_string_heap.IsEmpty ? 0 : this.metadata.user_string_heap.length, 4);
			textMap.AddMap(TextSegment.GuidHeap, this.metadata.guid_heap.length, 4);
			textMap.AddMap(TextSegment.BlobHeap, this.metadata.blob_heap.IsEmpty ? 0 : this.metadata.blob_heap.length, 4);
			textMap.AddMap(TextSegment.PdbHeap, (this.metadata.pdb_heap == null) ? 0 : this.metadata.pdb_heap.length, 4);
		}

		private uint GetStartupStubLength()
		{
			if (this.module.Architecture == TargetArchitecture.I386)
			{
				return 6U;
			}
			throw new NotSupportedException();
		}

		private int GetMetadataHeaderLength(string runtimeVersion)
		{
			return 20 + ImageWriter.GetZeroTerminatedStringLength(runtimeVersion) + 12 + 20 + (this.metadata.user_string_heap.IsEmpty ? 0 : 12) + 16 + (this.metadata.blob_heap.IsEmpty ? 0 : 16) + ((this.metadata.pdb_heap == null) ? 0 : 16);
		}

		private int GetStrongNameLength()
		{
			if (this.module.kind == ModuleKind.NetModule || this.module.Assembly == null)
			{
				return 0;
			}
			byte[] publicKey = this.module.Assembly.Name.PublicKey;
			if (publicKey.IsNullOrEmpty<byte>())
			{
				return 0;
			}
			int num = publicKey.Length;
			if (num > 32)
			{
				return num - 32;
			}
			return 128;
		}

		public DataDirectory GetStrongNameSignatureDirectory()
		{
			return this.text_map.GetDataDirectory(TextSegment.StrongNameSignature);
		}

		public uint GetHeaderSize()
		{
			return (uint)(152 + this.SizeOfOptionalHeader() + this.sections * 40);
		}

		private void PatchWin32Resources(ByteBuffer resources)
		{
			this.PatchResourceDirectoryTable(resources);
		}

		private void PatchResourceDirectoryTable(ByteBuffer resources)
		{
			resources.Advance(12);
			int num = (int)(resources.ReadUInt16() + resources.ReadUInt16());
			for (int i = 0; i < num; i++)
			{
				this.PatchResourceDirectoryEntry(resources);
			}
		}

		private void PatchResourceDirectoryEntry(ByteBuffer resources)
		{
			resources.Advance(4);
			uint num = resources.ReadUInt32();
			int position = resources.position;
			resources.position = (int)(num & 2147483647U);
			if ((num & 2147483648U) != 0U)
			{
				this.PatchResourceDirectoryTable(resources);
			}
			else
			{
				this.PatchResourceDataEntry(resources);
			}
			resources.position = position;
		}

		private void PatchResourceDataEntry(ByteBuffer resources)
		{
			uint num = resources.ReadUInt32();
			resources.position -= 4;
			resources.WriteUInt32(num - this.module.Image.Win32Resources.VirtualAddress + this.rsrc.VirtualAddress);
		}

		private readonly ModuleDefinition module;

		private readonly MetadataBuilder metadata;

		private readonly TextMap text_map;

		internal readonly Disposable<Stream> stream;

		private readonly string runtime_version;

		private ImageDebugHeader debug_header;

		private ByteBuffer win32_resources;

		private const uint pe_header_size = 152U;

		private const uint section_header_size = 40U;

		private const uint file_alignment = 512U;

		private const uint section_alignment = 8192U;

		private const ulong image_base = 4194304UL;

		internal const uint text_rva = 8192U;

		private readonly bool pe64;

		private readonly bool has_reloc;

		internal Section text;

		internal Section rsrc;

		internal Section reloc;

		private ushort sections;

		internal long debug_header_entries_position;
	}
}
