using System;
using System.Collections;

namespace ImGuiObjectDrawer
{
	public sealed class IEnumerableDrawer : CollectionDrawer
	{
		public override bool CanDraw(in MemberDrawContext context, in MemberDetails member)
		{
			return member.CanAssignToType<IEnumerable>();
		}

		public override bool IsEmpty(in MemberDrawContext context, in MemberDetails member)
		{
			return !((IEnumerable)member.value).GetEnumerator().MoveNext();
		}

		protected override void VisitElements(CollectionDrawer.ElementVisitor visit, in MemberDrawContext context, in MemberDetails member)
		{
			IEnumerable enumerable = (IEnumerable)member.value;
			int num = 0;
			using (IEnumerator enumerator = enumerable.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object el = enumerator.Current;
					visit(in context, new CollectionDrawer.Element(num, delegate
					{
						DrawerUtil.Tooltip(el.GetType());
					}, () => new
					{
						value = el
					}));
					num++;
				}
			}
		}
	}
}
