using System;
using System.ComponentModel;

namespace System.Timers
{
	[AttributeUsage(AttributeTargets.All)]
	public class TimersDescriptionAttribute : global::System.ComponentModel.DescriptionAttribute
	{
		public TimersDescriptionAttribute(string description)
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
