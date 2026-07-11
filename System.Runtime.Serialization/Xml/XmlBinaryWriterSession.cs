using System;
using System.Collections.Generic;

namespace System.Xml
{
	public class XmlBinaryWriterSession
	{
		public void Reset()
		{
			this.dic.Clear();
		}

		public virtual bool TryAdd(XmlDictionaryString value, out int key)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (this.TryLookup(value, out key))
			{
				throw new InvalidOperationException("Argument XmlDictionaryString was already added to the writer session");
			}
			key = this.dic.Count;
			this.dic.Add(key, value);
			return true;
		}

		internal bool TryLookup(XmlDictionaryString value, out int key)
		{
			foreach (KeyValuePair<int, XmlDictionaryString> keyValuePair in this.dic)
			{
				if (keyValuePair.Value.Value == value.Value)
				{
					key = keyValuePair.Key;
					return true;
				}
			}
			key = -1;
			return false;
		}

		private Dictionary<int, XmlDictionaryString> dic = new Dictionary<int, XmlDictionaryString>();
	}
}
