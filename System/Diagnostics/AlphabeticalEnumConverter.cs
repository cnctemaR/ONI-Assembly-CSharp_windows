using System;
using System.ComponentModel;

namespace System.Diagnostics
{
	internal sealed class AlphabeticalEnumConverter : EnumConverter
	{
		public AlphabeticalEnumConverter(Type type)
			: base(type)
		{
		}

		[MonoTODO("Create sorted standart values")]
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return base.Values;
		}
	}
}
