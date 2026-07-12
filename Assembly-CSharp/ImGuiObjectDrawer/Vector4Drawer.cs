using System;
using UnityEngine;

namespace ImGuiObjectDrawer
{
	public sealed class Vector4Drawer : InlineDrawer
	{
		public override bool CanDraw(in MemberDrawContext context, in MemberDetails member)
		{
			return member.value is Vector4;
		}

		protected override void DrawInline(in MemberDrawContext context, in MemberDetails member)
		{
			Vector4 vector = (Vector4)member.value;
			ImGuiEx.SimpleField(member.name, string.Format("( {0}, {1}, {2}, {3} )", new object[] { vector.x, vector.y, vector.z, vector.w }));
		}
	}
}
