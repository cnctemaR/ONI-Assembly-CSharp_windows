using System;

namespace System.Xml
{
	public class XmlDictionaryString
	{
		public XmlDictionaryString(IXmlDictionary dictionary, string value, int key)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (key < 0 || key > 536870911)
			{
				throw new ArgumentOutOfRangeException("key");
			}
			this.dict = dictionary;
			this.value = value;
			this.key = key;
		}

		public static XmlDictionaryString Empty
		{
			get
			{
				return XmlDictionaryString.empty;
			}
		}

		public IXmlDictionary Dictionary
		{
			get
			{
				return this.dict;
			}
		}

		public int Key
		{
			get
			{
				return this.key;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		public override string ToString()
		{
			return this.value;
		}

		private static XmlDictionaryString empty = new XmlDictionaryString(XmlDictionary.EmptyDictionary.Instance, string.Empty, 0);

		private readonly IXmlDictionary dict;

		private readonly string value;

		private readonly int key;
	}
}
