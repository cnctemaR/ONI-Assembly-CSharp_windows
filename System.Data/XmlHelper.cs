using System;
using System.Collections;
using System.Xml;

internal class XmlHelper
{
	internal static string Decode(string xmlName)
	{
		string text = (string)XmlHelper.localSchemaNameCache[xmlName];
		if (text == null)
		{
			text = XmlConvert.DecodeName(xmlName);
			XmlHelper.localSchemaNameCache[xmlName] = text;
		}
		return text;
	}

	internal static string Encode(string schemaName)
	{
		string text = (string)XmlHelper.localXmlNameCache[schemaName];
		if (text == null)
		{
			text = XmlConvert.EncodeLocalName(schemaName);
			XmlHelper.localXmlNameCache[schemaName] = text;
		}
		return text;
	}

	internal static void ClearCache()
	{
		XmlHelper.localSchemaNameCache.Clear();
		XmlHelper.localXmlNameCache.Clear();
	}

	private static Hashtable localSchemaNameCache = new Hashtable();

	private static Hashtable localXmlNameCache = new Hashtable();
}
