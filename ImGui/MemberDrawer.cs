using System;
using ImGuiNET;
using UnityEngine;

namespace ImGuiObjectDrawer
{
	public abstract class MemberDrawer
	{
		public abstract bool CanDraw(in MemberDrawContext context, in MemberDetails member);

		public virtual bool CanDrawAtDepth(int depth)
		{
			return depth < 100;
		}

		protected virtual void DrawTooltip(in MemberDrawContext context, in MemberDetails member)
		{
			DrawerUtil.Tooltip(member.type);
		}

		public void Draw(in MemberDrawContext context, in MemberDetails member, int depth)
		{
			try
			{
				MemberDrawType memberDrawType = this.GetDrawType(in context, in member);
				if (depth > 100)
				{
					memberDrawType = MemberDrawType.Inline;
				}
				if (memberDrawType != MemberDrawType.Inline)
				{
					if (memberDrawType == MemberDrawType.Custom)
					{
						this.DrawCustom(in context, in member, depth);
					}
				}
				else
				{
					ImGui.Indent();
					this.DrawInline(in context, in member);
					ImGui.Unindent();
					this.DrawTooltip(in context, in member);
				}
			}
			catch (Exception ex)
			{
				ImGui.Indent();
				ImGuiEx.SimpleField(member.name, "<ERROR: " + ex.Message + ">");
				ImGui.Unindent();
				Debug.Log(string.Format("IMGUI OBJECT DRAW ERROR:\n\n{0}", ex));
			}
		}

		public abstract MemberDrawType GetDrawType(in MemberDrawContext context, in MemberDetails member);

		protected abstract void DrawInline(in MemberDrawContext context, in MemberDetails member);

		protected abstract void DrawCustom(in MemberDrawContext context, in MemberDetails member, int depth);

		public const int MAX_DEPTH = 100;

		public const int DEFAULT_DEPTH = 0;
	}
}
