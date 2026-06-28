using System;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	public interface IEqualityComparer
	{
		bool Equals(object x, object y);

		int GetHashCode(object obj);
	}
}
