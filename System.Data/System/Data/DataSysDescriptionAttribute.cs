using System;
using System.ComponentModel;

namespace System.Data
{
	[AttributeUsage(AttributeTargets.All)]
	[Obsolete("DataSysDescriptionAttribute has been deprecated")]
	public class DataSysDescriptionAttribute : DescriptionAttribute
	{
		[Obsolete("DataSysDescriptionAttribute has been deprecated")]
		public DataSysDescriptionAttribute(string description)
		{
		}

		public override string Description
		{
			get
			{
				throw null;
			}
		}
	}
}
