using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;

namespace System.Data.Common
{
	internal sealed class ObjectStorage : DataStorage
	{
		internal ObjectStorage(DataColumn column, Type type)
			: base(column, type, ObjectStorage.s_defaultValue, DBNull.Value, typeof(ICloneable).IsAssignableFrom(type), DataStorage.GetStorageType(type))
		{
			this._implementsIXmlSerializable = typeof(IXmlSerializable).IsAssignableFrom(type);
		}

		public override object Aggregate(int[] records, AggregateType kind)
		{
			throw ExceptionBuilder.AggregateException(kind, this._dataType);
		}

		public override int Compare(int recordNo1, int recordNo2)
		{
			object obj = this._values[recordNo1];
			object obj2 = this._values[recordNo2];
			if (obj == obj2)
			{
				return 0;
			}
			if (obj == null)
			{
				return -1;
			}
			if (obj2 == null)
			{
				return 1;
			}
			IComparable comparable = obj as IComparable;
			if (comparable != null)
			{
				try
				{
					return comparable.CompareTo(obj2);
				}
				catch (ArgumentException ex)
				{
					ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
				}
			}
			return this.CompareWithFamilies(obj, obj2);
		}

		public override int CompareValueTo(int recordNo1, object value)
		{
			object obj = this.Get(recordNo1);
			if (obj is IComparable && value.GetType() == obj.GetType())
			{
				return ((IComparable)obj).CompareTo(value);
			}
			if (obj == value)
			{
				return 0;
			}
			if (obj == null)
			{
				if (this._nullValue == value)
				{
					return 0;
				}
				return -1;
			}
			else
			{
				if (this._nullValue == value || value == null)
				{
					return 1;
				}
				return this.CompareWithFamilies(obj, value);
			}
		}

		private int CompareTo(object valueNo1, object valueNo2)
		{
			if (valueNo1 == null)
			{
				return -1;
			}
			if (valueNo2 == null)
			{
				return 1;
			}
			if (valueNo1 == valueNo2)
			{
				return 0;
			}
			if (valueNo1 == this._nullValue)
			{
				return -1;
			}
			if (valueNo2 == this._nullValue)
			{
				return 1;
			}
			if (valueNo1 is IComparable)
			{
				try
				{
					return ((IComparable)valueNo1).CompareTo(valueNo2);
				}
				catch (ArgumentException ex)
				{
					ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
				}
			}
			return this.CompareWithFamilies(valueNo1, valueNo2);
		}

		private int CompareWithFamilies(object valueNo1, object valueNo2)
		{
			ObjectStorage.Families family = this.GetFamily(valueNo1.GetType());
			ObjectStorage.Families family2 = this.GetFamily(valueNo2.GetType());
			if (family < family2)
			{
				return -1;
			}
			if (family > family2)
			{
				return 1;
			}
			switch (family)
			{
			case ObjectStorage.Families.DATETIME:
				valueNo1 = Convert.ToDateTime(valueNo1, base.FormatProvider);
				valueNo2 = Convert.ToDateTime(valueNo1, base.FormatProvider);
				goto IL_0137;
			case ObjectStorage.Families.NUMBER:
				valueNo1 = Convert.ToDouble(valueNo1, base.FormatProvider);
				valueNo2 = Convert.ToDouble(valueNo2, base.FormatProvider);
				goto IL_0137;
			case ObjectStorage.Families.BOOLEAN:
				valueNo1 = Convert.ToBoolean(valueNo1, base.FormatProvider);
				valueNo2 = Convert.ToBoolean(valueNo2, base.FormatProvider);
				goto IL_0137;
			case ObjectStorage.Families.ARRAY:
			{
				Array array = (Array)valueNo1;
				Array array2 = (Array)valueNo2;
				if (array.Length > array2.Length)
				{
					return 1;
				}
				if (array.Length < array2.Length)
				{
					return -1;
				}
				for (int i = 0; i < array.Length; i++)
				{
					int num = this.CompareTo(array.GetValue(i), array2.GetValue(i));
					if (num != 0)
					{
						return num;
					}
				}
				return 0;
			}
			}
			valueNo1 = valueNo1.ToString();
			valueNo2 = valueNo2.ToString();
			IL_0137:
			return ((IComparable)valueNo1).CompareTo(valueNo2);
		}

		public override void Copy(int recordNo1, int recordNo2)
		{
			this._values[recordNo2] = this._values[recordNo1];
		}

		public override object Get(int recordNo)
		{
			object obj = this._values[recordNo];
			if (obj != null)
			{
				return obj;
			}
			return this._nullValue;
		}

