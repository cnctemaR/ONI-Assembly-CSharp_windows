using System;
using System.Security;
using System.Security.Permissions;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public sealed class DispatchWrapper
	{
		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public DispatchWrapper(object obj)
		{
			if (obj != null)
			{
				Marshal.Release(Marshal.GetIDispatchForObject(obj));
			}
			this.m_WrappedObject = obj;
		}

		public object WrappedObject
		{
			get
			{
				return this.m_WrappedObject;
			}
		}

		private object m_WrappedObject;
	}
}
