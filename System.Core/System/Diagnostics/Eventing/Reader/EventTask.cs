using System;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class EventTask
	{
		internal EventTask()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public string DisplayName
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public Guid EventGuid
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return default(Guid);
			}
		}

		public string Name
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public int Value
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}
	}
}
