using System;
using System.Diagnostics;
using System.Text;
using FileHelpers.Options;

namespace FileHelpers
{
	[DebuggerDisplay("DelimitedFileEngine for type: {RecordType.Name}. ErrorMode: {ErrorManager.ErrorMode.ToString()}. Encoding: {Encoding.EncodingName}")]
	public sealed class DelimitedFileEngine<T> : FileHelperEngine<T> where T : class
	{
		public DelimitedFileEngine()
		{
			if (!base.RecordInfo.IsDelimited)
			{
				throw new BadUsageException("The Delimited Engine only accepts Record Types marked with DelimitedRecordAttribute");
			}
		}

		public DelimitedFileEngine(Encoding encoding)
			: this()
		{
			base.Encoding = encoding;
		}

		public new DelimitedRecordOptions Options
		{
			get
			{
				return (DelimitedRecordOptions)base.Options;
			}
		}
	}
}