		private ObjectStorage.Families GetFamily(Type dataType)
		{
			switch (Type.GetTypeCode(dataType))
			{
			case TypeCode.Boolean:
				return ObjectStorage.Families.BOOLEAN;
			case TypeCode.Char:
				return ObjectStorage.Families.STRING;
			case TypeCode.SByte:
				return ObjectStorage.Families.STRING;
			case TypeCode.Byte:
				return ObjectStorage.Families.STRING;
			case TypeCode.Int16:
				return ObjectStorage.Families.NUMBER;
			case TypeCode.UInt16:
				return ObjectStorage.Families.NUMBER;
			case TypeCode.Int32:
				return ObjectStorage.Families.NUMBER;
			case TypeCode.UInt32:
				return ObjectStorage.Families.NUMBER;
			case TypeCode.Int64:
				return ObjectStorage.Families.NUMBER;
			case TypeCode.UInt64:
				return ObjectStorage.Families.NUMBER;
			case TypeCode.Single:
				return ObjectStorage.Families.NUMBER;
			case TypeCode.Double:
				return ObjectStorage.Families.NUMBER;
			case TypeCode.Decimal:
				return ObjectStorage.Families.NUMBER;
			case TypeCode.DateTime:
				return ObjectStorage.Families.DATETIME;
			case TypeCode.String:
				return ObjectStorage.Families.STRING;
			}
			if (typeof(TimeSpan) == dataType)
			{
				return ObjectStorage.Families.DATETIME;
			}
			if (dataType.IsArray)
			{
				return ObjectStorage.Families.ARRAY;
			}
			return ObjectStorage.Families.STRING;
		}

		public override bool IsNull(int record)
		{
			return this._values[record] == null;
		}

		public override void Set(int recordNo, object value)
		{
			if (this._nullValue == value)
			{
				this._values[recordNo] = null;
				return;
			}
			if (this._dataType == typeof(object) || this._dataType.IsInstanceOfType(value))
			{
				this._values[recordNo] = value;
				return;
			}
			Type type = value.GetType();
			if (this._dataType == typeof(Guid) && type == typeof(string))
			{
				this._values[recordNo] = new Guid((string)value);
				return;
			}
			if (!(this._dataType == typeof(byte[])))
			{
				throw ExceptionBuilder.StorageSetFailed();
			}
			if (type == typeof(bool))
			{
				this._values[recordNo] = BitConverter.GetBytes((bool)value);
				return;
			}
			if (type == typeof(char))
			{
				this._values[recordNo] = BitConverter.GetBytes((char)value);
				return;
			}
			if (type == typeof(short))
			{
				this._values[recordNo] = BitConverter.GetBytes((short)value);
				return;
			}
			if (type == typeof(int))
			{
				this._values[recordNo] = BitConverter.GetBytes((int)value);
				return;
			}
			if (type == typeof(long))
			{
				this._values[recordNo] = BitConverter.GetBytes((long)value);
				return;
			}
			if (type == typeof(ushort))
			{
				this._values[recordNo] = BitConverter.GetBytes((ushort)value);
				return;
			}
			if (type == typeof(uint))
			{
				this._values[recordNo] = BitConverter.GetBytes((uint)value);
				return;
			}
			if (type == typeof(ulong))
			{
				this._values[recordNo] = BitConverter.GetBytes((ulong)value);
				return;
			}
			if (type == typeof(float))
			{
				this._values[recordNo] = BitConverter.GetBytes((float)value);
				return;
			}
			if (type == typeof(double))
			{
				this._values[recordNo] = BitConverter.GetBytes((double)value);
				return;
			}
			throw ExceptionBuilder.StorageSetFailed();
		}

