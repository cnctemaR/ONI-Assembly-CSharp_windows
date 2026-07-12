using System;

namespace ImGuiObjectDrawer
{
	public sealed class EnumDrawer : InlineDrawer
	{
		public override bool CanDraw(in MemberDrawContext context, in MemberDetails member)
		{
			return member.type.IsEnum;
		}

		protected override void DrawInline(in MemberDrawContext context, in MemberDetails member)
		{
			ImGuiEx.SimpleField(member.name, member.value.ToString());
		}
	}
}
