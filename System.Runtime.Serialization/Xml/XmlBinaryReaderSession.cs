using System;
using System.Collections.Generic;

namespace System.Xml
{
	public class XmlBinaryReaderSession : IXmlDictionary
	{
		public XmlDictionaryString Add(int id, string value)
		{
			XmlDictionaryString xmlDictionaryString = this.dic.Add(value);
			this.store[id] = xmlDictionaryString;
			return xmlDictionaryString;
		}

		public void Clear()
		{
			this.store.Clear();
		}

		public bool TryLookup(int key, out XmlDictionaryString result)
		{
			return this.store.TryGetValue(key, out result);
		}

		public bool TryLookup(string value, out XmlDictionaryString result)
		{
			foreach (XmlDictionaryString xmlDictionaryString in this.store.Values)
			{
				if (xmlDictionaryString.Value == value)
				{
					result = xmlDictionaryString;
					return true;
				}
			}
			result = null;
			return false;
		}

		public bool TryLookup(XmlDictionaryString value, out XmlDictionaryString result)
		{
			foreach (XmlDictionaryString xmlDictionaryString in this.store.Values)
			{
				if (xmlDictionaryString == value)
				{
					result = xmlDictionaryString;
					return true;
				}
			}
			result = null;
			return false;
		}

		private XmlDictionary dic = new XmlDictionary();

		private Dictionary<int, XmlDictionaryString> store = new Dictionary<int, XmlDictionaryString>();
	}
}
