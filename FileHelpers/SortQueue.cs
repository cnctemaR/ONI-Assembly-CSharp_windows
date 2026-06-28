using System;
using System.IO;
using System.Text;

namespace FileHelpers
{
	internal sealed class SortQueue<T> : IDisposable where T : class
	{
		public FileHelperAsyncEngine<T> Engine { get; private set; }

		public T Current { get; private set; }

		public SortQueue(Encoding encoding, string file, bool deleteFile)
		{
			this.mFile = file;
			this.mDeleteFile = deleteFile;
			this.Engine = new FileHelperAsyncEngine<T>(encoding)
			{
				Options = 
				{
					IgnoreFirstLines = 0,
					IgnoreLastLines = 0
				}
			};
			this.Engine.BeginReadFile(file, 409600);
			this.MoveNext();
		}

		public void MoveNext()
		{
			this.Current = this.Engine.ReadNext();
		}

		public void Dispose()
		{
			this.Engine.Close();
			if (this.mDeleteFile)
			{
				File.Delete(this.mFile);
			}
			GC.SuppressFinalize(this);
		}

		private readonly string mFile;

		private readonly bool mDeleteFile;
	}
}
