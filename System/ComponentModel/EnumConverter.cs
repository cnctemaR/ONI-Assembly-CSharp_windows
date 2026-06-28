using System;
using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel
{
	public class EnumConverter : TypeConverter
	{
		public EnumConverter(Type type)
		{
			this.type = type;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(global::System.ComponentModel.Design.Serialization.InstanceDescriptor) || destinationType == typeof(Enum[]) || base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType != typeof(string) || value == null)
			{
				if (destinationType == typeof(global::System.ComponentModel.Design.Serialization.InstanceDescriptor) && value != null)
				{
					string text = base.ConvertToString(context, culture, value);
					if (this.IsFlags && text.IndexOf(",") != -1)
					{
						if (value is IConvertible)
						{
							Type underlyingType = Enum.GetUnderlyingType(this.type);
							object obj = ((IConvertible)value).ToType(underlyingType, culture);
							MethodInfo method = typeof(Enum).GetMethod("ToObject", new Type[]
							{
								typeof(Type),
								underlyingType
							});
							return new global::System.ComponentModel.Design.Serialization.InstanceDescriptor(method, new object[] { this.type, obj });
						}
					}
					else
					{
						FieldInfo field = this.type.GetField(text);
						if (field != null)
						{
							return new global::System.ComponentModel.Design.Serialization.InstanceDescriptor(field, null);
						}
					}
				}
				else if (destinationType == typeof(Enum[]) && value != null)
				{
					if (!this.IsFlags)
					{
						return new Enum[] { (Enum)Enum.ToObject(this.type, value) };
					}
					long num = Convert.ToInt64((Enum)value, culture);
					Array values = Enum.GetValues(this.type);
					long[] array = new long[values.Length];
					for (int i = 0; i < values.Length; i++)
					{
						array[i] = Convert.ToInt64(values.GetValue(i));
					}
					ArrayList arrayList = new ArrayList();
					bool flag = false;
					while (!flag)
					{
						flag = true;
						foreach (long num2 in array)
						{
							if ((num2 != 0L && (num2 & num) == num2) || num2 == num)
							{
								arrayList.Add(Enum.ToObject(this.type, num2));
								num &= ~num2;
								flag = false;
							}
						}
						if (num == 0L)
						{
							flag = true;
						}
					}
					if (num != 0L)
					{
						arrayList.Add(Enum.ToObject(this.type, num));
					}
					return arrayList.ToArray(typeof(Enum));
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
			if (value is IConvertible)
			{
				Type underlyingType2 = Enum.GetUnderlyingType(this.type);
				if (underlyingType2 != value.GetType())
				{
					value = ((IConvertible)value).ToType(underlyingType2, culture);
				}
			}
			if (!this.IsFlags && !this.IsValid(context, value))
			{
				throw this.CreateValueNotValidException(value);
			}
			return Enum.Format(this.type, value, "G");
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || sourceType == typeof(Enum[]) || base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				string text = value as string;
				try
				{
					if (text.IndexOf(',') == -1)
					{
						return Enum.Parse(this.type, text, true);
					}
					long num = 0L;
					string[] array = text.Split(new char[] { ',' });
					foreach (string text2 in array)
					{
						Enum @enum = (Enum)Enum.Parse(this.type, text2, true);
						num |= Convert.ToInt64(@enum, culture);
					}
					return Enum.ToObject(this.type, num);
				}
				catch (Exception ex)
				{
					throw new FormatException(text + " is not a valid value for " + this.type.Name, ex);
				}
			}
			if (value is Enum[])
			{
				long num2 = 0L;
				foreach (Enum enum2 in (Enum[])value)
				{
					num2 |= Convert.ToInt64(enum2, culture);
				}
				return Enum.ToObject(this.type, num2);
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			return Enum.IsDefined(this.type, value);
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return !this.IsFlags;
		}

		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (this.stdValues == null)
			{
				Array values = Enum.GetValues(this.type);
				Array.Sort(values);
				this.stdValues = new TypeConverter.StandardValuesCollection(values);
			}
			return this.stdValues;
		}

		protected virtual IComparer Comparer
		{
			get
			{
				return new EnumConverter.EnumComparer();
			}
		}

		protected Type EnumType
		{
			get
			{
				return this.type;
			}
		}

		protected TypeConverter.StandardValuesCollection Values
		{
			get
			{
				return this.stdValues;
			}
			set
			{
				this.stdValues = value;
			}
		}

		private ArgumentException CreateValueNotValidException(object value)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "The value '{0}' is not a valid value for the enum '{1}'", new object[]
			{
				value,
				this.type.Name
			});
			return new ArgumentException(text);
		}

		private bool IsFlags
		{
			get
			{
				return this.type.IsDefined(typeof(FlagsAttribute), false);
			}
		}

		private Type type;

		private TypeConverter.StandardValuesCollection stdValues;

		private class EnumComparer : IComparer
		{
			int IComparer.Compare(object compareObject1, object compareObject2)
			{
				string text = compareObject1 as string;
				string text2 = compareObject2 as string;
				if (text == null || text2 == null)
				{
					return global::System.Collections.Comparer.Default.Compare(compareObject1, compareObject2);
				}
				return CultureInfo.InvariantCulture.CompareInfo.Compare(text, text2);
			}
		}
	}
}
