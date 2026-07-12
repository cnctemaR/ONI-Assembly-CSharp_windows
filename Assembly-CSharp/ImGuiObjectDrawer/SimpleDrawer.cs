using System;

namespace ImGuiObjectDrawer
{
	public class SimpleDrawer : InlineDrawer
	{
		public override bool CanDrawAtDepth(int depth)
		{
			return true;
		}

		public override bool CanDraw(in MemberDrawContext context, in MemberDetails member)
		{
			return member.type.IsPrimitive || member.CanAssignToType<string>();
		}

		protected override void DrawInline(in MemberDrawContext context, in MemberDetails member)
		{
			ImGuiEx.SimpleField(member.name, member.value.ToString());
		}
	}
}
