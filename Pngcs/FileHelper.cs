using System;
using System.IO;

namespace Hjg.Pngcs
{
	public class FileHelper
	{
		public static Stream OpenFileForReading(string file)
		{
			if (file == null || !File.Exists(file))
			{
				throw new PngjInputException("Cannot open file for reading (" + file + ")");
			}
			return new FileStream(file, FileMode.Open);
		}

		public static Stream OpenFileForWriting(string file, bool allowOverwrite)
		{
			if (File.Exists(file) && !allowOverwrite)
			{
				throw new PngjOutputException("File already exists (" + file + ") and overwrite=false");
			}
			return new FileStream(file, FileMode.Create);
		}

		public static PngWriter CreatePngWriter(string fileName, ImageInfo imgInfo, bool allowOverwrite)
		{
			return new PngWriter(FileHelper.OpenFileForWriting(fileName, allowOverwrite), imgInfo, fileName);
		}

		public static PngReader CreatePngReader(string fileName)
		{
			return new PngReader(FileHelper.OpenFileForReading(fileName), fileName);
		}
	}
}
