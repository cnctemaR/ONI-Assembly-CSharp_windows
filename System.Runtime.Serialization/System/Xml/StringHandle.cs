using System;

namespace System.Xml
{
	internal class StringHandle
	{
		public StringHandle(XmlBufferReader bufferReader)
		{
			this.bufferReader = bufferReader;
			this.SetValue(0, 0);
		}

		public void SetValue(int offset, int length)
		{
			this.type = StringHandle.StringHandleType.UTF8;
			this.offset = offset;
			this.length = length;
		}

		public void SetConstantValue(StringHandleConstStringType constStringType)
		{
			this.type = StringHandle.StringHandleType.ConstString;
			this.key = (int)constStringType;
		}

		public void SetValue(int offset, int length, bool escaped)
		{
			this.type = (escaped ? StringHandle.StringHandleType.EscapedUTF8 : StringHandle.StringHandleType.UTF8);
			this.offset = offset;
			this.length = length;
		}

		public void SetValue(int key)
		{
			this.type = StringHandle.StringHandleType.Dictionary;
			this.key = key;
		}

		public void SetValue(StringHandle value)
		{
			this.type = value.type;
			this.key = value.key;
			this.offset = value.offset;
			this.length = value.length;
		}

		public bool IsEmpty
		{
			get
			{
				if (this.type == StringHandle.StringHandleType.UTF8)
				{
					return this.length == 0;
				}
				return this.Equals2(string.Empty);
			}
		}

		public bool IsXmlns
		{
			get
			{
				if (this.type != StringHandle.StringHandleType.UTF8)
				{
					return this.Equals2("xmlns");
				}
				if (this.length != 5)
				{
					return false;
				}
				byte[] buffer = this.bufferReader.Buffer;
				int num = this.offset;
				return buffer[num] == 120 && buffer[num + 1] == 109 && buffer[num + 2] == 108 && buffer[num + 3] == 110 && buffer[num + 4] == 115;
			}
		}

		public void ToPrefixHandle(PrefixHandle prefix)
		{
			prefix.SetValue(this.offset, this.length);
		}

		public string GetString(XmlNameTable nameTable)
		{
			StringHandle.StringHandleType stringHandleType = this.type;
			if (stringHandleType == StringHandle.StringHandleType.UTF8)
			{
				return this.bufferReader.GetString(this.offset, this.length, nameTable);
			}
			if (stringHandleType == StringHandle.StringHandleType.Dictionary)
			{
				return nameTable.Add(this.bufferReader.GetDictionaryString(this.key).Value);
			}
			if (stringHandleType == StringHandle.StringHandleType.ConstString)
			{
				return nameTable.Add(StringHandle.constStrings[this.key]);
			}
			return this.bufferReader.GetEscapedString(this.offset, this.length, nameTable);
		}

		public string GetString()
		{
			StringHandle.StringHandleType stringHandleType = this.type;
			if (stringHandleType == StringHandle.StringHandleType.UTF8)
			{
				return this.bufferReader.GetString(this.offset, this.length);
			}
			if (stringHandleType == StringHandle.StringHandleType.Dictionary)
			{
				return this.bufferReader.GetDictionaryString(this.key).Value;
			}
			if (stringHandleType == StringHandle.StringHandleType.ConstString)
			{
				return StringHandle.constStrings[this.key];
			}
			return this.bufferReader.GetEscapedString(this.offset, this.length);
		}

		public byte[] GetString(out int offset, out int length)
		{
			StringHandle.StringHandleType stringHandleType = this.type;
			if (stringHandleType == StringHandle.StringHandleType.UTF8)
			{
				offset = this.offset;
				length = this.length;
				return this.bufferReader.Buffer;
			}
			if (stringHandleType == StringHandle.StringHandleType.Dictionary)
			{
				byte[] array = this.bufferReader.GetDictionaryString(this.key).ToUTF8();
				offset = 0;
				length = array.Length;
				return array;
			}
			if (stringHandleType == StringHandle.StringHandleType.ConstString)
			{
				byte[] array2 = XmlConverter.ToBytes(StringHandle.constStrings[this.key]);
				offset = 0;
				length = array2.Length;
				return array2;
			}
			byte[] array3 = XmlConverter.ToBytes(this.bufferReader.GetEscapedString(this.offset, this.length));
			offset = 0;
			length = array3.Length;
			return array3;
		}

		public bool TryGetDictionaryString(out XmlDictionaryString value)
		{
			if (this.type == StringHandle.StringHandleType.Dictionary)
			{
				value = this.bufferReader.GetDictionaryString(this.key);
				return true;
			}
			if (this.IsEmpty)
			{
				value = XmlDictionaryString.Empty;
				return true;
			}
			value = null;
			return false;
		}

