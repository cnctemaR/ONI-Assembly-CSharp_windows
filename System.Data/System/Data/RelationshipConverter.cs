using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.Data
{
	internal sealed class RelationshipConverter : ExpandableObjectConverter
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
			if (destinationType == typeof(InstanceDescriptor) && value is DataRelation)
			{
				DataRelation dataRelation = (DataRelation)value;
				DataTable table = dataRelation.ParentKey.Table;
				DataTable table2 = dataRelation.ChildKey.Table;
				ConstructorInfo constructorInfo;
				object[] array;
				if (string.IsNullOrEmpty(table.Namespace) && string.IsNullOrEmpty(table2.Namespace))
				{
					constructorInfo = typeof(DataRelation).GetConstructor(new Type[]
					{
						typeof(string),
						typeof(string),
						typeof(string),
						typeof(string[]),
						typeof(string[]),
						typeof(bool)
					});
					array = new object[]
					{
						dataRelation.RelationName,
						dataRelation.ParentKey.Table.TableName,
						dataRelation.ChildKey.Table.TableName,
						dataRelation.ParentColumnNames,
						dataRelation.ChildColumnNames,
						dataRelation.Nested
					};
				}
				else
				{
					constructorInfo = typeof(DataRelation).GetConstructor(new Type[]
					{
						typeof(string),
						typeof(string),
						typeof(string),
						typeof(string),
						typeof(string),
						typeof(string[]),
						typeof(string[]),
						typeof(bool)
					});
					array = new object[]
					{
						dataRelation.RelationName,
						dataRelation.ParentKey.Table.TableName,
						dataRelation.ParentKey.Table.Namespace,
						dataRelation.ChildKey.Table.TableName,
						dataRelation.ChildKey.Table.Namespace,
						dataRelation.ParentColumnNames,
						dataRelation.ChildColumnNames,
						dataRelation.Nested
					};
				}
				return new InstanceDescriptor(constructorInfo, array);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
