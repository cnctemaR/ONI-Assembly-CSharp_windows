using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public static class UxmlDescriptionCache
	{
		public static void RegisterType(Type type, UxmlAttributeNames[] attributeNames, bool isEditorOnly = false)
		{
			UxmlDescriptionCache.s_NamesPerType[type] = new UxmlDescriptionCache.CachedDescription
			{
				attributeNames = attributeNames,
				editorOnly = isEditorOnly
			};
		}

		internal static bool TryGetCachedDescription(Type type, out UxmlDescriptionCache.CachedDescription description)
		{
			return UxmlDescriptionCache.s_NamesPerType.TryGetValue(type, out description);
		}

		private static readonly Dictionary<Type, UxmlDescriptionCache.CachedDescription> s_NamesPerType = new Dictionary<Type, UxmlDescriptionCache.CachedDescription>();

		internal struct CachedDescription
		{
			public UxmlAttributeNames[] attributeNames;

			public bool editorOnly;
		}
	}
}
