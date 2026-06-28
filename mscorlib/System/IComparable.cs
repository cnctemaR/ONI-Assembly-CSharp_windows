using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	public interface IComparable
	{
		int CompareTo(object obj);
	}
}
