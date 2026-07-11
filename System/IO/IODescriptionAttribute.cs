using System;
using System.ComponentModel;

namespace System.IO
{
	[AttributeUsage(AttributeTargets.All)]
	public class IODescriptionAttribute : DescriptionAttribute
	{
		public IODescriptionAttribute(string description)
			: base(description)
		{
		}

		public override string Description
		{
			get
			{
				return base.DescriptionValue;
			}
		}
	}
}
