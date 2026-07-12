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
			HashedString hashedString = (HashedString)member.value;
			string text = hashedString.ToString();
			string text2 = "0x" + hashedString.HashValue.ToString("X");
			ImGuiEx.SimpleField(member.name, text + " (" + text2 + ")");
		}
	}
}
