using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Data.SqlTypes;
using System.Globalization;
using System.Reflection;

namespace System.Data
{
	internal sealed class ColumnTypeConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (!(destinationType == typeof(string)))
			{
				if (value != null && destinationType == typeof(InstanceDescriptor))
				{
					object obj = value;
					if (value is string)
					{
						for (int i = 0; i < ColumnTypeConverter.s_types.Length; i++)
						{
							if (ColumnTypeConverter.s_types[i].ToString().Equals(value))
							{
								obj = ColumnTypeConverter.s_types[i];
							}
						}
					}
					if (value is Type || value is string)
					{
						MethodInfo method = typeof(Type).GetMethod("GetType", new Type[] { typeof(string) });
						if (method != null)
						{
							return new InstanceDescriptor(method, new object[] { ((Type)obj).AssemblyQualifiedName });
						}
					}
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
			if (value == null)
			{
				return string.Empty;
			}
			return value.ToString();
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertTo(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value != null && value.GetType() == typeof(string))
			{
				for (int i = 0; i < ColumnTypeConverter.s_types.Length; i++)
				{
					if (ColumnTypeConverter.s_types[i].ToString().Equals(value))
					{
						return ColumnTypeConverter.s_types[i];
					}
				}
				return typeof(string);
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (this._values == null)
			{
				object[] array;
				if (ColumnTypeConverter.s_types != null)
				{
					array = new object[ColumnTypeConverter.s_types.Length];
					Array.Copy(ColumnTypeConverter.s_types, 0, array, 0, ColumnTypeConverter.s_types.Length);
				}
				else
				{
					array = null;
				}
				this._values = new TypeConverter.StandardValuesCollection(array);
			}
			return this._values;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		private static readonly Type[] s_types = new Type[]
		{
			typeof(bool),
			typeof(byte),
			typeof(byte[]),
			typeof(char),
			typeof(DateTime),
			typeof(decimal),
			typeof(double),
			typeof(Guid),
			typeof(short),
			typeof(int),
			typeof(long),
			typeof(object),
			typeof(sbyte),
			typeof(float),
			typeof(string),
			typeof(TimeSpan),
			typeof(ushort),
			typeof(uint),
			typeof(ulong),
			typeof(SqlInt16),
			typeof(SqlInt32),
			typeof(SqlInt64),
			typeof(SqlDecimal),
			typeof(SqlSingle),
			typeof(SqlDouble),
			typeof(SqlString),
			typeof(SqlBoolean),
			typeof(SqlBinary),
			typeof(SqlByte),
			typeof(SqlDateTime),
			typeof(SqlGuid),
			typeof(SqlMoney),
			typeof(SqlBytes),
			typeof(SqlChars),
			typeof(SqlXml)
		};

		private TypeConverter.StandardValuesCollection _values;
	}
}
