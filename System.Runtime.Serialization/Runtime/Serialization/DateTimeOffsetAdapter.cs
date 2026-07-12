using System;
using System.Globalization;
using System.Xml;

namespace System.Runtime.Serialization
{
	[DataContract(Name = "DateTimeOffset", Namespace = "http://schemas.datacontract.org/2004/07/System")]
	internal struct DateTimeOffsetAdapter
	{
		public DateTimeOffsetAdapter(DateTime dateTime, short offsetMinutes)
		{
			this.utcDateTime = dateTime;
			this.offsetMinutes = offsetMinutes;
		}

		[DataMember(Name = "DateTime", IsRequired = true)]
		public DateTime UtcDateTime
		{
			get
			{
				return this.utcDateTime;
			}
			set
			{
				this.utcDateTime = value;
			}
		}

		[DataMember(Name = "OffsetMinutes", IsRequired = true)]
		public short OffsetMinutes
		{
			get
			{
				return this.offsetMinutes;
			}
			set
			{
				this.offsetMinutes = value;
			}
		}

		public static DateTimeOffset GetDateTimeOffset(DateTimeOffsetAdapter value)
		{
			DateTimeOffset dateTimeOffset;
			try
			{
				if (value.UtcDateTime.Kind == DateTimeKind.Unspecified)
				{
					dateTimeOffset = new DateTimeOffset(value.UtcDateTime, new TimeSpan(0, (int)value.OffsetMinutes, 0));
				}
				else
				{
					DateTimeOffset dateTimeOffset2 = new DateTimeOffset(value.UtcDateTime);
					dateTimeOffset = dateTimeOffset2.ToOffset(new TimeSpan(0, (int)value.OffsetMinutes, 0));
				}
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value.ToString(CultureInfo.InvariantCulture), "DateTimeOffset", ex));
			}
			return dateTimeOffset;
		}

		public static DateTimeOffsetAdapter GetDateTimeOffsetAdapter(DateTimeOffset value)
		{
			return new DateTimeOffsetAdapter(value.UtcDateTime, (short)value.Offset.TotalMinutes);
		}

		public string ToString(IFormatProvider provider)
		{
			return "DateTime: " + this.UtcDateTime.ToString() + ", Offset: " + this.OffsetMinutes.ToString();
		}

		private DateTime utcDateTime;

		private short offsetMinutes;
	}
}
