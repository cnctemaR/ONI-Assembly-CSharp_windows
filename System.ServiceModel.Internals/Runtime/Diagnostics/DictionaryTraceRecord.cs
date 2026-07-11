using System;
using System.Collections;
using System.Xml;

namespace System.Runtime.Diagnostics
{
	internal class DictionaryTraceRecord : TraceRecord
	{
		internal DictionaryTraceRecord(IDictionary dictionary)
		{
			this.dictionary = dictionary;
		}

		internal override string EventId
		{
			get
			{
				return "http://schemas.microsoft.com/2006/08/ServiceModel/DictionaryTraceRecord";
			}
		}

		internal override void WriteTo(XmlWriter xml)
		{
			if (this.dictionary != null)
			{
				foreach (object obj in this.dictionary.Keys)
				{
					object obj2 = this.dictionary[obj];
					xml.WriteElementString(obj.ToString(), (obj2 == null) ? string.Empty : obj2.ToString());
				}
			}
		}

		private IDictionary dictionary;
	}
}
