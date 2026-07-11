using System;
using System.Runtime.InteropServices;

namespace System.Security.AccessControl
{
	public sealed class FileSecurity : FileSystemSecurity
	{
		public FileSecurity()
			: base(false)
		{
		}

		public FileSecurity(string fileName, AccessControlSections includeSections)
			: base(false, fileName, includeSections)
		{
		}

		internal FileSecurity(SafeHandle handle, AccessControlSections includeSections)
			: base(false, handle, includeSections)
		{
		}
	}
}