		public override string ToString()
		{
			return this.GetString();
		}

		private bool Equals2(int key2, XmlBufferReader bufferReader2)
		{
			StringHandle.StringHandleType stringHandleType = this.type;
			if (stringHandleType == StringHandle.StringHandleType.Dictionary)
			{
				return this.bufferReader.Equals2(this.key, key2, bufferReader2);
			}
			if (stringHandleType == StringHandle.StringHandleType.UTF8)
			{
				return this.bufferReader.Equals2(this.offset, this.length, bufferReader2.GetDictionaryString(key2).Value);
			}
			return this.GetString() == this.bufferReader.GetDictionaryString(key2).Value;
		}

		private bool Equals2(XmlDictionaryString xmlString2)
		{
			StringHandle.StringHandleType stringHandleType = this.type;
			if (stringHandleType == StringHandle.StringHandleType.Dictionary)
			{
				return this.bufferReader.Equals2(this.key, xmlString2);
			}
			if (stringHandleType == StringHandle.StringHandleType.UTF8)
			{
				return this.bufferReader.Equals2(this.offset, this.length, xmlString2.ToUTF8());
			}
			return this.GetString() == xmlString2.Value;
		}

		private bool Equals2(string s2)
		{
			StringHandle.StringHandleType stringHandleType = this.type;
			if (stringHandleType == StringHandle.StringHandleType.Dictionary)
			{
				return this.bufferReader.GetDictionaryString(this.key).Value == s2;
			}
			if (stringHandleType == StringHandle.StringHandleType.UTF8)
			{
				return this.bufferReader.Equals2(this.offset, this.length, s2);
			}
			return this.GetString() == s2;
		}

		private bool Equals2(int offset2, int length2, XmlBufferReader bufferReader2)
		{
			StringHandle.StringHandleType stringHandleType = this.type;
			if (stringHandleType == StringHandle.StringHandleType.Dictionary)
			{
				return bufferReader2.Equals2(offset2, length2, this.bufferReader.GetDictionaryString(this.key).Value);
			}
			if (stringHandleType == StringHandle.StringHandleType.UTF8)
			{
				return this.bufferReader.Equals2(this.offset, this.length, bufferReader2, offset2, length2);
			}
			return this.GetString() == this.bufferReader.GetString(offset2, length2);
		}

		private bool Equals2(StringHandle s2)
		{
			StringHandle.StringHandleType stringHandleType = s2.type;
			if (stringHandleType == StringHandle.StringHandleType.Dictionary)
			{
				return this.Equals2(s2.key, s2.bufferReader);
			}
			if (stringHandleType == StringHandle.StringHandleType.UTF8)
			{
				return this.Equals2(s2.offset, s2.length, s2.bufferReader);
			}
			return this.Equals2(s2.GetString());
		}

		public static bool operator ==(StringHandle s1, XmlDictionaryString xmlString2)
		{
			return s1.Equals2(xmlString2);
		}

		public static bool operator !=(StringHandle s1, XmlDictionaryString xmlString2)
		{
			return !s1.Equals2(xmlString2);
		}

		public static bool operator ==(StringHandle s1, string s2)
		{
			return s1.Equals2(s2);
		}

		public static bool operator !=(StringHandle s1, string s2)
		{
			return !s1.Equals2(s2);
		}

		public static bool operator ==(StringHandle s1, StringHandle s2)
		{
			return s1.Equals2(s2);
		}

		public static bool operator !=(StringHandle s1, StringHandle s2)
		{
			return !s1.Equals2(s2);
		}

		public int CompareTo(StringHandle that)
		{
			if (this.type == StringHandle.StringHandleType.UTF8 && that.type == StringHandle.StringHandleType.UTF8)
			{
				return this.bufferReader.Compare(this.offset, this.length, that.offset, that.length);
			}
			return string.Compare(this.GetString(), that.GetString(), StringComparison.Ordinal);
		}

		public override bool Equals(object obj)
		{
			StringHandle stringHandle = obj as StringHandle;
			return stringHandle != null && this == stringHandle;
		}

		public override int GetHashCode()
		{
			return this.GetString().GetHashCode();
		}

		private XmlBufferReader bufferReader;

		private StringHandle.StringHandleType type;

		private int key;

		private int offset;

		private int length;

		private static string[] constStrings = new string[] { "type", "root", "item" };

		private enum StringHandleType
		{
			Dictionary,
			UTF8,
			EscapedUTF8,
			ConstString
		}
	}
}
