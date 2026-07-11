using System;
using System.Collections.Generic;

namespace System.Xml
{
	public class XmlDictionary : IXmlDictionary
	{
		public XmlDictionary()
		{
			this.dict = new Dictionary<string, XmlDictionaryString>();
			this.list = new List<XmlDictionaryString>();
		}

		public XmlDictionary(int capacity)
		{
			this.dict = new Dictionary<string, XmlDictionaryString>(capacity);
			this.list = new List<XmlDictionaryString>(capacity);
		}

		private XmlDictionary(bool isReadOnly)
			: this(1)
		{
			this.is_readonly = isReadOnly;
		}

		public static IXmlDictionary Empty
		{
			get
			{
				return XmlDictionary.empty;
			}
		}

		public virtual XmlDictionaryString Add(string value)
		{
			if (this.is_readonly)
			{
				throw new InvalidOperationException();
			}
			XmlDictionaryString xmlDictionaryString;
			if (this.dict.TryGetValue(value, out xmlDictionaryString))
			{
				return xmlDictionaryString;
			}
			xmlDictionaryString = new XmlDictionaryString(this, value, this.dict.Count);
			this.dict.Add(value, xmlDictionaryString);
			this.list.Add(xmlDictionaryString);
			return xmlDictionaryString;
		}

		public virtual bool TryLookup(int key, out XmlDictionaryString result)
		{
			if (key < 0 || this.dict.Count <= key)
			{
				result = null;
				return false;
			}
			result = this.list[key];
			return true;
		}

		public virtual bool TryLookup(string value, out XmlDictionaryString result)
		{
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			return this.dict.TryGetValue(value, out result);
		}

		public virtual bool TryLookup(XmlDictionaryString value, out XmlDictionaryString result)
		{
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			if (value.Dictionary != this)
			{
				result = null;
				return false;
			}
			for (int i = 0; i < this.list.Count; i++)
			{
				if (object.ReferenceEquals(this.list[i], value))
				{
					result = value;
					return true;
				}
			}
			result = null;
			return false;
		}

		private static XmlDictionary empty = new XmlDictionary(true);

		private readonly bool is_readonly;

		private Dictionary<string, XmlDictionaryString> dict;

		private List<XmlDictionaryString> list;

		internal class EmptyDictionary : XmlDictionary
		{
			public EmptyDictionary()
				: base(1)
			{
			}

			public static readonly XmlDictionary.EmptyDictionary Instance = new XmlDictionary.EmptyDictionary();
		}
	}
}
