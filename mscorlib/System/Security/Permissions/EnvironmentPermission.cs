using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[Serializable]
	public sealed class EnvironmentPermission : CodeAccessPermission, IBuiltInPermission, IUnrestrictedPermission
	{
		public EnvironmentPermission(PermissionState state)
		{
			this._state = CodeAccessPermission.CheckPermissionState(state, true);
			this.readList = new ArrayList();
			this.writeList = new ArrayList();
		}

		public EnvironmentPermission(EnvironmentPermissionAccess flag, string pathList)
		{
			this.readList = new ArrayList();
			this.writeList = new ArrayList();
			this.SetPathList(flag, pathList);
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 0;
		}

		public void AddPathList(EnvironmentPermissionAccess flag, string pathList)
		{
			if (pathList == null)
			{
				throw new ArgumentNullException("pathList");
			}
			switch (flag)
			{
			case EnvironmentPermissionAccess.NoAccess:
				break;
			case EnvironmentPermissionAccess.Read:
			{
				string[] array = pathList.Split(new char[] { ';' });
				foreach (string text in array)
				{
					if (!this.readList.Contains(text))
					{
						this.readList.Add(text);
					}
				}
				break;
			}
			case EnvironmentPermissionAccess.Write:
			{
				string[] array = pathList.Split(new char[] { ';' });
				foreach (string text2 in array)
				{
					if (!this.writeList.Contains(text2))
					{
						this.writeList.Add(text2);
					}
				}
				break;
			}
			case EnvironmentPermissionAccess.AllAccess:
			{
				string[] array = pathList.Split(new char[] { ';' });
				foreach (string text3 in array)
				{
					if (!this.readList.Contains(text3))
					{
						this.readList.Add(text3);
					}
					if (!this.writeList.Contains(text3))
					{
						this.writeList.Add(text3);
					}
				}
				break;
			}
			default:
				this.ThrowInvalidFlag(flag, false);
				break;
			}
		}

		public override IPermission Copy()
		{
			EnvironmentPermission environmentPermission = new EnvironmentPermission(this._state);
			string text = this.GetPathList(EnvironmentPermissionAccess.Read);
			if (text != null)
			{
				environmentPermission.SetPathList(EnvironmentPermissionAccess.Read, text);
			}
			text = this.GetPathList(EnvironmentPermissionAccess.Write);
			if (text != null)
			{
				environmentPermission.SetPathList(EnvironmentPermissionAccess.Write, text);
			}
			return environmentPermission;
		}

		public override void FromXml(SecurityElement esd)
		{
			CodeAccessPermission.CheckSecurityElement(esd, "esd", 1, 1);
			if (CodeAccessPermission.IsUnrestricted(esd))
			{
				this._state = PermissionState.Unrestricted;
			}
			string text = esd.Attribute("Read");
			if (text != null && text.Length > 0)
			{
				this.SetPathList(EnvironmentPermissionAccess.Read, text);
			}
			string text2 = esd.Attribute("Write");
			if (text2 != null && text2.Length > 0)
			{
				this.SetPathList(EnvironmentPermissionAccess.Write, text2);
			}
		}

		public string GetPathList(EnvironmentPermissionAccess flag)
		{
			switch (flag)
			{
			case EnvironmentPermissionAccess.NoAccess:
			case EnvironmentPermissionAccess.AllAccess:
				this.ThrowInvalidFlag(flag, true);
				break;
			case EnvironmentPermissionAccess.Read:
				return this.GetPathList(this.readList);
			case EnvironmentPermissionAccess.Write:
				return this.GetPathList(this.writeList);
			default:
				this.ThrowInvalidFlag(flag, false);
				break;
			}
			return null;
		}

		public override IPermission Intersect(IPermission target)
		{
			EnvironmentPermission environmentPermission = this.Cast(target);
			if (environmentPermission == null)
			{
				return null;
			}
			if (this.IsUnrestricted())
			{
				return environmentPermission.Copy();
			}
			if (environmentPermission.IsUnrestricted())
			{
				return this.Copy();
			}
			int num = 0;
			EnvironmentPermission environmentPermission2 = new EnvironmentPermission(PermissionState.None);
			string pathList = environmentPermission.GetPathList(EnvironmentPermissionAccess.Read);
			if (pathList != null)
			{
				string[] array = pathList.Split(new char[] { ';' });
				foreach (string text in array)
				{
					if (this.readList.Contains(text))
					{
						environmentPermission2.AddPathList(EnvironmentPermissionAccess.Read, text);
						num++;
					}
				}
			}
			string pathList2 = environmentPermission.GetPathList(EnvironmentPermissionAccess.Write);
			if (pathList2 != null)
			{
				string[] array3 = pathList2.Split(new char[] { ';' });
				foreach (string text2 in array3)
				{
					if (this.writeList.Contains(text2))
					{
						environmentPermission2.AddPathList(EnvironmentPermissionAccess.Write, text2);
						num++;
					}
				}
			}
			return (num <= 0) ? null : environmentPermission2;
		}

		public override bool IsSubsetOf(IPermission target)
		{
			EnvironmentPermission environmentPermission = this.Cast(target);
			if (environmentPermission == null)
			{
				return false;
			}
			if (this.IsUnrestricted())
			{
				return environmentPermission.IsUnrestricted();
			}
			if (environmentPermission.IsUnrestricted())
			{
				return true;
			}
			foreach (object obj in this.readList)
			{
				string text = (string)obj;
				if (!environmentPermission.readList.Contains(text))
				{
					return false;
				}
			}
			foreach (object obj2 in this.writeList)
			{
				string text2 = (string)obj2;
				if (!environmentPermission.writeList.Contains(text2))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsUnrestricted()
		{
			return this._state == PermissionState.Unrestricted;
		}

		public void SetPathList(EnvironmentPermissionAccess flag, string pathList)
		{
			if (pathList == null)
			{
				throw new ArgumentNullException("pathList");
			}
			switch (flag)
			{
			case EnvironmentPermissionAccess.NoAccess:
				break;
			case EnvironmentPermissionAccess.Read:
			{
				this.readList.Clear();
				string[] array = pathList.Split(new char[] { ';' });
				foreach (string text in array)
				{
					this.readList.Add(text);
				}
				break;
			}
			case EnvironmentPermissionAccess.Write:
			{
				this.writeList.Clear();
				string[] array = pathList.Split(new char[] { ';' });
				foreach (string text2 in array)
				{
					this.writeList.Add(text2);
				}
				break;
			}
			case EnvironmentPermissionAccess.AllAccess:
			{
				this.readList.Clear();
				this.writeList.Clear();
				string[] array = pathList.Split(new char[] { ';' });
				foreach (string text3 in array)
				{
					this.readList.Add(text3);
					this.writeList.Add(text3);
				}
				break;
			}
			default:
				this.ThrowInvalidFlag(flag, false);
				break;
			}
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = base.Element(1);
			if (this._state == PermissionState.Unrestricted)
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			else
			{
				string text = this.GetPathList(EnvironmentPermissionAccess.Read);
				if (text != null)
				{
					securityElement.AddAttribute("Read", text);
				}
				text = this.GetPathList(EnvironmentPermissionAccess.Write);
				if (text != null)
				{
					securityElement.AddAttribute("Write", text);
				}
			}
			return securityElement;
		}

		public override IPermission Union(IPermission other)
		{
			EnvironmentPermission environmentPermission = this.Cast(other);
			if (environmentPermission == null)
			{
				return this.Copy();
			}
			if (this.IsUnrestricted() || environmentPermission.IsUnrestricted())
			{
				return new EnvironmentPermission(PermissionState.Unrestricted);
			}
			if (this.IsEmpty() && environmentPermission.IsEmpty())
			{
				return null;
			}
			EnvironmentPermission environmentPermission2 = (EnvironmentPermission)this.Copy();
			string text = environmentPermission.GetPathList(EnvironmentPermissionAccess.Read);
			if (text != null)
			{
				environmentPermission2.AddPathList(EnvironmentPermissionAccess.Read, text);
			}
			text = environmentPermission.GetPathList(EnvironmentPermissionAccess.Write);
			if (text != null)
			{
				environmentPermission2.AddPathList(EnvironmentPermissionAccess.Write, text);
			}
			return environmentPermission2;
		}

		private bool IsEmpty()
		{
			return this._state == PermissionState.None && this.readList.Count == 0 && this.writeList.Count == 0;
		}

		private EnvironmentPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			EnvironmentPermission environmentPermission = target as EnvironmentPermission;
			if (environmentPermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(EnvironmentPermission));
			}
			return environmentPermission;
		}

		internal void ThrowInvalidFlag(EnvironmentPermissionAccess flag, bool context)
		{
			string text;
			if (context)
			{
				text = Locale.GetText("Unknown flag '{0}'.");
			}
			else
			{
				text = Locale.GetText("Invalid flag '{0}' in this context.");
			}
			throw new ArgumentException(string.Format(text, flag), "flag");
		}

		private string GetPathList(ArrayList list)
		{
			if (this.IsUnrestricted())
			{
				return string.Empty;
			}
			if (list.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in list)
			{
				string text = (string)obj;
				stringBuilder.Append(text);
				stringBuilder.Append(";");
			}
			string text2 = stringBuilder.ToString();
			int length = text2.Length;
			if (length > 0)
			{
				return text2.Substring(0, length - 1);
			}
			return string.Empty;
		}

		private const int version = 1;

		private PermissionState _state;

		private ArrayList readList;

		private ArrayList writeList;
	}
}