		public override void SetCapacity(int capacity)
		{
			object[] array = new object[capacity];
			if (this._values != null)
			{
				Array.Copy(this._values, 0, array, 0, Math.Min(capacity, this._values.Length));
			}
			this._values = array;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override object ConvertXmlToObject(string s)
		{
			Type dataType = this._dataType;
			if (dataType == typeof(byte[]))
			{
				return Convert.FromBase64String(s);
			}
			if (dataType == typeof(Type))
			{
				return Type.GetType(s);
			}
			if (dataType == typeof(Guid))
			{
				return new Guid(s);
			}
			if (dataType == typeof(Uri))
			{
				return new Uri(s);
			}
			if (this._implementsIXmlSerializable)
			{
				object obj = Activator.CreateInstance(this._dataType, true);
				using (XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(s)))
				{
					((IXmlSerializable)obj).ReadXml(xmlTextReader);
				}
				return obj;
			}
			StringReader stringReader = new StringReader(s);
			return ObjectStorage.GetXmlSerializer(dataType).Deserialize(stringReader);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override object ConvertXmlToObject(XmlReader xmlReader, XmlRootAttribute xmlAttrib)
		{
			bool flag = false;
			bool flag2 = false;
			object obj;
			if (xmlAttrib == null)
			{
				Type type = null;
				string attribute = xmlReader.GetAttribute("InstanceType", "urn:schemas-microsoft-com:xml-msdata");
				if (attribute == null || attribute.Length == 0)
				{
					string text = xmlReader.GetAttribute("type", "http://www.w3.org/2001/XMLSchema-instance");
					if (text != null && text.Length > 0)
					{
						string[] array = text.Split(new char[] { ':' });
						if (array.Length == 2 && xmlReader.LookupNamespace(array[0]) == "http://www.w3.org/2001/XMLSchema")
						{
							text = array[1];
						}
						type = XSDSchema.XsdtoClr(text);
						flag = true;
					}
					else if (this._dataType == typeof(object))
					{
						flag2 = true;
					}
				}
				if (flag2)
				{
					obj = xmlReader.ReadString();
				}
				else if (attribute == "Type")
				{
					obj = Type.GetType(xmlReader.ReadString());
					xmlReader.Read();
				}
				else
				{
					if (null == type)
					{
						type = ((attribute == null) ? this._dataType : DataStorage.GetType(attribute));
					}
					if (type == typeof(char) || type == typeof(Guid))
					{
						flag = true;
					}
					if (type == typeof(object))
					{
						throw ExceptionBuilder.CanNotDeserializeObjectType();
					}
					if (!flag)
					{
						obj = Activator.CreateInstance(type, true);
						((IXmlSerializable)obj).ReadXml(xmlReader);
					}
					else
					{
						if (type == typeof(string) && xmlReader.NodeType == XmlNodeType.Element && xmlReader.IsEmptyElement)
						{
							obj = string.Empty;
						}
						else
						{
							obj = xmlReader.ReadString();
							if (type != typeof(byte[]))
							{
								obj = SqlConvert.ChangeTypeForXML(obj, type);
							}
							else
							{
								obj = Convert.FromBase64String(obj.ToString());
							}
						}
						xmlReader.Read();
					}
				}
			}
			else
			{
				obj = ObjectStorage.GetXmlSerializer(this._dataType, xmlAttrib).Deserialize(xmlReader);
			}
			return obj;
		}

		public override string ConvertObjectToXml(object value)
		{
			if (value == null || value == this._nullValue)
			{
				return string.Empty;
			}
			Type dataType = this._dataType;
			if (dataType == typeof(byte[]) || (dataType == typeof(object) && value is byte[]))
			{
				return Convert.ToBase64String((byte[])value);
			}
			if (dataType == typeof(Type) || (dataType == typeof(object) && value is Type))
			{
				return ((Type)value).AssemblyQualifiedName;
			}
			if (!DataStorage.IsTypeCustomType(value.GetType()))
			{
				return (string)SqlConvert.ChangeTypeForXML(value, typeof(string));
			}
			if (Type.GetTypeCode(value.GetType()) != TypeCode.Object)
			{
				return value.ToString();
			}
			StringWriter stringWriter = new StringWriter(base.FormatProvider);
			if (this._implementsIXmlSerializable)
			{
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
				{
					((IXmlSerializable)value).WriteXml(xmlTextWriter);
				}
				return stringWriter.ToString();
			}
			ObjectStorage.GetXmlSerializer(value.GetType()).Serialize(stringWriter, value);
			return stringWriter.ToString();
		}

		public override void ConvertObjectToXml(object value, XmlWriter xmlWriter, XmlRootAttribute xmlAttrib)
		{
			if (xmlAttrib == null)
			{
				((IXmlSerializable)value).WriteXml(xmlWriter);
				return;
			}
			ObjectStorage.GetXmlSerializer(value.GetType(), xmlAttrib).Serialize(xmlWriter, value);
		}

		protected override object GetEmptyStorage(int recordCount)
		{
			return new object[recordCount];
		}

		protected override void CopyValue(int record, object store, BitArray nullbits, int storeIndex)
		{
			object[] array = (object[])store;
			array[storeIndex] = this._values[record];
			bool flag = this.IsNull(record);
			nullbits.Set(storeIndex, flag);
			if (!flag && array[storeIndex] is DateTime)
			{
				DateTime dateTime = (DateTime)array[storeIndex];
				if (dateTime.Kind == DateTimeKind.Local)
				{
					array[storeIndex] = DateTime.SpecifyKind(dateTime.ToUniversalTime(), DateTimeKind.Local);
				}
			}
		}

		protected override void SetStorage(object store, BitArray nullbits)
		{
			this._values = (object[])store;
			for (int i = 0; i < this._values.Length; i++)
			{
				if (this._values[i] is DateTime)
				{
					DateTime dateTime = (DateTime)this._values[i];
					if (dateTime.Kind == DateTimeKind.Local)
					{
						this._values[i] = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc).ToLocalTime();
					}
				}
			}
		}

