using System;

namespace ImGuiObjectDrawer
{
	public sealed class KAnimHashedStringDrawer : InlineDrawer
	{
		public override bool CanDraw(in MemberDrawContext context, in MemberDetails member)
		{
			return member.value is KAnimHashedString;
		}

		protected override void DrawInline(in MemberDrawContext context, in MemberDetails member)
		{
			ImGuiEx.SimpleField(member.name, string.Format("{0}({1})", member.value, ((KAnimHashedString)member.value).ToString()));
		}
	}
}
