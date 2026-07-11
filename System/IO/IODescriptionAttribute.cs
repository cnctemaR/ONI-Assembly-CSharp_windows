using System;
using System.ComponentModel;

namespace System.IO
{
	[AttributeUsage(AttributeTargets.All)]
	public class IODescriptionAttribute : global::System.ComponentModel.DescriptionAttribute
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
