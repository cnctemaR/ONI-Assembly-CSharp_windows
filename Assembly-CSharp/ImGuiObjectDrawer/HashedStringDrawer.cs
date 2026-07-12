using System;

namespace ImGuiObjectDrawer
{
	public sealed class HashedStringDrawer : InlineDrawer
	{
		public override bool CanDraw(in MemberDrawContext context, in MemberDetails member)
		{
			return member.value is HashedString;
		}

		protected override void DrawInline(in MemberDrawContext context, in MemberDetails member)
		{
			ImGuiEx.SimpleField(member.name, string.Format("{0}({1})", member.value, ((HashedString)member.value).ToString()));
		}
	}
}
