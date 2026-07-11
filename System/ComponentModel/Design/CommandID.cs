using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	public class CommandID
	{
		public CommandID(Guid menuGroup, int commandID)
		{
			this.menuGroup = menuGroup;
			this.commandID = commandID;
		}

		public virtual int ID
		{
			get
			{
				return this.commandID;
			}
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CommandID))
			{
				return false;
			}
			CommandID commandID = (CommandID)obj;
			return commandID.menuGroup.Equals(this.menuGroup) && commandID.commandID == this.commandID;
		}

		public override int GetHashCode()
		{
			return (this.menuGroup.GetHashCode() << 2) | this.commandID;
		}

		public virtual Guid Guid
		{
			get
			{
				return this.menuGroup;
			}
		}

		public override string ToString()
		{
			return this.menuGroup.ToString() + " : " + this.commandID.ToString(CultureInfo.CurrentCulture);
		}

		private readonly Guid menuGroup;

		private readonly int commandID;
	}
}
