using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public sealed class SqlBytes : IXmlSerializable, INullable, ISerializable
	{
		public SqlBytes()
		{
			this.buffer = null;
			this.notNull = false;
		}

		public SqlBytes(byte[] buffer)
		{
			if (buffer == null)
			{
				this.notNull = false;
				buffer = null;
			}
			else
			{
				this.notNull = true;
				this.buffer = buffer;
				this.storage = StorageState.Buffer;
			}
		}

		public SqlBytes(SqlBinary value)
		{
			if (value.IsNull)
			{
				this.notNull = false;
				this.buffer = null;
			}
			else
			{
				this.notNull = true;
				this.buffer = value.Value;
				this.storage = StorageState.Buffer;
			}
		}

		public SqlBytes(Stream s)
		{
			if (s == null)
			{
				this.notNull = false;
				this.buffer = null;
			}
			else
			{
				this.notNull = true;
				int num = (int)s.Length;
				this.buffer = new byte[num];
				s.Read(this.buffer, 0, num);
				this.storage = StorageState.Stream;
				this.stream = s;
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

		[MonoTODO]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		public byte[] Buffer
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

		public byte this[long offset]
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

		public static SqlBytes Null
		{
			get
			{
				return new SqlBytes();
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

		public Stream Stream
		{
			get
			{
				return this.stream;
			}
			set
			{
				this.stream = value;
			}
		}

		public byte[] Value
		{
			get
			{
				if (this.buffer == null)
				{
					return this.buffer;
				}
				return (byte[])this.buffer.Clone();
			}
		}

		public void SetLength(long value)
		{
			if (this.buffer == null)
			{
				throw new SqlTypeException("There is no buffer. Read or write operation failed.");
			}
			if (value < 0L || value > (long)this.buffer.Length)
			{
				throw new ArgumentOutOfRangeException("Specified argument was out of the range of valid values.");
			}
			Array.Resize<byte>(ref this.buffer, (int)value);
		}

		public void SetNull()
		{
			this.buffer = null;
			this.notNull = false;
		}

		public SqlBinary ToSqlBinary()
		{
			return new SqlBinary(this.buffer);
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("base64Binary", "http://www.w3.org/2001/XMLSchema");
		}

		public long Read(long offset, byte[] buffer, int offsetInBuffer, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (this.IsNull)
			{
				throw new SqlNullValueException("There is no buffer. Read or write failed");
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

		public void Write(long offset, byte[] buffer, int offsetInBuffer, int count)
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

		public static explicit operator SqlBytes(SqlBinary value)
		{
			if (value.IsNull)
			{
				return SqlBytes.Null;
			}
			return new SqlBytes(value.Value);
		}

		public static explicit operator SqlBinary(SqlBytes value)
		{
			if (value.IsNull)
			{
				return SqlBinary.Null;
			}
			return new SqlBinary(value.Value);
		}

		private bool notNull;

		private byte[] buffer;

		private StorageState storage = StorageState.UnmanagedBuffer;

		private Stream stream;
	}
}
