using System;
using System.Reflection;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class FieldConverterAttribute : Attribute
	{
		public FieldConverterAttribute(ConverterKind converter)
			: this(converter, new string[0])
		{
		}

		public FieldConverterAttribute(ConverterKind converter, string arg1)
			: this(converter, new string[] { arg1 })
		{
		}

		public FieldConverterAttribute(ConverterKind converter, string arg1, string arg2)
			: this(converter, new string[] { arg1, arg2 })
		{
		}

		public FieldConverterAttribute(ConverterKind converter, string arg1, string arg2, string arg3)
			: this(converter, new string[] { arg1, arg2, arg3 })
		{
		}

		private FieldConverterAttribute(ConverterKind converter, params string[] args)
		{
			this.Kind = converter;
			Type type;
			switch (converter)
			{
			case ConverterKind.Date:
				type = typeof(ConvertHelpers.DateTimeConverter);
				break;
			case ConverterKind.Boolean:
				type = typeof(ConvertHelpers.BooleanConverter);
				break;
			case ConverterKind.Byte:
				type = typeof(ConvertHelpers.ByteConverter);
				break;
			case ConverterKind.Int16:
				type = typeof(ConvertHelpers.Int16Converter);
				break;
			case ConverterKind.Int32:
				type = typeof(ConvertHelpers.Int32Converter);
				break;
			case ConverterKind.Int64:
				type = typeof(ConvertHelpers.Int64Converter);
				break;
			case ConverterKind.Decimal:
				type = typeof(ConvertHelpers.DecimalConverter);
				break;
			case ConverterKind.Double:
				type = typeof(ConvertHelpers.DoubleConverter);
				break;
			case ConverterKind.PercentDouble:
				type = typeof(ConvertHelpers.PercentDoubleConverter);
				break;
			case ConverterKind.Single:
				type = typeof(ConvertHelpers.SingleConverter);
				break;
			case ConverterKind.SByte:
				type = typeof(ConvertHelpers.SByteConverter);
				break;
			case ConverterKind.UInt16:
				type = typeof(ConvertHelpers.UInt16Converter);
				break;
			case ConverterKind.UInt32:
				type = typeof(ConvertHelpers.UInt32Converter);
				break;
			case ConverterKind.UInt64:
				type = typeof(ConvertHelpers.UInt64Converter);
				break;
			case ConverterKind.DateMultiFormat:
				type = typeof(ConvertHelpers.DateTimeMultiFormatConverter);
				break;
			case ConverterKind.Char:
				type = typeof(ConvertHelpers.CharConverter);
				break;
			case ConverterKind.Guid:
				type = typeof(ConvertHelpers.GuidConverter);
				break;
			default:
				throw new BadUsageException("Converter '" + converter.ToString() + "' not found, you must specify a valid converter.");
			}
			this.CreateConverter(type, args);
		}

		public FieldConverterAttribute(Type customConverter, string arg1)
			: this(customConverter, new string[] { arg1 })
		{
		}

		public FieldConverterAttribute(Type customConverter, string arg1, string arg2)
			: this(customConverter, new string[] { arg1, arg2 })
		{
		}

		public FieldConverterAttribute(Type customConverter, string arg1, string arg2, string arg3)
			: this(customConverter, new string[] { arg1, arg2, arg3 })
		{
		}

		public FieldConverterAttribute(Type customConverter, params object[] args)
		{
			this.CreateConverter(customConverter, args);
		}

		public FieldConverterAttribute(Type customConverter)
		{
			this.CreateConverter(customConverter, new object[0]);
		}

		public ConverterBase Converter { get; private set; }

		public ConverterKind Kind { get; private set; }

		private void CreateConverter(Type convType, object[] args)
		{
			if (typeof(ConverterBase).IsAssignableFrom(convType))
			{
				ConstructorInfo constructor = convType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, FieldConverterAttribute.ArgsToTypes(args), null);
				if (constructor == null)
				{
					if (args.Length == 0)
					{
						throw new BadUsageException("Empty constructor for converter: " + convType.Name + " was not found. You must add a constructor without args (can be public or private)");
					}
					throw new BadUsageException(string.Concat(new string[]
					{
						"Constructor for converter: ",
						convType.Name,
						" with these arguments: (",
						FieldConverterAttribute.ArgsDesc(args),
						") was not found. You must add a constructor with this signature (can be public or private)"
					}));
				}
				else
				{
					try
					{
						this.Converter = (ConverterBase)constructor.Invoke(args);
						return;
					}
					catch (TargetInvocationException ex)
					{
						throw ex.InnerException;
					}
				}
			}
			if (convType.IsEnum)
			{
				this.Converter = new EnumConverter(convType);
				return;
			}
			throw new BadUsageException("The custom converter must inherit from ConverterBase");
		}

		private static Type[] ArgsToTypes(object[] args)
		{
			if (args == null)
			{
				throw new BadUsageException("The args to the constructor can be null, if you do not want to pass anything into them.");
			}
			Type[] array = new Type[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == null)
				{
					array[i] = typeof(object);
				}
				else
				{
					array[i] = args[i].GetType();
				}
			}
			return array;
		}

		private static string ArgsDesc(object[] args)
		{
			string text = FieldConverterAttribute.DisplayType(args[0]);
			for (int i = 1; i < args.Length; i++)
			{
				text = text + ", " + FieldConverterAttribute.DisplayType(args[i]);
			}
			return text;
		}

		private static string DisplayType(object o)
		{
			if (o == null)
			{
				return "Object";
			}
			return o.GetType().Name;
		}

		internal void ValidateTypes(FieldInfo fi)
		{
			bool flag = false;
			Type type = fi.FieldType;
			if (type.IsValueType && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				type = type.GetGenericArguments()[0];
			}
			switch (this.Kind)
			{
			case ConverterKind.None:
				flag = true;
				break;
			case ConverterKind.Date:
			case ConverterKind.DateMultiFormat:
				flag = typeof(DateTime) == type;
				break;
			case ConverterKind.Boolean:
			case ConverterKind.Byte:
			case ConverterKind.Int16:
			case ConverterKind.Int32:
			case ConverterKind.Int64:
			case ConverterKind.Decimal:
			case ConverterKind.Double:
			case ConverterKind.Single:
			case ConverterKind.SByte:
			case ConverterKind.UInt16:
			case ConverterKind.UInt32:
			case ConverterKind.UInt64:
			case ConverterKind.Char:
			case ConverterKind.Guid:
				flag = this.Kind.ToString() == type.UnderlyingSystemType.Name;
				break;
			case ConverterKind.PercentDouble:
				flag = typeof(double) == type;
				break;
			}
			if (!flag)
			{
				throw new BadUsageException(string.Concat(new string[]
				{
					"The converter of the field: '",
					fi.Name,
					"' is wrong. The field is of Type: ",
					type.Name,
					" and the converter is for type: ",
					this.Kind.ToString()
				}));
			}
		}
	}
}
