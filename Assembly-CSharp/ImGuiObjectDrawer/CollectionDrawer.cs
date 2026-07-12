using System;
using ImGuiNET;

namespace ImGuiObjectDrawer
{
	public abstract class CollectionDrawer : MemberDrawer
	{
		public abstract bool IsEmpty(in MemberDrawContext context, in MemberDetails member);

		public override MemberDrawType GetDrawType(in MemberDrawContext context, in MemberDetails member)
		{
			if (this.IsEmpty(in context, in member))
			{
				return MemberDrawType.Inline;
			}
			return MemberDrawType.Custom;
		}

		protected sealed override void DrawInline(in MemberDrawContext context, in MemberDetails member)
		{
			Debug.Assert(this.IsEmpty(in context, in member));
			this.DrawEmpty(in context, in member);
		}

		protected sealed override void DrawCustom(in MemberDrawContext context, in MemberDetails member, int depth)
		{
			Debug.Assert(!this.IsEmpty(in context, in member));
			this.DrawWithContents(in context, in member, depth);
		}

		private void DrawEmpty(in MemberDrawContext context, in MemberDetails member)
		{
			ImGui.Text(member.name + "(empty)");
		}

		private void DrawWithContents(in MemberDrawContext context, in MemberDetails member, int depth)
		{
			CollectionDrawer.<>c__DisplayClass5_0 CS$<>8__locals1 = new CollectionDrawer.<>c__DisplayClass5_0();
			CS$<>8__locals1.depth = depth;
			ImGuiTreeNodeFlags imGuiTreeNodeFlags = ImGuiTreeNodeFlags.None;
			if (context.default_open && CS$<>8__locals1.depth <= 0)
			{
				imGuiTreeNodeFlags |= ImGuiTreeNodeFlags.DefaultOpen;
			}
			bool flag = ImGui.TreeNodeEx(member.name, imGuiTreeNodeFlags);
			DrawerUtil.Tooltip(member.type);
			if (flag)
			{
				this.VisitElements(new CollectionDrawer.ElementVisitor(CS$<>8__locals1.<DrawWithContents>g__Visitor|0), in context, in member);
				ImGui.TreePop();
			}
		}

		protected abstract void VisitElements(CollectionDrawer.ElementVisitor visit, in MemberDrawContext context, in MemberDetails member);

		protected delegate void ElementVisitor(in MemberDrawContext context, CollectionDrawer.Element element);

		protected struct Element
		{
			public Element(string node_name, global::System.Action draw_tooltip, Func<object> get_object_to_inspect)
			{
				this.node_name = node_name;
				this.draw_tooltip = draw_tooltip;
				this.get_object_to_inspect = get_object_to_inspect;
			}

			public Element(int index, global::System.Action draw_tooltip, Func<object> get_object_to_inspect)
			{
				this = new CollectionDrawer.Element(string.Format("[{0}]", index), draw_tooltip, get_object_to_inspect);
			}

			public readonly string node_name;

			public readonly global::System.Action draw_tooltip;

			public readonly Func<object> get_object_to_inspect;
		}
	}
}
