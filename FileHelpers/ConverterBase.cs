using System;

namespace FileHelpers
{
	public abstract class ConverterBase
	{
		public static string DefaultDateTimeFormat
		{
			get
			{
				return ConverterBase.mDefaultDateTimeFormat;
			}
			set
			{
				try
				{
					DateTime.Now.ToString(value);
				}
				catch
				{
					throw new BadUsageException("The format: '" + value + " is invalid for the DateTime Converter.");
				}
				ConverterBase.mDefaultDateTimeFormat = value;
			}
		}

		public abstract object StringToField(string from);

		public virtual string FieldToString(object from)
		{
			if (from == null)
			{
				return string.Empty;
			}
			return from.ToString();
		}

		protected internal virtual bool CustomNullHandling
		{
			get
			{
				return false;
			}
		}

		protected void ThrowConvertException(string from, string errorMsg)
		{
			throw new ConvertException(from, this.mDestinationType, errorMsg);
		}

		private static string mDefaultDateTimeFormat = "ddMMyyyy";

		internal Type mDestinationType;
	}
}
