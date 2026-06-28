using System;
using System.Diagnostics;
using System.Text;
using FileHelpers.Options;

namespace FileHelpers
{
	[DebuggerDisplay("DelimitedFileEngine for type: {RecordType.Name}. ErrorMode: {ErrorManager.ErrorMode.ToString()}. Encoding: {Encoding.EncodingName}")]
	public sealed class DelimitedFileEngine : FileHelperEngine
	{
		public DelimitedFileEngine(Type recordType)
			: base(recordType)
		{
			if (!base.RecordInfo.IsDelimited)
			{
				throw new BadUsageException("The Delimited Engine only accepts record types marked with DelimitedRecordAttribute");
			}
		}

		public DelimitedFileEngine(Type recordType, Encoding encoding)
			: this(recordType)
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
