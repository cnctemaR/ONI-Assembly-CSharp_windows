using System;

namespace System.IO
{
	internal struct timespec
	{
		public IntPtr tv_sec;

		public IntPtr tv_nsec;
	}
}
