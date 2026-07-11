using System;

namespace System.Xml
{
	internal class PrefixHandle
	{
		public PrefixHandle(XmlBufferReader bufferReader)
		{
			this.bufferReader = bufferReader;
		}

		public void SetValue(PrefixHandleType type)
		{
			this.type = type;
		}

		public void SetValue(PrefixHandle prefix)
		{
			this.type = prefix.type;
			this.offset = prefix.offset;
			this.length = prefix.length;
		}

		public void SetValue(int offset, int length)
		{
			if (length == 0)
			{
				this.SetValue(PrefixHandleType.Empty);
				return;
			}
			if (length == 1)
			{
				byte @byte = this.bufferReader.GetByte(offset);
				if (@byte >= 97 && @byte <= 122)
				{
					this.SetValue(PrefixHandle.GetAlphaPrefix((int)(@byte - 97)));
					return;
				}
			}
			this.type = PrefixHandleType.Buffer;
			this.offset = offset;
			this.length = length;
		}

		public bool IsEmpty
		{
			get
			{
				return this.type == PrefixHandleType.Empty;
			}
		}

		public bool IsXmlns
		{
			get
			{
				if (this.type != PrefixHandleType.Buffer)
				{
					return false;
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

		public bool IsXml
		{
			get
			{
				if (this.type != PrefixHandleType.Buffer)
				{
					return false;
				}
				if (this.length != 3)
				{
					return false;
				}
				byte[] buffer = this.bufferReader.Buffer;
				int num = this.offset;
				return buffer[num] == 120 && buffer[num + 1] == 109 && buffer[num + 2] == 108;
			}
		}

		public bool TryGetShortPrefix(out PrefixHandleType type)
		{
			type = this.type;
			return type != PrefixHandleType.Buffer;
		}

		public static string GetString(PrefixHandleType type)
		{
			return PrefixHandle.prefixStrings[(int)type];
		}

		public static PrefixHandleType GetAlphaPrefix(int index)
		{
			return PrefixHandleType.A + index;
		}

		public static byte[] GetString(PrefixHandleType type, out int offset, out int length)
		{
			if (type == PrefixHandleType.Empty)
			{
				offset = 0;
				length = 0;
			}
			else
			{
				length = 1;
				offset = type - PrefixHandleType.A;
			}
			return PrefixHandle.prefixBuffer;
		}

		public string GetString(XmlNameTable nameTable)
		{
			PrefixHandleType prefixHandleType = this.type;
			if (prefixHandleType != PrefixHandleType.Buffer)
			{
				return PrefixHandle.GetString(prefixHandleType);
			}
			return this.bufferReader.GetString(this.offset, this.length, nameTable);
		}

		public string GetString()
		{
			PrefixHandleType prefixHandleType = this.type;
			if (prefixHandleType != PrefixHandleType.Buffer)
			{
				return PrefixHandle.GetString(prefixHandleType);
			}
			return this.bufferReader.GetString(this.offset, this.length);
		}

		public byte[] GetString(out int offset, out int length)
		{
			PrefixHandleType prefixHandleType = this.type;
			if (prefixHandleType != PrefixHandleType.Buffer)
			{
				return PrefixHandle.GetString(prefixHandleType, out offset, out length);
			}
			offset = this.offset;
			length = this.length;
			return this.bufferReader.Buffer;
		}

		public int CompareTo(PrefixHandle that)
		{
			return this.GetString().CompareTo(that.GetString());
		}

		private bool Equals2(PrefixHandle prefix2)
		{
			PrefixHandleType prefixHandleType = this.type;
			PrefixHandleType prefixHandleType2 = prefix2.type;
			if (prefixHandleType != prefixHandleType2)
			{
				return false;
			}
			if (prefixHandleType != PrefixHandleType.Buffer)
			{
				return true;
			}
			if (this.bufferReader == prefix2.bufferReader)
			{
				return this.bufferReader.Equals2(this.offset, this.length, prefix2.offset, prefix2.length);
			}
			return this.bufferReader.Equals2(this.offset, this.length, prefix2.bufferReader, prefix2.offset, prefix2.length);
		}

		private bool Equals2(string prefix2)
		{
			PrefixHandleType prefixHandleType = this.type;
			if (prefixHandleType != PrefixHandleType.Buffer)
			{
				return PrefixHandle.GetString(prefixHandleType) == prefix2;
			}
			return this.bufferReader.Equals2(this.offset, this.length, prefix2);
		}

		private bool Equals2(XmlDictionaryString prefix2)
		{
			return this.Equals2(prefix2.Value);
		}

		public static bool operator ==(PrefixHandle prefix1, string prefix2)
		{
			return prefix1.Equals2(prefix2);
		}

		public static bool operator !=(PrefixHandle prefix1, string prefix2)
		{
			return !prefix1.Equals2(prefix2);
		}

		public static bool operator ==(PrefixHandle prefix1, XmlDictionaryString prefix2)
		{
			return prefix1.Equals2(prefix2);
		}

		public static bool operator !=(PrefixHandle prefix1, XmlDictionaryString prefix2)
		{
			return !prefix1.Equals2(prefix2);
		}

		public static bool operator ==(PrefixHandle prefix1, PrefixHandle prefix2)
		{
			return prefix1.Equals2(prefix2);
		}

		public static bool operator !=(PrefixHandle prefix1, PrefixHandle prefix2)
		{
			return !prefix1.Equals2(prefix2);
		}

		public override bool Equals(object obj)
		{
			PrefixHandle prefixHandle = obj as PrefixHandle;
			return prefixHandle != null && this == prefixHandle;
		}

		public override string ToString()
		{
			return this.GetString();
		}

		public override int GetHashCode()
		{
			return this.GetString().GetHashCode();
		}

		private XmlBufferReader bufferReader;

		private PrefixHandleType type;

		private int offset;

		private int length;

		private static string[] prefixStrings = new string[]
		{
			"", "a", "b", "c", "d", "e", "f", "g", "h", "i",
			"j", "k", "l", "m", "n", "o", "p", "q", "r", "s",
			"t", "u", "v", "w", "x", "y", "z"
		};

		private static byte[] prefixBuffer = new byte[]
		{
			97, 98, 99, 100, 101, 102, 103, 104, 105, 106,
			107, 108, 109, 110, 111, 112, 113, 114, 115, 116,
			117, 118, 119, 120, 121, 122
		};
	}
}
