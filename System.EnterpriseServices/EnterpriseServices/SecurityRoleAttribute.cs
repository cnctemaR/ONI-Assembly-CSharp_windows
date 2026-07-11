using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[ComVisible(false)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true)]
	public sealed class SecurityRoleAttribute : Attribute
	{
		public SecurityRoleAttribute(string role)
			: this(role, false)
		{
		}

		public SecurityRoleAttribute(string role, bool everyone)
		{
			this.description = string.Empty;
			this.everyone = everyone;
			this.role = role;
		}

		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		public string Role
		{
			get
			{
				return this.role;
			}
			set
			{
				this.role = value;
			}
		}

		public bool SetEveryoneAccess
		{
			get
			{
				return this.everyone;
			}
			set
			{
				this.everyone = value;
			}
		}

		private string description;

		private bool everyone;

		private string role;
	}
}
