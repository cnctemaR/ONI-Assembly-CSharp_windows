using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal static class GroupBoxUtility
	{
		public static void RegisterGroupBoxOption<T>(this T option) where T : VisualElement, IGroupBoxOption
		{
			VisualElement visualElement = option;
			IGroupBox groupBox = null;
			for (VisualElement visualElement2 = visualElement.hierarchy.parent; visualElement2 != null; visualElement2 = visualElement2.hierarchy.parent)
			{
				IGroupBox groupBox2 = visualElement2 as IGroupBox;
				bool flag = groupBox2 != null;
				if (flag)
				{
					groupBox = groupBox2;
					break;
				}
			}
			IGroupBox groupBox3 = groupBox ?? visualElement.elementPanel;
			IGroupManager groupManager = GroupBoxUtility.FindOrCreateGroupManager(groupBox3);
			groupManager.RegisterOption(option);
			GroupBoxUtility.s_GroupOptionManagerCache[option] = groupManager;
		}

		public static void UnregisterGroupBoxOption<T>(this T option) where T : VisualElement, IGroupBoxOption
		{
			bool flag = !GroupBoxUtility.s_GroupOptionManagerCache.ContainsKey(option);
			if (!flag)
			{
				GroupBoxUtility.s_GroupOptionManagerCache[option].UnregisterOption(option);
				GroupBoxUtility.s_GroupOptionManagerCache.Remove(option);
			}
		}

		public static void OnOptionSelected<T>(this T selectedOption) where T : VisualElement, IGroupBoxOption
		{
			bool flag = !GroupBoxUtility.s_GroupOptionManagerCache.ContainsKey(selectedOption);
			if (!flag)
			{
				GroupBoxUtility.s_GroupOptionManagerCache[selectedOption].OnOptionSelectionChanged(selectedOption);
			}
		}

		public static IGroupBoxOption GetSelectedOption(this IGroupBox groupBox)
		{
			return (!GroupBoxUtility.s_GroupManagers.ContainsKey(groupBox)) ? null : GroupBoxUtility.s_GroupManagers[groupBox].GetSelectedOption();
		}

		public static IGroupManager GetGroupManager(this IGroupBox groupBox)
		{
			return GroupBoxUtility.s_GroupManagers.ContainsKey(groupBox) ? GroupBoxUtility.s_GroupManagers[groupBox] : null;
		}

		private static IGroupManager FindOrCreateGroupManager(IGroupBox groupBox)
		{
			bool flag = GroupBoxUtility.s_GroupManagers.ContainsKey(groupBox);
			IGroupManager groupManager;
			if (flag)
			{
				groupManager = GroupBoxUtility.s_GroupManagers[groupBox];
			}
			else
			{
				Type type = null;
				foreach (Type type2 in groupBox.GetType().GetInterfaces())
				{
					bool flag2 = type2.IsGenericType && GroupBoxUtility.k_GenericGroupBoxType.IsAssignableFrom(type2.GetGenericTypeDefinition());
					if (flag2)
					{
						type = type2.GetGenericArguments()[0];
						break;
					}
				}
				IGroupManager groupManager3;
				if (!(type != null))
				{
					IGroupManager groupManager2 = new DefaultGroupManager();
					groupManager3 = groupManager2;
				}
				else
				{
					groupManager3 = (IGroupManager)Activator.CreateInstance(type);
				}
				IGroupManager groupManager4 = groupManager3;
				groupManager4.Init(groupBox);
				BaseVisualElementPanel baseVisualElementPanel = groupBox as BaseVisualElementPanel;
				bool flag3 = baseVisualElementPanel != null;
				if (flag3)
				{
					baseVisualElementPanel.panelDisposed += GroupBoxUtility.OnPanelDestroyed;
				}
				else
				{
					VisualElement visualElement = groupBox as VisualElement;
					bool flag4 = visualElement != null;
					if (flag4)
					{
						visualElement.RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(GroupBoxUtility.OnGroupBoxDetachedFromPanel), TrickleDown.NoTrickleDown);
					}
				}
				GroupBoxUtility.s_GroupManagers[groupBox] = groupManager4;
				groupManager = groupManager4;
			}
			return groupManager;
		}

		private static void OnGroupBoxDetachedFromPanel(DetachFromPanelEvent evt)
		{
			GroupBoxUtility.s_GroupManagers.Remove(evt.currentTarget as IGroupBox);
		}

		private static void OnPanelDestroyed(BaseVisualElementPanel panel)
		{
			GroupBoxUtility.s_GroupManagers.Remove(panel);
			panel.panelDisposed -= GroupBoxUtility.OnPanelDestroyed;
		}

		private static Dictionary<IGroupBox, IGroupManager> s_GroupManagers = new Dictionary<IGroupBox, IGroupManager>();

		private static Dictionary<IGroupBoxOption, IGroupManager> s_GroupOptionManagerCache = new Dictionary<IGroupBoxOption, IGroupManager>();

		private static readonly Type k_GenericGroupBoxType = typeof(IGroupBox<>);
	}
}
