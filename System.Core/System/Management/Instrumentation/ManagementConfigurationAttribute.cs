using System;
using System.Security.Permissions;
using Unity;

namespace System.Management.Instrumentation
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class ManagementConfigurationAttribute : ManagementMemberAttribute
	{
		public ManagementConfigurationType Mode
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return ManagementConfigurationType.Apply;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public Type Schema
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}
	}
}
