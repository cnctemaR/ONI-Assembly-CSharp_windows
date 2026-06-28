using System;
using System.Security;
using System.Security.Permissions;

namespace System.Drawing.Printing
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	public sealed class PrintingPermissionAttribute : CodeAccessSecurityAttribute
	{
		public PrintingPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public PrintingPermissionLevel Level
		{
			get
			{
				return this._level;
			}
			set
			{
				if (!Enum.IsDefined(typeof(PrintingPermissionLevel), value))
				{
					string text = Locale.GetText("Invalid enum {0}");
					throw new ArgumentException(string.Format(text, value), "Level");
				}
				this._level = value;
			}
		}

		public override IPermission CreatePermission()
		{
			if (base.Unrestricted)
			{
				return new PrintingPermission(PermissionState.Unrestricted);
			}
			return new PrintingPermission(this._level);
		}

		private PrintingPermissionLevel _level;
	}
}
