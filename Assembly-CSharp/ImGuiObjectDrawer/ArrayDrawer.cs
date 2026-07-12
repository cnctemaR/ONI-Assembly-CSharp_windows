using System;

namespace ImGuiObjectDrawer
{
	public sealed class ArrayDrawer : CollectionDrawer
	{
		public override bool CanDraw(in MemberDrawContext context, in MemberDetails member)
		{
			return member.type.IsArray;
		}

		public override bool IsEmpty(in MemberDrawContext context, in MemberDetails member)
		{
			return ((Array)member.value).Length == 0;
		}

		protected override void VisitElements(CollectionDrawer.ElementVisitor visit, in MemberDrawContext context, in MemberDetails member)
		{
			ArrayDrawer.<>c__DisplayClass2_0 CS$<>8__locals1 = new ArrayDrawer.<>c__DisplayClass2_0();
			CS$<>8__locals1.array = (Array)member.value;
			int i;
			int num;
			for (i = 0; i < CS$<>8__locals1.array.Length; i = num)
			{
				int j = i;
				global::System.Action action;
				if ((action = CS$<>8__locals1.<>9__0) == null)
				{
					action = (CS$<>8__locals1.<>9__0 = delegate
					{
						DrawerUtil.Tooltip(CS$<>8__locals1.array.GetType().GetElementType());
					});
				}
				visit(in context, new CollectionDrawer.Element(j, action, () => new
				{
					value = CS$<>8__locals1.array.GetValue(i)
				}));
				num = i + 1;
			}
		}
	}
}
