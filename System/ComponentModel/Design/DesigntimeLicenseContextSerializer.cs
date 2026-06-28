using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.ComponentModel.Design
{
	public class DesigntimeLicenseContextSerializer
	{
		private DesigntimeLicenseContextSerializer()
		{
		}

		public static void Serialize(Stream o, string cryptoKey, DesigntimeLicenseContext context)
		{
			object[] array = new object[2];
			array[0] = cryptoKey;
			Hashtable hashtable = new Hashtable();
			foreach (object obj in context.keys)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				hashtable.Add(((Type)dictionaryEntry.Key).AssemblyQualifiedName, dictionaryEntry.Value);
			}
			array[1] = hashtable;
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(o, array);
		}
	}
}
