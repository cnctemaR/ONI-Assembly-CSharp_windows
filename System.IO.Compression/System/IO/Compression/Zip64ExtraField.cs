using System;
using System.Collections.Generic;

namespace System.IO.Compression
{
	internal struct Zip64ExtraField
	{
		public ushort TotalSize
		{
			get
			{
				return this._size + 4;
			}
		}

		public long? UncompressedSize
		{
			get
			{
				return this._uncompressedSize;
			}
			set
			{
				this._uncompressedSize = value;
				this.UpdateSize();
			}
		}

		public long? CompressedSize
		{
			get
			{
				return this._compressedSize;
			}
			set
			{
				this._compressedSize = value;
				this.UpdateSize();
			}
		}

		public long? LocalHeaderOffset
		{
			get
			{
				return this._localHeaderOffset;
			}
			set
			{
				this._localHeaderOffset = value;
				this.UpdateSize();
			}
		}

		public int? StartDiskNumber
		{
			get
			{
				return this._startDiskNumber;
			}
		}

		private void UpdateSize()
		{
			this._size = 0;
			if (this._uncompressedSize != null)
			{
				this._size += 8;
			}
			if (this._compressedSize != null)
			{
				this._size += 8;
			}
			if (this._localHeaderOffset != null)
			{
				this._size += 8;
			}
			if (this._startDiskNumber != null)
			{
				this._size += 4;
			}
		}

		public static Zip64ExtraField GetJustZip64Block(Stream extraFieldStream, bool readUncompressedSize, bool readCompressedSize, bool readLocalHeaderOffset, bool readStartDiskNumber)
		{
			Zip64ExtraField zip64ExtraField;
			using (BinaryReader binaryReader = new BinaryReader(extraFieldStream))
			{
				ZipGenericExtraField zipGenericExtraField;
				while (ZipGenericExtraField.TryReadBlock(binaryReader, extraFieldStream.Length, out zipGenericExtraField))
				{
					if (Zip64ExtraField.TryGetZip64BlockFromGenericExtraField(zipGenericExtraField, readUncompressedSize, readCompressedSize, readLocalHeaderOffset, readStartDiskNumber, out zip64ExtraField))
					{
						return zip64ExtraField;
					}
				}
			}
			zip64ExtraField = new Zip64ExtraField
			{
				_compressedSize = null,
				_uncompressedSize = null,
				_localHeaderOffset = null,
				_startDiskNumber = null
			};
			return zip64ExtraField;
		}

		private static bool TryGetZip64BlockFromGenericExtraField(ZipGenericExtraField extraField, bool readUncompressedSize, bool readCompressedSize, bool readLocalHeaderOffset, bool readStartDiskNumber, out Zip64ExtraField zip64Block)
		{
			zip64Block = default(Zip64ExtraField);
			zip64Block._compressedSize = null;
			zip64Block._uncompressedSize = null;
			zip64Block._localHeaderOffset = null;
			zip64Block._startDiskNumber = null;
			if (extraField.Tag != 1)
			{
				return false;
			}
			MemoryStream memoryStream = null;
			bool flag;
			try
			{
				memoryStream = new MemoryStream(extraField.Data);
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					memoryStream = null;
					zip64Block._size = extraField.Size;
					ushort num = 0;
					if (readUncompressedSize)
					{
						num += 8;
					}
					if (readCompressedSize)
					{
						num += 8;
					}
					if (readLocalHeaderOffset)
					{
						num += 8;
					}
					if (readStartDiskNumber)
					{
						num += 4;
					}
					if (num != zip64Block._size)
					{
						flag = false;
					}
					else
					{
						if (readUncompressedSize)
						{
							zip64Block._uncompressedSize = new long?(binaryReader.ReadInt64());
						}
						if (readCompressedSize)
						{
							zip64Block._compressedSize = new long?(binaryReader.ReadInt64());
						}
						if (readLocalHeaderOffset)
						{
							zip64Block._localHeaderOffset = new long?(binaryReader.ReadInt64());
						}
						if (readStartDiskNumber)
						{
							zip64Block._startDiskNumber = new int?(binaryReader.ReadInt32());
						}
						long? num2 = zip64Block._uncompressedSize;
						long num3 = 0L;
						if ((num2.GetValueOrDefault() < num3) & (num2 != null))
						{
							throw new InvalidDataException("Uncompressed Size cannot be held in an Int64.");
						}
						num2 = zip64Block._compressedSize;
						num3 = 0L;
						if ((num2.GetValueOrDefault() < num3) & (num2 != null))
						{
							throw new InvalidDataException("Compressed Size cannot be held in an Int64.");
						}
						num2 = zip64Block._localHeaderOffset;
						num3 = 0L;
						if ((num2.GetValueOrDefault() < num3) & (num2 != null))
						{
							throw new InvalidDataException("Local Header Offset cannot be held in an Int64.");
						}
						int? startDiskNumber = zip64Block._startDiskNumber;
						int num4 = 0;
						if ((startDiskNumber.GetValueOrDefault() < num4) & (startDiskNumber != null))
						{
							throw new InvalidDataException("Start Disk Number cannot be held in an Int64.");
						}
						flag = true;
					}
				}
			}
			finally
			{
				if (memoryStream != null)
				{
					memoryStream.Dispose();
				}
			}
			return flag;
		}

