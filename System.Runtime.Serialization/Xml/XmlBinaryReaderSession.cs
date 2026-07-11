using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace System.Xml
{
	public class XmlBinaryReaderSession : IXmlDictionary
	{
		public XmlDictionaryString Add(int id, string value)
		{
			if (id < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(global::System.Runtime.Serialization.SR.GetString("ID must be >= 0.")));
			}
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
			}
			XmlDictionaryString xmlDictionaryString;
			if (this.TryLookup(id, out xmlDictionaryString))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("ID already defined.")));
			}
			xmlDictionaryString = new XmlDictionaryString(this, value, id);
			if (id >= 2048)
			{
				if (this.stringDict == null)
				{
					this.stringDict = new Dictionary<int, XmlDictionaryString>();
				}
				this.stringDict.Add(id, xmlDictionaryString);
			}
			else
			{
				if (this.strings == null)
				{
					this.strings = new XmlDictionaryString[Math.Max(id + 1, 16)];
				}
				else if (id >= this.strings.Length)
				{
					XmlDictionaryString[] array = new XmlDictionaryString[Math.Min(Math.Max(id + 1, this.strings.Length * 2), 2048)];
					Array.Copy(this.strings, array, this.strings.Length);
					this.strings = array;
				}
				this.strings[id] = xmlDictionaryString;
			}
			return xmlDictionaryString;
		}

		public bool TryLookup(int key, out XmlDictionaryString result)
		{
			if (this.strings != null && key >= 0 && key < this.strings.Length)
			{
				result = this.strings[key];
				return result != null;
			}
			if (key >= 2048 && this.stringDict != null)
			{
				return this.stringDict.TryGetValue(key, out result);
			}
			result = null;
			return false;
		}

		public bool TryLookup(string value, out XmlDictionaryString result)
		{
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
			}
			if (this.strings != null)
			{
				for (int i = 0; i < this.strings.Length; i++)
				{
					XmlDictionaryString xmlDictionaryString = this.strings[i];
					if (xmlDictionaryString != null && xmlDictionaryString.Value == value)
					{
						result = xmlDictionaryString;
						return true;
					}
				}
			}
			if (this.stringDict != null)
			{
				foreach (XmlDictionaryString xmlDictionaryString2 in this.stringDict.Values)
				{
					if (xmlDictionaryString2.Value == value)
					{
						result = xmlDictionaryString2;
						return true;
					}
				}
			}
			result = null;
			return false;
		}

		public bool TryLookup(XmlDictionaryString value, out XmlDictionaryString result)
		{
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
			}
			if (value.Dictionary != this)
			{
				result = null;
				return false;
			}
			result = value;
			return true;
		}

		public void Clear()
		{
			if (this.strings != null)
			{
				Array.Clear(this.strings, 0, this.strings.Length);
			}
			if (this.stringDict != null)
			{
				this.stringDict.Clear();
			}
		}

		private const int MaxArrayEntries = 2048;

		private XmlDictionaryString[] strings;

		private Dictionary<int, XmlDictionaryString> stringDict;
	}
}
