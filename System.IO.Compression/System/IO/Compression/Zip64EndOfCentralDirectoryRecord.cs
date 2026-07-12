using System;

namespace System.IO.Compression
{
	internal struct Zip64EndOfCentralDirectoryRecord
	{
		public static bool TryReadBlock(BinaryReader reader, out Zip64EndOfCentralDirectoryRecord zip64EOCDRecord)
		{
			zip64EOCDRecord = default(Zip64EndOfCentralDirectoryRecord);
			if (reader.ReadUInt32() != 101075792U)
			{
				return false;
			}
			zip64EOCDRecord.SizeOfThisRecord = reader.ReadUInt64();
			zip64EOCDRecord.VersionMadeBy = reader.ReadUInt16();
			zip64EOCDRecord.VersionNeededToExtract = reader.ReadUInt16();
			zip64EOCDRecord.NumberOfThisDisk = reader.ReadUInt32();
			zip64EOCDRecord.NumberOfDiskWithStartOfCD = reader.ReadUInt32();
			zip64EOCDRecord.NumberOfEntriesOnThisDisk = reader.ReadUInt64();
			zip64EOCDRecord.NumberOfEntriesTotal = reader.ReadUInt64();
			zip64EOCDRecord.SizeOfCentralDirectory = reader.ReadUInt64();
			zip64EOCDRecord.OffsetOfCentralDirectory = reader.ReadUInt64();
			return true;
		}

		public static void WriteBlock(Stream stream, long numberOfEntries, long startOfCentralDirectory, long sizeOfCentralDirectory)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			binaryWriter.Write(101075792U);
			binaryWriter.Write(44UL);
			binaryWriter.Write(45);
			binaryWriter.Write(45);
			binaryWriter.Write(0U);
			binaryWriter.Write(0U);
			binaryWriter.Write(numberOfEntries);
			binaryWriter.Write(numberOfEntries);
			binaryWriter.Write(sizeOfCentralDirectory);
			binaryWriter.Write(startOfCentralDirectory);
		}

		private const uint SignatureConstant = 101075792U;

		private const ulong NormalSize = 44UL;

		public ulong SizeOfThisRecord;

		public ushort VersionMadeBy;

		public ushort VersionNeededToExtract;

		public uint NumberOfThisDisk;

		public uint NumberOfDiskWithStartOfCD;

		public ulong NumberOfEntriesOnThisDisk;

		public ulong NumberOfEntriesTotal;

		public ulong SizeOfCentralDirectory;

		public ulong OffsetOfCentralDirectory;
	}
}
