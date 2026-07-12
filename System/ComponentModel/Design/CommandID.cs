using System;
using System.Globalization;

namespace System.ComponentModel.Design
{
	public class CommandID
	{
		public CommandID(Guid menuGroup, int commandID)
		{
			this.Guid = menuGroup;
			this.ID = commandID;
		}

		public virtual int ID { get; }

		public override bool Equals(object obj)
		{
			if (!(obj is CommandID))
			{
				return false;
			}
			CommandID commandID = (CommandID)obj;
			return commandID.Guid.Equals(this.Guid) && commandID.ID == this.ID;
		}

		public override int GetHashCode()
		{
			return (this.Guid.GetHashCode() << 2) | this.ID;
		}

		public virtual Guid Guid { get; }

		public override string ToString()
		{
			return this.Guid.ToString() + " : " + this.ID.ToString(CultureInfo.CurrentCulture);
		}
	}
}
