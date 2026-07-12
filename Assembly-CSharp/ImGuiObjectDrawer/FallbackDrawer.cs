using System;

namespace ImGuiObjectDrawer
{
	public sealed class FallbackDrawer : SimpleDrawer
	{
		public override bool CanDraw(in MemberDrawContext context, in MemberDetails member)
		{
			return true;
		}

		public override bool CanDrawAtDepth(int depth)
		{
			return true;
		}
	}
}
