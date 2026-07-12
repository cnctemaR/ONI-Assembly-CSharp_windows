using System;
using ImGuiNET;
using UnityEngine;

namespace ImGuiObjectDrawer
{
	public class UnityObjectDrawer : PlainCSharpObjectDrawer
	{
		public override bool CanDraw(in MemberDrawContext context, in MemberDetails member)
		{
			return member.value is global::UnityEngine.Object;
		}

		protected override void DrawCustom(in MemberDrawContext context, in MemberDetails member, int depth)
		{
			global::UnityEngine.Object @object = (global::UnityEngine.Object)member.value;
			ImGuiTreeNodeFlags imGuiTreeNodeFlags = ImGuiTreeNodeFlags.None;
			if (context.default_open && depth <= 0)
			{
				imGuiTreeNodeFlags |= ImGuiTreeNodeFlags.DefaultOpen;
			}
			bool flag = ImGui.TreeNodeEx(member.name, imGuiTreeNodeFlags);
			DrawerUtil.Tooltip(member.type);
			if (flag)
			{
				base.DrawContents(in context, in member, depth);
				ImGui.TreePop();
			}
		}
	}
}
