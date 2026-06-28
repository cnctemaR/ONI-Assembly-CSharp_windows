using System;
using FileHelpers.Options;

namespace FileHelpers.Dynamic
{
	public sealed class CsvClassBuilder : DelimitedClassBuilder
	{
		public CsvClassBuilder(string className, char delimiter, string sampleFile)
			: this(new CsvOptions(className, delimiter, sampleFile))
		{
		}

		public CsvClassBuilder(string className, char delimiter, int numberOfFields)
			: this(new CsvOptions(className, delimiter, numberOfFields))
		{
		}

		public CsvClassBuilder(CsvOptions options)
			: base(options.RecordClassName, options.Delimiter.ToString())
		{
			base.IgnoreFirstLines = options.HeaderLines;
			base.IgnoreEmptyLines = options.IgnoreEmptyLines;
			if (options.SampleFileName != string.Empty)
			{
				string text = CommonEngine.RawReadFirstLines(options.SampleFileName, 1);
				if (options.HeaderLines > 0)
				{
					foreach (string text2 in text.Split(new char[] { (options.HeaderDelimiter == '\0') ? options.Delimiter : options.HeaderDelimiter }))
					{
						this.AddField(StringHelper.ToValidIdentifier(text2));
					}
					return;
				}
				int num = text.Split(new char[] { options.Delimiter }).Length;
				for (int j = 0; j < num; j++)
				{
					this.AddField(options.FieldsPrefix + j.ToString());
				}
				return;
			}
			else
			{
				if (options.NumberOfFields > 0)
				{
					this.AddFields(options.NumberOfFields, options.FieldsPrefix);
					return;
				}
				throw new BadUsageException("You must provide a SampleFileName or a NumberOfFields to parse a genric CSV file.");
			}
		}

		public override DelimitedFieldBuilder AddField(string fieldName, string fieldType)
		{
			base.AddField(fieldName, fieldType);
			if (this.mFields.Count > 1)
			{
				base.LastField.FieldOptional = true;
				base.LastField.FieldQuoted = true;
				base.LastField.QuoteMode = QuoteMode.OptionalForBoth;
				base.LastField.QuoteMultiline = MultilineMode.AllowForBoth;
			}
			return base.LastField;
		}

		public override void AddFields(int number)
		{
			this.AddFields(number, "Field");
		}

		public void AddFields(int number, string prefix)
		{
			int count = this.mFields.Count;
			for (int i = 0; i < number; i++)
			{
				this.AddField(prefix + (i + count + 1).ToString());
			}
		}
	}
}
