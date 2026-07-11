using System;
using System.Xml;

namespace System.Data.SqlClient
{
	internal class XmlDataFeed : DataFeed
	{
		internal XmlDataFeed(XmlReader source)
		{
			this._source = source;
		}

		internal XmlReader _source;
	}
}
