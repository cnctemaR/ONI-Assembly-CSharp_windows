using System;

namespace System.Security.AccessControl
{
	public sealed class DirectorySecurity : FileSystemSecurity
	{
		public DirectorySecurity()
			: base(true)
		{
		}

		public DirectorySecurity(string name, AccessControlSections includeSections)
			: base(true, name, includeSections)
		{
		}
	}
}
