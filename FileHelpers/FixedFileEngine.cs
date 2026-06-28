using System;
using System.Diagnostics;
using System.Text;
using FileHelpers.Options;

namespace FileHelpers
{
	[DebuggerDisplay("FixedFileEngine for type: {RecordType.Name}. ErrorMode: {ErrorManager.ErrorMode.ToString()}. Encoding: {Encoding.EncodingName}")]
	public sealed class FixedFileEngine : FileHelperEngine
	{
		public FixedFileEngine(Type recordType)
			: base(recordType)
		{
			if (base.RecordInfo.IsDelimited)
			{
				throw new BadUsageException("The FixedFileEngine only accepts Record Types marked with FixedLengthRecord attribute");
			}
		}

		public FixedFileEngine(Type recordType, Encoding encoding)
			: this(recordType)
		{
			base.Encoding = encoding;
		}

		public new FixedRecordOptions Options
		{
			get
			{
				return (FixedRecordOptions)base.Options;
			}
		}
	}
}
