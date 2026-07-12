using System;

namespace ImGuiObjectDrawer
{
	public sealed class LocStringDrawer : InlineDrawer
	{
		public override bool CanDraw(in MemberDrawContext context, in MemberDetails member)
		{
			return member.CanAssignToType<LocString>();
		}

		protected override void DrawInline(in MemberDrawContext context, in MemberDetails member)
		{
			ImGuiEx.SimpleField(member.name, string.Format("{0}({1})", member.value, ((LocString)member.value).text));
		}
	}
}
