using System;

namespace System.Net
{
	internal delegate PooledStream CreateConnectionDelegate(ConnectionPool pool);
}
