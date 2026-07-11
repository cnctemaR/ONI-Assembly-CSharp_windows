using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.Experimental.UIElements.StyleSheets;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements
{
	[Serializable]
	public class VisualTreeAsset : ScriptableObject
	{
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

		public VisualElement CloneTree(Dictionary<string, VisualElement> slotInsertionPoints)
		{
			TemplateContainer templateContainer = new TemplateContainer(base.name);
			this.CloneTree(templateContainer, slotInsertionPoints ?? new Dictionary<string, VisualElement>());
			return templateContainer;
		}

		public VisualElement CloneTree(Dictionary<string, VisualElement> slotInsertionPoints, string bindingPath)
		{
			TemplateContainer templateContainer = this.CloneTree(slotInsertionPoints) as TemplateContainer;
			templateContainer.bindingPath = bindingPath;
			return templateContainer;
		}

		public void CloneTree(VisualElement target, Dictionary<string, VisualElement> slotInsertionPoints)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target", "Cannot clone a Visual Tree in a null target");
			}
			if ((this.m_VisualElementAssets != null && this.m_VisualElementAssets.Count > 0) || (this.m_TemplateAssets != null && this.m_TemplateAssets.Count > 0))
			{
				Dictionary<int, List<VisualElementAsset>> dictionary = new Dictionary<int, List<VisualElementAsset>>();
				int num = ((this.m_VisualElementAssets != null) ? this.m_VisualElementAssets.Count : 0);
				int num2 = ((this.m_TemplateAssets != null) ? this.m_TemplateAssets.Count : 0);
				for (int i = 0; i < num + num2; i++)
				{
					VisualElementAsset visualElementAsset = ((i >= num) ? this.m_TemplateAssets[i - num] : this.m_VisualElementAssets[i]);
					List<VisualElementAsset> list;
					if (!dictionary.TryGetValue(visualElementAsset.parentId, out list))
					{
						list = new List<VisualElementAsset>();
						dictionary.Add(visualElementAsset.parentId, list);
					}
					list.Add(visualElementAsset);
				}
				List<VisualElementAsset> list2;
				if (dictionary.TryGetValue(0, out list2) && list2 != null)
				{
					foreach (VisualElementAsset visualElementAsset2 in list2)
					{
						Assert.IsNotNull<VisualElementAsset>(visualElementAsset2);
						VisualElement visualElement = this.CloneSetupRecursively(visualElementAsset2, dictionary, new CreationContext(slotInsertionPoints, this, target));
						target.shadow.Add(visualElement);
					}
				}
			}
		}

		private VisualElement CloneSetupRecursively(VisualElementAsset root, Dictionary<int, List<VisualElementAsset>> idToChildren, CreationContext context)
		{
			VisualElement visualElement = root.Create(context);
			if (root.id == context.visualTreeAsset.contentContainerId)
			{
				if (context.target is TemplateContainer)
				{
					((TemplateContainer)context.target).SetContentContainer(visualElement);
				}
				else
				{
					Debug.LogError("Trying to clone a VisualTreeAsset with a custom content container into a element which is not a template container");
				}
			}
			string text;
			if (context.slotInsertionPoints != null && this.TryGetSlotInsertionPoint(root.id, out text))
			{
				context.slotInsertionPoints.Add(text, visualElement);
			}
			if (root.classes != null)
			{
				for (int i = 0; i < root.classes.Length; i++)
				{
					visualElement.AddToClassList(root.classes[i]);
				}
			}
			if (root.ruleIndex != -1)
			{
				if (this.inlineSheet == null)
				{
					Debug.LogWarning("VisualElementAsset has a RuleIndex but no inlineStyleSheet");
				}
				else
				{
					StyleRule styleRule = this.inlineSheet.rules[root.ruleIndex];
					VisualElementStylesData visualElementStylesData = new VisualElementStylesData(false);
					visualElement.SetInlineStyles(visualElementStylesData);
					visualElementStylesData.ApplyRule(this.inlineSheet, int.MaxValue, styleRule, StyleSheetCache.GetPropertyIDs(this.inlineSheet, root.ruleIndex));
				}
			}
			TemplateAsset templateAsset = root as TemplateAsset;
			List<VisualElementAsset> list;
			if (idToChildren.TryGetValue(root.id, out list))
			{
				using (List<VisualElementAsset>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						VisualElementAsset childVea = enumerator.Current;
						VisualElement visualElement2 = this.CloneSetupRecursively(childVea, idToChildren, context);
						if (visualElement2 != null)
						{
							if (templateAsset == null)
							{
								visualElement.Add(visualElement2);
							}
							else
							{
								int num = ((templateAsset.slotUsages != null) ? templateAsset.slotUsages.FindIndex((VisualTreeAsset.SlotUsageEntry u) => u.assetId == childVea.id) : (-1));
								if (num != -1)
								{
									string slotName = templateAsset.slotUsages[num].slotName;
									Assert.IsFalse(string.IsNullOrEmpty(slotName), "a lost name should not be null or empty, this probably points to an importer or serialization bug");
									VisualElement visualElement3;
									if (context.slotInsertionPoints == null || !context.slotInsertionPoints.TryGetValue(slotName, out visualElement3))
									{
										Debug.LogErrorFormat("Slot '{0}' was not found. Existing slots: {1}", new object[]
										{
											slotName,
											(context.slotInsertionPoints != null) ? string.Join(", ", context.slotInsertionPoints.Keys.ToArray<string>()) : string.Empty
										});
										visualElement.Add(visualElement2);
									}
									else
									{
										visualElement3.Add(visualElement2);
									}
								}
								else
								{
									visualElement.Add(visualElement2);
								}
							}
						}
					}
				}
			}
			if (templateAsset != null && context.slotInsertionPoints != null)
			{
				context.slotInsertionPoints.Clear();
			}
			return visualElement;
		}

		internal bool TryGetSlotInsertionPoint(int insertionPointId, out string slotName)
		{
			bool flag;
			if (this.m_Slots == null)
			{
				slotName = null;
				flag = false;
			}
			else
			{
				for (int i = 0; i < this.m_Slots.Count; i++)
				{
					VisualTreeAsset.SlotDefinition slotDefinition = this.m_Slots[i];
					if (slotDefinition.insertionPointId == insertionPointId)
					{
						slotName = slotDefinition.name;
						return true;
					}
				}
				slotName = null;
				flag = false;
			}
			return flag;
		}

		internal VisualTreeAsset ResolveTemplate(string templateName)
		{
			VisualTreeAsset visualTreeAsset;
			if (this.m_Usings == null || this.m_Usings.Count == 0)
			{
				visualTreeAsset = null;
			}
			else
			{
				int num = this.m_Usings.BinarySearch(new VisualTreeAsset.UsingEntry(templateName, null), VisualTreeAsset.UsingEntry.comparer);
				if (num < 0)
				{
					visualTreeAsset = null;
				}
				else
				{
					string path = this.m_Usings[num].path;
					visualTreeAsset = ((Panel.loadResourceFunc != null) ? (Panel.loadResourceFunc(path, typeof(VisualTreeAsset)) as VisualTreeAsset) : null);
				}
			}
			return visualTreeAsset;
		}

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

		[Serializable]
		internal struct UsingEntry
		{
			public UsingEntry(string alias, string path)
			{
				this.alias = alias;
				this.path = path;
			}

			internal static readonly IComparer<VisualTreeAsset.UsingEntry> comparer = new VisualTreeAsset.UsingEntryComparer();

			[SerializeField]
			public string alias;

			[SerializeField]
			public string path;
		}

		private class UsingEntryComparer : IComparer<VisualTreeAsset.UsingEntry>
		{
			public int Compare(VisualTreeAsset.UsingEntry x, VisualTreeAsset.UsingEntry y)
			{
				return Comparer<string>.Default.Compare(x.alias, y.alias);
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
