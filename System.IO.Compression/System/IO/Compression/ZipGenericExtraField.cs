using System;
using System.Collections.Generic;

namespace System.IO.Compression
{
	internal struct ZipGenericExtraField
	{
		public ushort Tag
		{
			get
			{
				return this._tag;
			}
		}

		public ushort Size
		{
			get
			{
				return this._size;
			}
		}

		public byte[] Data
		{
			get
			{
				return this._data;
			}
		}

		public void WriteBlock(Stream stream)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			binaryWriter.Write(this.Tag);
			binaryWriter.Write(this.Size);
			binaryWriter.Write(this.Data);
		}

		public static bool TryReadBlock(BinaryReader reader, long endExtraField, out ZipGenericExtraField field)
		{
			field = default(ZipGenericExtraField);
			if (endExtraField - reader.BaseStream.Position < 4L)
			{
				return false;
			}
			field._tag = reader.ReadUInt16();
			field._size = reader.ReadUInt16();
			if (endExtraField - reader.BaseStream.Position < (long)((ulong)field._size))
			{
				return false;
			}
			field._data = reader.ReadBytes((int)field._size);
			return true;
		}

		public static List<ZipGenericExtraField> ParseExtraField(Stream extraFieldData)
		{
			List<ZipGenericExtraField> list = new List<ZipGenericExtraField>();
			using (BinaryReader binaryReader = new BinaryReader(extraFieldData))
			{
				ZipGenericExtraField zipGenericExtraField;
				while (ZipGenericExtraField.TryReadBlock(binaryReader, extraFieldData.Length, out zipGenericExtraField))
				{
					list.Add(zipGenericExtraField);
				}
			}
			return list;
		}

		public static int TotalSize(List<ZipGenericExtraField> fields)
		{
			int num = 0;
			foreach (ZipGenericExtraField zipGenericExtraField in fields)
			{
				num += (int)(zipGenericExtraField.Size + 4);
			}
			return num;
		}

		public static void WriteAllBlocks(List<ZipGenericExtraField> fields, Stream stream)
		{
			foreach (ZipGenericExtraField zipGenericExtraField in fields)
			{
				zipGenericExtraField.WriteBlock(stream);
			}
		}

		private const int SizeOfHeader = 4;

		private ushort _tag;

		private ushort _size;

		private byte[] _data;
	}
}
