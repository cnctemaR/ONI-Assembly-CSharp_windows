using System;
using System.Text;

namespace FileHelpers
{
	public class FileHelperEngine : FileHelperEngine<object>
	{
		public FileHelperEngine(Type recordType)
			: this(recordType, Encoding.Default)
		{
		}

		public FileHelperEngine(Type recordType, Encoding encoding)
			: base(recordType, encoding)
		{
		}

		internal FileHelperEngine(RecordInfo ri)
			: base(ri)
		{
		}
	}
}
