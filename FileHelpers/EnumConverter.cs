using System;

namespace FileHelpers
{
	internal sealed class EnumConverter : ConverterBase
	{
		public EnumConverter(Type sourceEnum)
		{
			if (!sourceEnum.IsEnum)
			{
				throw new BadUsageException("The Input sourceType must be an Enum but is of type " + sourceEnum.Name);
			}
			this.mEnumType = sourceEnum;
		}

		public override object StringToField(string from)
		{
			object obj;
			try
			{
				obj = Enum.Parse(this.mEnumType, from.Trim(), true);
			}
			catch (ArgumentException)
			{
				throw new ConvertException(from, this.mEnumType, "The value " + from + " is not present in the Enum.");
			}
			return obj;
		}

		private readonly Type mEnumType;
	}
}
