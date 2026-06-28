using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
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
				string text = string.Format(Locale.GetText("Only {0} is accepted."), SecurityAction.LinkDemand);
				throw new ArgumentException(text, "action");
			}
		}

		public bool ExternalProcessMgmt
		{
			get
			{
				return (this._resources & HostProtectionResource.ExternalProcessMgmt) != HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.ExternalProcessMgmt;
				}
				else
				{
					this._resources &= ~HostProtectionResource.ExternalProcessMgmt;
				}
			}
		}

		public bool ExternalThreading
		{
			get
			{
				return (this._resources & HostProtectionResource.ExternalThreading) != HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.ExternalThreading;
				}
				else
				{
					this._resources &= ~HostProtectionResource.ExternalThreading;
				}
			}
		}

		public bool MayLeakOnAbort
		{
			get
			{
				return (this._resources & HostProtectionResource.MayLeakOnAbort) != HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.MayLeakOnAbort;
				}
				else
				{
					this._resources &= ~HostProtectionResource.MayLeakOnAbort;
				}
			}
		}

		[ComVisible(true)]
		public bool SecurityInfrastructure
		{
			get
			{
				return (this._resources & HostProtectionResource.SecurityInfrastructure) != HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.SecurityInfrastructure;
				}
				else
				{
					this._resources &= ~HostProtectionResource.SecurityInfrastructure;
				}
			}
		}

		public bool SelfAffectingProcessMgmt
		{
			get
			{
				return (this._resources & HostProtectionResource.SelfAffectingProcessMgmt) != HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.SelfAffectingProcessMgmt;
				}
				else
				{
					this._resources &= ~HostProtectionResource.SelfAffectingProcessMgmt;
				}
			}
		}

		public bool SelfAffectingThreading
		{
			get
			{
				return (this._resources & HostProtectionResource.SelfAffectingThreading) != HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.SelfAffectingThreading;
				}
				else
				{
					this._resources &= ~HostProtectionResource.SelfAffectingThreading;
				}
			}
		}

		public bool SharedState
		{
			get
			{
				return (this._resources & HostProtectionResource.SharedState) != HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.SharedState;
				}
				else
				{
					this._resources &= ~HostProtectionResource.SharedState;
				}
			}
		}

		public bool Synchronization
		{
			get
			{
				return (this._resources & HostProtectionResource.Synchronization) != HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.Synchronization;
				}
				else
				{
					this._resources &= ~HostProtectionResource.Synchronization;
				}
			}
		}

		public bool UI
		{
			get
			{
				return (this._resources & HostProtectionResource.UI) != HostProtectionResource.None;
			}
			set
			{
				if (value)
				{
					this._resources |= HostProtectionResource.UI;
				}
				else
				{
					this._resources &= ~HostProtectionResource.UI;
				}
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
