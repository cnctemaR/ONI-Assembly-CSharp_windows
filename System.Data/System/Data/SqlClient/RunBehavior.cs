using System;

namespace System.Data.SqlClient
{
	internal enum RunBehavior
	{
		UntilDone = 1,
		ReturnImmediately,
		Clean = 5,
		Attention = 13
	}
}
