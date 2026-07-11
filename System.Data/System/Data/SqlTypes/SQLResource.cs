using System;

namespace System.Data.SqlTypes
{
	internal static class SQLResource
	{
		internal static string NullString
		{
			get
			{
				return "Null";
			}
		}

		internal static string MessageString
		{
			get
			{
				return "Message";
			}
		}

		internal static string ArithOverflowMessage
		{
			get
			{
				return "Arithmetic Overflow.";
			}
		}

		internal static string DivideByZeroMessage
		{
			get
			{
				return "Divide by zero error encountered.";
			}
		}

		internal static string NullValueMessage
		{
			get
			{
				return "Data is Null. This method or property cannot be called on Null values.";
			}
		}

		internal static string TruncationMessage
		{
			get
			{
				return "Numeric arithmetic causes truncation.";
			}
		}

		internal static string DateTimeOverflowMessage
		{
			get
			{
				return "SqlDateTime overflow. Must be between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM.";
			}
		}

		internal static string ConcatDiffCollationMessage
		{
			get
			{
				return "Two strings to be concatenated have different collation.";
			}
		}

		internal static string CompareDiffCollationMessage
		{
			get
			{
				return "Two strings to be compared have different collation.";
			}
		}

		internal static string InvalidFlagMessage
		{
			get
			{
				return "Invalid flag value.";
			}
		}

		internal static string NumeToDecOverflowMessage
		{
			get
			{
				return "Conversion from SqlDecimal to Decimal overflows.";
			}
		}

		internal static string ConversionOverflowMessage
		{
			get
			{
				return "Conversion overflows.";
			}
		}

		internal static string InvalidDateTimeMessage
		{
			get
			{
				return "Invalid SqlDateTime.";
			}
		}

		internal static string TimeZoneSpecifiedMessage
		{
			get
			{
				return "A time zone was specified. SqlDateTime does not support time zones.";
			}
		}

		internal static string InvalidArraySizeMessage
		{
			get
			{
				return "Invalid array size.";
			}
		}

		internal static string InvalidPrecScaleMessage
		{
			get
			{
				return "Invalid numeric precision/scale.";
			}
		}

		internal static string FormatMessage
		{
			get
			{
				return "The input wasn't in a correct format.";
			}
		}

		internal static string NotFilledMessage
		{
			get
			{
				return "SQL Type has not been loaded with data.";
			}
		}

		internal static string AlreadyFilledMessage
		{
			get
			{
				return "SQL Type has already been loaded with data.";
			}
		}

		internal static string ClosedXmlReaderMessage
		{
			get
			{
				return "Invalid attempt to access a closed XmlReader.";
			}
		}

		internal static string InvalidOpStreamClosed(string method)
		{
			return SR.Format("Invalid attempt to call {0} when the stream is closed.", method);
		}

		internal static string InvalidOpStreamNonWritable(string method)
		{
			return SR.Format("Invalid attempt to call {0} when the stream non-writable.", method);
		}

		internal static string InvalidOpStreamNonReadable(string method)
		{
			return SR.Format("Invalid attempt to call {0} when the stream non-readable.", method);
		}

		internal static string InvalidOpStreamNonSeekable(string method)
		{
			return SR.Format("Invalid attempt to call {0} when the stream is non-seekable.", method);
		}
	}
}
