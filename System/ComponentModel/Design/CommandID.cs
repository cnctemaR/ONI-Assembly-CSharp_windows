using System;
using System.Runtime.InteropServices;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	public class CommandID
	{
		public CommandID(Guid menuGroup, int commandID)
		{
			this.cID = commandID;
			this.guid = menuGroup;
		}

		public virtual Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		public virtual int ID
		{
			get
			{
				return this.cID;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is CommandID && (obj == this || (((CommandID)obj).Guid.Equals(this.guid) && ((CommandID)obj).ID.Equals(this.cID)));
		}

		public override int GetHashCode()
		{
			return this.guid.GetHashCode() ^ this.cID.GetHashCode();
		}

		public override string ToString()
		{
			return this.guid.ToString() + " : " + this.cID.ToString();
		}

		private int cID;

		private Guid guid;
	}
}
