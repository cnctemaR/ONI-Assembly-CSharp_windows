using System;

namespace System.Security
{
	public abstract class SecurityState
	{
		public abstract void EnsureState();

		public bool IsStateAvailable()
		{
			AppDomainManager domainManager = AppDomain.CurrentDomain.DomainManager;
			return domainManager != null && domainManager.CheckSecuritySettings(this);
		}
	}
}
