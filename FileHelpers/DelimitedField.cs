using System;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace FileHelpers
{
	public sealed class DelimitedField : FieldBase
	{
		private DelimitedField()
		{
		}

		internal DelimitedField(FieldInfo fi, string sep)
			: base(fi)
		{
			this.QuoteChar = '\0';
			this.QuoteMultiline = MultilineMode.AllowForBoth;
			this.Separator = sep;
		}

		internal string Separator
		{
			get
			{
				return this.mSeparator;
			}
			set
			{
				this.mSeparator = value;
				if (base.IsLast && !base.IsArray)
				{
					base.CharsToDiscard = 0;
					return;
				}
				base.CharsToDiscard = this.mSeparator.Length;
			}
		}

		internal MultilineMode QuoteMultiline { get; set; }

		internal QuoteMode QuoteMode { get; set; }

		internal char QuoteChar { get; set; }

		internal override ExtractedInfo ExtractFieldString(LineInfo line)
		{
			if (base.IsOptional && line.IsEOL())
			{
				return ExtractedInfo.Empty;
			}
			if (this.QuoteChar == '\0')
			{
				return this.BasicExtractString(line);
			}
			if (base.TrimMode == TrimMode.Both || base.TrimMode == TrimMode.Left)
			{
				line.TrimStart(base.TrimChars);
			}
			string text = this.QuoteChar.ToString();
			if (line.StartsWith(text))
			{
				ExtractedInfo extractedInfo = StringHelper.ExtractQuotedString(line, this.QuoteChar, this.QuoteMultiline == MultilineMode.AllowForBoth || this.QuoteMultiline == MultilineMode.AllowForRead);
				if (base.TrimMode == TrimMode.Both || base.TrimMode == TrimMode.Right)
				{
					line.TrimStart(base.TrimChars);
				}
				if (!base.IsLast && !line.StartsWith(this.Separator) && !line.IsEOL())
				{
					throw new BadUsageException(line, string.Concat(new string[]
					{
						"The field ",
						base.FieldInfo.Name,
						" is quoted but the quoted char: ",
						text,
						" not is just before the separator (You can use [FieldTrim] to avoid this error)"
					}));
				}
				return extractedInfo;
			}
			else
			{
				if (this.QuoteMode == QuoteMode.OptionalForBoth || this.QuoteMode == QuoteMode.OptionalForRead)
				{
					return this.BasicExtractString(line);
				}
				if (line.StartsWithTrim(text))
				{
					throw new BadUsageException(string.Format("The field '{0}' has spaces before the QuotedChar at line {1}. Use the TrimAttribute to by pass this error. Field String: {2}", base.FieldInfo.Name, line.mReader.LineNumber, line.CurrentString));
				}
				throw new BadUsageException(string.Format("The field '{0}' does not begin with the QuotedChar at line {1}. You can use FieldQuoted(QuoteMode.OptionalForRead) to allow optional quoted field. Field String: {2}", base.FieldInfo.Name, line.mReader.LineNumber, line.CurrentString));
			}
		}

		private ExtractedInfo BasicExtractString(LineInfo line)
		{
			if (!base.IsLast || base.IsArray)
			{
				int num = line.IndexOf(this.mSeparator);
				if (num == -1)
				{
					if (base.IsLast && base.IsArray)
					{
						return new ExtractedInfo(line);
					}
					if (!base.NextIsOptional)
					{
						string text;
						if (base.IsFirst && line.EmptyFromPos())
						{
							text = string.Format("The line {0} is empty. Maybe you need to use the attribute [IgnoreEmptyLines] in your record class.", line.mReader.LineNumber);
						}
						else
						{
							text = string.Format("Delimiter '{0}' not found after field '{1}' (the record has less fields, the delimiter is wrong or the next field must be marked as optional).", this.mSeparator, base.FieldInfo.Name, line.mReader.LineNumber);
						}
						throw new FileHelpersException(line.mReader.LineNumber, line.mCurrentPos, text);
					}
					num = line.mLineStr.Length;
				}
				return new ExtractedInfo(line, num);
			}
			int num2 = line.IndexOf(this.mSeparator);
			if (num2 == -1)
			{
				return new ExtractedInfo(line);
			}
			string text2 = string.Format("Delimiter '{0}' found after the last field '{1}' (the file is wrong or you need to add a field to the record class)", this.mSeparator, base.FieldInfo.Name, line.mReader.LineNumber);
			throw new BadUsageException(line.mReader.LineNumber, line.mCurrentPos, text2);
		}

		internal override void CreateFieldString(StringBuilder sb, object fieldValue, bool isLast)
		{
			string text = base.CreateFieldString(fieldValue);
			bool flag = DelimitedField.mCompare.IndexOf(text, StringHelper.NewLine, CompareOptions.Ordinal) >= 0;
			if (flag && (this.QuoteMultiline == MultilineMode.AllowForRead || this.QuoteMultiline == MultilineMode.NotAllow))
			{
				throw new BadUsageException("One value for the field " + base.FieldInfo.Name + " has a new line inside. To allow write this value you must add a FieldQuoted attribute with the multiline option in true.");
			}
			if (this.QuoteChar != '\0' && (this.QuoteMode == QuoteMode.AlwaysQuoted || this.QuoteMode == QuoteMode.OptionalForRead || ((this.QuoteMode == QuoteMode.OptionalForWrite || this.QuoteMode == QuoteMode.OptionalForBoth) && DelimitedField.mCompare.IndexOf(text, this.mSeparator, CompareOptions.Ordinal) >= 0) || flag))
			{
				StringHelper.CreateQuotedString(sb, text, this.QuoteChar);
			}
			else
			{
				sb.Append(text);
			}
			if (!isLast)
			{
				sb.Append(this.mSeparator);
			}
		}

		protected override FieldBase CreateClone()
		{
			return new DelimitedField
			{
				mSeparator = this.mSeparator,
				QuoteChar = this.QuoteChar,
				QuoteMode = this.QuoteMode,
				QuoteMultiline = this.QuoteMultiline
			};
		}

		private static readonly CompareInfo mCompare = StringHelper.CreateComparer();

		private string mSeparator;
	}
}
