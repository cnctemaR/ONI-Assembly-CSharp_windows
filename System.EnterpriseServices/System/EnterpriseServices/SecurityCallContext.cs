using System;

namespace System.EnterpriseServices
{
	public sealed class SecurityCallContext
	{
		internal SecurityCallContext()
		{
		}

		internal SecurityCallContext(ISecurityCallContext context)
		{
		}

		public SecurityCallers Callers
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		public static SecurityCallContext CurrentCall
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		public SecurityIdentity DirectCaller
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsSecurityEnabled
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		public int MinAuthenticationLevel
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		public int NumCallers
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		public SecurityIdentity OriginalCaller
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		[MonoTODO]
		public bool IsCallerInRole(string role)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public bool IsUserInRole(string user, string role)
		{
			throw new NotImplementedException();
		}
	}
}
