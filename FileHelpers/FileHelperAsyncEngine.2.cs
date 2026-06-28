using System;
using System.Text;

namespace FileHelpers
{
	public sealed class FileHelperAsyncEngine : FileHelperAsyncEngine<object>
	{
		public FileHelperAsyncEngine(Type recordType)
			: base(recordType)
		{
		}

		public FileHelperAsyncEngine(Type recordType, Encoding encoding)
			: base(recordType, encoding)
		{
		}
	}
}
