using System;
using System.Security.Permissions;

namespace System.Reflection.Emit
{
	internal struct RefEmitPermissionSet
	{
		public RefEmitPermissionSet(SecurityAction action, string pset)
		{
			this.action = action;
			this.pset = pset;
		}

		public SecurityAction action;

		public string pset;
	}
}
