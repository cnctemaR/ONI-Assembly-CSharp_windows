using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal static class VisualElementUtils
	{
		public static string GetUniqueName(string nameBase)
		{
			string text = nameBase;
			int num = 2;
			while (VisualElementUtils.s_usedNames.Contains(text))
			{
				text = nameBase + num.ToString();
				num++;
			}
			VisualElementUtils.s_usedNames.Add(text);
			return text;
		}

		internal static int GetFoldoutDepth(this VisualElement element)
		{
			int num = 0;
			bool flag = element.parent != null;
			if (flag)
			{
				for (VisualElement visualElement = element.parent; visualElement != null; visualElement = visualElement.parent)
				{
					bool flag2 = VisualElementUtils.s_FoldoutType.IsAssignableFrom(visualElement.GetType());
					if (flag2)
					{
						num++;
					}
				}
			}
			return num;
		}

		internal static void AssignInspectorStyleIfNecessary(this VisualElement element, string classNameToEnable)
		{
			VisualElement firstAncestorWhere = element.GetFirstAncestorWhere((VisualElement i) => i.ClassListContains(VisualElementUtils.s_InspectorElementUssClassName));
			element.EnableInClassList(classNameToEnable, firstAncestorWhere != null);
		}

		private static readonly HashSet<string> s_usedNames = new HashSet<string>();

		private static readonly Type s_FoldoutType = typeof(Foldout);

		private static readonly string s_InspectorElementUssClassName = "unity-inspector-element";
	}
}
