using System;
using System.IO;

namespace System.Data.SqlClient
{
	internal class TextDataFeed : DataFeed
	{
		internal TextDataFeed(TextReader source)
		{
			this._source = source;
		}

		internal TextReader _source;
	}
}
