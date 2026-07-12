using System;
using UnityEngine;

namespace ImGuiObjectDrawer
{
	public sealed class Vector2Drawer : InlineDrawer
	{
		public override bool CanDraw(in MemberDrawContext context, in MemberDetails member)
		{
			return member.value is Vector2;
		}

		protected override void DrawInline(in MemberDrawContext context, in MemberDetails member)
		{
			Vector2 vector = (Vector2)member.value;
			ImGuiEx.SimpleField(member.name, string.Format("( {0}, {1} )", vector.x, vector.y));
		}
	}
}
