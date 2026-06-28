using System;

namespace System.Data.SqlTypes
{
	public interface INullable
	{
		bool IsNull { get; }
	}
}
