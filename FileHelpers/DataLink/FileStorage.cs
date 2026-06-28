using System;

namespace FileHelpers.DataLink
{
	public sealed class FileStorage : DataStorage
	{
		public FileStorage(Type type, string fileName)
			: base(type)
		{
			if (type == null)
			{
				throw new BadUsageException("You need to pass a non null Type to the FileStorage.");
			}
			this.mEngine = new FileHelperEngine(type);
			this.mErrorManager = this.mEngine.ErrorManager;
			this.mFileName = fileName;
		}

		public override object[] ExtractRecords()
		{
			return this.mEngine.ReadFile(this.mFileName);
		}

		public FileHelperEngine Engine
		{
			get
			{
				return this.mEngine;
			}
		}

		public string FileName
		{
			get
			{
				return this.mFileName;
			}
			set
			{
				this.mFileName = value;
			}
		}

		public override void InsertRecords(object[] records)
		{
			if (this.mFileName == null || this.mFileName.Length == 0)
			{
				throw new BadUsageException("You need to set a FileName to the FileStorage Provider.");
			}
			this.mEngine.WriteFile(this.mFileName, records);
		}

		private FileHelperEngine mEngine;

		private string mFileName;
	}
}
