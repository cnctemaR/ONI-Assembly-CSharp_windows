using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security.Policy
{
	[ComVisible(true)]
	public sealed class ApplicationSecurityInfo
	{
		public ApplicationSecurityInfo(ActivationContext activationContext)
		{
			if (activationContext == null)
			{
				throw new ArgumentNullException("activationContext");
			}
		}

		public Evidence ApplicationEvidence
		{
			get
			{
				return this._evidence;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("ApplicationEvidence");
				}
				this._evidence = value;
			}
		}

		public ApplicationId ApplicationId
		{
			get
			{
				return this._appid;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("ApplicationId");
				}
				this._appid = value;
			}
		}

		public PermissionSet DefaultRequestSet
		{
			get
			{
				if (this._defaultSet == null)
				{
					return new PermissionSet(PermissionState.None);
				}
				return this._defaultSet;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("DefaultRequestSet");
				}
				this._defaultSet = value;
			}
		}

		public ApplicationId DeploymentId
		{
			get
			{
				return this._deployid;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("DeploymentId");
				}
				this._deployid = value;
			}
		}

		private Evidence _evidence;

		private ApplicationId _appid;

		private PermissionSet _defaultSet;

		private ApplicationId _deployid;
	}
}
