using System;
using System.CodeDom;

namespace Microsoft.VisualBasic
{
	internal sealed class VBMemberAttributeConverter : VBModifierAttributeConverter
	{
		private VBMemberAttributeConverter()
		{
		}

		public static VBMemberAttributeConverter Default { get; } = new VBMemberAttributeConverter();

		protected override string[] Names { get; } = new string[] { "Public", "Protected", "Protected Friend", "Friend", "Private" };

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
