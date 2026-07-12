using System;

namespace System.IO.Compression
{
	internal struct ZipEndOfCentralDirectoryBlock
	{
		public static void WriteBlock(Stream stream, long numberOfEntries, long startOfCentralDirectory, long sizeOfCentralDirectory, byte[] archiveComment)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			ushort num = ((numberOfEntries > 65535L) ? ushort.MaxValue : ((ushort)numberOfEntries));
			uint num2 = ((startOfCentralDirectory > (long)((ulong)(-1))) ? uint.MaxValue : ((uint)startOfCentralDirectory));
			uint num3 = ((sizeOfCentralDirectory > (long)((ulong)(-1))) ? uint.MaxValue : ((uint)sizeOfCentralDirectory));
			binaryWriter.Write(101010256U);
			binaryWriter.Write(0);
			binaryWriter.Write(0);
			binaryWriter.Write(num);
			binaryWriter.Write(num);
			binaryWriter.Write(num3);
			binaryWriter.Write(num2);
			binaryWriter.Write((archiveComment != null) ? ((ushort)archiveComment.Length) : 0);
			if (archiveComment != null)
			{
				binaryWriter.Write(archiveComment);
			}
		}

		public static bool TryReadBlock(BinaryReader reader, out ZipEndOfCentralDirectoryBlock eocdBlock)
		{
			eocdBlock = default(ZipEndOfCentralDirectoryBlock);
			if (reader.ReadUInt32() != 101010256U)
			{
				return false;
			}
			eocdBlock.Signature = 101010256U;
			eocdBlock.NumberOfThisDisk = reader.ReadUInt16();
			eocdBlock.NumberOfTheDiskWithTheStartOfTheCentralDirectory = reader.ReadUInt16();
			eocdBlock.NumberOfEntriesInTheCentralDirectoryOnThisDisk = reader.ReadUInt16();
			eocdBlock.NumberOfEntriesInTheCentralDirectory = reader.ReadUInt16();
			eocdBlock.SizeOfCentralDirectory = reader.ReadUInt32();
			eocdBlock.OffsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber = reader.ReadUInt32();
			ushort num = reader.ReadUInt16();
			eocdBlock.ArchiveComment = reader.ReadBytes((int)num);
			return true;
		}

		public const uint SignatureConstant = 101010256U;

		public const int SizeOfBlockWithoutSignature = 18;

		public uint Signature;

		public ushort NumberOfThisDisk;

		public ushort NumberOfTheDiskWithTheStartOfTheCentralDirectory;

		public ushort NumberOfEntriesInTheCentralDirectoryOnThisDisk;

		public ushort NumberOfEntriesInTheCentralDirectory;

		public uint SizeOfCentralDirectory;

		public uint OffsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber;

		public byte[] ArchiveComment;
	}
}
