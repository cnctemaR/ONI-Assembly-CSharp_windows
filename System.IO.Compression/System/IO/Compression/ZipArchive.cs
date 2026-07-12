using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace System.IO.Compression
{
	public class ZipArchive : IDisposable
	{
		public ZipArchive(Stream stream)
			: this(stream, ZipArchiveMode.Read, false, null)
		{
		}

		public ZipArchive(Stream stream, ZipArchiveMode mode)
			: this(stream, mode, false, null)
		{
		}

		public ZipArchive(Stream stream, ZipArchiveMode mode, bool leaveOpen)
			: this(stream, mode, leaveOpen, null)
		{
		}

		public ZipArchive(Stream stream, ZipArchiveMode mode, bool leaveOpen, Encoding entryNameEncoding)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			this.EntryNameEncoding = entryNameEncoding;
			this.Init(stream, mode, leaveOpen);
		}

		public ReadOnlyCollection<ZipArchiveEntry> Entries
		{
			get
			{
				if (this._mode == ZipArchiveMode.Create)
				{
					throw new NotSupportedException("Cannot access entries in Create mode.");
				}
				this.ThrowIfDisposed();
				this.EnsureCentralDirectoryRead();
				return this._entriesCollection;
			}
		}

		public ZipArchiveMode Mode
		{
			get
			{
				return this._mode;
			}
		}

		public ZipArchiveEntry CreateEntry(string entryName)
		{
			return this.DoCreateEntry(entryName, null);
		}

		public ZipArchiveEntry CreateEntry(string entryName, CompressionLevel compressionLevel)
		{
			return this.DoCreateEntry(entryName, new CompressionLevel?(compressionLevel));
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this._isDisposed)
			{
				try
				{
					ZipArchiveMode mode = this._mode;
					if (mode != ZipArchiveMode.Read)
					{
						int num = mode - ZipArchiveMode.Create;
						this.WriteFile();
					}
				}
				finally
				{
					this.CloseStreams();
					this._isDisposed = true;
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public ZipArchiveEntry GetEntry(string entryName)
		{
			if (entryName == null)
			{
				throw new ArgumentNullException("entryName");
			}
			if (this._mode == ZipArchiveMode.Create)
			{
				throw new NotSupportedException("Cannot access entries in Create mode.");
			}
			this.EnsureCentralDirectoryRead();
			ZipArchiveEntry zipArchiveEntry;
			this._entriesDictionary.TryGetValue(entryName, out zipArchiveEntry);
			return zipArchiveEntry;
		}

		internal BinaryReader ArchiveReader
		{
			get
			{
				return this._archiveReader;
			}
		}

		internal Stream ArchiveStream
		{
			get
			{
				return this._archiveStream;
			}
		}

		internal uint NumberOfThisDisk
		{
			get
			{
				return this._numberOfThisDisk;
			}
		}

		internal Encoding EntryNameEncoding
		{
			get
			{
				return this._entryNameEncoding;
			}
			private set
			{
				if (value != null && (value.Equals(Encoding.BigEndianUnicode) || value.Equals(Encoding.Unicode)))
				{
					throw new ArgumentException("The specified entry name encoding is not supported.", "EntryNameEncoding");
				}
				this._entryNameEncoding = value;
			}
		}

		private ZipArchiveEntry DoCreateEntry(string entryName, CompressionLevel? compressionLevel)
		{
			if (entryName == null)
			{
				throw new ArgumentNullException("entryName");
			}
			if (string.IsNullOrEmpty(entryName))
			{
				throw new ArgumentException("String cannot be empty.", "entryName");
			}
			if (this._mode == ZipArchiveMode.Read)
			{
				throw new NotSupportedException("Cannot create entries on an archive opened in read mode.");
			}
			this.ThrowIfDisposed();
			ZipArchiveEntry zipArchiveEntry = ((compressionLevel != null) ? new ZipArchiveEntry(this, entryName, compressionLevel.Value) : new ZipArchiveEntry(this, entryName));
			this.AddEntry(zipArchiveEntry);
			return zipArchiveEntry;
		}

		internal void AcquireArchiveStream(ZipArchiveEntry entry)
		{
			if (this._archiveStreamOwner != null)
			{
				if (this._archiveStreamOwner.EverOpenedForWrite)
				{
					throw new IOException("Entries cannot be created while previously created entries are still open.");
				}
				this._archiveStreamOwner.WriteAndFinishLocalEntry();
			}
			this._archiveStreamOwner = entry;
		}

		private void AddEntry(ZipArchiveEntry entry)
		{
			this._entries.Add(entry);
			string fullName = entry.FullName;
			if (!this._entriesDictionary.ContainsKey(fullName))
			{
				this._entriesDictionary.Add(fullName, entry);
			}
		}

		[Conditional("DEBUG")]
		internal void DebugAssertIsStillArchiveStreamOwner(ZipArchiveEntry entry)
		{
		}

		internal void ReleaseArchiveStream(ZipArchiveEntry entry)
		{
			this._archiveStreamOwner = null;
		}

		internal void RemoveEntry(ZipArchiveEntry entry)
		{
			this._entries.Remove(entry);
			this._entriesDictionary.Remove(entry.FullName);
		}

		internal void ThrowIfDisposed()
		{
			if (this._isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private void CloseStreams()
		{
			if (this._leaveOpen)
			{
				if (this._backingStream != null)
				{
					this._archiveStream.Dispose();
				}
				return;
			}
			this._archiveStream.Dispose();
			Stream backingStream = this._backingStream;
			if (backingStream != null)
			{
				backingStream.Dispose();
			}
			BinaryReader archiveReader = this._archiveReader;
			if (archiveReader == null)
			{
				return;
			}
			archiveReader.Dispose();
		}

		private void EnsureCentralDirectoryRead()
		{
			if (!this._readEntries)
			{
				this.ReadCentralDirectory();
				this._readEntries = true;
			}
		}

		private void Init(Stream stream, ZipArchiveMode mode, bool leaveOpen)
		{
			Stream stream2 = null;
			try
			{
				this._backingStream = null;
				switch (mode)
				{
				case ZipArchiveMode.Read:
					if (!stream.CanRead)
					{
						throw new ArgumentException("Cannot use read mode on a non-readable stream.");
					}
					if (!stream.CanSeek)
					{
						this._backingStream = stream;
						stream = (stream2 = new MemoryStream());
						this._backingStream.CopyTo(stream);
						stream.Seek(0L, SeekOrigin.Begin);
					}
					break;
				case ZipArchiveMode.Create:
					if (!stream.CanWrite)
					{
						throw new ArgumentException("Cannot use create mode on a non-writable stream.");
					}
					break;
				case ZipArchiveMode.Update:
					if (!stream.CanRead || !stream.CanWrite || !stream.CanSeek)
					{
						throw new ArgumentException("Update mode requires a stream with read, write, and seek capabilities.");
					}
					break;
				default:
					throw new ArgumentOutOfRangeException("mode");
				}
				this._mode = mode;
				if (mode == ZipArchiveMode.Create && !stream.CanSeek)
				{
					this._archiveStream = new PositionPreservingWriteOnlyStreamWrapper(stream);
				}
				else
				{
					this._archiveStream = stream;
				}
				this._archiveStreamOwner = null;
				if (mode == ZipArchiveMode.Create)
				{
					this._archiveReader = null;
				}
				else
				{
					this._archiveReader = new BinaryReader(this._archiveStream);
				}
				this._entries = new List<ZipArchiveEntry>();
				this._entriesCollection = new ReadOnlyCollection<ZipArchiveEntry>(this._entries);
				this._entriesDictionary = new Dictionary<string, ZipArchiveEntry>();
				this._readEntries = false;
				this._leaveOpen = leaveOpen;
				this._centralDirectoryStart = 0L;
				this._isDisposed = false;
				this._numberOfThisDisk = 0U;
				this._archiveComment = null;
				switch (mode)
				{
				case ZipArchiveMode.Read:
					this.ReadEndOfCentralDirectory();
					goto IL_01BC;
				case ZipArchiveMode.Create:
					this._readEntries = true;
					goto IL_01BC;
				}
				if (this._archiveStream.Length == 0L)
				{
					this._readEntries = true;
				}
				else
				{
					this.ReadEndOfCentralDirectory();
					this.EnsureCentralDirectoryRead();
					foreach (ZipArchiveEntry zipArchiveEntry in this._entries)
					{
						zipArchiveEntry.ThrowIfNotOpenable(false, true);
					}
				}
				IL_01BC:;
			}
			catch
			{
				if (stream2 != null)
				{
					stream2.Dispose();
				}
				throw;
			}
		}

		private void ReadCentralDirectory()
		{
			try
			{
				this._archiveStream.Seek(this._centralDirectoryStart, SeekOrigin.Begin);
				long num = 0L;
				bool flag = this.Mode == ZipArchiveMode.Update;
				ZipCentralDirectoryFileHeader zipCentralDirectoryFileHeader;
				while (ZipCentralDirectoryFileHeader.TryReadBlock(this._archiveReader, flag, out zipCentralDirectoryFileHeader))
				{
					this.AddEntry(new ZipArchiveEntry(this, zipCentralDirectoryFileHeader));
					num += 1L;
				}
				if (num != this._expectedNumberOfEntries)
				{
					throw new InvalidDataException("Number of entries expected in End Of Central Directory does not correspond to number of entries in Central Directory.");
				}
			}
			catch (EndOfStreamException ex)
			{
				throw new InvalidDataException(SR.Format("Central Directory is invalid.", ex));
			}
		}

		private void ReadEndOfCentralDirectory()
		{
			try
			{
				this._archiveStream.Seek(-18L, SeekOrigin.End);
				if (!ZipHelper.SeekBackwardsToSignature(this._archiveStream, 101010256U))
				{
					throw new InvalidDataException("End of Central Directory record could not be found.");
				}
				long position = this._archiveStream.Position;
				ZipEndOfCentralDirectoryBlock zipEndOfCentralDirectoryBlock;
				ZipEndOfCentralDirectoryBlock.TryReadBlock(this._archiveReader, out zipEndOfCentralDirectoryBlock);
				if (zipEndOfCentralDirectoryBlock.NumberOfThisDisk != zipEndOfCentralDirectoryBlock.NumberOfTheDiskWithTheStartOfTheCentralDirectory)
				{
					throw new InvalidDataException("Split or spanned archives are not supported.");
				}
				this._numberOfThisDisk = (uint)zipEndOfCentralDirectoryBlock.NumberOfThisDisk;
				this._centralDirectoryStart = (long)((ulong)zipEndOfCentralDirectoryBlock.OffsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber);
				if (zipEndOfCentralDirectoryBlock.NumberOfEntriesInTheCentralDirectory != zipEndOfCentralDirectoryBlock.NumberOfEntriesInTheCentralDirectoryOnThisDisk)
				{
					throw new InvalidDataException("Split or spanned archives are not supported.");
				}
				this._expectedNumberOfEntries = (long)((ulong)zipEndOfCentralDirectoryBlock.NumberOfEntriesInTheCentralDirectory);
				if (this._mode == ZipArchiveMode.Update)
				{
					this._archiveComment = zipEndOfCentralDirectoryBlock.ArchiveComment;
				}
				if (zipEndOfCentralDirectoryBlock.NumberOfThisDisk == 65535 || zipEndOfCentralDirectoryBlock.OffsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber == 4294967295U || zipEndOfCentralDirectoryBlock.NumberOfEntriesInTheCentralDirectory == 65535)
				{
					this._archiveStream.Seek(position - 16L, SeekOrigin.Begin);
					if (ZipHelper.SeekBackwardsToSignature(this._archiveStream, 117853008U))
					{
						Zip64EndOfCentralDirectoryLocator zip64EndOfCentralDirectoryLocator;
						Zip64EndOfCentralDirectoryLocator.TryReadBlock(this._archiveReader, out zip64EndOfCentralDirectoryLocator);
						if (zip64EndOfCentralDirectoryLocator.OffsetOfZip64EOCD > 9223372036854775807UL)
						{
							throw new InvalidDataException("Offset to Zip64 End Of Central Directory record cannot be held in an Int64.");
						}
						long offsetOfZip64EOCD = (long)zip64EndOfCentralDirectoryLocator.OffsetOfZip64EOCD;
						this._archiveStream.Seek(offsetOfZip64EOCD, SeekOrigin.Begin);
						Zip64EndOfCentralDirectoryRecord zip64EndOfCentralDirectoryRecord;
						if (!Zip64EndOfCentralDirectoryRecord.TryReadBlock(this._archiveReader, out zip64EndOfCentralDirectoryRecord))
						{
							throw new InvalidDataException("Zip 64 End of Central Directory Record not where indicated.");
						}
						this._numberOfThisDisk = zip64EndOfCentralDirectoryRecord.NumberOfThisDisk;
						if (zip64EndOfCentralDirectoryRecord.NumberOfEntriesTotal > 9223372036854775807UL)
						{
							throw new InvalidDataException("Number of Entries cannot be held in an Int64.");
						}
						if (zip64EndOfCentralDirectoryRecord.OffsetOfCentralDirectory > 9223372036854775807UL)
						{
							throw new InvalidDataException("Offset to Central Directory cannot be held in an Int64.");
						}
						if (zip64EndOfCentralDirectoryRecord.NumberOfEntriesTotal != zip64EndOfCentralDirectoryRecord.NumberOfEntriesOnThisDisk)
						{
							throw new InvalidDataException("Split or spanned archives are not supported.");
						}
						this._expectedNumberOfEntries = (long)zip64EndOfCentralDirectoryRecord.NumberOfEntriesTotal;
						this._centralDirectoryStart = (long)zip64EndOfCentralDirectoryRecord.OffsetOfCentralDirectory;
					}
				}
				if (this._centralDirectoryStart > this._archiveStream.Length)
				{
					throw new InvalidDataException("Offset to Central Directory cannot be held in an Int64.");
				}
			}
			catch (EndOfStreamException ex)
			{
				throw new InvalidDataException("Central Directory corrupt.", ex);
			}
			catch (IOException ex2)
			{
				throw new InvalidDataException("Central Directory corrupt.", ex2);
			}
		}

		private void WriteFile()
		{
			if (this._mode == ZipArchiveMode.Update)
			{
				List<ZipArchiveEntry> list = new List<ZipArchiveEntry>();
				foreach (ZipArchiveEntry zipArchiveEntry in this._entries)
				{
					if (!zipArchiveEntry.LoadLocalHeaderExtraFieldAndCompressedBytesIfNeeded())
					{
						list.Add(zipArchiveEntry);
					}
				}
				foreach (ZipArchiveEntry zipArchiveEntry2 in list)
				{
					zipArchiveEntry2.Delete();
				}
				this._archiveStream.Seek(0L, SeekOrigin.Begin);
				this._archiveStream.SetLength(0L);
			}
			foreach (ZipArchiveEntry zipArchiveEntry3 in this._entries)
			{
				zipArchiveEntry3.WriteAndFinishLocalEntry();
			}
			long position = this._archiveStream.Position;
			foreach (ZipArchiveEntry zipArchiveEntry4 in this._entries)
			{
				zipArchiveEntry4.WriteCentralDirectoryFileHeader();
			}
			long num = this._archiveStream.Position - position;
			this.WriteArchiveEpilogue(position, num);
		}

		private void WriteArchiveEpilogue(long startOfCentralDirectory, long sizeOfCentralDirectory)
		{
			if (startOfCentralDirectory >= (long)((ulong)(-1)) || sizeOfCentralDirectory >= (long)((ulong)(-1)) || this._entries.Count >= 65535)
			{
				long position = this._archiveStream.Position;
				Zip64EndOfCentralDirectoryRecord.WriteBlock(this._archiveStream, (long)this._entries.Count, startOfCentralDirectory, sizeOfCentralDirectory);
				Zip64EndOfCentralDirectoryLocator.WriteBlock(this._archiveStream, position);
			}
			ZipEndOfCentralDirectoryBlock.WriteBlock(this._archiveStream, (long)this._entries.Count, startOfCentralDirectory, sizeOfCentralDirectory, this._archiveComment);
		}

		private Stream _archiveStream;

		private ZipArchiveEntry _archiveStreamOwner;

		private BinaryReader _archiveReader;

		private ZipArchiveMode _mode;

		private List<ZipArchiveEntry> _entries;

		private ReadOnlyCollection<ZipArchiveEntry> _entriesCollection;

		private Dictionary<string, ZipArchiveEntry> _entriesDictionary;

		private bool _readEntries;

		private bool _leaveOpen;

		private long _centralDirectoryStart;

		private bool _isDisposed;

		private uint _numberOfThisDisk;

		private long _expectedNumberOfEntries;

		private Stream _backingStream;

		private byte[] _archiveComment;

		private Encoding _entryNameEncoding;
	}
}
