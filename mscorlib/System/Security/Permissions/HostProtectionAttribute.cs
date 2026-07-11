using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class HostProtectionAttribute : CodeAccessSecurityAttribute
	{
		public HostProtectionAttribute()
			: base(SecurityAction.LinkDemand)
		{
		}

		public HostProtectionAttribute(SecurityAction action)
			: base(action)
		{
			if (action != SecurityAction.LinkDemand)
			{
				throw new ArgumentException(string.Format(Locale.GetText("Only {0} is accepted."), SecurityAction.LinkDemand), "action");
			}
		}

		public bool ExternalProcessMgmt
		{
			get
			{
				return (this._resources & HostProtectionResource.ExternalProcessMgmt) > HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.ExternalProcessMgmt;
					return;
				}
				this._resources &= ~HostProtectionResource.ExternalProcessMgmt;
			}
		}

		public bool ExternalThreading
		{
			get
			{
				return (this._resources & HostProtectionResource.ExternalThreading) > HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.ExternalThreading;
					return;
				}
				this._resources &= ~HostProtectionResource.ExternalThreading;
			}
		}

		public bool MayLeakOnAbort
		{
			get
			{
				return (this._resources & HostProtectionResource.MayLeakOnAbort) > HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.MayLeakOnAbort;
					return;
				}
				this._resources &= ~HostProtectionResource.MayLeakOnAbort;
			}
		}

		[ComVisible(true)]
		public bool SecurityInfrastructure
		{
			get
			{
				return (this._resources & HostProtectionResource.SecurityInfrastructure) > HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.SecurityInfrastructure;
					return;
				}
				this._resources &= ~HostProtectionResource.SecurityInfrastructure;
			}
		}

		public bool SelfAffectingProcessMgmt
		{
			get
			{
				return (this._resources & HostProtectionResource.SelfAffectingProcessMgmt) > HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.SelfAffectingProcessMgmt;
					return;
				}
				this._resources &= ~HostProtectionResource.SelfAffectingProcessMgmt;
			}
		}

		public bool SelfAffectingThreading
		{
			get
			{
				return (this._resources & HostProtectionResource.SelfAffectingThreading) > HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.SelfAffectingThreading;
					return;
				}
				this._resources &= ~HostProtectionResource.SelfAffectingThreading;
			}
		}

		public bool SharedState
		{
			get
			{
				return (this._resources & HostProtectionResource.SharedState) > HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.SharedState;
					return;
				}
				this._resources &= ~HostProtectionResource.SharedState;
			}
		}

		public bool Synchronization
		{
			get
			{
				return (this._resources & HostProtectionResource.Synchronization) > HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.Synchronization;
					return;
				}
				this._resources &= ~HostProtectionResource.Synchronization;
			}
		}

		public bool UI
		{
			get
			{
				return (this._resources & HostProtectionResource.UI) > HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.UI;
					return;
				}
				this._resources &= ~HostProtectionResource.UI;
			}
		}

		public HostProtectionResource Resources
		{
			get
			{
				return this._resources;
			}
			set
			{
				this._resources = value;
			}
		}

		public override IPermission CreatePermission()
		{
			return new HostProtectionPermission(this._resources);
		}

		private HostProtectionResource _resources;
	}
}
