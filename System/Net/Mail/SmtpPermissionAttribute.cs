using System;
using System.Security;
using System.Security.Permissions;

namespace System.Net.Mail
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class SmtpPermissionAttribute : CodeAccessSecurityAttribute
	{
		public SmtpPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public string Access
		{
			get
			{
				return this.access;
			}
			set
			{
				this.access = value;
			}
		}

		private SmtpAccess GetSmtpAccess()
		{
			if (this.access == null)
			{
				return SmtpAccess.None;
			}
			string text = this.access.ToLowerInvariant();
			if (text == "connecttounrestrictedport")
			{
				return SmtpAccess.ConnectToUnrestrictedPort;
			}
			if (text == "connect")
			{
				return SmtpAccess.Connect;
			}
			if (!(text == "none"))
			{
				string text2 = global::Locale.GetText("Invalid Access='{0}' value.", new object[] { this.access });
				throw new ArgumentException("Access", text2);
			}
			return SmtpAccess.None;
		}

		public override IPermission CreatePermission()
		{
			if (base.Unrestricted)
			{
				return new SmtpPermission(true);
			}
			return new SmtpPermission(this.GetSmtpAccess());
		}

		private string access;
	}
}
