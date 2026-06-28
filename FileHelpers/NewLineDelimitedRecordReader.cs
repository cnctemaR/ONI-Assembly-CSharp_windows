using System;
using System.IO;

namespace FileHelpers
{
	internal sealed class NewLineDelimitedRecordReader : IRecordReader
	{
		public NewLineDelimitedRecordReader(TextReader reader)
		{
			this.mReader = reader;
		}

		public string ReadRecordString()
		{
			return this.mReader.ReadLine();
		}

		public void Close()
		{
			this.mReader.Close();
		}

		private readonly TextReader mReader;
	}
}
