using System;
using System.Globalization;
using System.Security;
using System.Security.Permissions;

namespace System.Drawing.Printing
{
	[Serializable]
	public sealed class PrintingPermission : CodeAccessPermission, IUnrestrictedPermission
	{
		public PrintingPermission(PermissionState state)
		{
			if (PrintingPermission.CheckPermissionState(state, true) == PermissionState.Unrestricted)
			{
				this._Level = PrintingPermissionLevel.AllPrinting;
			}
		}

		public PrintingPermission(PrintingPermissionLevel printingLevel)
		{
			this.Level = printingLevel;
		}

		public PrintingPermissionLevel Level
		{
			get
			{
				return this._Level;
			}
			set
			{
				if (!Enum.IsDefined(typeof(PrintingPermissionLevel), value))
				{
					string text = Locale.GetText("Invalid enum {0}");
					throw new ArgumentException(string.Format(text, value), "Level");
				}
				this._Level = value;
			}
		}

		public override IPermission Copy()
		{
			return new PrintingPermission(this.Level);
		}

		public override void FromXml(SecurityElement esd)
		{
			PrintingPermission.CheckSecurityElement(esd, "esd", 1, 1);
			if (PrintingPermission.IsUnrestricted(esd))
			{
				this._Level = PrintingPermissionLevel.AllPrinting;
			}
			else
			{
				string text = esd.Attribute("Level");
				if (text != null)
				{
					this._Level = (PrintingPermissionLevel)((int)Enum.Parse(typeof(PrintingPermissionLevel), text));
				}
				else
				{
					this._Level = PrintingPermissionLevel.NoPrinting;
				}
			}
		}

		public override IPermission Intersect(IPermission target)
		{
			PrintingPermission printingPermission = this.Cast(target);
			if (printingPermission == null || this.IsEmpty() || printingPermission.IsEmpty())
			{
				return null;
			}
			PrintingPermissionLevel printingPermissionLevel = ((this._Level > printingPermission.Level) ? printingPermission.Level : this._Level);
			return new PrintingPermission(printingPermissionLevel);
		}

		public override bool IsSubsetOf(IPermission target)
		{
			PrintingPermission printingPermission = this.Cast(target);
			if (printingPermission == null)
			{
				return this.IsEmpty();
			}
			return this._Level <= printingPermission.Level;
		}

		public bool IsUnrestricted()
		{
			return this._Level == PrintingPermissionLevel.AllPrinting;
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = this.Element(1);
			if (this.IsUnrestricted())
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			else
			{
				securityElement.AddAttribute("Level", this._Level.ToString());
			}
			return securityElement;
		}

		public override IPermission Union(IPermission target)
		{
			PrintingPermission printingPermission = this.Cast(target);
			if (printingPermission == null)
			{
				return new PrintingPermission(this._Level);
			}
			if (this.IsUnrestricted() || printingPermission.IsUnrestricted())
			{
				return new PrintingPermission(PermissionState.Unrestricted);
			}
			if (this.IsEmpty() && printingPermission.IsEmpty())
			{
				return null;
			}
			PrintingPermissionLevel printingPermissionLevel = ((this._Level <= printingPermission.Level) ? printingPermission.Level : this._Level);
			return new PrintingPermission(printingPermissionLevel);
		}

		private bool IsEmpty()
		{
			return this._Level == PrintingPermissionLevel.NoPrinting;
		}

		private PrintingPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			PrintingPermission printingPermission = target as PrintingPermission;
			if (printingPermission == null)
			{
				PrintingPermission.ThrowInvalidPermission(target, typeof(PrintingPermission));
			}
			return printingPermission;
		}

		internal SecurityElement Element(int version)
		{
			SecurityElement securityElement = new SecurityElement("IPermission");
			Type type = base.GetType();
			securityElement.AddAttribute("class", type.FullName + ", " + type.Assembly.ToString().Replace('"', '\''));
			securityElement.AddAttribute("version", version.ToString());
			return securityElement;
		}

		internal static PermissionState CheckPermissionState(PermissionState state, bool allowUnrestricted)
		{
			if (state != PermissionState.None)
			{
				if (state != PermissionState.Unrestricted)
				{
					string text = string.Format(Locale.GetText("Invalid enum {0}"), state);
					throw new ArgumentException(text, "state");
				}
				if (!allowUnrestricted)
				{
					string text = Locale.GetText("Unrestricted isn't not allowed for identity permissions.");
					throw new ArgumentException(text, "state");
				}
			}
			return state;
		}

		internal static int CheckSecurityElement(SecurityElement se, string parameterName, int minimumVersion, int maximumVersion)
		{
			if (se == null)
			{
				throw new ArgumentNullException(parameterName);
			}
			if (se.Attribute("class") == null)
			{
				string text = Locale.GetText("Missing 'class' attribute.");
				throw new ArgumentException(text, parameterName);
			}
			int num = minimumVersion;
			string text2 = se.Attribute("version");
			if (text2 != null)
			{
				try
				{
					num = int.Parse(text2);
				}
				catch (Exception ex)
				{
					string text3 = Locale.GetText("Couldn't parse version from '{0}'.");
					text3 = string.Format(text3, text2);
					throw new ArgumentException(text3, parameterName, ex);
				}
			}
			if (num < minimumVersion || num > maximumVersion)
			{
				string text4 = Locale.GetText("Unknown version '{0}', expected versions between ['{1}','{2}'].");
				text4 = string.Format(text4, num, minimumVersion, maximumVersion);
				throw new ArgumentException(text4, parameterName);
			}
			return num;
		}

		internal static bool IsUnrestricted(SecurityElement se)
		{
			string text = se.Attribute("Unrestricted");
			return text != null && string.Compare(text, bool.TrueString, true, CultureInfo.InvariantCulture) == 0;
		}

		internal static void ThrowInvalidPermission(IPermission target, Type expected)
		{
			string text = Locale.GetText("Invalid permission type '{0}', expected type '{1}'.");
			text = string.Format(text, target.GetType(), expected);
			throw new ArgumentException(text, "target");
		}

		private const int version = 1;

		private PrintingPermissionLevel _Level;
	}
}
