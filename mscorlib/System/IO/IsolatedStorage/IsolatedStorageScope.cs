using System;

namespace System.IO.IsolatedStorage
{
	[Flags]
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
