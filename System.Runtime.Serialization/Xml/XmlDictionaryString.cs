using System;
using System.Runtime.Serialization;
using System.Text;

namespace System.Xml
{
	public class XmlDictionaryString
	{
		public XmlDictionaryString(IXmlDictionary dictionary, string value, int key)
		{
			if (dictionary == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("dictionary"));
			}
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
			}
			if (key < 0 || key > 536870911)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("key", global::System.Runtime.Serialization.SR.GetString("The value of this argument must fall within the range {0} to {1}.", new object[] { 0, 536870911 })));
			}
			this.dictionary = dictionary;
			this.value = value;
			this.key = key;
		}

		internal static string GetString(XmlDictionaryString s)
		{
			if (s == null)
			{
				return null;
			}
			return s.Value;
		}

		public static XmlDictionaryString Empty
		{
			get
			{
				return XmlDictionaryString.emptyStringDictionary.EmptyString;
			}
		}

		public IXmlDictionary Dictionary
		{
			get
			{
				return this.dictionary;
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

		internal byte[] ToUTF8()
		{
			if (this.buffer == null)
			{
				this.buffer = Encoding.UTF8.GetBytes(this.value);
			}
			return this.buffer;
		}

		public override string ToString()
		{
			return this.value;
		}

		internal const int MinKey = 0;

		internal const int MaxKey = 536870911;

		private IXmlDictionary dictionary;

		private string value;

		private int key;

		private byte[] buffer;

		private static XmlDictionaryString.EmptyStringDictionary emptyStringDictionary = new XmlDictionaryString.EmptyStringDictionary();

		private class EmptyStringDictionary : IXmlDictionary
		{
			public EmptyStringDictionary()
			{
				this.empty = new XmlDictionaryString(this, string.Empty, 0);
			}

			public XmlDictionaryString EmptyString
			{
				get
				{
					return this.empty;
				}
			}

			public bool TryLookup(string value, out XmlDictionaryString result)
			{
				if (value == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
				}
				if (value.Length == 0)
				{
					result = this.empty;
					return true;
				}
				result = null;
				return false;
			}

			public bool TryLookup(int key, out XmlDictionaryString result)
			{
				if (key == 0)
				{
					result = this.empty;
					return true;
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

			private XmlDictionaryString empty;
		}
	}
}
