using System;
using System.Security.Permissions;
using Unity;

namespace System.Management.Instrumentation
{
	[AttributeUsage(AttributeTargets.Assembly)]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class WmiConfigurationAttribute : Attribute
	{
		public WmiConfigurationAttribute(string scope)
		{
		}

		public string HostingGroup
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

		public ManagementHostingModel HostingModel
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return ManagementHostingModel.Decoupled;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public bool IdentifyLevel
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public string NamespaceSecurity
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

		public string Scope
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public string SecurityRestriction
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
