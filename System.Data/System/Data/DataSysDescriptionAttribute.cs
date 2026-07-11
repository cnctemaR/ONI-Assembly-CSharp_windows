using System;
using System.ComponentModel;

namespace System.Data
{
	[AttributeUsage(AttributeTargets.All)]
	[Obsolete("DataSysDescriptionAttribute has been deprecated.  http://go.microsoft.com/fwlink/?linkid=14202", false)]
	public class DataSysDescriptionAttribute : DescriptionAttribute
	{
		[Obsolete("DataSysDescriptionAttribute has been deprecated.  http://go.microsoft.com/fwlink/?linkid=14202", false)]
		public DataSysDescriptionAttribute(string description)
			: base(description)
		{
		}

		public override string Description
		{
			get
			{
				if (!this._replaced)
				{
					this._replaced = true;
					base.DescriptionValue = base.Description;
				}
				return base.Description;
			}
		}

		private bool _replaced;
	}
}
