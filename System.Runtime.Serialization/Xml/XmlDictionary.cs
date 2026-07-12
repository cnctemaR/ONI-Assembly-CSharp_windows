using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace System.Xml
{
	public class XmlDictionary : IXmlDictionary
	{
		public static IXmlDictionary Empty
		{
			get
			{
				if (XmlDictionary.empty == null)
				{
					XmlDictionary.empty = new XmlDictionary.EmptyDictionary();
				}
				return XmlDictionary.empty;
			}
		}

		public XmlDictionary()
		{
			this.lookup = new Dictionary<string, XmlDictionaryString>();
			this.strings = null;
			this.nextId = 0;
		}

		public XmlDictionary(int capacity)
		{
			this.lookup = new Dictionary<string, XmlDictionaryString>(capacity);
			this.strings = new XmlDictionaryString[capacity];
			this.nextId = 0;
		}

		public virtual XmlDictionaryString Add(string value)
		{
			XmlDictionaryString xmlDictionaryString;
			if (!this.lookup.TryGetValue(value, out xmlDictionaryString))
			{
				if (this.strings == null)
				{
					this.strings = new XmlDictionaryString[4];
				}
				else if (this.nextId == this.strings.Length)
				{
					int num = this.nextId * 2;
					if (num == 0)
					{
						num = 4;
					}
					Array.Resize<XmlDictionaryString>(ref this.strings, num);
				}
				xmlDictionaryString = new XmlDictionaryString(this, value, this.nextId);
				this.strings[this.nextId] = xmlDictionaryString;
				this.lookup.Add(value, xmlDictionaryString);
				this.nextId++;
			}
			return xmlDictionaryString;
		}

		public virtual bool TryLookup(string value, out XmlDictionaryString result)
		{
			return this.lookup.TryGetValue(value, out result);
		}

		public virtual bool TryLookup(int key, out XmlDictionaryString result)
		{
			if (key < 0 || key >= this.nextId)
			{
				result = null;
				return false;
			}
			result = this.strings[key];
			return true;
		}

		public virtual bool TryLookup(XmlDictionaryString value, out XmlDictionaryString result)
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

		private static IXmlDictionary empty;

		private Dictionary<string, XmlDictionaryString> lookup;

		private XmlDictionaryString[] strings;

		private int nextId;

		private class EmptyDictionary : IXmlDictionary
		{
			public bool TryLookup(string value, out XmlDictionaryString result)
			{
				result = null;
				return false;
			}

			public bool TryLookup(int key, out XmlDictionaryString result)
			{
				result = null;
				return false;
			}

			public bool TryLookup(XmlDictionaryString value, out XmlDictionaryString result)
			{
				result = null;
				return false;
			}
		}
	}
}
