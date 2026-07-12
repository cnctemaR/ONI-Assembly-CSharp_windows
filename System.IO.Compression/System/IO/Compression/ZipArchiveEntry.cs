using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace System.IO.Compression
{
	public class ZipArchiveEntry
	{
		internal ZipArchiveEntry(ZipArchive archive, ZipCentralDirectoryFileHeader cd)
		{
			this._archive = archive;
			this._originallyInArchive = true;
			this._diskNumberStart = cd.DiskNumberStart;
			this._versionMadeByPlatform = (ZipVersionMadeByPlatform)cd.VersionMadeByCompatibility;
			this._versionMadeBySpecification = (ZipVersionNeededValues)cd.VersionMadeBySpecification;
			this._versionToExtract = (ZipVersionNeededValues)cd.VersionNeededToExtract;
			this._generalPurposeBitFlag = (ZipArchiveEntry.BitFlagValues)cd.GeneralPurposeBitFlag;
			this.CompressionMethod = (ZipArchiveEntry.CompressionMethodValues)cd.CompressionMethod;
			this._lastModified = new DateTimeOffset(ZipHelper.DosTimeToDateTime(cd.LastModified));
			this._compressedSize = cd.CompressedSize;
			this._uncompressedSize = cd.UncompressedSize;
			this._externalFileAttr = cd.ExternalFileAttributes;
			this._offsetOfLocalHeader = cd.RelativeOffsetOfLocalHeader;
			this._storedOffsetOfCompressedData = null;
			this._crc32 = cd.Crc32;
			this._compressedBytes = null;
			this._storedUncompressedData = null;
			this._currentlyOpenForWrite = false;
			this._everOpenedForWrite = false;
			this._outstandingWriteStream = null;
			this.FullName = this.DecodeEntryName(cd.Filename);
			this._lhUnknownExtraFields = null;
			this._cdUnknownExtraFields = cd.ExtraFields;
			this._fileComment = cd.FileComment;
			this._compressionLevel = null;
		}

		internal ZipArchiveEntry(ZipArchive archive, string entryName, CompressionLevel compressionLevel)
			: this(archive, entryName)
		{
			this._compressionLevel = new CompressionLevel?(compressionLevel);
		}

		internal ZipArchiveEntry(ZipArchive archive, string entryName)
		{
			this._archive = archive;
			this._originallyInArchive = false;
			this._diskNumberStart = 0;
			this._versionMadeByPlatform = ZipArchiveEntry.CurrentZipPlatform;
			this._versionMadeBySpecification = ZipVersionNeededValues.Default;
			this._versionToExtract = ZipVersionNeededValues.Default;
			this._generalPurposeBitFlag = (ZipArchiveEntry.BitFlagValues)0;
			this.CompressionMethod = ZipArchiveEntry.CompressionMethodValues.Deflate;
			this._lastModified = DateTimeOffset.Now;
			this._compressedSize = 0L;
			this._uncompressedSize = 0L;
			this._externalFileAttr = 0U;
			this._offsetOfLocalHeader = 0L;
			this._storedOffsetOfCompressedData = null;
			this._crc32 = 0U;
			this._compressedBytes = null;
			this._storedUncompressedData = null;
			this._currentlyOpenForWrite = false;
			this._everOpenedForWrite = false;
			this._outstandingWriteStream = null;
			this.FullName = entryName;
			this._cdUnknownExtraFields = null;
			this._lhUnknownExtraFields = null;
			this._fileComment = null;
			this._compressionLevel = null;
			if (this._storedEntryNameBytes.Length > 65535)
			{
				throw new ArgumentException("Entry names cannot require more than 2^16 bits.");
			}
			if (this._archive.Mode == ZipArchiveMode.Create)
			{
				this._archive.AcquireArchiveStream(this);
			}
		}

		public ZipArchive Archive
		{
			get
			{
				return this._archive;
			}
		}

		public long CompressedLength
		{
			get
			{
				if (this._everOpenedForWrite)
				{
					throw new InvalidOperationException("Length properties are unavailable once an entry has been opened for writing.");
				}
				return this._compressedSize;
			}
		}

		public int ExternalAttributes
		{
			get
			{
				return (int)this._externalFileAttr;
			}
			set
			{
				this.ThrowIfInvalidArchive();
				this._externalFileAttr = (uint)value;
			}
		}

		public string FullName
		{
			get
			{
				return this._storedEntryName;
			}
			private set
			{
				if (value == null)
				{
					throw new ArgumentNullException("FullName");
				}
				bool flag;
				this._storedEntryNameBytes = this.EncodeEntryName(value, out flag);
				this._storedEntryName = value;
				if (flag)
				{
					this._generalPurposeBitFlag |= ZipArchiveEntry.BitFlagValues.UnicodeFileName;
				}
				else
				{
					this._generalPurposeBitFlag &= ~ZipArchiveEntry.BitFlagValues.UnicodeFileName;
				}
				if (ZipArchiveEntry.ParseFileName(value, this._versionMadeByPlatform) == "")
				{
					this.VersionToExtractAtLeast(ZipVersionNeededValues.ExplicitDirectory);
				}
			}
		}

		public DateTimeOffset LastWriteTime
		{
			get
			{
				return this._lastModified;
			}
			set
			{
				this.ThrowIfInvalidArchive();
				if (this._archive.Mode == ZipArchiveMode.Read)
				{
					throw new NotSupportedException("Cannot modify read-only archive.");
				}
				if (this._archive.Mode == ZipArchiveMode.Create && this._everOpenedForWrite)
				{
					throw new IOException("Cannot modify entry in Create mode after entry has been opened for writing.");
				}
				if (value.DateTime.Year < 1980 || value.DateTime.Year > 2107)
				{
					throw new ArgumentOutOfRangeException("value", "The DateTimeOffset specified cannot be converted into a Zip file timestamp.");
				}
				this._lastModified = value;
			}
		}

		public long Length
		{
			get
			{
				if (this._everOpenedForWrite)
				{
					throw new InvalidOperationException("Length properties are unavailable once an entry has been opened for writing.");
				}
				return this._uncompressedSize;
			}
		}

		public string Name
		{
			get
			{
				return ZipArchiveEntry.ParseFileName(this.FullName, this._versionMadeByPlatform);
			}
		}

		public void Delete()
		{
			if (this._archive == null)
			{
				return;
			}
			if (this._currentlyOpenForWrite)
			{
				throw new IOException("Cannot delete an entry currently open for writing.");
			}
			if (this._archive.Mode != ZipArchiveMode.Update)
			{
				throw new NotSupportedException("Delete can only be used when the archive is in Update mode.");
			}
			this._archive.ThrowIfDisposed();
			this._archive.RemoveEntry(this);
			this._archive = null;
			this.UnloadStreams();
		}

		public Stream Open()
		{
			this.ThrowIfInvalidArchive();
			switch (this._archive.Mode)
			{
			case ZipArchiveMode.Read:
				return this.OpenInReadMode(true);
			case ZipArchiveMode.Create:
				return this.OpenInWriteMode();
			}
			return this.OpenInUpdateMode();
		}

		public override string ToString()
		{
			return this.FullName;
		}

		internal bool EverOpenedForWrite
		{
			get
			{
				return this._everOpenedForWrite;
			}
		}

		private long OffsetOfCompressedData
		{
			get
			{
				if (this._storedOffsetOfCompressedData == null)
				{
					this._archive.ArchiveStream.Seek(this._offsetOfLocalHeader, SeekOrigin.Begin);
					if (!ZipLocalFileHeader.TrySkipBlock(this._archive.ArchiveReader))
					{
						throw new InvalidDataException("A local file header is corrupt.");
					}
					this._storedOffsetOfCompressedData = new long?(this._archive.ArchiveStream.Position);
				}
				return this._storedOffsetOfCompressedData.Value;
			}
		}

		private MemoryStream UncompressedData
		{
			get
			{
				if (this._storedUncompressedData == null)
				{
					this._storedUncompressedData = new MemoryStream((int)this._uncompressedSize);
					if (this._originallyInArchive)
					{
						using (Stream stream = this.OpenInReadMode(false))
						{
							try
							{
								stream.CopyTo(this._storedUncompressedData);
							}
							catch (InvalidDataException)
							{
								this._storedUncompressedData.Dispose();
								this._storedUncompressedData = null;
								this._currentlyOpenForWrite = false;
								this._everOpenedForWrite = false;
								throw;
							}
						}
					}
					this.CompressionMethod = ZipArchiveEntry.CompressionMethodValues.Deflate;
				}
				return this._storedUncompressedData;
			}
		}

		private ZipArchiveEntry.CompressionMethodValues CompressionMethod
		{
			get
			{
				return this._storedCompressionMethod;
			}
			set
			{
				if (value == ZipArchiveEntry.CompressionMethodValues.Deflate)
				{
					this.VersionToExtractAtLeast(ZipVersionNeededValues.ExplicitDirectory);
				}
				else if (value == ZipArchiveEntry.CompressionMethodValues.Deflate64)
				{
					this.VersionToExtractAtLeast(ZipVersionNeededValues.Deflate64);
				}
				this._storedCompressionMethod = value;
			}
		}

		private string DecodeEntryName(byte[] entryNameBytes)
		{
			Encoding encoding;
			if ((this._generalPurposeBitFlag & ZipArchiveEntry.BitFlagValues.UnicodeFileName) == (ZipArchiveEntry.BitFlagValues)0)
			{
				encoding = ((this._archive == null) ? Encoding.UTF8 : (this._archive.EntryNameEncoding ?? Encoding.UTF8));
			}
			else
			{
				encoding = Encoding.UTF8;
			}
			return encoding.GetString(entryNameBytes);
		}

		private byte[] EncodeEntryName(string entryName, out bool isUTF8)
		{
			Encoding encoding;
			if (this._archive != null && this._archive.EntryNameEncoding != null)
			{
				encoding = this._archive.EntryNameEncoding;
			}
			else
			{
				encoding = (ZipHelper.RequiresUnicode(entryName) ? Encoding.UTF8 : Encoding.ASCII);
			}
			isUTF8 = encoding.Equals(Encoding.UTF8);
			return encoding.GetBytes(entryName);
		}

		internal void WriteAndFinishLocalEntry()
		{
			this.CloseStreams();
			this.WriteLocalFileHeaderAndDataIfNeeded();
			this.UnloadStreams();
		}

		internal void WriteCentralDirectoryFileHeader()
		{
			BinaryWriter binaryWriter = new BinaryWriter(this._archive.ArchiveStream);
			Zip64ExtraField zip64ExtraField = default(Zip64ExtraField);
			bool flag = false;
			uint num;
			uint num2;
			if (this.SizesTooLarge())
			{
				flag = true;
				num = uint.MaxValue;
				num2 = uint.MaxValue;
				zip64ExtraField.CompressedSize = new long?(this._compressedSize);
				zip64ExtraField.UncompressedSize = new long?(this._uncompressedSize);
			}
			else
			{
				num = (uint)this._compressedSize;
				num2 = (uint)this._uncompressedSize;
			}
			uint num3;
			if (this._offsetOfLocalHeader > (long)((ulong)(-1)))
			{
				flag = true;
				num3 = uint.MaxValue;
				zip64ExtraField.LocalHeaderOffset = new long?(this._offsetOfLocalHeader);
			}
			else
			{
				num3 = (uint)this._offsetOfLocalHeader;
			}
			if (flag)
			{
				this.VersionToExtractAtLeast(ZipVersionNeededValues.Zip64);
			}
			int num4 = (int)(flag ? zip64ExtraField.TotalSize : 0) + ((this._cdUnknownExtraFields != null) ? ZipGenericExtraField.TotalSize(this._cdUnknownExtraFields) : 0);
			ushort num5;
			if (num4 > 65535)
			{
				num5 = (flag ? zip64ExtraField.TotalSize : 0);
				this._cdUnknownExtraFields = null;
			}
			else
			{
				num5 = (ushort)num4;
			}
			binaryWriter.Write(33639248U);
			binaryWriter.Write((byte)this._versionMadeBySpecification);
			binaryWriter.Write((byte)ZipArchiveEntry.CurrentZipPlatform);
			binaryWriter.Write((ushort)this._versionToExtract);
			binaryWriter.Write((ushort)this._generalPurposeBitFlag);
			binaryWriter.Write((ushort)this.CompressionMethod);
			binaryWriter.Write(ZipHelper.DateTimeToDosTime(this._lastModified.DateTime));
			binaryWriter.Write(this._crc32);
			binaryWriter.Write(num);
			binaryWriter.Write(num2);
			binaryWriter.Write((ushort)this._storedEntryNameBytes.Length);
			binaryWriter.Write(num5);
			binaryWriter.Write((this._fileComment != null) ? ((ushort)this._fileComment.Length) : 0);
			binaryWriter.Write(0);
			binaryWriter.Write(0);
			binaryWriter.Write(this._externalFileAttr);
			binaryWriter.Write(num3);
			binaryWriter.Write(this._storedEntryNameBytes);
			if (flag)
			{
				zip64ExtraField.WriteBlock(this._archive.ArchiveStream);
			}
			if (this._cdUnknownExtraFields != null)
			{
				ZipGenericExtraField.WriteAllBlocks(this._cdUnknownExtraFields, this._archive.ArchiveStream);
			}
			if (this._fileComment != null)
			{
				binaryWriter.Write(this._fileComment);
			}
		}

		internal bool LoadLocalHeaderExtraFieldAndCompressedBytesIfNeeded()
		{
			if (this._originallyInArchive)
			{
				this._archive.ArchiveStream.Seek(this._offsetOfLocalHeader, SeekOrigin.Begin);
				this._lhUnknownExtraFields = ZipLocalFileHeader.GetExtraFields(this._archive.ArchiveReader);
			}
			if (!this._everOpenedForWrite && this._originallyInArchive)
			{
				this._compressedBytes = new byte[this._compressedSize / 2147483591L + 1L][];
				for (int i = 0; i < this._compressedBytes.Length - 1; i++)
				{
					this._compressedBytes[i] = new byte[2147483591];
				}
				this._compressedBytes[this._compressedBytes.Length - 1] = new byte[this._compressedSize % 2147483591L];
				this._archive.ArchiveStream.Seek(this.OffsetOfCompressedData, SeekOrigin.Begin);
				for (int j = 0; j < this._compressedBytes.Length - 1; j++)
				{
					ZipHelper.ReadBytes(this._archive.ArchiveStream, this._compressedBytes[j], 2147483591);
				}
				ZipHelper.ReadBytes(this._archive.ArchiveStream, this._compressedBytes[this._compressedBytes.Length - 1], (int)(this._compressedSize % 2147483591L));
			}
			return true;
		}

		internal void ThrowIfNotOpenable(bool needToUncompress, bool needToLoadIntoMemory)
		{
			string text;
			if (!this.IsOpenable(needToUncompress, needToLoadIntoMemory, out text))
			{
				throw new InvalidDataException(text);
			}
		}

		private CheckSumAndSizeWriteStream GetDataCompressor(Stream backingStream, bool leaveBackingStreamOpen, EventHandler onClose)
		{
			Stream stream = ((this._compressionLevel != null) ? new DeflateStream(backingStream, this._compressionLevel.Value, leaveBackingStreamOpen) : new DeflateStream(backingStream, CompressionMode.Compress, leaveBackingStreamOpen));
			bool flag = true;
			bool flag2 = leaveBackingStreamOpen && !flag;
			return new CheckSumAndSizeWriteStream(stream, backingStream, flag2, this, onClose, delegate(long initialPosition, long currentPosition, uint checkSum, Stream backing, ZipArchiveEntry thisRef, EventHandler closeHandler)
			{
				thisRef._crc32 = checkSum;
				thisRef._uncompressedSize = currentPosition;
				thisRef._compressedSize = backing.Position - initialPosition;
				if (closeHandler != null)
				{
					closeHandler(thisRef, EventArgs.Empty);
				}
			});
		}

		private Stream GetDataDecompressor(Stream compressedStreamToRead)
		{
			ZipArchiveEntry.CompressionMethodValues compressionMethod = this.CompressionMethod;
			if (compressionMethod != ZipArchiveEntry.CompressionMethodValues.Stored)
			{
				if (compressionMethod == ZipArchiveEntry.CompressionMethodValues.Deflate)
				{
					return new DeflateStream(compressedStreamToRead, CompressionMode.Decompress);
				}
				if (compressionMethod == ZipArchiveEntry.CompressionMethodValues.Deflate64)
				{
					return new DeflateManagedStream(compressedStreamToRead, ZipArchiveEntry.CompressionMethodValues.Deflate64);
				}
			}
			return compressedStreamToRead;
		}

		private Stream OpenInReadMode(bool checkOpenable)
		{
			if (checkOpenable)
			{
				this.ThrowIfNotOpenable(true, false);
			}
			Stream stream = new SubReadStream(this._archive.ArchiveStream, this.OffsetOfCompressedData, this._compressedSize);
			return this.GetDataDecompressor(stream);
		}

		private Stream OpenInWriteMode()
		{
			if (this._everOpenedForWrite)
			{
				throw new IOException("Entries in create mode may only be written to once, and only one entry may be held open at a time.");
			}
			this._everOpenedForWrite = true;
			CheckSumAndSizeWriteStream dataCompressor = this.GetDataCompressor(this._archive.ArchiveStream, true, delegate(object o, EventArgs e)
			{
				ZipArchiveEntry zipArchiveEntry = (ZipArchiveEntry)o;
				zipArchiveEntry._archive.ReleaseArchiveStream(zipArchiveEntry);
				zipArchiveEntry._outstandingWriteStream = null;
			});
			this._outstandingWriteStream = new ZipArchiveEntry.DirectToArchiveWriterStream(dataCompressor, this);
			return new WrappedStream(this._outstandingWriteStream, true);
		}

		private Stream OpenInUpdateMode()
		{
			if (this._currentlyOpenForWrite)
			{
				throw new IOException("Entries cannot be opened multiple times in Update mode.");
			}
			this.ThrowIfNotOpenable(true, true);
			this._everOpenedForWrite = true;
			this._currentlyOpenForWrite = true;
			this.UncompressedData.Seek(0L, SeekOrigin.Begin);
			return new WrappedStream(this.UncompressedData, this, delegate(ZipArchiveEntry thisRef)
			{
				thisRef._currentlyOpenForWrite = false;
			});
		}

		private bool IsOpenable(bool needToUncompress, bool needToLoadIntoMemory, out string message)
		{
			message = null;
			if (this._originallyInArchive)
			{
				if (needToUncompress && this.CompressionMethod != ZipArchiveEntry.CompressionMethodValues.Stored && this.CompressionMethod != ZipArchiveEntry.CompressionMethodValues.Deflate && this.CompressionMethod != ZipArchiveEntry.CompressionMethodValues.Deflate64)
				{
					ZipArchiveEntry.CompressionMethodValues compressionMethod = this.CompressionMethod;
					if (compressionMethod == ZipArchiveEntry.CompressionMethodValues.BZip2 || compressionMethod == ZipArchiveEntry.CompressionMethodValues.LZMA)
					{
						message = SR.Format("The archive entry was compressed using {0} and is not supported.", this.CompressionMethod.ToString());
					}
					else
					{
						message = "The archive entry was compressed using an unsupported compression method.";
					}
					return false;
				}
				if ((long)this._diskNumberStart != (long)((ulong)this._archive.NumberOfThisDisk))
				{
					message = "Split or spanned archives are not supported.";
					return false;
				}
				if (this._offsetOfLocalHeader > this._archive.ArchiveStream.Length)
				{
					message = "A local file header is corrupt.";
					return false;
				}
				this._archive.ArchiveStream.Seek(this._offsetOfLocalHeader, SeekOrigin.Begin);
				if (!ZipLocalFileHeader.TrySkipBlock(this._archive.ArchiveReader))
				{
					message = "A local file header is corrupt.";
					return false;
				}
				if (this.OffsetOfCompressedData + this._compressedSize > this._archive.ArchiveStream.Length)
				{
					message = "A local file header is corrupt.";
					return false;
				}
				if (needToLoadIntoMemory && this._compressedSize > 2147483647L && !ZipArchiveEntry.s_allowLargeZipArchiveEntriesInUpdateMode)
				{
					message = "Entries larger than 4GB are not supported in Update mode.";
					return false;
				}
			}
			return true;
		}

		private bool SizesTooLarge()
		{
			return this._compressedSize > (long)((ulong)(-1)) || this._uncompressedSize > (long)((ulong)(-1));
		}

		private bool WriteLocalFileHeader(bool isEmptyFile)
		{
			BinaryWriter binaryWriter = new BinaryWriter(this._archive.ArchiveStream);
			Zip64ExtraField zip64ExtraField = default(Zip64ExtraField);
			bool flag = false;
			uint num;
			uint num2;
			if (isEmptyFile)
			{
				this.CompressionMethod = ZipArchiveEntry.CompressionMethodValues.Stored;
				num = 0U;
				num2 = 0U;
			}
			else if (this._archive.Mode == ZipArchiveMode.Create && !this._archive.ArchiveStream.CanSeek && !isEmptyFile)
			{
				this._generalPurposeBitFlag |= ZipArchiveEntry.BitFlagValues.DataDescriptor;
				flag = false;
				num = 0U;
				num2 = 0U;
			}
			else if (this.SizesTooLarge())
			{
				flag = true;
				num = uint.MaxValue;
				num2 = uint.MaxValue;
				zip64ExtraField.CompressedSize = new long?(this._compressedSize);
				zip64ExtraField.UncompressedSize = new long?(this._uncompressedSize);
				this.VersionToExtractAtLeast(ZipVersionNeededValues.Zip64);
			}
			else
			{
				flag = false;
				num = (uint)this._compressedSize;
				num2 = (uint)this._uncompressedSize;
			}
			this._offsetOfLocalHeader = binaryWriter.BaseStream.Position;
			int num3 = (int)(flag ? zip64ExtraField.TotalSize : 0) + ((this._lhUnknownExtraFields != null) ? ZipGenericExtraField.TotalSize(this._lhUnknownExtraFields) : 0);
			ushort num4;
			if (num3 > 65535)
			{
				num4 = (flag ? zip64ExtraField.TotalSize : 0);
				this._lhUnknownExtraFields = null;
			}
			else
			{
				num4 = (ushort)num3;
			}
			binaryWriter.Write(67324752U);
			binaryWriter.Write((ushort)this._versionToExtract);
			binaryWriter.Write((ushort)this._generalPurposeBitFlag);
			binaryWriter.Write((ushort)this.CompressionMethod);
			binaryWriter.Write(ZipHelper.DateTimeToDosTime(this._lastModified.DateTime));
			binaryWriter.Write(this._crc32);
			binaryWriter.Write(num);
			binaryWriter.Write(num2);
			binaryWriter.Write((ushort)this._storedEntryNameBytes.Length);
			binaryWriter.Write(num4);
			binaryWriter.Write(this._storedEntryNameBytes);
			if (flag)
			{
				zip64ExtraField.WriteBlock(this._archive.ArchiveStream);
			}
			if (this._lhUnknownExtraFields != null)
			{
				ZipGenericExtraField.WriteAllBlocks(this._lhUnknownExtraFields, this._archive.ArchiveStream);
			}
			return flag;
		}

		private void WriteLocalFileHeaderAndDataIfNeeded()
		{
			if (this._storedUncompressedData != null || this._compressedBytes != null)
			{
				if (this._storedUncompressedData != null)
				{
					this._uncompressedSize = this._storedUncompressedData.Length;
					using (Stream stream = new ZipArchiveEntry.DirectToArchiveWriterStream(this.GetDataCompressor(this._archive.ArchiveStream, true, null), this))
					{
						this._storedUncompressedData.Seek(0L, SeekOrigin.Begin);
						this._storedUncompressedData.CopyTo(stream);
						this._storedUncompressedData.Dispose();
						this._storedUncompressedData = null;
						return;
					}
				}
				if (this._uncompressedSize == 0L)
				{
					this.CompressionMethod = ZipArchiveEntry.CompressionMethodValues.Stored;
				}
				this.WriteLocalFileHeader(false);
				foreach (byte[] array in this._compressedBytes)
				{
					this._archive.ArchiveStream.Write(array, 0, array.Length);
				}
				return;
			}
			if (this._archive.Mode == ZipArchiveMode.Update || !this._everOpenedForWrite)
			{
				this._everOpenedForWrite = true;
				this.WriteLocalFileHeader(true);
			}
		}

		private void WriteCrcAndSizesInLocalHeader(bool zip64HeaderUsed)
		{
			long position = this._archive.ArchiveStream.Position;
			BinaryWriter binaryWriter = new BinaryWriter(this._archive.ArchiveStream);
			bool flag = this.SizesTooLarge();
			bool flag2 = flag && !zip64HeaderUsed;
			uint num = (flag ? uint.MaxValue : ((uint)this._compressedSize));
			uint num2 = (flag ? uint.MaxValue : ((uint)this._uncompressedSize));
			if (flag2)
			{
				this._generalPurposeBitFlag |= ZipArchiveEntry.BitFlagValues.DataDescriptor;
				this._archive.ArchiveStream.Seek(this._offsetOfLocalHeader + 6L, SeekOrigin.Begin);
				binaryWriter.Write((ushort)this._generalPurposeBitFlag);
			}
			this._archive.ArchiveStream.Seek(this._offsetOfLocalHeader + 14L, SeekOrigin.Begin);
			if (!flag2)
			{
				binaryWriter.Write(this._crc32);
				binaryWriter.Write(num);
				binaryWriter.Write(num2);
			}
			else
			{
				binaryWriter.Write(0U);
				binaryWriter.Write(0U);
				binaryWriter.Write(0U);
			}
			if (zip64HeaderUsed)
			{
				this._archive.ArchiveStream.Seek(this._offsetOfLocalHeader + 30L + (long)this._storedEntryNameBytes.Length + 4L, SeekOrigin.Begin);
				binaryWriter.Write(this._uncompressedSize);
				binaryWriter.Write(this._compressedSize);
				this._archive.ArchiveStream.Seek(position, SeekOrigin.Begin);
			}
			this._archive.ArchiveStream.Seek(position, SeekOrigin.Begin);
			if (flag2)
			{
				binaryWriter.Write(this._crc32);
				binaryWriter.Write(this._compressedSize);
				binaryWriter.Write(this._uncompressedSize);
			}
		}

		private void WriteDataDescriptor()
		{
			BinaryWriter binaryWriter = new BinaryWriter(this._archive.ArchiveStream);
			binaryWriter.Write(134695760U);
			binaryWriter.Write(this._crc32);
			if (this.SizesTooLarge())
			{
				binaryWriter.Write(this._compressedSize);
				binaryWriter.Write(this._uncompressedSize);
				return;
			}
			binaryWriter.Write((uint)this._compressedSize);
			binaryWriter.Write((uint)this._uncompressedSize);
		}

		private void UnloadStreams()
		{
			if (this._storedUncompressedData != null)
			{
				this._storedUncompressedData.Dispose();
			}
			this._compressedBytes = null;
			this._outstandingWriteStream = null;
		}

		private void CloseStreams()
		{
			if (this._outstandingWriteStream != null)
			{
				this._outstandingWriteStream.Dispose();
			}
		}

		private void VersionToExtractAtLeast(ZipVersionNeededValues value)
		{
			if (this._versionToExtract < value)
			{
				this._versionToExtract = value;
			}
			if (this._versionMadeBySpecification < value)
			{
				this._versionMadeBySpecification = value;
			}
		}

		private void ThrowIfInvalidArchive()
		{
			if (this._archive == null)
			{
				throw new InvalidOperationException("Cannot modify deleted entry.");
			}
			this._archive.ThrowIfDisposed();
		}

		private static string GetFileName_Windows(string path)
		{
			int num = path.Length;
			while (--num >= 0)
			{
				char c = path[num];
				if (c == '\\' || c == '/' || c == ':')
				{
					return path.Substring(num + 1);
				}
			}
			return path;
		}

		private static string GetFileName_Unix(string path)
		{
			int num = path.Length;
			while (--num >= 0)
			{
				if (path[num] == '/')
				{
					return path.Substring(num + 1);
				}
			}
			return path;
		}

		internal static string ParseFileName(string path, ZipVersionMadeByPlatform madeByPlatform)
		{
			if (madeByPlatform == ZipVersionMadeByPlatform.Windows)
			{
				return ZipArchiveEntry.GetFileName_Windows(path);
			}
			if (madeByPlatform != ZipVersionMadeByPlatform.Unix)
			{
				return ZipArchiveEntry.ParseFileName(path, ZipArchiveEntry.CurrentZipPlatform);
			}
			return ZipArchiveEntry.GetFileName_Unix(path);
		}

		internal ZipArchiveEntry()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private const ushort DefaultVersionToExtract = 10;

		private const int MaxSingleBufferSize = 2147483591;

		private ZipArchive _archive;

		private readonly bool _originallyInArchive;

		private readonly int _diskNumberStart;

		private readonly ZipVersionMadeByPlatform _versionMadeByPlatform;

		private ZipVersionNeededValues _versionMadeBySpecification;

		private ZipVersionNeededValues _versionToExtract;

		private ZipArchiveEntry.BitFlagValues _generalPurposeBitFlag;

		private ZipArchiveEntry.CompressionMethodValues _storedCompressionMethod;

		private DateTimeOffset _lastModified;

		private long _compressedSize;

		private long _uncompressedSize;

		private long _offsetOfLocalHeader;

		private long? _storedOffsetOfCompressedData;

		private uint _crc32;

		private byte[][] _compressedBytes;

		private MemoryStream _storedUncompressedData;

		private bool _currentlyOpenForWrite;

		private bool _everOpenedForWrite;

		private Stream _outstandingWriteStream;

		private uint _externalFileAttr;

		private string _storedEntryName;

		private byte[] _storedEntryNameBytes;

		private List<ZipGenericExtraField> _cdUnknownExtraFields;

		private List<ZipGenericExtraField> _lhUnknownExtraFields;

		private byte[] _fileComment;

		private CompressionLevel? _compressionLevel;

		private static readonly bool s_allowLargeZipArchiveEntriesInUpdateMode = IntPtr.Size > 4;

		internal static readonly ZipVersionMadeByPlatform CurrentZipPlatform = ((Path.PathSeparator == '/') ? ZipVersionMadeByPlatform.Unix : ZipVersionMadeByPlatform.Windows);

		private sealed class DirectToArchiveWriterStream : Stream
		{
			public DirectToArchiveWriterStream(CheckSumAndSizeWriteStream crcSizeStream, ZipArchiveEntry entry)
			{
				this._position = 0L;
				this._crcSizeStream = crcSizeStream;
				this._everWritten = false;
				this._isDisposed = false;
				this._entry = entry;
				this._usedZip64inLH = false;
				this._canWrite = true;
			}

			public override long Length
			{
				get
				{
					this.ThrowIfDisposed();
					throw new NotSupportedException("This stream from ZipArchiveEntry does not support seeking.");
				}
			}

			public override long Position
			{
				get
				{
					this.ThrowIfDisposed();
					return this._position;
				}
				set
				{
					this.ThrowIfDisposed();
					throw new NotSupportedException("This stream from ZipArchiveEntry does not support seeking.");
				}
			}

			public override bool CanRead
			{
				get
				{
					return false;
				}
			}

			public override bool CanSeek
			{
				get
				{
					return false;
				}
			}

			public override bool CanWrite
			{
				get
				{
					return this._canWrite;
				}
			}

			private void ThrowIfDisposed()
			{
				if (this._isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().ToString(), "A stream from ZipArchiveEntry has been disposed.");
				}
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				this.ThrowIfDisposed();
				throw new NotSupportedException("This stream from ZipArchiveEntry does not support reading.");
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				this.ThrowIfDisposed();
				throw new NotSupportedException("This stream from ZipArchiveEntry does not support seeking.");
			}

			public override void SetLength(long value)
			{
				this.ThrowIfDisposed();
				throw new NotSupportedException("SetLength requires a stream that supports seeking and writing.");
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				if (buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				if (offset < 0)
				{
					throw new ArgumentOutOfRangeException("offset", "The argument must be non-negative.");
				}
				if (count < 0)
				{
					throw new ArgumentOutOfRangeException("count", "The argument must be non-negative.");
				}
				if (buffer.Length - offset < count)
				{
					throw new ArgumentException("The offset and length parameters are not valid for the array that was given.");
				}
				this.ThrowIfDisposed();
				if (count == 0)
				{
					return;
				}
				if (!this._everWritten)
				{
					this._everWritten = true;
					this._usedZip64inLH = this._entry.WriteLocalFileHeader(false);
				}
				this._crcSizeStream.Write(buffer, offset, count);
				this._position += (long)count;
			}

			public override void Flush()
			{
				this.ThrowIfDisposed();
				this._crcSizeStream.Flush();
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing && !this._isDisposed)
				{
					this._crcSizeStream.Dispose();
					if (!this._everWritten)
					{
						this._entry.WriteLocalFileHeader(true);
					}
					else if (this._entry._archive.ArchiveStream.CanSeek)
					{
						this._entry.WriteCrcAndSizesInLocalHeader(this._usedZip64inLH);
					}
					else
					{
						this._entry.WriteDataDescriptor();
					}
					this._canWrite = false;
					this._isDisposed = true;
				}
				base.Dispose(disposing);
			}

			private long _position;

			private CheckSumAndSizeWriteStream _crcSizeStream;

			private bool _everWritten;

			private bool _isDisposed;

			private ZipArchiveEntry _entry;

			private bool _usedZip64inLH;

			private bool _canWrite;
		}

		[Flags]
		private enum BitFlagValues : ushort
		{
			DataDescriptor = 8,
			UnicodeFileName = 2048
		}

		internal enum CompressionMethodValues : ushort
		{
			Stored,
			Deflate = 8,
			Deflate64,
			BZip2 = 12,
			LZMA = 14
		}
	}
}
