using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public sealed class SqlXml : IXmlSerializable, INullable
	{
		public SqlXml()
		{
			this.notNull = false;
			this.xmlValue = null;
		}

		public SqlXml(Stream value)
		{
			if (value == null)
			{
				this.notNull = false;
				this.xmlValue = null;
			}
			else
			{
				int i = (int)value.Length;
				if (i < 1)
				{
					this.xmlValue = string.Empty;
				}
				else
				{
					int num = 8192;
					StringBuilder stringBuilder = new StringBuilder(i);
					value.Position = 0L;
					if (i < num)
					{
						num = i;
					}
					byte[] array = new byte[num];
					while (i > 0)
					{
						int num2 = value.Read(array, 0, num);
						stringBuilder.Append(Encoding.Unicode.GetString(array, 0, num2));
						if (num2 == 0)
						{
							break;
						}
						i -= num2;
					}
					this.xmlValue = stringBuilder.ToString();
				}
				this.notNull = true;
			}
		}

		public SqlXml(XmlReader value)
		{
			if (value == null)
			{
				this.notNull = false;
				this.xmlValue = null;
			}
			else
			{
				if (value.Read())
				{
					value.MoveToContent();
					this.xmlValue = value.ReadOuterXml();
				}
				else
				{
					this.xmlValue = string.Empty;
				}
				this.notNull = true;
			}
		}

		[MonoTODO]
		XmlSchema IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		void IXmlSerializable.ReadXml(XmlReader r)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			throw new NotImplementedException();
		}

		public bool IsNull
		{
			get
			{
				return !this.notNull;
			}
		}

		public static SqlXml Null
		{
			get
			{
				return new SqlXml();
			}
		}

		public string Value
		{
			get
			{
				if (this.notNull)
				{
					return this.xmlValue;
				}
				throw new SqlNullValueException();
			}
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema");
		}

		public XmlReader CreateReader()
		{
			if (this.notNull)
			{
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
				return XmlReader.Create(new StringReader(this.xmlValue), xmlReaderSettings);
			}
			throw new SqlNullValueException();
		}

		private bool notNull;

		private string xmlValue;
	}
}
