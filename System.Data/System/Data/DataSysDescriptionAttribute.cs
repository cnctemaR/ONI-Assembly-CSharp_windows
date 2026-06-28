using System;
using System.ComponentModel;

namespace System.Data
{
	[Obsolete("DataSysDescriptionAttribute has been deprecated")]
	[AttributeUsage(AttributeTargets.All)]
	public class DataSysDescriptionAttribute : DescriptionAttribute
	{
		[Obsolete("DataSysDescriptionAttribute has been deprecated")]
		public DataSysDescriptionAttribute(string description)
			: base(description)
		{
			this.description = description;
		}

		public override string Description
		{
			get
			{
				return this.description;
			}
		}

		private string description;
	}
}
