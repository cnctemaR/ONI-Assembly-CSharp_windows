using System;
using System.IO;

namespace System.Data.SqlClient
{
	internal class StreamDataFeed : DataFeed
	{
		internal StreamDataFeed(Stream source)
		{
			this._source = source;
		}

		internal Stream _source;
	}
}
