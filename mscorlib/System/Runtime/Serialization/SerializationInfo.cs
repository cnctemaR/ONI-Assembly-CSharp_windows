using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public sealed class SerializationInfo
	{
		private SerializationInfo(Type type)
		{
			this.assemblyName = type.Assembly.FullName;
			this.fullTypeName = type.FullName;
			this.converter = new FormatterConverter();
		}

		private SerializationInfo(Type type, SerializationEntry[] data)
		{
			int num = data.Length;
			this.assemblyName = type.Assembly.FullName;
			this.fullTypeName = type.FullName;
			this.converter = new FormatterConverter();
			for (int i = 0; i < num; i++)
			{
				this.serialized.Add(data[i].Name, data[i]);
				this.values.Add(data[i]);
			}
		}

		[CLSCompliant(false)]
		public SerializationInfo(Type type, IFormatterConverter converter)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type", "Null argument");
			}
			if (converter == null)
			{
				throw new ArgumentNullException("converter", "Null argument");
			}
			this.converter = converter;
			this.assemblyName = type.Assembly.FullName;
			this.fullTypeName = type.FullName;
		}

		public string AssemblyName
		{
			get
			{
				return this.assemblyName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Argument is null.");
				}
				this.assemblyName = value;
			}
		}

		public string FullTypeName
		{
			get
			{
				return this.fullTypeName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Argument is null.");
				}
				this.fullTypeName = value;
			}
		}

		public int MemberCount
		{
			get
			{
				return this.serialized.Count;
			}
		}

		public void AddValue(string name, object value, Type type)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name is null");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type is null");
			}
			if (this.serialized.ContainsKey(name))
			{
				throw new SerializationException("Value has been serialized already.");
			}
			SerializationEntry serializationEntry = new SerializationEntry(name, type, value);
			this.serialized.Add(name, serializationEntry);
			this.values.Add(serializationEntry);
		}

		public object GetValue(string name, Type type)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name is null.");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (!this.serialized.ContainsKey(name))
			{
				throw new SerializationException("No element named " + name + " could be found.");
			}
			SerializationEntry serializationEntry = (SerializationEntry)this.serialized[name];
			if (serializationEntry.Value != null && !type.IsInstanceOfType(serializationEntry.Value))
			{
				return this.converter.Convert(serializationEntry.Value, type);
			}
			return serializationEntry.Value;
		}

		public void SetType(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type is null.");
			}
			this.fullTypeName = type.FullName;
			this.assemblyName = type.Assembly.FullName;
		}

		public SerializationInfoEnumerator GetEnumerator()
		{
			return new SerializationInfoEnumerator(this.values);
		}

		public void AddValue(string name, short value)
		{
			this.AddValue(name, value, typeof(short));
		}

		[CLSCompliant(false)]
		public void AddValue(string name, ushort value)
		{
			this.AddValue(name, value, typeof(ushort));
		}

		public void AddValue(string name, int value)
		{
			this.AddValue(name, value, typeof(int));
		}

		public void AddValue(string name, byte value)
		{
			this.AddValue(name, value, typeof(byte));
		}

		public void AddValue(string name, bool value)
		{
			this.AddValue(name, value, typeof(bool));
		}

		public void AddValue(string name, char value)
		{
			this.AddValue(name, value, typeof(char));
		}

		[CLSCompliant(false)]
		public void AddValue(string name, sbyte value)
		{
			this.AddValue(name, value, typeof(sbyte));
		}

		public void AddValue(string name, double value)
		{
			this.AddValue(name, value, typeof(double));
		}

		public void AddValue(string name, decimal value)
		{
			this.AddValue(name, value, typeof(decimal));
		}

		public void AddValue(string name, DateTime value)
		{
			this.AddValue(name, value, typeof(DateTime));
		}

		public void AddValue(string name, float value)
		{
			this.AddValue(name, value, typeof(float));
		}

		[CLSCompliant(false)]
		public void AddValue(string name, uint value)
		{
			this.AddValue(name, value, typeof(uint));
		}

		public void AddValue(string name, long value)
		{
			this.AddValue(name, value, typeof(long));
		}

		[CLSCompliant(false)]
		public void AddValue(string name, ulong value)
		{
			this.AddValue(name, value, typeof(ulong));
		}

		public void AddValue(string name, object value)
		{
			if (value == null)
			{
				this.AddValue(name, value, typeof(object));
			}
			else
			{
				this.AddValue(name, value, value.GetType());
			}
		}

		public bool GetBoolean(string name)
		{
			object value = this.GetValue(name, typeof(bool));
			return this.converter.ToBoolean(value);
		}

		public byte GetByte(string name)
		{
			object value = this.GetValue(name, typeof(byte));
			return this.converter.ToByte(value);
		}

		public char GetChar(string name)
		{
			object value = this.GetValue(name, typeof(char));
			return this.converter.ToChar(value);
		}

		public DateTime GetDateTime(string name)
		{
			object value = this.GetValue(name, typeof(DateTime));
			return this.converter.ToDateTime(value);
		}

		public decimal GetDecimal(string name)
		{
			object value = this.GetValue(name, typeof(decimal));
			return this.converter.ToDecimal(value);
		}

		public double GetDouble(string name)
		{
			object value = this.GetValue(name, typeof(double));
			return this.converter.ToDouble(value);
		}

		public short GetInt16(string name)
		{
			object value = this.GetValue(name, typeof(short));
			return this.converter.ToInt16(value);
		}

		public int GetInt32(string name)
		{
			object value = this.GetValue(name, typeof(int));
			return this.converter.ToInt32(value);
		}

		public long GetInt64(string name)
		{
			object value = this.GetValue(name, typeof(long));
			return this.converter.ToInt64(value);
		}

		[CLSCompliant(false)]
		public sbyte GetSByte(string name)
		{
			object value = this.GetValue(name, typeof(sbyte));
			return this.converter.ToSByte(value);
		}

		public float GetSingle(string name)
		{
			object value = this.GetValue(name, typeof(float));
			return this.converter.ToSingle(value);
		}

		public string GetString(string name)
		{
			object value = this.GetValue(name, typeof(string));
			if (value == null)
			{
				return null;
			}
			return this.converter.ToString(value);
		}

		[CLSCompliant(false)]
		public ushort GetUInt16(string name)
		{
			object value = this.GetValue(name, typeof(ushort));
			return this.converter.ToUInt16(value);
		}

		[CLSCompliant(false)]
		public uint GetUInt32(string name)
		{
			object value = this.GetValue(name, typeof(uint));
			return this.converter.ToUInt32(value);
		}

		[CLSCompliant(false)]
		public ulong GetUInt64(string name)
		{
			object value = this.GetValue(name, typeof(ulong));
			return this.converter.ToUInt64(value);
		}

		private SerializationEntry[] get_entries()
		{
			SerializationEntry[] array = new SerializationEntry[this.MemberCount];
			int num = 0;
			foreach (SerializationEntry serializationEntry in this)
			{
				array[num++] = serializationEntry;
			}
			return array;
		}

		private Hashtable serialized = new Hashtable();

		private ArrayList values = new ArrayList();

		private string assemblyName;

		private string fullTypeName;

		private IFormatterConverter converter;
	}
}
