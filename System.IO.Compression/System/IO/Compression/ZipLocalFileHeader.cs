using System;
using System.Collections.Generic;

namespace System.IO.Compression
{
	internal readonly struct ZipLocalFileHeader
	{
		public static List<ZipGenericExtraField> GetExtraFields(BinaryReader reader)
		{
			reader.BaseStream.Seek(26L, SeekOrigin.Current);
			ushort num = reader.ReadUInt16();
			ushort num2 = reader.ReadUInt16();
			reader.BaseStream.Seek((long)((ulong)num), SeekOrigin.Current);
			List<ZipGenericExtraField> list;
			using (Stream stream = new SubReadStream(reader.BaseStream, reader.BaseStream.Position, (long)((ulong)num2)))
			{
				list = ZipGenericExtraField.ParseExtraField(stream);
			}
			Zip64ExtraField.RemoveZip64Blocks(list);
			return list;
		}

		public static bool TrySkipBlock(BinaryReader reader)
		{
			if (reader.ReadUInt32() != 67324752U)
			{
				return false;
			}
			if (reader.BaseStream.Length < reader.BaseStream.Position + 22L)
			{
				return false;
			}
			reader.BaseStream.Seek(22L, SeekOrigin.Current);
			ushort num = reader.ReadUInt16();
			ushort num2 = reader.ReadUInt16();
			if (reader.BaseStream.Length < reader.BaseStream.Position + (long)((ulong)num) + (long)((ulong)num2))
			{
				return false;
			}
			reader.BaseStream.Seek((long)(num + num2), SeekOrigin.Current);
			return true;
		}

		public const uint DataDescriptorSignature = 134695760U;

		public const uint SignatureConstant = 67324752U;

		public const int OffsetToCrcFromHeaderStart = 14;

		public const int OffsetToBitFlagFromHeaderStart = 6;

		public const int SizeOfLocalHeader = 30;
	}
}
