using System;
using System.Security.Permissions;
using System.Threading;

namespace System.ComponentModel
{
	public static class AsyncOperationManager
	{
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static SynchronizationContext SynchronizationContext
		{
			get
			{
				if (SynchronizationContext.Current == null)
				{
					SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
				}
				return SynchronizationContext.Current;
			}
			[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"NoFlags\"/>\n</PermissionSet>\n")]
			set
			{
				SynchronizationContext.SetSynchronizationContext(value);
			}
		}

		public static AsyncOperation CreateOperation(object userSuppliedState)
		{
			return new AsyncOperation(AsyncOperationManager.SynchronizationContext, userSuppliedState);
		}
	}
}
