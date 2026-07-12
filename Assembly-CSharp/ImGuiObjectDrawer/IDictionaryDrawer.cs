using System;
using System.Collections;

namespace ImGuiObjectDrawer
{
	public sealed class IDictionaryDrawer : CollectionDrawer
	{
		public override bool CanDraw(in MemberDrawContext context, in MemberDetails member)
		{
			return member.CanAssignToType<IDictionary>();
		}

		public override bool IsEmpty(in MemberDrawContext context, in MemberDetails member)
		{
			return ((IDictionary)member.value).Count == 0;
		}

		protected override void VisitElements(CollectionDrawer.ElementVisitor visit, in MemberDrawContext context, in MemberDetails member)
		{
			IDictionary dictionary = (IDictionary)member.value;
			int num = 0;
			using (IDictionaryEnumerator enumerator = dictionary.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DictionaryEntry kvp = (DictionaryEntry)enumerator.Current;
					visit(in context, new CollectionDrawer.Element(num, delegate
					{
						DrawerUtil.Tooltip(string.Format("{0} -> {1}", kvp.Key.GetType(), kvp.Value.GetType()));
					}, () => new
					{
						key = kvp.Key,
						value = kvp.Value
					}));
					num++;
				}
			}
		}
	}
}
