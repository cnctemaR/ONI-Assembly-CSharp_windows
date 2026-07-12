using System;
using System.Collections.Generic;
using System.Reflection;
using ImGuiNET;

namespace ImGuiObjectDrawer
{
	public static class MemberDrawerCollection
	{
		public static void DrawFor(in MemberDrawContext context, in MemberDetails member, int depth)
		{
			string name = member.name;
			Type type = member.type;
			ImGui.PushID(name + ((type != null) ? type.ToString() : null) + depth.ToString() + context.GetHashCode().ToString());
			MemberDrawerCollection.GetFor(in context, in member, depth).Draw(in context, in member, depth);
			ImGui.PopID();
		}

		public static MemberDrawer GetFor(in MemberDrawContext context, in MemberDetails member, int depth)
		{
			for (int i = 0; i < MemberDrawerCollection.Drawers.Length; i++)
			{
				if (MemberDrawerCollection.Drawers[i].CanDrawAtDepth(depth) && MemberDrawerCollection.Drawers[i].CanDraw(in context, in member))
				{
					return MemberDrawerCollection.Drawers[i];
				}
			}
			throw new NotSupportedException();
		}

		public static MemberDrawer[] CollectAllLoadedMemberDrawers()
		{
			List<IMemberDrawerProvider> list = new List<IMemberDrawerProvider>();
			foreach (Type type in ReflectionUtil.CollectTypesThatInheritOrImplement<IMemberDrawerProvider>(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
			{
				if (!type.IsAbstract && !type.IsInterface)
				{
					list.Add((IMemberDrawerProvider)Activator.CreateInstance(type));
				}
			}
			List<MemberDrawer> list2 = new List<MemberDrawer>();
			foreach (IMemberDrawerProvider memberDrawerProvider in list)
			{
				memberDrawerProvider.AppendDrawersTo(list2);
			}
			return list2.ToArray();
		}

		public static readonly MemberDrawer[] Drawers = MemberDrawerCollection.CollectAllLoadedMemberDrawers();
	}
}
