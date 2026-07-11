using System;
using System.Reflection;

namespace Microsoft.CSharp
{
	internal sealed class CSharpTypeAttributeConverter : CSharpModifierAttributeConverter
	{
		private CSharpTypeAttributeConverter()
		{
		}

		public static CSharpTypeAttributeConverter Default { get; } = new CSharpTypeAttributeConverter();

		protected override string[] Names { get; } = new string[] { "Public", "Internal" };

		protected override object[] Values { get; } = new object[]
		{
			TypeAttributes.Public,
			TypeAttributes.NotPublic
		};

		protected override object DefaultValue
		{
			get
			{
				return TypeAttributes.NotPublic;
			}
		}
	}
}
