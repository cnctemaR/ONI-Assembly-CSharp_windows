using System;
using System.Diagnostics;

namespace FileHelpers
{
	[IgnoreFirst(2)]
	[DelimitedRecord("|")]
	[DebuggerDisplay("Line: {LineNumber}. Error: {ExceptionInfo.Message}.")]
	public sealed class ErrorInfo
	{
		internal ErrorInfo()
		{
		}

		public int LineNumber
		{
			get
			{
				return this.mLineNumber;
			}
		}

		public string RecordString
		{
			get
			{
				return this.mRecordString;
			}
		}

		public Exception ExceptionInfo
		{
			get
			{
				return this.mExceptionInfo;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal int mLineNumber;

		[FieldQuoted(QuoteMode.OptionalForBoth)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal string mRecordString = string.Empty;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[FieldQuoted(QuoteMode.OptionalForBoth)]
		[FieldConverter(typeof(ErrorInfo.ExceptionConverter))]
		internal Exception mExceptionInfo;

		internal class ExceptionConverter : ConverterBase
		{
			public override string FieldToString(object from)
			{
				if (from == null)
				{
					return string.Empty;
				}
				if (from is ConvertException)
				{
					return "In the field '" + ((ConvertException)from).FieldName + "': " + ((ConvertException)from).Message.Replace(StringHelper.NewLine, " -> ");
				}
				return ((Exception)from).Message.Replace(StringHelper.NewLine, " -> ");
			}

			public override object StringToField(string from)
			{
				return new Exception(from);
			}
		}
	}
}
