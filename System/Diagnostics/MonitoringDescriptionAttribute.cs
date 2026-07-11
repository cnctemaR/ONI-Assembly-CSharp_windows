using System;
using System.ComponentModel;

namespace System.Diagnostics
{
	[AttributeUsage(AttributeTargets.All)]
	public class MonitoringDescriptionAttribute : global::System.ComponentModel.DescriptionAttribute
	{
		public MonitoringDescriptionAttribute(string description)
			: base(description)
		{
		}

		public override string Description
		{
			get
			{
				return base.Description;
			}
		}
	}
}
