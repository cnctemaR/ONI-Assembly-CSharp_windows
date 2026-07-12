using System;
using System.Globalization;

namespace System.Runtime.Serialization
{
	public class DateTimeFormat
	{
		public DateTimeFormat(string formatString)
			: this(formatString, DateTimeFormatInfo.CurrentInfo)
		{
		}

		public DateTimeFormat(string formatString, IFormatProvider formatProvider)
		{
			if (formatString == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("formatString");
			}
			if (formatProvider == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("formatProvider");
			}
			this.formatString = formatString;
			this.formatProvider = formatProvider;
			this.dateTimeStyles = DateTimeStyles.RoundtripKind;
		}

		public string FormatString
		{
			get
			{
				return this.formatString;
			}
		}

		public IFormatProvider FormatProvider
		{
			get
			{
				return this.formatProvider;
			}
		}

		public DateTimeStyles DateTimeStyles
		{
			get
			{
				return this.dateTimeStyles;
			}
			set
			{
				this.dateTimeStyles = value;
			}
		}

		private string formatString;

		private IFormatProvider formatProvider;

		private DateTimeStyles dateTimeStyles;
	}
}
