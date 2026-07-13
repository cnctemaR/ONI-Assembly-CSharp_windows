using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static class VisualTreeAssetUtilities
	{
		public static IEnumerable<string> EnumerateEnclosingNamespaces(string fullTypeName)
		{
			int startIndex = fullTypeName.Length - 1;
			for (;;)
			{
				int lastDot = fullTypeName.LastIndexOf(".", startIndex, StringComparison.Ordinal);
				bool flag = lastDot >= 0;
				if (!flag)
				{
					break;
				}
				yield return fullTypeName.Substring(0, lastDot);
				startIndex = lastDot - 1;
			}
			yield break;
			yield break;
		}

		public static UxmlNamespaceDefinition FindUxmlNamespaceDefinitionFromPrefix(this VisualTreeAsset vta, UxmlAsset asset, string prefix)
		{
			for (UxmlAsset uxmlAsset = asset; uxmlAsset != null; uxmlAsset = uxmlAsset.parentAsset)
			{
				for (int i = 0; i < uxmlAsset.namespaceDefinitions.Count; i++)
				{
					UxmlNamespaceDefinition uxmlNamespaceDefinition = uxmlAsset.namespaceDefinitions[i];
					bool flag = string.Compare(uxmlNamespaceDefinition.prefix, prefix, StringComparison.Ordinal) == 0;
					if (flag)
					{
						return uxmlNamespaceDefinition;
					}
				}
			}
			return UxmlNamespaceDefinition.Empty;
		}

		public static UxmlNamespaceDefinition FindUxmlNamespaceDefinitionForTypeName(this VisualTreeAsset vta, UxmlAsset asset, string fullTypeName)
		{
			List<UxmlNamespaceDefinition> list;
			UxmlNamespaceDefinition uxmlNamespaceDefinition;
			using (CollectionPool<List<UxmlNamespaceDefinition>, UxmlNamespaceDefinition>.Get(out list))
			{
				for (UxmlAsset uxmlAsset = asset; uxmlAsset != null; uxmlAsset = uxmlAsset.parentAsset)
				{
					list.AddRange(uxmlAsset.namespaceDefinitions);
				}
				bool flag = list.Count == 0;
				if (flag)
				{
					uxmlNamespaceDefinition = UxmlNamespaceDefinition.Empty;
				}
				else
				{
					foreach (string text in VisualTreeAssetUtilities.EnumerateEnclosingNamespaces(fullTypeName))
					{
						for (int i = 0; i < list.Count; i++)
						{
							bool flag2 = list[i].resolvedNamespace.Equals(text, StringComparison.Ordinal);
							if (flag2)
							{
								return list[i];
							}
						}
					}
					uxmlNamespaceDefinition = UxmlNamespaceDefinition.Empty;
				}
			}
			return uxmlNamespaceDefinition;
		}

		public static void GatherUxmlNamespaceDefinitions(this VisualTreeAsset vta, UxmlAsset asset, List<UxmlNamespaceDefinition> definitions)
		{
			for (UxmlAsset uxmlAsset = asset; uxmlAsset != null; uxmlAsset = uxmlAsset.parentAsset)
			{
				definitions.InsertRange(0, uxmlAsset.namespaceDefinitions);
			}
		}
	}
}
