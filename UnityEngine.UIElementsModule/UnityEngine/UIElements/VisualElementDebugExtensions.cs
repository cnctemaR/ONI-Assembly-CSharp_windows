using System;

namespace UnityEngine.UIElements
{
	public static class VisualElementDebugExtensions
	{
		internal static string GetDisplayName(this VisualElement ve, bool withHashCode = true)
		{
			bool flag = ve == null;
			string text;
			if (flag)
			{
				text = string.Empty;
			}
			else
			{
				string text2 = ve.GetType().Name;
				bool flag2 = !string.IsNullOrEmpty(ve.name);
				if (flag2)
				{
					text2 = text2 + "#" + ve.name;
				}
				if (withHashCode)
				{
					text2 = text2 + " (" + ve.GetHashCode().ToString("x8") + ")";
				}
				text = text2;
			}
			return text;
		}

		public static void DebugIncrementVersionChange(VisualElement ve, VersionChangeType changeType)
		{
			ve.IncrementVersion(changeType);
		}
	}
}
