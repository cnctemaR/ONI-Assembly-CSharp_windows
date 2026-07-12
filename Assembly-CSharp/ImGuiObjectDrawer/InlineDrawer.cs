using System;

namespace ImGuiObjectDrawer
{
	public abstract class InlineDrawer : MemberDrawer
	{
		public sealed override MemberDrawType GetDrawType(in MemberDrawContext context, in MemberDetails member)
		{
			return MemberDrawType.Inline;
		}

		protected sealed override void DrawCustom(in MemberDrawContext context, in MemberDetails member, int depth)
		{
			this.DrawInline(in context, in member);
		}
	}
}
