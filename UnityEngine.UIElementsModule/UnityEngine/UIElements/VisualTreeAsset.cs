using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace UnityEngine.UIElements
{
	[Serializable]
	public class VisualTreeAsset : ScriptableObject
	{
		internal int GetNextChildSerialNumber()
		{
			List<VisualElementAsset> visualElementAssets = this.m_VisualElementAssets;
			int num = ((visualElementAssets != null) ? visualElementAssets.Count : 0);
			int num2 = num;
			List<TemplateAsset> templateAssets = this.m_TemplateAssets;
			return num2 + ((templateAssets != null) ? templateAssets.Count : 0);
		}

		public IEnumerable<VisualTreeAsset> templateDependencies
		{
			get
			{
				HashSet<VisualTreeAsset> sent = new HashSet<VisualTreeAsset>();
				foreach (VisualTreeAsset.UsingEntry entry in this.m_Usings)
				{
					bool flag = entry.asset != null && !sent.Contains(entry.asset);
					if (flag)
					{
						sent.Add(entry.asset);
						yield return entry.asset;
					}
					else
					{
						bool flag2 = !string.IsNullOrEmpty(entry.path);
						if (flag2)
						{
							VisualTreeAsset vta = Panel.LoadResource(entry.path, typeof(VisualTreeAsset), GUIUtility.pixelsPerPoint) as VisualTreeAsset;
							bool flag3 = vta != null && !sent.Contains(entry.asset);
							if (flag3)
							{
								sent.Add(entry.asset);
								yield return vta;
							}
							vta = null;
						}
					}
					entry = default(VisualTreeAsset.UsingEntry);
				}
				List<VisualTreeAsset.UsingEntry>.Enumerator enumerator = default(List<VisualTreeAsset.UsingEntry>.Enumerator);
				yield break;
				yield break;
			}
		}

		public IEnumerable<StyleSheet> stylesheets
		{
			get
			{
				HashSet<StyleSheet> sent = new HashSet<StyleSheet>();
				foreach (VisualElementAsset vea in this.m_VisualElementAssets)
				{
					bool hasStylesheets = vea.hasStylesheets;
					if (hasStylesheets)
					{
						foreach (StyleSheet stylesheet in vea.stylesheets)
						{
							bool flag = !sent.Contains(stylesheet);
							if (flag)
							{
								sent.Add(stylesheet);
								yield return stylesheet;
							}
							stylesheet = null;
						}
						List<StyleSheet>.Enumerator enumerator2 = default(List<StyleSheet>.Enumerator);
					}
					bool hasStylesheetPaths = vea.hasStylesheetPaths;
					if (hasStylesheetPaths)
					{
						foreach (string stylesheetPath in vea.stylesheetPaths)
						{
							StyleSheet stylesheet2 = Panel.LoadResource(stylesheetPath, typeof(StyleSheet), GUIUtility.pixelsPerPoint) as StyleSheet;
							bool flag2 = stylesheet2 != null && !sent.Contains(stylesheet2);
							if (flag2)
							{
								sent.Add(stylesheet2);
								yield return stylesheet2;
							}
							stylesheet2 = null;
							stylesheetPath = null;
						}
						List<string>.Enumerator enumerator3 = default(List<string>.Enumerator);
					}
					vea = null;
				}
				List<VisualElementAsset>.Enumerator enumerator = default(List<VisualElementAsset>.Enumerator);
				yield break;
				yield break;
			}
		}

		internal List<VisualElementAsset> visualElementAssets
		{
			get
			{
				return this.m_VisualElementAssets;
			}
			set
			{
				this.m_VisualElementAssets = value;
			}
		}

		internal List<TemplateAsset> templateAssets
		{
			get
			{
				return this.m_TemplateAssets;
			}
			set
			{
				this.m_TemplateAssets = value;
			}
		}

		internal List<VisualTreeAsset.SlotDefinition> slots
		{
			get
			{
				return this.m_Slots;
			}
			set
			{
				this.m_Slots = value;
			}
		}

		internal int contentContainerId
		{
			get
			{
				return this.m_ContentContainerId;
			}
			set
			{
				this.m_ContentContainerId = value;
			}
		}

		public TemplateContainer Instantiate()
		{
			TemplateContainer templateContainer = new TemplateContainer(base.name);
			try
			{
				this.CloneTree(templateContainer, VisualTreeAsset.s_TemporarySlotInsertionPoints, null);
			}
			finally
			{
				VisualTreeAsset.s_TemporarySlotInsertionPoints.Clear();
			}
			return templateContainer;
		}

		public TemplateContainer Instantiate(string bindingPath)
		{
			TemplateContainer templateContainer = this.Instantiate();
			templateContainer.bindingPath = bindingPath;
			return templateContainer;
		}

		public TemplateContainer CloneTree()
		{
			return this.Instantiate();
		}

		public TemplateContainer CloneTree(string bindingPath)
		{
			return this.Instantiate(bindingPath);
		}

		public void CloneTree(VisualElement target)
		{
			int num;
			int num2;
			this.CloneTree(target, out num, out num2);
		}

		public void CloneTree(VisualElement target, out int firstElementIndex, out int elementAddedCount)
		{
			bool flag = target == null;
			if (flag)
			{
				throw new ArgumentNullException("target");
			}
			firstElementIndex = target.childCount;
			try
			{
				this.CloneTree(target, VisualTreeAsset.s_TemporarySlotInsertionPoints, null);
			}
			finally
			{
				elementAddedCount = target.childCount - firstElementIndex;
				VisualTreeAsset.s_TemporarySlotInsertionPoints.Clear();
			}
		}

		internal void CloneTree(VisualElement target, Dictionary<string, VisualElement> slotInsertionPoints, List<TemplateAsset.AttributeOverride> attributeOverrides)
		{
			bool flag = target == null;
			if (flag)
			{
				throw new ArgumentNullException("target");
			}
			bool flag2 = (this.visualElementAssets == null || this.visualElementAssets.Count <= 0) && (this.templateAssets == null || this.templateAssets.Count <= 0);
			if (!flag2)
			{
				Dictionary<int, List<VisualElementAsset>> dictionary = new Dictionary<int, List<VisualElementAsset>>();
				int num = ((this.visualElementAssets == null) ? 0 : this.visualElementAssets.Count);
				int num2 = ((this.templateAssets == null) ? 0 : this.templateAssets.Count);
				for (int i = 0; i < num + num2; i++)
				{
					VisualElementAsset visualElementAsset = ((i < num) ? this.visualElementAssets[i] : this.templateAssets[i - num]);
					List<VisualElementAsset> list;
					bool flag3 = !dictionary.TryGetValue(visualElementAsset.parentId, out list);
					if (flag3)
					{
						list = new List<VisualElementAsset>();
						dictionary.Add(visualElementAsset.parentId, list);
					}
					list.Add(visualElementAsset);
				}
				List<VisualElementAsset> list2;
				dictionary.TryGetValue(0, out list2);
				bool flag4 = list2 == null || list2.Count == 0;
				if (!flag4)
				{
					Debug.Assert(list2.Count == 1);
					VisualElementAsset visualElementAsset2 = list2[0];
					VisualTreeAsset.AssignClassListFromAssetToElement(visualElementAsset2, target);
					VisualTreeAsset.AssignStyleSheetFromAssetToElement(visualElementAsset2, target);
					list2.Clear();
					dictionary.TryGetValue(visualElementAsset2.id, out list2);
					bool flag5 = list2 == null || list2.Count == 0;
					if (!flag5)
					{
						list2.Sort(new Comparison<VisualElementAsset>(VisualTreeAsset.CompareForOrder));
						foreach (VisualElementAsset visualElementAsset3 in list2)
						{
							Assert.IsNotNull<VisualElementAsset>(visualElementAsset3);
							VisualElement visualElement = this.CloneSetupRecursively(visualElementAsset3, dictionary, new CreationContext(slotInsertionPoints, attributeOverrides, this, target));
							bool flag6 = visualElement != null;
							if (flag6)
							{
								target.hierarchy.Add(visualElement);
							}
							else
							{
								Debug.LogWarning("VisualTreeAsset instantiated an empty UI. Check the syntax of your UXML document.");
							}
						}
					}
				}
			}
		}

		private VisualElement CloneSetupRecursively(VisualElementAsset root, Dictionary<int, List<VisualElementAsset>> idToChildren, CreationContext context)
		{
			VisualElement visualElement = VisualTreeAsset.Create(root, context);
			bool flag = visualElement == null;
			VisualElement visualElement2;
			if (flag)
			{
				visualElement2 = null;
			}
			else
			{
				bool flag2 = root.id == context.visualTreeAsset.contentContainerId;
				if (flag2)
				{
					bool flag3 = context.target is TemplateContainer;
					if (flag3)
					{
						((TemplateContainer)context.target).SetContentContainer(visualElement);
					}
					else
					{
						Debug.LogError("Trying to clone a VisualTreeAsset with a custom content container into a element which is not a template container");
					}
				}
				string text;
				bool flag4 = context.slotInsertionPoints != null && this.TryGetSlotInsertionPoint(root.id, out text);
				if (flag4)
				{
					context.slotInsertionPoints.Add(text, visualElement);
				}
				bool flag5 = root.ruleIndex != -1;
				if (flag5)
				{
					bool flag6 = this.inlineSheet == null;
					if (flag6)
					{
						Debug.LogWarning("VisualElementAsset has a RuleIndex but no inlineStyleSheet");
					}
					else
					{
						StyleRule styleRule = this.inlineSheet.rules[root.ruleIndex];
						visualElement.SetInlineRule(this.inlineSheet, styleRule);
					}
				}
				TemplateAsset templateAsset = root as TemplateAsset;
				List<VisualElementAsset> list;
				bool flag7 = idToChildren.TryGetValue(root.id, out list);
				if (flag7)
				{
					list.Sort(new Comparison<VisualElementAsset>(VisualTreeAsset.CompareForOrder));
					using (List<VisualElementAsset>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							VisualElementAsset childVea = enumerator.Current;
							VisualElement visualElement3 = this.CloneSetupRecursively(childVea, idToChildren, context);
							bool flag8 = visualElement3 == null;
							if (!flag8)
							{
								bool flag9 = templateAsset == null;
								if (flag9)
								{
									visualElement.Add(visualElement3);
								}
								else
								{
									int num = ((templateAsset.slotUsages == null) ? (-1) : templateAsset.slotUsages.FindIndex((VisualTreeAsset.SlotUsageEntry u) => u.assetId == childVea.id));
									bool flag10 = num != -1;
									if (flag10)
									{
										string slotName = templateAsset.slotUsages[num].slotName;
										Assert.IsFalse(string.IsNullOrEmpty(slotName), "a lost name should not be null or empty, this probably points to an importer or serialization bug");
										VisualElement visualElement4;
										bool flag11 = context.slotInsertionPoints == null || !context.slotInsertionPoints.TryGetValue(slotName, out visualElement4);
										if (flag11)
										{
											Debug.LogErrorFormat("Slot '{0}' was not found. Existing slots: {1}", new object[]
											{
												slotName,
												(context.slotInsertionPoints == null) ? string.Empty : string.Join(", ", context.slotInsertionPoints.Keys.ToArray<string>())
											});
											visualElement.Add(visualElement3);
										}
										else
										{
											visualElement4.Add(visualElement3);
										}
									}
									else
									{
										visualElement.Add(visualElement3);
									}
								}
							}
						}
					}
				}
				bool flag12 = templateAsset != null && context.slotInsertionPoints != null;
				if (flag12)
				{
					context.slotInsertionPoints.Clear();
				}
				visualElement2 = visualElement;
			}
			return visualElement2;
		}

		private static int CompareForOrder(VisualElementAsset a, VisualElementAsset b)
		{
			return a.orderInDocument.CompareTo(b.orderInDocument);
		}

		internal bool TryGetSlotInsertionPoint(int insertionPointId, out string slotName)
		{
			bool flag = this.m_Slots == null;
			bool flag2;
			if (flag)
			{
				slotName = null;
				flag2 = false;
			}
			else
			{
				for (int i = 0; i < this.m_Slots.Count; i++)
				{
					VisualTreeAsset.SlotDefinition slotDefinition = this.m_Slots[i];
					bool flag3 = slotDefinition.insertionPointId == insertionPointId;
					if (flag3)
					{
						slotName = slotDefinition.name;
						return true;
					}
				}
				slotName = null;
				flag2 = false;
			}
			return flag2;
		}

		internal VisualTreeAsset ResolveTemplate(string templateName)
		{
			bool flag = this.m_Usings == null || this.m_Usings.Count == 0;
			VisualTreeAsset visualTreeAsset;
			if (flag)
			{
				visualTreeAsset = null;
			}
			else
			{
				int num = this.m_Usings.BinarySearch(new VisualTreeAsset.UsingEntry(templateName, string.Empty), VisualTreeAsset.UsingEntry.comparer);
				bool flag2 = num < 0;
				if (flag2)
				{
					visualTreeAsset = null;
				}
				else
				{
					bool flag3 = this.m_Usings[num].asset;
					if (flag3)
					{
						visualTreeAsset = this.m_Usings[num].asset;
					}
					else
					{
						string path = this.m_Usings[num].path;
						visualTreeAsset = Panel.LoadResource(path, typeof(VisualTreeAsset), GUIUtility.pixelsPerPoint) as VisualTreeAsset;
					}
				}
			}
			return visualTreeAsset;
		}

		internal static VisualElement Create(VisualElementAsset asset, CreationContext ctx)
		{
			List<IUxmlFactory> list;
			bool flag = !VisualElementFactoryRegistry.TryGetValue(asset.fullTypeName, out list);
			if (flag)
			{
				bool flag2 = asset.fullTypeName.StartsWith("UnityEngine.Experimental.UIElements.") || asset.fullTypeName.StartsWith("UnityEditor.Experimental.UIElements.");
				if (flag2)
				{
					string text = asset.fullTypeName.Replace(".Experimental.UIElements", ".UIElements");
					bool flag3 = !VisualElementFactoryRegistry.TryGetValue(text, out list);
					if (flag3)
					{
						Debug.LogErrorFormat("Element '{0}' has no registered factory method.", new object[] { asset.fullTypeName });
						return new Label(string.Format("Unknown type: '{0}'", asset.fullTypeName));
					}
				}
				else
				{
					bool flag4 = asset.fullTypeName == "UXML";
					if (!flag4)
					{
						Debug.LogErrorFormat("Element '{0}' has no registered factory method.", new object[] { asset.fullTypeName });
						return new Label(string.Format("Unknown type: '{0}'", asset.fullTypeName));
					}
					VisualElementFactoryRegistry.TryGetValue(typeof(UxmlRootElementFactory).Namespace + "." + asset.fullTypeName, out list);
				}
			}
			IUxmlFactory uxmlFactory = null;
			foreach (IUxmlFactory uxmlFactory2 in list)
			{
				bool flag5 = uxmlFactory2.AcceptsAttributeBag(asset, ctx);
				if (flag5)
				{
					uxmlFactory = uxmlFactory2;
					break;
				}
			}
			bool flag6 = uxmlFactory == null;
			VisualElement visualElement;
			if (flag6)
			{
				Debug.LogErrorFormat("Element '{0}' has a no factory that accept the set of XML attributes specified.", new object[] { asset.fullTypeName });
				visualElement = new Label(string.Format("Type with no factory: '{0}'", asset.fullTypeName));
			}
			else
			{
				VisualElement visualElement2 = uxmlFactory.Create(asset, ctx);
				bool flag7 = visualElement2 != null;
				if (flag7)
				{
					VisualTreeAsset.AssignClassListFromAssetToElement(asset, visualElement2);
					VisualTreeAsset.AssignStyleSheetFromAssetToElement(asset, visualElement2);
				}
				visualElement = visualElement2;
			}
			return visualElement;
		}

		private static void AssignClassListFromAssetToElement(VisualElementAsset asset, VisualElement element)
		{
			bool flag = asset.classes != null;
			if (flag)
			{
				for (int i = 0; i < asset.classes.Length; i++)
				{
					element.AddToClassList(asset.classes[i]);
				}
			}
		}

		private static void AssignStyleSheetFromAssetToElement(VisualElementAsset asset, VisualElement element)
		{
			bool hasStylesheetPaths = asset.hasStylesheetPaths;
			if (hasStylesheetPaths)
			{
				for (int i = 0; i < asset.stylesheetPaths.Count; i++)
				{
					element.AddStyleSheetPath(asset.stylesheetPaths[i]);
				}
			}
			bool hasStylesheets = asset.hasStylesheets;
			if (hasStylesheets)
			{
				for (int j = 0; j < asset.stylesheets.Count; j++)
				{
					bool flag = asset.stylesheets[j] != null;
					if (flag)
					{
						element.styleSheets.Add(asset.stylesheets[j]);
					}
				}
			}
		}

		public int contentHash
		{
			get
			{
				return this.m_ContentHash;
			}
			set
			{
				this.m_ContentHash = value;
			}
		}

		private static readonly Dictionary<string, VisualElement> s_TemporarySlotInsertionPoints = new Dictionary<string, VisualElement>();

		[SerializeField]
		private List<VisualTreeAsset.UsingEntry> m_Usings;

		[SerializeField]
		internal StyleSheet inlineSheet;

		[SerializeField]
		private List<VisualElementAsset> m_VisualElementAssets;

		[SerializeField]
		private List<TemplateAsset> m_TemplateAssets;

		[SerializeField]
		private List<VisualTreeAsset.SlotDefinition> m_Slots;

		[SerializeField]
		private int m_ContentContainerId;

		[SerializeField]
		private int m_ContentHash;

		[Serializable]
		internal struct UsingEntry
		{
			public UsingEntry(string alias, string path)
			{
				this.alias = alias;
				this.path = path;
				this.asset = null;
			}

			public UsingEntry(string alias, VisualTreeAsset asset)
			{
				this.alias = alias;
				this.path = null;
				this.asset = asset;
			}

			internal static readonly IComparer<VisualTreeAsset.UsingEntry> comparer = new VisualTreeAsset.UsingEntryComparer();

			[SerializeField]
			public string alias;

			[SerializeField]
			public string path;

			[SerializeField]
			public VisualTreeAsset asset;
		}

		private class UsingEntryComparer : IComparer<VisualTreeAsset.UsingEntry>
		{
			public int Compare(VisualTreeAsset.UsingEntry x, VisualTreeAsset.UsingEntry y)
			{
				return string.CompareOrdinal(x.alias, y.alias);
			}
		}

		[Serializable]
		internal struct SlotDefinition
		{
			[SerializeField]
			public string name;

			[SerializeField]
			public int insertionPointId;
		}

		[Serializable]
		internal struct SlotUsageEntry
		{
			public SlotUsageEntry(string slotName, int assetId)
			{
				this.slotName = slotName;
				this.assetId = assetId;
			}

			[SerializeField]
			public string slotName;

			[SerializeField]
			public int assetId;
		}
	}
}