		internal static void VerifyIDynamicMetaObjectProvider(Type type)
		{
			if (typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type) && !typeof(IXmlSerializable).IsAssignableFrom(type))
			{
				throw ADP.InvalidOperation("DataSet will not serialize types that implement IDynamicMetaObjectProvider but do not also implement IXmlSerializable.");
			}
		}

		internal static XmlSerializer GetXmlSerializer(Type type)
		{
			ObjectStorage.VerifyIDynamicMetaObjectProvider(type);
			return ObjectStorage.s_serializerFactory.CreateSerializer(type);
		}

		internal static XmlSerializer GetXmlSerializer(Type type, XmlRootAttribute attribute)
		{
			XmlSerializer xmlSerializer = null;
			KeyValuePair<Type, XmlRootAttribute> keyValuePair = new KeyValuePair<Type, XmlRootAttribute>(type, attribute);
			Dictionary<KeyValuePair<Type, XmlRootAttribute>, XmlSerializer> dictionary = ObjectStorage.s_tempAssemblyCache;
			if (dictionary == null || !dictionary.TryGetValue(keyValuePair, out xmlSerializer))
			{
				object obj = ObjectStorage.s_tempAssemblyCacheLock;
				lock (obj)
				{
					dictionary = ObjectStorage.s_tempAssemblyCache;
					if (dictionary == null || !dictionary.TryGetValue(keyValuePair, out xmlSerializer))
					{
						ObjectStorage.VerifyIDynamicMetaObjectProvider(type);
						if (dictionary != null)
						{
							Dictionary<KeyValuePair<Type, XmlRootAttribute>, XmlSerializer> dictionary2 = new Dictionary<KeyValuePair<Type, XmlRootAttribute>, XmlSerializer>(1 + dictionary.Count, ObjectStorage.TempAssemblyComparer.s_default);
							foreach (KeyValuePair<KeyValuePair<Type, XmlRootAttribute>, XmlSerializer> keyValuePair2 in dictionary)
							{
								dictionary2.Add(keyValuePair2.Key, keyValuePair2.Value);
							}
							dictionary = dictionary2;
						}
						else
						{
							dictionary = new Dictionary<KeyValuePair<Type, XmlRootAttribute>, XmlSerializer>(ObjectStorage.TempAssemblyComparer.s_default);
						}
						keyValuePair = new KeyValuePair<Type, XmlRootAttribute>(type, new XmlRootAttribute());
						keyValuePair.Value.ElementName = attribute.ElementName;
						keyValuePair.Value.Namespace = attribute.Namespace;
						keyValuePair.Value.DataType = attribute.DataType;
						keyValuePair.Value.IsNullable = attribute.IsNullable;
						xmlSerializer = ObjectStorage.s_serializerFactory.CreateSerializer(type, attribute);
						dictionary.Add(keyValuePair, xmlSerializer);
						ObjectStorage.s_tempAssemblyCache = dictionary;
					}
				}
			}
			return xmlSerializer;
		}

		private static readonly object s_defaultValue = null;

		private object[] _values;

		private readonly bool _implementsIXmlSerializable;

		private static readonly object s_tempAssemblyCacheLock = new object();

		private static Dictionary<KeyValuePair<Type, XmlRootAttribute>, XmlSerializer> s_tempAssemblyCache;

		private static readonly XmlSerializerFactory s_serializerFactory = new XmlSerializerFactory();

		private enum Families
		{
			DATETIME,
			NUMBER,
			STRING,
			BOOLEAN,
			ARRAY
		}

		private class TempAssemblyComparer : IEqualityComparer<KeyValuePair<Type, XmlRootAttribute>>
		{
			private TempAssemblyComparer()
			{
			}

			public bool Equals(KeyValuePair<Type, XmlRootAttribute> x, KeyValuePair<Type, XmlRootAttribute> y)
			{
				return x.Key == y.Key && ((x.Value == null && y.Value == null) || (x.Value != null && y.Value != null && x.Value.ElementName == y.Value.ElementName && x.Value.Namespace == y.Value.Namespace && x.Value.DataType == y.Value.DataType && x.Value.IsNullable == y.Value.IsNullable));
			}

			public int GetHashCode(KeyValuePair<Type, XmlRootAttribute> obj)
			{
				return obj.Key.GetHashCode() + obj.Value.ElementName.GetHashCode();
			}

			internal static readonly IEqualityComparer<KeyValuePair<Type, XmlRootAttribute>> s_default = new ObjectStorage.TempAssemblyComparer();
		}
	}
}
