using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[AttributeUsage(AttributeTargets.Assembly)]
	[ComVisible(false)]
	public sealed class ApplicationAccessControlAttribute : Attribute, IConfigurationAttribute
	{
		public ApplicationAccessControlAttribute()
		{
			this.val = false;
		}

		public ApplicationAccessControlAttribute(bool val)
		{
			this.val = val;
		}

		bool IConfigurationAttribute.AfterSaveChanges(Hashtable info)
		{
			return false;
		}

		[MonoTODO]
		bool IConfigurationAttribute.Apply(Hashtable cache)
		{
			throw new NotImplementedException();
		}

		bool IConfigurationAttribute.IsValidTarget(string s)
		{
			return s == "Application";
		}

		public AccessChecksLevelOption AccessChecksLevel
		{
			get
			{
				return this.accessChecksLevel;
			}
			set
			{
				this.accessChecksLevel = value;
			}
		}

		public AuthenticationOption Authentication
		{
			get
			{
				return this.authentication;
			}
			set
			{
				this.authentication = value;
			}
		}

		public ImpersonationLevelOption ImpersonationLevel
		{
			get
			{
				return this.impersonation;
			}
			set
			{
				this.impersonation = value;
			}
		}

		public bool Value
		{
			get
			{
				return this.val;
			}
			set
			{
				this.val = value;
			}
		}

		private AccessChecksLevelOption accessChecksLevel;

		private AuthenticationOption authentication;

		private ImpersonationLevelOption impersonation;

		private bool val;
	}
}
