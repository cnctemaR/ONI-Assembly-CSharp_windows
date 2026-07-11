using System;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	[Obsolete("Please use IEqualityComparer instead.")]
	public interface IHashCodeProvider
	{
		int GetHashCode(object obj);
	}
}
