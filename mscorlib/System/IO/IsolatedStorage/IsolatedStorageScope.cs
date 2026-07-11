using System;
using System.Runtime.InteropServices;

namespace System.IO.IsolatedStorage
{
	[ComVisible(true)]
	[Flags]
	[Serializable]
	public enum IsolatedStorageScope
	{
		None = 0,
		User = 1,
		Domain = 2,
		Assembly = 4,
		Roaming = 8,
		Machine = 16,
		Application = 32
	}
}
