using System;
using System.Runtime.InteropServices;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public enum SeekOrigin
	{
		Begin,
		Current,
		End
	}
}
