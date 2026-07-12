using System;

namespace ImGuiObjectDrawer
{
	public class NullDrawer : InlineDrawer
	{
		public override bool CanDrawAtDepth(int depth)
		{
			return true;
		}

		public override bool CanDraw(in MemberDrawContext context, in MemberDetails member)
		{
			return member.value == null;
		}

		protected override void DrawInline(in MemberDrawContext context, in MemberDetails member)
		{
			ImGuiEx.SimpleField(member.name, "null");
		}
	}
}
