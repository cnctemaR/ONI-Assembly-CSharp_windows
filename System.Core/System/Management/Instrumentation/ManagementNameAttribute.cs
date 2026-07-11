using System;
using System.Security.Permissions;
using Unity;

namespace System.Management.Instrumentation
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class ManagementNameAttribute : Attribute
	{
		public ManagementNameAttribute(string name)
		{
		}

		public string Name
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}
	}
}
