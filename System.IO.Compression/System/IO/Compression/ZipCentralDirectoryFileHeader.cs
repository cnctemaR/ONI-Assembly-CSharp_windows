using System;
using System.Collections.Generic;

namespace System.IO.Compression
{
	internal struct ZipCentralDirectoryFileHeader
	{
		public static bool TryReadBlock(BinaryReader reader, bool saveExtraFieldsAndComments, out ZipCentralDirectoryFileHeader header)
		{
			header = default(ZipCentralDirectoryFileHeader);
			if (reader.ReadUInt32() != 33639248U)
			{
				return false;
			}
			header.VersionMadeBySpecification = reader.ReadByte();
			header.VersionMadeByCompatibility = reader.ReadByte();
			header.VersionNeededToExtract = reader.ReadUInt16();
			header.GeneralPurposeBitFlag = reader.ReadUInt16();
			header.CompressionMethod = reader.ReadUInt16();
			header.LastModified = reader.ReadUInt32();
			header.Crc32 = reader.ReadUInt32();
			uint num = reader.ReadUInt32();
			uint num2 = reader.ReadUInt32();
			header.FilenameLength = reader.ReadUInt16();
			header.ExtraFieldLength = reader.ReadUInt16();
			header.FileCommentLength = reader.ReadUInt16();
			ushort num3 = reader.ReadUInt16();
			header.InternalFileAttributes = reader.ReadUInt16();
			header.ExternalFileAttributes = reader.ReadUInt32();
			uint num4 = reader.ReadUInt32();
			header.Filename = reader.ReadBytes((int)header.FilenameLength);
			bool flag = num2 == uint.MaxValue;
			bool flag2 = num == uint.MaxValue;
			bool flag3 = num4 == uint.MaxValue;
			bool flag4 = num3 == ushort.MaxValue;
			long num5 = reader.BaseStream.Position + (long)((ulong)header.ExtraFieldLength);
			Zip64ExtraField zip64ExtraField;
			using (Stream stream = new SubReadStream(reader.BaseStream, reader.BaseStream.Position, (long)((ulong)header.ExtraFieldLength)))
			{
				if (saveExtraFieldsAndComments)
				{
					header.ExtraFields = ZipGenericExtraField.ParseExtraField(stream);
					zip64ExtraField = Zip64ExtraField.GetAndRemoveZip64Block(header.ExtraFields, flag, flag2, flag3, flag4);
				}
				else
				{
					header.ExtraFields = null;
					zip64ExtraField = Zip64ExtraField.GetJustZip64Block(stream, flag, flag2, flag3, flag4);
				}
			}
			reader.BaseStream.AdvanceToPosition(num5);
			if (saveExtraFieldsAndComments)
			{
				header.FileComment = reader.ReadBytes((int)header.FileCommentLength);
			}
			else
			{
				reader.BaseStream.Position += (long)((ulong)header.FileCommentLength);
				header.FileComment = null;
			}
			header.UncompressedSize = (long)((zip64ExtraField.UncompressedSize == null) ? ((ulong)num2) : ((ulong)zip64ExtraField.UncompressedSize.Value));
			header.CompressedSize = (long)((zip64ExtraField.CompressedSize == null) ? ((ulong)num) : ((ulong)zip64ExtraField.CompressedSize.Value));
			header.RelativeOffsetOfLocalHeader = (long)((zip64ExtraField.LocalHeaderOffset == null) ? ((ulong)num4) : ((ulong)zip64ExtraField.LocalHeaderOffset.Value));
			header.DiskNumberStart = ((zip64ExtraField.StartDiskNumber == null) ? ((int)num3) : zip64ExtraField.StartDiskNumber.Value);
			return true;
		}

		public const uint SignatureConstant = 33639248U;

		public byte VersionMadeByCompatibility;

		public byte VersionMadeBySpecification;

		public ushort VersionNeededToExtract;

		public ushort GeneralPurposeBitFlag;

		public ushort CompressionMethod;

		public uint LastModified;

		public uint Crc32;

		public long CompressedSize;

		public long UncompressedSize;

		public ushort FilenameLength;

		public ushort ExtraFieldLength;

		public ushort FileCommentLength;

		public int DiskNumberStart;

		public ushort InternalFileAttributes;

		public uint ExternalFileAttributes;

		public long RelativeOffsetOfLocalHeader;

		public byte[] Filename;

		public byte[] FileComment;

		public List<ZipGenericExtraField> ExtraFields;
	}
}
