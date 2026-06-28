using System;
using System.IO;

namespace FileHelpers.DataLink
{
	public sealed class FileDataLink
	{
		public FileDataLink(DataStorage provider)
		{
			this.mProvider = provider;
			if (this.mProvider != null)
			{
				this.mHelperEngine = new FileHelperEngine(this.mProvider.RecordType);
				return;
			}
			throw new ArgumentException("provider can't be null", "provider");
		}

		public FileHelperEngine FileHelperEngine
		{
			get
			{
				return this.mHelperEngine;
			}
		}

		public DataStorage DataStorage
		{
			get
			{
				return this.mProvider;
			}
		}

		public object[] LastExtractedRecords
		{
			get
			{
				return this.mLastExtractedRecords;
			}
		}

		public object[] LastInsertedRecords
		{
			get
			{
				return this.mLastInsertedRecords;
			}
		}

		public object[] ExtractToFile(string fileName)
		{
			this.mLastExtractedRecords = this.mProvider.ExtractRecords();
			this.FileHelperEngine.WriteFile(fileName, this.mLastExtractedRecords);
			return this.mLastExtractedRecords;
		}

		public object[] ExtractToStream(StreamWriter writer)
		{
			this.mLastExtractedRecords = this.mProvider.ExtractRecords();
			this.FileHelperEngine.WriteStream(writer, this.mLastExtractedRecords);
			return this.mLastExtractedRecords;
		}

		public object[] InsertFromFile(string fileName)
		{
			this.mLastInsertedRecords = this.FileHelperEngine.ReadFile(fileName);
			this.mProvider.InsertRecords(this.mLastInsertedRecords);
			return this.mLastInsertedRecords;
		}

		public object[] InsertFromStream(StreamReader reader)
		{
			this.mLastInsertedRecords = this.FileHelperEngine.ReadStream(reader);
			this.mProvider.InsertRecords(this.mLastInsertedRecords);
			return this.mLastInsertedRecords;
		}

		public static object[] EasyExtractToFile(DataStorage storage, string filename)
		{
			FileDataLink fileDataLink = new FileDataLink(storage);
			return fileDataLink.ExtractToFile(filename);
		}

		public static object[] EasyInsertFromFile(DataStorage storage, string filename)
		{
			FileDataLink fileDataLink = new FileDataLink(storage);
			return fileDataLink.InsertFromFile(filename);
		}

		private readonly FileHelperEngine mHelperEngine;

		private readonly DataStorage mProvider;

		private object[] mLastExtractedRecords;

		private object[] mLastInsertedRecords;
	}
}
