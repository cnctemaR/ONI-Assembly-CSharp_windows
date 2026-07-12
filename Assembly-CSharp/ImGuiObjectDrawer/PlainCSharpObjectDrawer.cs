using System;
using ImGuiNET;

namespace ImGuiObjectDrawer
{
	public class PlainCSharpObjectDrawer : MemberDrawer
	{
		public override bool CanDraw(in MemberDrawContext context, in MemberDetails member)
		{
			return true;
		}

		public override MemberDrawType GetDrawType(in MemberDrawContext context, in MemberDetails member)
		{
			return MemberDrawType.Custom;
		}

		protected override void DrawInline(in MemberDrawContext context, in MemberDetails member)
		{
			throw new InvalidOperationException();
		}

		protected override void DrawCustom(in MemberDrawContext context, in MemberDetails member, int depth)
		{
			ImGuiTreeNodeFlags imGuiTreeNodeFlags = ImGuiTreeNodeFlags.None;
			if (context.default_open && depth <= 0)
			{
				imGuiTreeNodeFlags |= ImGuiTreeNodeFlags.DefaultOpen;
			}
			bool flag = ImGui.TreeNodeEx(member.name, imGuiTreeNodeFlags);
			DrawerUtil.Tooltip(member.type);
			if (flag)
			{
				this.DrawContents(in context, in member, depth);
				ImGui.TreePop();
			}
		}

		protected virtual void DrawContents(in MemberDrawContext context, in MemberDetails member, int depth)
		{
			DrawerUtil.DrawObjectContents(member.value, in context, depth + 1);
		}
	}
}
