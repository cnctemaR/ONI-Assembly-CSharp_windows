using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public sealed class SqlChars : IXmlSerializable, INullable, ISerializable
	{
		public SqlChars()
		{
			this.notNull = false;
			this.buffer = null;
		}

		public SqlChars(char[] buffer)
		{
			if (buffer == null)
			{
				this.notNull = false;
				this.buffer = null;
			}
			else
			{
				this.notNull = true;
				this.buffer = buffer;
				this.storage = StorageState.Buffer;
			}
		}

		public SqlChars(SqlString value)
		{
			if (value.IsNull)
			{
				this.notNull = false;
				this.buffer = null;
			}
			else
			{
				this.notNull = true;
				this.buffer = value.Value.ToCharArray();
				this.storage = StorageState.Buffer;
			}
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			if (reader == null)
			{
				return;
			}
			switch (reader.ReadState)
			{
			case ReadState.Error:
			case ReadState.EndOfFile:
			case ReadState.Closed:
				return;
			default:
				reader.MoveToContent();
				if (reader.EOF)
				{
					return;
				}
				reader.Read();
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					return;
				}
				if (reader.Value.Length > 0)
				{
					if (string.Compare("Null", reader.Value) == 0)
					{
						this.notNull = false;
						return;
					}
					this.buffer = reader.Value.ToCharArray();
					this.notNull = true;
					this.storage = StorageState.Buffer;
				}
				return;
			}
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteString(this.buffer.ToString());
		}

		[MonoTODO]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		public char[] Buffer
		{
			get
			{
				return this.buffer;
			}
		}

		public bool IsNull
		{
			get
			{
				return !this.notNull;
			}
		}

		public char this[long offset]
		{
			get
			{
				if (this.buffer == null)
				{
					throw new SqlNullValueException("Data is Null");
				}
				if (offset < 0L || offset >= (long)this.buffer.Length)
				{
					throw new ArgumentOutOfRangeException("Parameter name: offset");
				}
				return this.buffer[(int)(checked((IntPtr)offset))];
			}
			set
			{
				if (this.notNull && offset >= 0L && offset < (long)this.buffer.Length)
				{
					this.buffer[(int)(checked((IntPtr)offset))] = value;
				}
			}
		}

		public long Length
		{
			get
			{
				if (!this.notNull || this.buffer == null)
				{
					throw new SqlNullValueException("Data is Null");
				}
				if (this.buffer.Length < 0)
				{
					return -1L;
				}
				return (long)this.buffer.Length;
			}
		}

		public long MaxLength
		{
			get
			{
				if (!this.notNull || this.buffer == null || this.storage == StorageState.Stream)
				{
					return -1L;
				}
				return (long)this.buffer.Length;
			}
		}

		public static SqlChars Null
		{
			get
			{
				return new SqlChars();
			}
		}

		public StorageState Storage
		{
			get
			{
				if (this.storage == StorageState.UnmanagedBuffer)
				{
					throw new SqlNullValueException("Data is Null");
				}
				return this.storage;
			}
		}

		public char[] Value
		{
			get
			{
				if (this.buffer == null)
				{
					return this.buffer;
				}
				return (char[])this.buffer.Clone();
			}
		}

		public void SetLength(long value)
		{
			if (this.buffer == null)
			{
				throw new SqlTypeException("There is no buffer");
			}
			if (value < 0L || value > (long)this.buffer.Length)
			{
				throw new ArgumentOutOfRangeException("Specified argument was out of the range of valid values.");
			}
			Array.Resize<char>(ref this.buffer, (int)value);
		}

		public void SetNull()
		{
			this.buffer = null;
			this.notNull = false;
		}

		public SqlString ToSqlString()
		{
			if (this.buffer == null)
			{
				return SqlString.Null;
			}
			return new SqlString(this.buffer.ToString());
		}

		public long Read(long offset, char[] buffer, int offsetInBuffer, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (this.IsNull)
			{
				throw new SqlNullValueException("There is no buffer. Read or write operation failed");
			}
			if ((long)count > this.MaxLength || count > buffer.Length || count < 0 || offsetInBuffer + count > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (offset < 0L || offset > this.MaxLength)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (offsetInBuffer < 0 || offsetInBuffer > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offsetInBuffer");
			}
			long num = (long)count;
			if ((long)count + offset > this.Length)
			{
				num = this.Length - offset;
			}
			Array.Copy(this.buffer, offset, buffer, (long)offsetInBuffer, num);
			return num;
		}

		public void Write(long offset, char[] buffer, int offsetInBuffer, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (this.IsNull)
			{
				throw new SqlTypeException("There is no buffer. Read or write operation failed.");
			}
			if (offset < 0L)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (offsetInBuffer < 0 || offsetInBuffer > buffer.Length || (long)offsetInBuffer > this.Length || (long)(offsetInBuffer + count) > this.Length || offsetInBuffer + count > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offsetInBuffer");
			}
			if (count < 0 || (long)count > this.MaxLength)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (offset > this.MaxLength || offset + (long)count > this.MaxLength)
			{
				throw new SqlTypeException("The buffer is insufficient. Read or write operation failed.");
			}
			if ((long)count + offset > this.Length && (long)count + offset <= this.MaxLength)
			{
				this.SetLength((long)count);
			}
			Array.Copy(buffer, (long)offsetInBuffer, this.buffer, offset, (long)count);
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			if (schemaSet != null && schemaSet.Count == 0)
			{
				XmlSchema xmlSchema = new XmlSchema();
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				xmlSchemaComplexType.Name = "string";
				xmlSchema.Items.Add(xmlSchemaComplexType);
				schemaSet.Add(xmlSchema);
			}
			return new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
		}

		public static explicit operator SqlString(SqlChars value)
		{
			if (value.IsNull)
			{
				return SqlString.Null;
			}
			return new SqlString(new string(value.Value));
		}

		public static explicit operator SqlChars(SqlString value)
		{
			if (value.IsNull)
			{
				return SqlChars.Null;
			}
			return new SqlChars(value.Value);
		}

		private bool notNull;

		private char[] buffer;

		private StorageState storage = StorageState.UnmanagedBuffer;
	}
}
