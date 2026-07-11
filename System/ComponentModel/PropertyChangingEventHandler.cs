using System;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public delegate void PropertyChangingEventHandler(object sender, PropertyChangingEventArgs e);
}
