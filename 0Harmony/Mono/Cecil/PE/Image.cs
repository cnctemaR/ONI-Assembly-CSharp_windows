using System;
using System.IO;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;

namespace Mono.Cecil.PE
{
	internal sealed class Image : IDisposable
	{
		public Image()
		{
			this.counter = new Func<Table, int>(this.GetTableLength);
		}

		public bool HasTable(Table table)
		{
			return this.GetTableLength(table) > 0;
		}

		public int GetTableLength(Table table)
		{
			return (int)this.TableHeap[table].Length;
		}

		public int GetTableIndexSize(Table table)
		{
			if (this.GetTableLength(table) >= 65536)
			{
				return 4;
			}
			return 2;
		}

		public int GetCodedIndexSize(CodedIndex coded_index)
		{
			int num = this.coded_index_sizes[(int)coded_index];
			if (num != 0)
			{
				return num;
			}
			return this.coded_index_sizes[(int)coded_index] = coded_index.GetSize(this.counter);
		}

		public uint ResolveVirtualAddress(uint rva)
		{
			Section sectionAtVirtualAddress = this.GetSectionAtVirtualAddress(rva);
			if (sectionAtVirtualAddress == null)
			{
				throw new ArgumentOutOfRangeException();
			}
			return this.ResolveVirtualAddressInSection(rva, sectionAtVirtualAddress);
		}

		public uint ResolveVirtualAddressInSection(uint rva, Section section)
		{
			return rva + section.PointerToRawData - section.VirtualAddress;
		}

		public Section GetSection(string name)
		{
			foreach (Section section in this.Sections)
			{
				if (section.Name == name)
				{
					return section;
				}
			}
			return null;
		}

		public Section GetSectionAtVirtualAddress(uint rva)
		{
			foreach (Section section in this.Sections)
			{
				if (rva >= section.VirtualAddress && rva < section.VirtualAddress + section.SizeOfRawData)
				{
					return section;
				}
			}
			return null;
		}

		private BinaryStreamReader GetReaderAt(uint rva)
		{
			Section sectionAtVirtualAddress = this.GetSectionAtVirtualAddress(rva);
			if (sectionAtVirtualAddress == null)
			{
				return null;
			}
			BinaryStreamReader binaryStreamReader = new BinaryStreamReader(this.Stream.value);
			binaryStreamReader.MoveTo(this.ResolveVirtualAddressInSection(rva, sectionAtVirtualAddress));
			return binaryStreamReader;
		}

		public TRet GetReaderAt<TItem, TRet>(uint rva, TItem item, Func<TItem, BinaryStreamReader, TRet> read) where TRet : class
		{
			long position = this.Stream.value.Position;
			TRet tret;
			try
			{
				BinaryStreamReader readerAt = this.GetReaderAt(rva);
				if (readerAt == null)
				{
					tret = default(TRet);
					tret = tret;
				}
				else
				{
					tret = read(item, readerAt);
				}
			}
			finally
			{
				this.Stream.value.Position = position;
			}
			return tret;
		}

		public bool HasDebugTables()
		{
			return this.HasTable(Table.Document) || this.HasTable(Table.MethodDebugInformation) || this.HasTable(Table.LocalScope) || this.HasTable(Table.LocalVariable) || this.HasTable(Table.LocalConstant) || this.HasTable(Table.StateMachineMethod) || this.HasTable(Table.CustomDebugInformation);
		}

		public void Dispose()
		{
			this.Stream.Dispose();
		}

		public Disposable<Stream> Stream;

		public string FileName;

		public ModuleKind Kind;

		public uint Characteristics;

		public string RuntimeVersion;

		public TargetArchitecture Architecture;

		public ModuleCharacteristics DllCharacteristics;

		public ushort LinkerVersion;

		public ushort SubSystemMajor;

		public ushort SubSystemMinor;

		public ImageDebugHeader DebugHeader;

		public Section[] Sections;

		public Section MetadataSection;

		public uint EntryPointToken;

		public uint Timestamp;

		public ModuleAttributes Attributes;

		public DataDirectory Win32Resources;

		public DataDirectory Debug;

		public DataDirectory Resources;

		public DataDirectory StrongName;

		public StringHeap StringHeap;

		public BlobHeap BlobHeap;

		public UserStringHeap UserStringHeap;

		public GuidHeap GuidHeap;

		public TableHeap TableHeap;

		public PdbHeap PdbHeap;

		private readonly int[] coded_index_sizes = new int[14];

		private readonly Func<Table, int> counter;
	}
}
