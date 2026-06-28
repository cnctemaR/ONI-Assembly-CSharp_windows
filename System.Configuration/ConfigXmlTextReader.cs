using System;
using System.Configuration.Internal;
using System.IO;
using System.Xml;

internal class ConfigXmlTextReader : XmlTextReader, IConfigErrorInfo
{
	public ConfigXmlTextReader(Stream s, string fileName)
		: base(s)
	{
		if (fileName == null)
		{
			throw new ArgumentNullException("fileName");
		}
		this.fileName = fileName;
	}

	public ConfigXmlTextReader(TextReader input, string fileName)
		: base(input)
	{
		if (fileName == null)
		{
			throw new ArgumentNullException("fileName");
		}
		this.fileName = fileName;
	}

	public string Filename
	{
		get
		{
			return this.fileName;
		}
	}

	private readonly string fileName;
}
