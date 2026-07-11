using System;

namespace System.Security
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
	[MonoTODO("Only supported by the runtime when CoreCLR is enabled")]
	public sealed class SecurityCriticalAttribute : Attribute
	{
		public SecurityCriticalAttribute()
		{
			this._scope = SecurityCriticalScope.Explicit;
		}

		public SecurityCriticalAttribute(SecurityCriticalScope scope)
		{
			if (scope != SecurityCriticalScope.Everything)
			{
				this._scope = SecurityCriticalScope.Explicit;
			}
			else
			{
				this._scope = SecurityCriticalScope.Everything;
			}
		}

		public SecurityCriticalScope Scope
		{
			get
			{
				return this._scope;
			}
		}

		private SecurityCriticalScope _scope;
	}
}
