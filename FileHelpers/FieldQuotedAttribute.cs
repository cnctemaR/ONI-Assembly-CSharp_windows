using System;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class FieldQuotedAttribute : Attribute
	{
		public char QuoteChar { get; private set; }

		public QuoteMode QuoteMode
		{
			get
			{
				return this.mQuoteMode;
			}
			internal set
			{
				this.mQuoteMode = value;
			}
		}

		public MultilineMode QuoteMultiline
		{
			get
			{
				return this.mQuoteMultiline;
			}
			internal set
			{
				this.mQuoteMultiline = value;
			}
		}

		public FieldQuotedAttribute()
			: this('"')
		{
		}

		public FieldQuotedAttribute(char quoteChar)
			: this(quoteChar, QuoteMode.OptionalForRead, MultilineMode.AllowForBoth)
		{
		}

		public FieldQuotedAttribute(QuoteMode mode)
			: this('"', mode)
		{
		}

		public FieldQuotedAttribute(QuoteMode mode, MultilineMode multiline)
			: this('"', mode, multiline)
		{
		}

		public FieldQuotedAttribute(char quoteChar, QuoteMode mode)
			: this(quoteChar, mode, MultilineMode.AllowForBoth)
		{
		}

		public FieldQuotedAttribute(char quoteChar, QuoteMode mode, MultilineMode multiline)
		{
			if (quoteChar == '\0')
			{
				throw new BadUsageException("You can't use the null char (\\0) as quoted.");
			}
			this.QuoteChar = quoteChar;
			this.QuoteMode = mode;
			this.QuoteMultiline = multiline;
		}

		public FieldQuotedAttribute(MultilineMode multiline)
			: this('"', QuoteMode.OptionalForRead, multiline)
		{
		}

		internal QuoteMode mQuoteMode;

		internal MultilineMode mQuoteMultiline;
	}
}
