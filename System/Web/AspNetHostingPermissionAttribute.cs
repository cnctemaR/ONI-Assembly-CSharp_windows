using System;
using System.Security;
using System.Security.Permissions;

namespace System.Web
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class AspNetHostingPermissionAttribute : CodeAccessSecurityAttribute
	{
		public AspNetHostingPermissionAttribute(SecurityAction action)
			: base(action)
		{
			this._level = AspNetHostingPermissionLevel.None;
		}

		public override IPermission CreatePermission()
		{
			if (base.Unrestricted)
			{
				return new AspNetHostingPermission(PermissionState.Unrestricted);
			}
			return new AspNetHostingPermission(this._level);
		}

		public AspNetHostingPermissionLevel Level
		{
			get
			{
				return this._level;
			}
			set
			{
				if (value < AspNetHostingPermissionLevel.None || value > AspNetHostingPermissionLevel.Unrestricted)
				{
					string text = global::Locale.GetText("Invalid enum {0}.");
					throw new ArgumentException(string.Format(text, value), "Level");
				}
				this._level = value;
			}
		}

		private AspNetHostingPermissionLevel _level;
	}
}
