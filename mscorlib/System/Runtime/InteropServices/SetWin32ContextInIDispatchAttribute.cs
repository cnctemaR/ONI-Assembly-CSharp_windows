using System;

namespace System.Runtime.InteropServices
{
	[Obsolete("This attribute has been deprecated.  Application Domains no longer respect Activation Context boundaries in IDispatch calls.", false)]
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class SetWin32ContextInIDispatchAttribute : Attribute
	{
	}
}
