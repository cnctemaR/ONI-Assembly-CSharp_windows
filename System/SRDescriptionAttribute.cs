using System;
using System.ComponentModel;

namespace System
{
	[AttributeUsage(AttributeTargets.All)]
	internal class SRDescriptionAttribute : DescriptionAttribute
	{
		public SRDescriptionAttribute(string description)
			: base(description)
		{
		}

		public override string Description
		{
			get
			{
				if (!this.isReplaced)
				{
					this.isReplaced = true;
					base.DescriptionValue = global::Locale.GetText(base.DescriptionValue);
				}
				return base.DescriptionValue;
			}
		}

		private bool isReplaced;
	}
}
