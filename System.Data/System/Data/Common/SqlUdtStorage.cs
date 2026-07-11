using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;

namespace System.Data.Common
{
	internal sealed class SqlUdtStorage : DataStorage
	{
		public SqlUdtStorage(DataColumn column, Type type)
			: this(column, type, SqlUdtStorage.GetStaticNullForUdtType(type))
		{
		}

		private SqlUdtStorage(DataColumn column, Type type, object nullValue)
			: base(column, type, nullValue, nullValue, typeof(ICloneable).IsAssignableFrom(type), DataStorage.GetStorageType(type))
		{
			this._implementsIXmlSerializable = typeof(IXmlSerializable).IsAssignableFrom(type);
			this._implementsIComparable = typeof(IComparable).IsAssignableFrom(type);
		}

		internal static object GetStaticNullForUdtType(Type type)
		{
			object obj;
			if (!SqlUdtStorage.s_typeToNull.TryGetValue(type, out obj))
			{
				PropertyInfo property = type.GetProperty("Null", BindingFlags.Static | BindingFlags.Public);
				if (property != null)
				{
					obj = property.GetValue(null, null);
				}
				else
				{
					FieldInfo field = type.GetField("Null", BindingFlags.Static | BindingFlags.Public);
					if (!(field != null))
					{
						throw ExceptionBuilder.INullableUDTwithoutStaticNull(type.AssemblyQualifiedName);
					}
					obj = field.GetValue(null);
				}
				Dictionary<Type, object> dictionary = SqlUdtStorage.s_typeToNull;
				lock (dictionary)
				{
					SqlUdtStorage.s_typeToNull[type] = obj;
				}
			}
			return obj;
		}

		public override bool IsNull(int record)
		{
			return ((INullable)this._values[record]).IsNull;
		}

		public override object Aggregate(int[] records, AggregateType kind)
		{
			throw ExceptionBuilder.AggregateException(kind, this._dataType);
		}

		public override int Compare(int recordNo1, int recordNo2)
		{
			return this.CompareValueTo(recordNo1, this._values[recordNo2]);
		}

		public override int CompareValueTo(int recordNo1, object value)
		{
			if (DBNull.Value == value)
			{
				value = this._nullValue;
			}
			if (this._implementsIComparable)
			{
				return ((IComparable)this._values[recordNo1]).CompareTo(value);
			}
			if (this._nullValue != value)
			{
				throw ExceptionBuilder.IComparableNotImplemented(this._dataType.AssemblyQualifiedName);
			}
			if (!((INullable)this._values[recordNo1]).IsNull)
			{
				return 1;
			}
			return 0;
		}

		public override void Copy(int recordNo1, int recordNo2)
		{
			base.CopyBits(recordNo1, recordNo2);
			this._values[recordNo2] = this._values[recordNo1];
		}

		public override object Get(int recordNo)
		{
			return this._values[recordNo];
		}

		public override void Set(int recordNo, object value)
		{
			if (DBNull.Value == value)
			{
				this._values[recordNo] = this._nullValue;
				base.SetNullBit(recordNo, true);
				return;
			}
			if (value == null)
			{
				if (this._isValueType)
				{
					throw ExceptionBuilder.StorageSetFailed();
				}
				this._values[recordNo] = this._nullValue;
				base.SetNullBit(recordNo, true);
				return;
			}
			else
			{
				if (!this._dataType.IsInstanceOfType(value))
				{
					throw ExceptionBuilder.StorageSetFailed();
				}
				this._values[recordNo] = value;
				base.SetNullBit(recordNo, false);
				return;
			}
		}

		public override void SetCapacity(int capacity)
		{
			object[] array = new object[capacity];
			if (this._values != null)
			{
				Array.Copy(this._values, 0, array, 0, Math.Min(capacity, this._values.Length));
			}
			this._values = array;
			base.SetCapacity(capacity);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override object ConvertXmlToObject(string s)
		{
			if (this._implementsIXmlSerializable)
			{
				object obj = Activator.CreateInstance(this._dataType, true);
				using (XmlTextReader xmlTextReader = new XmlTextReader(new StringReader("<col>" + s + "</col>")))
				{
					((IXmlSerializable)obj).ReadXml(xmlTextReader);
				}
				return obj;
			}
			StringReader stringReader = new StringReader(s);
			return ObjectStorage.GetXmlSerializer(this._dataType).Deserialize(stringReader);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override object ConvertXmlToObject(XmlReader xmlReader, XmlRootAttribute xmlAttrib)
		{
			if (xmlAttrib == null)
			{
				string text = xmlReader.GetAttribute("InstanceType", "urn:schemas-microsoft-com:xml-msdata");
				if (text == null)
				{
					string attribute = xmlReader.GetAttribute("InstanceType", "http://www.w3.org/2001/XMLSchema-instance");
					if (attribute != null)
					{
						text = XSDSchema.XsdtoClr(attribute).FullName;
					}
				}
				object obj = Activator.CreateInstance((text == null) ? this._dataType : Type.GetType(text), true);
				((IXmlSerializable)obj).ReadXml(xmlReader);
				return obj;
			}
			return ObjectStorage.GetXmlSerializer(this._dataType, xmlAttrib).Deserialize(xmlReader);
		}

		public override string ConvertObjectToXml(object value)
		{
			StringWriter stringWriter = new StringWriter(base.FormatProvider);
			if (this._implementsIXmlSerializable)
			{
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
				{
					((IXmlSerializable)value).WriteXml(xmlTextWriter);
					goto IL_0045;
				}
			}
			ObjectStorage.GetXmlSerializer(value.GetType()).Serialize(stringWriter, value);
			IL_0045:
			return stringWriter.ToString();
		}

		public override void ConvertObjectToXml(object value, XmlWriter xmlWriter, XmlRootAttribute xmlAttrib)
		{
			if (xmlAttrib == null)
			{
				((IXmlSerializable)value).WriteXml(xmlWriter);
				return;
			}
			ObjectStorage.GetXmlSerializer(this._dataType, xmlAttrib).Serialize(xmlWriter, value);
		}

		protected override object GetEmptyStorage(int recordCount)
		{
			return new object[recordCount];
		}

		protected override void CopyValue(int record, object store, BitArray nullbits, int storeIndex)
		{
			((object[])store)[storeIndex] = this._values[record];
			nullbits.Set(storeIndex, this.IsNull(record));
		}

		protected override void SetStorage(object store, BitArray nullbits)
		{
			this._values = (object[])store;
		}

		private object[] _values;

		private readonly bool _implementsIXmlSerializable;

		private readonly bool _implementsIComparable;

		private static readonly Dictionary<Type, object> s_typeToNull = new Dictionary<Type, object>();
	}
}
