using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal static class UxmlDescriptionRegistry
	{
		public static UxmlTypeDescription GetDescription(Type type)
		{
			UxmlTypeDescription uxmlTypeDescription;
			bool flag = !UxmlDescriptionRegistry.s_UxmlDescriptions.TryGetValue(type, out uxmlTypeDescription);
			if (flag)
			{
				Dictionary<Type, UxmlTypeDescription> dictionary = UxmlDescriptionRegistry.s_UxmlDescriptions;
				uxmlTypeDescription = new UxmlTypeDescription(type);
				dictionary.Add(type, uxmlTypeDescription);
			}
			return uxmlTypeDescription;
		}

		public static void Clear()
		{
			UxmlDescriptionRegistry.s_UxmlDescriptions.Clear();
		}

		private static readonly Dictionary<Type, UxmlTypeDescription> s_UxmlDescriptions = new Dictionary<Type, UxmlTypeDescription>();
	}
}
