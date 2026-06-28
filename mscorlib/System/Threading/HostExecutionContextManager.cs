using System;
using System.Runtime.ConstrainedExecution;
using System.Security.Permissions;

namespace System.Threading
{
	public class HostExecutionContextManager
	{
		[MonoTODO]
		public virtual HostExecutionContext Capture()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public virtual void Revert(object previousState)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"Infrastructure\"/>\n</PermissionSet>\n")]
		public virtual object SetHostExecutionContext(HostExecutionContext hostExecutionContext)
		{
			throw new NotImplementedException();
		}
	}
}
