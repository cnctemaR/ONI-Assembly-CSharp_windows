using System;
using System.Runtime.InteropServices;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public enum DriveType
	{
		CDRom = 5,
		Fixed = 3,
		Network,
		NoRootDirectory = 1,
		Ram = 6,
		Removable = 2,
		Unknown = 0
	}
}
