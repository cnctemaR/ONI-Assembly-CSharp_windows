using System;
using System.Collections;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class PropertyCollection : Hashtable, ICloneable
	{
		public PropertyCollection()
		{
		}

		protected PropertyCollection(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public override object Clone()
		{
			PropertyCollection propertyCollection = new PropertyCollection();
			foreach (object obj in this)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				propertyCollection.Add(dictionaryEntry.Key, dictionaryEntry.Value);
			}
			return propertyCollection;
		}
	}
}
