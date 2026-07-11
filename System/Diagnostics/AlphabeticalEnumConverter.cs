using System;
using System.ComponentModel;

namespace System.Diagnostics
{
	internal sealed class AlphabeticalEnumConverter : global::System.ComponentModel.EnumConverter
	{
		public AlphabeticalEnumConverter(Type type)
			: base(type)
		{
		}

		[global::System.MonoTODO("Create sorted standart values")]
		public override global::System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(global::System.ComponentModel.ITypeDescriptorContext context)
		{
			return base.Values;
		}
	}
}
