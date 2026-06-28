using System;
using System.Text;

namespace FileHelpers.Options
{
	public sealed class CsvOptions
	{
		public CsvOptions(string className, char delimiter, int numberOfFields)
			: this(className, delimiter, numberOfFields, 1)
		{
		}

		public CsvOptions(string className, char delimiter, int numberOfFields, int headerLines)
		{
			this.mSampleFileName = string.Empty;
			this.mDelimiter = ',';
			this.mHeaderLines = 1;
			this.mRecordClassName = string.Empty;
			this.mNumberOfFields = -1;
			this.mFieldsPrefix = "Field_";
			this.mDateFormat = "dd/MM/yyyy";
			this.mDecimalSeparator = ".";
			this.mEncoding = Encoding.Default;
			base..ctor();
			this.mHeaderLines = headerLines;
			this.mRecordClassName = className;
			this.mDelimiter = delimiter;
			this.mNumberOfFields = numberOfFields;
		}

		public CsvOptions(string className, char delimiter, string sampleFile)
		{
			this.mSampleFileName = string.Empty;
			this.mDelimiter = ',';
			this.mHeaderLines = 1;
			this.mRecordClassName = string.Empty;
			this.mNumberOfFields = -1;
			this.mFieldsPrefix = "Field_";
			this.mDateFormat = "dd/MM/yyyy";
			this.mDecimalSeparator = ".";
			this.mEncoding = Encoding.Default;
			base..ctor();
			this.mRecordClassName = className;
			this.mDelimiter = delimiter;
			this.mSampleFileName = sampleFile;
		}

		public CsvOptions(string className, char delimiter, char headerDelimiter, string sampleFile)
		{
			this.mSampleFileName = string.Empty;
			this.mDelimiter = ',';
			this.mHeaderLines = 1;
			this.mRecordClassName = string.Empty;
			this.mNumberOfFields = -1;
			this.mFieldsPrefix = "Field_";
			this.mDateFormat = "dd/MM/yyyy";
			this.mDecimalSeparator = ".";
			this.mEncoding = Encoding.Default;
			base..ctor();
			this.mHeaderDelimiter = headerDelimiter;
			this.mRecordClassName = className;
			this.mDelimiter = delimiter;
			this.mSampleFileName = sampleFile;
		}

		public string SampleFileName
		{
			get
			{
				return this.mSampleFileName;
			}
			set
			{
				this.mSampleFileName = value;
			}
		}

		public char Delimiter
		{
			get
			{
				return this.mDelimiter;
			}
			set
			{
				this.mDelimiter = value;
			}
		}

		public char HeaderDelimiter
		{
			get
			{
				return this.mHeaderDelimiter;
			}
			set
			{
				this.mHeaderDelimiter = value;
			}
		}

		public string RecordClassName
		{
			get
			{
				return this.mRecordClassName;
			}
			set
			{
				this.mRecordClassName = value;
			}
		}

		public string FieldsPrefix
		{
			get
			{
				return this.mFieldsPrefix;
			}
			set
			{
				this.mFieldsPrefix = StringHelper.ToValidIdentifier(value);
			}
		}

		public int NumberOfFields
		{
			get
			{
				return this.mNumberOfFields;
			}
			set
			{
				this.mNumberOfFields = value;
			}
		}

		public int HeaderLines
		{
			get
			{
				return this.mHeaderLines;
			}
			set
			{
				this.mHeaderLines = value;
			}
		}

		public string DateFormat
		{
			get
			{
				return this.mDateFormat;
			}
			set
			{
				this.mDateFormat = value;
			}
		}

		public string DecimalSeparator
		{
			get
			{
				return this.mDecimalSeparator;
			}
			set
			{
				this.mDecimalSeparator = value;
			}
		}

		public Encoding Encoding
		{
			get
			{
				return this.mEncoding;
			}
			set
			{
				this.mEncoding = value;
			}
		}

		public bool IgnoreEmptyLines
		{
			get
			{
				return this.mIgnoreEmptyLines;
			}
			set
			{
				this.mIgnoreEmptyLines = value;
			}
		}

		internal string ValueToString(object o)
		{
			if (this.mDecimalConv == null)
			{
				this.mDecimalConv = new ConvertHelpers.DecimalConverter(this.DecimalSeparator);
				this.mDoubleConv = new ConvertHelpers.DoubleConverter(this.DecimalSeparator);
				this.mSingleConv = new ConvertHelpers.SingleConverter(this.DecimalSeparator);
				this.mDateConv = new ConvertHelpers.DateTimeConverter(this.DateFormat);
			}
			if (o == null)
			{
				return string.Empty;
			}
			if (o is DateTime)
			{
				return this.mDateConv.FieldToString(o);
			}
			if (o is decimal)
			{
				return this.mDecimalConv.FieldToString(o);
			}
			if (o is double)
			{
				return this.mDoubleConv.FieldToString(o);
			}
			if (o is float)
			{
				return this.mSingleConv.FieldToString(o);
			}
			return o.ToString();
		}

		private string mSampleFileName;

		private char mDelimiter;

		private char mHeaderDelimiter;

		private int mHeaderLines;

		private string mRecordClassName;

		private int mNumberOfFields;

		private string mFieldsPrefix;

		private string mDateFormat;

		private string mDecimalSeparator;

		private Encoding mEncoding;

		private bool mIgnoreEmptyLines;

		private ConvertHelpers.DecimalConverter mDecimalConv;

		private ConvertHelpers.DoubleConverter mDoubleConv;

		private ConvertHelpers.SingleConverter mSingleConv;

		private ConvertHelpers.DateTimeConverter mDateConv;
	}
}