		public static Zip64ExtraField GetAndRemoveZip64Block(List<ZipGenericExtraField> extraFields, bool readUncompressedSize, bool readCompressedSize, bool readLocalHeaderOffset, bool readStartDiskNumber)
		{
			Zip64ExtraField zip64ExtraField = default(Zip64ExtraField);
			zip64ExtraField._compressedSize = null;
			zip64ExtraField._uncompressedSize = null;
			zip64ExtraField._localHeaderOffset = null;
			zip64ExtraField._startDiskNumber = null;
			List<ZipGenericExtraField> list = new List<ZipGenericExtraField>();
			bool flag = false;
			foreach (ZipGenericExtraField zipGenericExtraField in extraFields)
			{
				if (zipGenericExtraField.Tag == 1)
				{
					list.Add(zipGenericExtraField);
					if (!flag && Zip64ExtraField.TryGetZip64BlockFromGenericExtraField(zipGenericExtraField, readUncompressedSize, readCompressedSize, readLocalHeaderOffset, readStartDiskNumber, out zip64ExtraField))
					{
						flag = true;
					}
				}
			}
			foreach (ZipGenericExtraField zipGenericExtraField2 in list)
			{
				extraFields.Remove(zipGenericExtraField2);
			}
			return zip64ExtraField;
		}

		public static void RemoveZip64Blocks(List<ZipGenericExtraField> extraFields)
		{
			List<ZipGenericExtraField> list = new List<ZipGenericExtraField>();
			foreach (ZipGenericExtraField zipGenericExtraField in extraFields)
			{
				if (zipGenericExtraField.Tag == 1)
				{
					list.Add(zipGenericExtraField);
				}
			}
			foreach (ZipGenericExtraField zipGenericExtraField2 in list)
			{
				extraFields.Remove(zipGenericExtraField2);
			}
		}

		public void WriteBlock(Stream stream)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			binaryWriter.Write(1);
			binaryWriter.Write(this._size);
			if (this._uncompressedSize != null)
			{
				binaryWriter.Write(this._uncompressedSize.Value);
			}
			if (this._compressedSize != null)
			{
				binaryWriter.Write(this._compressedSize.Value);
			}
			if (this._localHeaderOffset != null)
			{
				binaryWriter.Write(this._localHeaderOffset.Value);
			}
			if (this._startDiskNumber != null)
			{
				binaryWriter.Write(this._startDiskNumber.Value);
			}
		}

		public const int OffsetToFirstField = 4;

		private const ushort TagConstant = 1;

		private ushort _size;

		private long? _uncompressedSize;

		private long? _compressedSize;

		private long? _localHeaderOffset;

		private int? _startDiskNumber;
	}
}
