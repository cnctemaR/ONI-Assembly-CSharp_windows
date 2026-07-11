using System;
using System.CodeDom;

namespace Microsoft.CSharp
{
	internal sealed class CSharpMemberAttributeConverter : CSharpModifierAttributeConverter
	{
		private CSharpMemberAttributeConverter()
		{
		}

		public static CSharpMemberAttributeConverter Default { get; } = new CSharpMemberAttributeConverter();

		protected override string[] Names { get; } = new string[] { "Public", "Protected", "Protected Internal", "Internal", "Private" };

		protected override object[] Values { get; } = new object[]
		{
			MemberAttributes.Public,
			MemberAttributes.Family,
			MemberAttributes.FamilyOrAssembly,
			MemberAttributes.Assembly,
			MemberAttributes.Private
		};

		protected override object DefaultValue
		{
			get
			{
				return MemberAttributes.Private;
			}
		}
	}
}
