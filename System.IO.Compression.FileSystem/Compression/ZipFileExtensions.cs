using System;
using System.ComponentModel;

namespace System.IO.Compression
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class ZipFileExtensions
	{
		public static ZipArchiveEntry CreateEntryFromFile(this ZipArchive destination, string sourceFileName, string entryName)
		{
			return ZipFileExtensions.DoCreateEntryFromFile(destination, sourceFileName, entryName, null);
		}

		public static ZipArchiveEntry CreateEntryFromFile(this ZipArchive destination, string sourceFileName, string entryName, CompressionLevel compressionLevel)
		{
			return ZipFileExtensions.DoCreateEntryFromFile(destination, sourceFileName, entryName, new CompressionLevel?(compressionLevel));
		}

		public static void ExtractToDirectory(this ZipArchive source, string destinationDirectoryName)
		{
			source.ExtractToDirectory(destinationDirectoryName, false);
		}

		public static void ExtractToDirectory(this ZipArchive source, string destinationDirectoryName, bool overwrite)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (destinationDirectoryName == null)
			{
				throw new ArgumentNullException("destinationDirectoryName");
			}
			string text = Directory.CreateDirectory(destinationDirectoryName).FullName;
			if (!text.EndsWith(Path.DirectorySeparatorChar))
			{
				text += Path.DirectorySeparatorChar.ToString();
			}
			foreach (ZipArchiveEntry zipArchiveEntry in source.Entries)
			{
				string fullPath = Path.GetFullPath(Path.Combine(text, zipArchiveEntry.FullName));
				if (!fullPath.StartsWith(text, PathInternal.StringComparison))
				{
					throw new IOException("Extracting Zip entry would have resulted in a file outside the specified destination directory.");
				}
				if (Path.GetFileName(fullPath).Length == 0)
				{
					if (zipArchiveEntry.Length != 0L)
					{
						throw new IOException("Zip entry name ends in directory separator character but contains data.");
					}
					Directory.CreateDirectory(fullPath);
				}
				else
				{
					Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
					zipArchiveEntry.ExtractToFile(fullPath, overwrite);
				}
			}
		}

		internal static ZipArchiveEntry DoCreateEntryFromFile(ZipArchive destination, string sourceFileName, string entryName, CompressionLevel? compressionLevel)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (sourceFileName == null)
			{
				throw new ArgumentNullException("sourceFileName");
			}
			if (entryName == null)
			{
				throw new ArgumentNullException("entryName");
			}
			ZipArchiveEntry zipArchiveEntry2;
			using (Stream stream = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, false))
			{
				ZipArchiveEntry zipArchiveEntry = ((compressionLevel != null) ? destination.CreateEntry(entryName, compressionLevel.Value) : destination.CreateEntry(entryName));
				DateTime lastWriteTime = File.GetLastWriteTime(sourceFileName);
				if (lastWriteTime.Year < 1980 || lastWriteTime.Year > 2107)
				{
					lastWriteTime = new DateTime(1980, 1, 1, 0, 0, 0);
				}
				zipArchiveEntry.LastWriteTime = lastWriteTime;
				using (Stream stream2 = zipArchiveEntry.Open())
				{
					stream.CopyTo(stream2);
				}
				zipArchiveEntry2 = zipArchiveEntry;
			}
			return zipArchiveEntry2;
		}

		public static void ExtractToFile(this ZipArchiveEntry source, string destinationFileName)
		{
			source.ExtractToFile(destinationFileName, false);
		}

		public static void ExtractToFile(this ZipArchiveEntry source, string destinationFileName, bool overwrite)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (destinationFileName == null)
			{
				throw new ArgumentNullException("destinationFileName");
			}
			FileMode fileMode = (overwrite ? FileMode.Create : FileMode.CreateNew);
			using (Stream stream = new FileStream(destinationFileName, fileMode, FileAccess.Write, FileShare.None, 4096, false))
			{
				using (Stream stream2 = source.Open())
				{
					stream2.CopyTo(stream);
				}
			}
			File.SetLastWriteTime(destinationFileName, source.LastWriteTime.DateTime);
		}
	}
}
