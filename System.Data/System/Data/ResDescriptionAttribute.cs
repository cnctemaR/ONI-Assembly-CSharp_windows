using System;
using System.ComponentModel;

namespace System.Data
{
	[AttributeUsage(AttributeTargets.All)]
	internal sealed class ResDescriptionAttribute : DescriptionAttribute
	{
		public ResDescriptionAttribute(string description)
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
