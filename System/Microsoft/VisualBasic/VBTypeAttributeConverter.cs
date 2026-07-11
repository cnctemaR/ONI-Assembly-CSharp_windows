using System;
using System.Reflection;

namespace Microsoft.VisualBasic
{
	internal sealed class VBTypeAttributeConverter : VBModifierAttributeConverter
	{
		private VBTypeAttributeConverter()
		{
		}

		public static VBTypeAttributeConverter Default { get; } = new VBTypeAttributeConverter();

		protected override string[] Names { get; } = new string[] { "Public", "Friend" };

		protected override object[] Values { get; } = new object[]
		{
			TypeAttributes.Public,
			TypeAttributes.NotPublic
		};

		protected override object DefaultValue
		{
			get
			{
				return TypeAttributes.Public;
			}
		}
	}
}
