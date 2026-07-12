using System;
using ImGuiNET;

namespace ImGuiObjectDrawer
{
	public static class DrawerUtil
	{
		public static void Tooltip(Type type)
		{
			DrawerUtil.Tooltip(type.FullName);
		}

		public static void Tooltip(string tooltip)
		{
			if (ImGui.IsItemHovered())
			{
				ImGui.BeginTooltip();
				ImGui.Text(tooltip);
				ImGui.EndTooltip();
			}
		}

		public static void DrawObjectContents(object obj, in MemberDrawContext context, int depth)
		{
			DrawerUtil.<>c__DisplayClass2_0 CS$<>8__locals1 = new DrawerUtil.<>c__DisplayClass2_0();
			CS$<>8__locals1.depth = depth;
			MemberDetails.Visit(obj, in context, new MemberDetails.MemberDetailsVisitor(CS$<>8__locals1.<DrawObjectContents>g__Visitor|0));
		}
	}
}
