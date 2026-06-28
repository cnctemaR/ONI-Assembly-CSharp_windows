using System;
using System.Diagnostics;
using System.Text;
using FileHelpers.Options;

namespace FileHelpers
{
	[DebuggerDisplay("FixedFileEngine for type: {RecordType.Name}. ErrorMode: {ErrorManager.ErrorMode.ToString()}. Encoding: {Encoding.EncodingName}")]
	public sealed class FixedFileEngine<T> : FileHelperEngine<T> where T : class
	{
		public FixedFileEngine()
		{
			if (base.RecordInfo.IsDelimited)
			{
				throw new BadUsageException("The FixedFileEngine only accepts Record Types marked with FixedLengthRecord attribute");
			}
		}

		public FixedFileEngine(Encoding encoding)
			: this()
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
