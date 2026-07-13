using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	[HelpURL("UIE-VisualTree-landing")]
	[Serializable]
	public class VisualTreeAsset : ScriptableObject
	{
		public bool importedWithErrors
		{
			get
			{
				return this.m_ImportedWithErrors;
			}
			internal set
			{
				this.m_ImportedWithErrors = value;
			}
		}

		internal bool hasEditorElements
		{
			get
			{
				return this.m_HasEditorElements;
			}
			set
			{
				this.m_HasEditorElements = value;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal bool importerWithUpdatedUrls
		{
			get
			{
				return this.m_HasUpdatedUrls;
			}
			set
			{
				this.m_HasUpdatedUrls = value;
			}
		}

		public bool importedWithWarnings
		{
			get
			{
				return this.m_ImportedWithWarnings;
			}
			internal set
			{
				this.m_ImportedWithWarnings = value;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal int GetNextChildSerialNumber()
		{
			return this.DepthFirstTraversal().GetCount();
		}

		internal List<VisualTreeAsset.UsingEntry> usings
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				return this.m_Usings;
			}
		}

		public IEnumerable<VisualTreeAsset> templateDependencies
		{
			get
			{
				bool flag = this.m_Usings.Count == 0;
				if (flag)
				{
					yield break;
				}
				HashSet<VisualTreeAsset> sent = new HashSet<VisualTreeAsset>();
				foreach (VisualTreeAsset.UsingEntry entry in this.m_Usings)
				{
					bool flag2 = entry.asset != null && !sent.Contains(entry.asset);
					if (flag2)
					{
						sent.Add(entry.asset);
						yield return entry.asset;
					}
					else
					{
						bool flag3 = !string.IsNullOrEmpty(entry.path);
						if (flag3)
						{
							VisualTreeAsset vta = Panel.LoadResource(entry.path, typeof(VisualTreeAsset), 1f) as VisualTreeAsset;
							bool flag4 = vta != null && !sent.Contains(entry.asset);
							if (flag4)
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

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal StyleSheet GetOrCreateInlineStyleSheet()
		{
			bool flag = this.inlineSheet == null;
			if (flag)
			{
				this.inlineSheet = StyleSheetUtility.CreateInstanceWithHideFlags();
			}
			return this.inlineSheet;
		}

		internal VisualElementAsset visualTreeNoAlloc
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				return this.m_VisualTree;
			}
		}

		internal VisualElementAsset visualTree
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				bool flag = this.m_VisualTree != null;
				VisualElementAsset visualElementAsset;
				if (flag)
				{
					visualElementAsset = this.m_VisualTree;
				}
				else
				{
					VisualElementAsset visualElementAsset2 = new VisualElementAsset("UnityEngine.UIElements.UXML", default(UxmlNamespaceDefinition));
					this.SetRootAsset(visualElementAsset2);
					visualElementAsset = visualElementAsset2;
				}
				return visualElementAsset;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void SetRootAsset(VisualElementAsset root)
		{
			bool flag = this.m_VisualTree != null;
			if (flag)
			{
				throw new InvalidOperationException("Trying to set a root asset, but it already exists");
			}
			this.m_VisualTree = root;
			root.SetVisualTreeAsset(this);
		}

		public IEnumerable<StyleSheet> stylesheets
		{
			get
			{
				VisualTreeAsset.<get_stylesheets>d__40 <get_stylesheets>d__ = new VisualTreeAsset.<get_stylesheets>d__40(-2);
				<get_stylesheets>d__.<>4__this = this;
				return <get_stylesheets>d__;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal UxmlObjectAsset AddUxmlObject(UxmlAsset parent, string fieldUxmlName, string fullTypeName, UxmlNamespaceDefinition xmlNamespace = default(UxmlNamespaceDefinition))
		{
			bool flag = string.IsNullOrEmpty(fieldUxmlName);
			UxmlObjectAsset uxmlObjectAsset2;
			if (flag)
			{
				UxmlObjectAsset uxmlObjectAsset = new UxmlObjectAsset(fullTypeName, false, xmlNamespace)
				{
					parentId = parent.id,
					id = this.GetNextUxmlAssetId(parent.id)
				};
				parent.Add(uxmlObjectAsset);
				uxmlObjectAsset2 = uxmlObjectAsset;
			}
			else
			{
				UxmlObjectAsset uxmlObjectAsset3 = parent.GetField(fieldUxmlName);
				bool flag2 = uxmlObjectAsset3 == null;
				if (flag2)
				{
					uxmlObjectAsset3 = new UxmlObjectAsset(fieldUxmlName, true, xmlNamespace);
					parent.Add(uxmlObjectAsset3);
					uxmlObjectAsset3.parentId = parent.id;
					UxmlAsset uxmlAsset = uxmlObjectAsset3;
					UxmlAsset parentAsset = parent.parentAsset;
					uxmlAsset.id = this.GetNextUxmlAssetId((parentAsset != null) ? parentAsset.id : 0);
				}
				uxmlObjectAsset2 = this.AddUxmlObject(uxmlObjectAsset3, null, fullTypeName, xmlNamespace);
			}
			return uxmlObjectAsset2;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal int GetNextUxmlAssetId(int parentId)
		{
			int hashCode = Guid.NewGuid().GetHashCode();
			return (this.GetNextChildSerialNumber() + 585386304) * -1521134295 + parentId + hashCode;
		}

		private void Awake__Internal()
		{
			this.SetupReferences();
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void SetupReferences()
		{
			foreach (UxmlAsset uxmlAsset in this.DepthFirstTraversal())
			{
				uxmlAsset.SetVisualTreeAssetWithOutNotify(this);
			}
		}

		internal List<T> GetUxmlObjects<T>(IUxmlAttributes asset, CreationContext cc) where T : new()
		{
			UxmlAsset uxmlAsset = asset as UxmlAsset;
			bool flag = uxmlAsset != null;
			if (flag)
			{
				List<UxmlObjectAsset> list;
				using (CollectionPool<List<UxmlObjectAsset>, UxmlObjectAsset>.Get(out list))
				{
					uxmlAsset.GetChildrenUxmlObjectAssets(list);
					bool flag2 = list != null;
					if (flag2)
					{
						List<T> list2 = null;
						foreach (UxmlObjectAsset uxmlObjectAsset in list)
						{
							IBaseUxmlObjectFactory uxmlObjectFactory = this.GetUxmlObjectFactory(uxmlObjectAsset);
							IUxmlObjectFactory<T> uxmlObjectFactory2 = uxmlObjectFactory as IUxmlObjectFactory<T>;
							bool flag3 = uxmlObjectFactory2 == null;
							if (!flag3)
							{
								T t = uxmlObjectFactory2.CreateObject(uxmlObjectAsset, cc);
								bool flag4 = list2 == null;
								if (flag4)
								{
									list2 = new List<T> { t };
								}
								else
								{
									list2.Add(t);
								}
							}
						}
						return list2;
					}
				}
			}
			return null;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal bool AssetEntryExists(string path, Type type)
		{
			foreach (VisualTreeAsset.AssetEntry assetEntry in this.m_AssetEntries)
			{
				bool flag = assetEntry.path == path && assetEntry.type == type;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void RegisterAssetEntry(string path, Type type, Object asset)
		{
			this.m_AssetEntries.Add(new VisualTreeAsset.AssetEntry(path, type, asset));
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void TransferAssetEntries(VisualTreeAsset otherVta)
		{
			this.m_AssetEntries.Clear();
			this.m_AssetEntries.AddRange(otherVta.m_AssetEntries);
		}

		internal T GetAsset<T>(string path) where T : Object
		{
			return this.GetAsset(path, typeof(T)) as T;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal Object GetAsset(string path, Type type)
		{
			foreach (VisualTreeAsset.AssetEntry assetEntry in this.m_AssetEntries)
			{
				bool flag = assetEntry.path == path && type.IsAssignableFrom(assetEntry.type);
				if (flag)
				{
					return assetEntry.asset;
				}
			}
			return null;
		}

		internal Type GetAssetType(string path)
		{
			foreach (VisualTreeAsset.AssetEntry assetEntry in this.m_AssetEntries)
			{
				bool flag = assetEntry.path == path;
				if (flag)
				{
					return assetEntry.type;
				}
			}
			return null;
		}

		internal IBaseUxmlObjectFactory GetUxmlObjectFactory(UxmlObjectAsset uxmlObjectAsset)
		{
			List<IBaseUxmlObjectFactory> list;
			bool flag = !UxmlObjectFactoryRegistry.factories.TryGetValue(uxmlObjectAsset.fullTypeName, out list);
			IBaseUxmlObjectFactory baseUxmlObjectFactory;
			if (flag)
			{
				Debug.LogErrorFormat("Element '{0}' has no registered factory method.", new object[] { uxmlObjectAsset.fullTypeName });
				baseUxmlObjectFactory = null;
			}
			else
			{
				IBaseUxmlObjectFactory baseUxmlObjectFactory2 = null;
				CreationContext creationContext = new CreationContext(this);
				foreach (IBaseUxmlObjectFactory baseUxmlObjectFactory3 in list)
				{
					bool flag2 = baseUxmlObjectFactory3.AcceptsAttributeBag(uxmlObjectAsset, creationContext);
					if (flag2)
					{
						baseUxmlObjectFactory2 = baseUxmlObjectFactory3;
						break;
					}
				}
				bool flag3 = baseUxmlObjectFactory2 == null;
				if (flag3)
				{
					Debug.LogErrorFormat("Element '{0}' has a no factory that accept the set of XML attributes specified.", new object[] { uxmlObjectAsset.fullTypeName });
					baseUxmlObjectFactory = null;
				}
				else
				{
					baseUxmlObjectFactory = baseUxmlObjectFactory2;
				}
			}
			return baseUxmlObjectFactory;
		}

		internal List<VisualTreeAsset.SlotDefinition> slots
		{
			get
			{
				return this.m_Slots;
			}
		}

		internal int contentContainerId
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
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
			TemplateContainer templateContainer = new TemplateContainer(base.name, this);
			try
			{
				CreationContext creationContext = new CreationContext(VisualTreeAsset.s_TemporarySlotInsertionPoints, null, null, null, null, VisualTreeAsset.s_VeaIdsPath, null, null);
				this.CloneTree(templateContainer, creationContext);
			}
			finally
			{
				VisualTreeAsset.s_TemporarySlotInsertionPoints.Clear();
				VisualTreeAsset.s_VeaIdsPath.Clear();
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
				CreationContext creationContext = new CreationContext(VisualTreeAsset.s_TemporarySlotInsertionPoints, null, null, null, null, VisualTreeAsset.s_VeaIdsPath, null, null);
				this.CloneTree(target, creationContext);
			}
			finally
			{
				elementAddedCount = target.childCount - firstElementIndex;
				VisualTreeAsset.s_TemporarySlotInsertionPoints.Clear();
				VisualTreeAsset.s_VeaIdsPath.Clear();
			}
		}

		internal void CloneTree(VisualElement target, CreationContext cc)
		{
			bool flag = target == null;
			if (flag)
			{
				throw new ArgumentNullException("target");
			}
			bool flag2 = this.m_VisualTree == null;
			if (!flag2)
			{
				VisualElementAsset visualTree = this.m_VisualTree;
				VisualTreeAsset.AssignClassListFromAssetToElement(visualTree, target);
				VisualTreeAsset.AssignStyleSheetFromAssetToElement(visualTree, target);
				for (int i = 0; i < visualTree.childCount; i++)
				{
					VisualElementAsset visualElementAsset = visualTree[i] as VisualElementAsset;
					bool flag3 = false;
					bool flag4 = visualElementAsset is TemplateAsset;
					if (flag4)
					{
						cc.veaIdsPath.Add(visualElementAsset.id);
						flag3 = true;
					}
					CreationContext creationContext = new CreationContext(cc.slotInsertionPoints, cc.attributeOverrides, cc.serializedDataOverrides, this, target, cc.veaIdsPath, null, cc.templateAsset);
					VisualElement visualElement = this.CloneSetupRecursively(visualElementAsset, creationContext);
					bool flag5 = flag3;
					if (flag5)
					{
						cc.veaIdsPath.Remove(visualElementAsset.id);
					}
					bool flag6 = visualElement != null;
					if (flag6)
					{
						target.hierarchy.Add(visualElement);
					}
				}
			}
		}

		private VisualElement CloneSetupRecursively(VisualElementAsset asset, CreationContext context)
		{
			bool skipClone = asset.skipClone;
			VisualElement visualElement;
			if (skipClone)
			{
				visualElement = null;
			}
			else
			{
				VisualElement visualElement2 = VisualTreeAsset.Create(asset, context);
				bool flag = visualElement2 == null;
				if (flag)
				{
					visualElement = null;
				}
				else
				{
					visualElement2.visualTreeAssetSource = this;
					bool flag2 = asset.id == context.visualTreeAsset.contentContainerId;
					if (flag2)
					{
						TemplateContainer templateContainer = context.target as TemplateContainer;
						bool flag3 = templateContainer != null;
						if (flag3)
						{
							templateContainer.SetContentContainer(visualElement2);
						}
						else
						{
							Debug.LogError("Trying to clone a VisualTreeAsset with a custom content container into a element which is not a template container");
						}
					}
					string text;
					bool flag4 = context.slotInsertionPoints != null && this.TryGetSlotInsertionPoint(asset.id, out text);
					if (flag4)
					{
						context.slotInsertionPoints.Add(text, visualElement2);
					}
					bool flag5 = asset.ruleIndex != -1;
					if (flag5)
					{
						bool flag6 = this.inlineSheet == null;
						if (flag6)
						{
							Debug.LogWarning("VisualElementAsset has a RuleIndex but no inlineStyleSheet");
						}
						else
						{
							StyleRule styleRule = this.inlineSheet.rules[asset.ruleIndex];
							visualElement2.SetInlineRule(this.inlineSheet, styleRule);
						}
					}
					TemplateAsset templateAsset = asset as TemplateAsset;
					for (int i = 0; i < asset.childCount; i++)
					{
						VisualElementAsset childVea = asset[i] as VisualElementAsset;
						bool flag7 = childVea == null;
						if (!flag7)
						{
							bool flag8 = false;
							bool flag9 = childVea is TemplateAsset;
							if (flag9)
							{
								context.veaIdsPath.Add(childVea.id);
								flag8 = true;
							}
							VisualElement visualElement3 = this.CloneSetupRecursively(childVea, context);
							bool flag10 = flag8;
							if (flag10)
							{
								context.veaIdsPath.Remove(childVea.id);
							}
							bool flag11 = visualElement3 == null;
							if (!flag11)
							{
								int? num;
								if (templateAsset == null)
								{
									num = null;
								}
								else
								{
									List<VisualTreeAsset.SlotUsageEntry> slotUsages = templateAsset.slotUsages;
									num = ((slotUsages != null) ? new int?(slotUsages.FindIndex((VisualTreeAsset.SlotUsageEntry u) => u.assetId == childVea.id)) : null);
								}
								int num2 = num ?? (-1);
								bool flag12 = num2 != -1;
								if (flag12)
								{
									string slotName = templateAsset.slotUsages[num2].slotName;
									Assert.IsFalse(string.IsNullOrEmpty(slotName), "a lost name should not be null or empty, this probably points to an importer or serialization bug");
									VisualElement visualElement4;
									bool flag13 = context.slotInsertionPoints == null || !context.slotInsertionPoints.TryGetValue(slotName, out visualElement4);
									if (flag13)
									{
										Debug.LogErrorFormat("Slot '{0}' was not found. Existing slots: {1}", new object[]
										{
											slotName,
											(context.slotInsertionPoints == null) ? string.Empty : string.Join(", ", context.slotInsertionPoints.Keys.ToArray<string>())
										});
										visualElement2.Add(visualElement3);
									}
									else
									{
										visualElement4.Add(visualElement3);
									}
								}
								else
								{
									visualElement2.Add(visualElement3);
								}
							}
						}
					}
					bool flag14 = templateAsset != null && context.slotInsertionPoints != null;
					if (flag14)
					{
						context.slotInsertionPoints.Clear();
					}
					visualElement = visualElement2;
				}
			}
			return visualElement;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal bool TryGetSlotInsertionPoint(int insertionPointId, out string slotName)
		{
			for (int i = 0; i < this.m_Slots.Count; i++)
			{
				VisualTreeAsset.SlotDefinition slotDefinition = this.m_Slots[i];
				bool flag = slotDefinition.insertionPointId == insertionPointId;
				if (flag)
				{
					slotName = slotDefinition.name;
					return true;
				}
			}
			slotName = null;
			return false;
		}

		internal bool TryGetUsingEntry(string templateName, out VisualTreeAsset.UsingEntry entry)
		{
			entry = default(VisualTreeAsset.UsingEntry);
			bool flag = this.m_Usings.Count == 0;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				int num = this.m_Usings.BinarySearch(new VisualTreeAsset.UsingEntry(templateName, string.Empty), VisualTreeAsset.UsingEntry.comparer);
				bool flag3 = num < 0;
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					entry = this.m_Usings[num];
					flag2 = true;
				}
			}
			return flag2;
		}

		private void RemoveUsingEntry(VisualTreeAsset.UsingEntry entry)
		{
			this.m_Usings.Remove(entry);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal VisualTreeAsset ResolveTemplate(string templateName)
		{
			VisualTreeAsset.UsingEntry usingEntry;
			bool flag = !this.TryGetUsingEntry(templateName, out usingEntry);
			VisualTreeAsset visualTreeAsset;
			if (flag)
			{
				visualTreeAsset = null;
			}
			else
			{
				bool flag2 = usingEntry.asset;
				if (flag2)
				{
					visualTreeAsset = usingEntry.asset;
				}
				else
				{
					string path = usingEntry.path;
					visualTreeAsset = Panel.LoadResource(path, typeof(VisualTreeAsset), 1f) as VisualTreeAsset;
				}
			}
			return visualTreeAsset;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal bool TemplateExists(string templateName)
		{
			bool flag = this.m_Usings.Count == 0;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				int num = this.m_Usings.BinarySearch(new VisualTreeAsset.UsingEntry(templateName, string.Empty), VisualTreeAsset.UsingEntry.comparer);
				flag2 = num >= 0;
			}
			return flag2;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void RegisterTemplate(string templateName, string path)
		{
			this.InsertUsingEntry(new VisualTreeAsset.UsingEntry(templateName, path));
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void RegisterTemplate(string templateName, VisualTreeAsset asset)
		{
			this.InsertUsingEntry(new VisualTreeAsset.UsingEntry(templateName, asset));
		}

		internal bool TryRegisterTemplate(string templateName, VisualTreeAsset asset)
		{
			bool flag = !asset || asset == null;
			if (flag)
			{
				throw new ArgumentNullException("asset");
			}
			bool flag2 = this.TemplateExists(templateName);
			bool flag4;
			if (flag2)
			{
				VisualTreeAsset.UsingEntry usingEntry;
				bool flag3 = this.TryGetUsingEntry(templateName, out usingEntry) && asset == usingEntry.asset;
				if (flag3)
				{
					flag4 = false;
				}
				else
				{
					Debug.LogWarningFormat("VisualTreeAsset: could not register a template alias for asset `{0}`, alias is already defined for asset '{1}'", new object[] { asset, usingEntry.asset });
					flag4 = false;
				}
			}
			else
			{
				this.RegisterTemplate(templateName, asset);
				flag4 = true;
			}
			return flag4;
		}

		internal bool TryUnregisterTemplate(string templateName)
		{
			List<TemplateAsset> list;
			bool flag2;
			using (CollectionPool<List<TemplateAsset>, TemplateAsset>.Get(out list))
			{
				list.AddRange(this.DepthFirstTraversalOfType<TemplateAsset>());
				VisualTreeAsset.UsingEntry usingEntry;
				bool flag = !this.TryGetUsingEntry(templateName, out usingEntry);
				if (flag)
				{
					flag2 = false;
				}
				else
				{
					bool flag3 = list.Count == 0;
					if (flag3)
					{
						this.RemoveUsingEntry(usingEntry);
						flag2 = true;
					}
					else
					{
						foreach (TemplateAsset templateAsset in list)
						{
							bool flag4 = string.CompareOrdinal(templateName, templateAsset.templateAlias) == 0;
							if (flag4)
							{
								return false;
							}
						}
						this.RemoveUsingEntry(usingEntry);
						flag2 = true;
					}
				}
			}
			return flag2;
		}

		private void InsertUsingEntry(VisualTreeAsset.UsingEntry entry)
		{
			int num = 0;
			while (num < this.m_Usings.Count && string.CompareOrdinal(entry.alias, this.m_Usings[num].alias) > 0)
			{
				num++;
			}
			this.m_Usings.Insert(num, entry);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static VisualElement Create(VisualElementAsset asset, CreationContext ctx)
		{
			VisualTreeAsset.<>c__DisplayClass80_0 CS$<>8__locals1;
			CS$<>8__locals1.asset = asset;
			bool flag = CS$<>8__locals1.asset.serializedData != null;
			VisualElement visualElement;
			if (flag)
			{
				visualElement = CS$<>8__locals1.asset.Instantiate(ctx);
			}
			else
			{
				List<IUxmlFactory> list;
				bool flag2 = !VisualElementFactoryRegistry.TryGetValue(CS$<>8__locals1.asset.fullTypeName, out list);
				if (flag2)
				{
					bool flag3 = CS$<>8__locals1.asset.fullTypeName.StartsWith("UnityEngine.Experimental.UIElements.") || CS$<>8__locals1.asset.fullTypeName.StartsWith("UnityEditor.Experimental.UIElements.");
					if (flag3)
					{
						string text = CS$<>8__locals1.asset.fullTypeName.Replace(".Experimental.UIElements", ".UIElements");
						bool flag4 = !VisualElementFactoryRegistry.TryGetValue(text, out list);
						if (flag4)
						{
							return VisualTreeAsset.<Create>g__CreateError|80_0(ref CS$<>8__locals1);
						}
					}
					else
					{
						bool flag5 = CS$<>8__locals1.asset.fullTypeName == "UXML";
						if (!flag5)
						{
							return VisualTreeAsset.<Create>g__CreateError|80_0(ref CS$<>8__locals1);
						}
						VisualElementFactoryRegistry.TryGetValue(typeof(UxmlRootElementFactory).Namespace + "." + CS$<>8__locals1.asset.fullTypeName, out list);
					}
				}
				IUxmlFactory uxmlFactory = null;
				foreach (IUxmlFactory uxmlFactory2 in list)
				{
					bool flag6 = uxmlFactory2.AcceptsAttributeBag(CS$<>8__locals1.asset, ctx);
					if (flag6)
					{
						uxmlFactory = uxmlFactory2;
						break;
					}
				}
				bool flag7 = uxmlFactory == null;
				if (flag7)
				{
					Debug.LogErrorFormat("Element '{0}' has a no factory that accept the set of XML attributes specified.", new object[] { CS$<>8__locals1.asset.fullTypeName });
					visualElement = new Label(string.Format("Type with no factory: '{0}'", CS$<>8__locals1.asset.fullTypeName));
				}
				else
				{
					VisualElement visualElement2 = uxmlFactory.Create(CS$<>8__locals1.asset, ctx);
					bool flag8 = visualElement2 != null;
					if (flag8)
					{
						VisualTreeAsset.AssignClassListFromAssetToElement(CS$<>8__locals1.asset, visualElement2);
						VisualTreeAsset.AssignStyleSheetFromAssetToElement(CS$<>8__locals1.asset, visualElement2);
					}
					visualElement = visualElement2;
				}
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

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal IEnumerable<UxmlAsset> DepthFirstTraversal()
		{
			bool flag = this.m_VisualTree == null;
			IEnumerable<UxmlAsset> enumerable;
			if (flag)
			{
				enumerable = Array.Empty<UxmlAsset>();
			}
			else
			{
				enumerable = this.DepthFirstTraversal(this.m_VisualTree);
			}
			return enumerable;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal IEnumerable<T> DepthFirstTraversalOfType<T>()
		{
			IEnumerable<UxmlAsset> elements = this.DepthFirstTraversal();
			foreach (UxmlAsset element in elements)
			{
				T tElement;
				bool flag;
				if (element is T)
				{
					tElement = element as T;
					flag = true;
				}
				else
				{
					flag = false;
				}
				bool flag2 = flag;
				if (flag2)
				{
					yield return tElement;
				}
				tElement = default(T);
				element = null;
			}
			IEnumerator<UxmlAsset> enumerator = null;
			yield break;
			yield break;
		}

		internal IEnumerable<UxmlAsset> DepthFirstTraversal(UxmlAsset asset)
		{
			yield return asset;
			int num;
			for (int i = 0; i < asset.childCount; i = num)
			{
				foreach (UxmlAsset child in this.DepthFirstTraversal(asset[i]))
				{
					yield return child;
					child = null;
				}
				IEnumerator<UxmlAsset> enumerator = null;
				num = i + 1;
			}
			yield break;
			yield break;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal int DepthFirstTraversalIndexOf(UxmlAsset uxmlAsset)
		{
			int num = 0;
			IEnumerable<UxmlAsset> enumerable = this.DepthFirstTraversal();
			foreach (UxmlAsset uxmlAsset2 in enumerable)
			{
				bool flag = uxmlAsset2 == uxmlAsset;
				if (flag)
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal int GenerateNewId(VisualElementAsset vea)
		{
			bool flag = !vea.HasParent();
			int num;
			if (flag)
			{
				num = this.GetHashCode();
			}
			else
			{
				num = vea.parentAsset.id;
			}
			int hashCode = Guid.NewGuid().GetHashCode();
			return (this.GetNextChildSerialNumber() + 585386304) * -1521134295 + num + hashCode;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal VisualElementAsset AddElementToDocument(VisualElementAsset vea, VisualElementAsset parent)
		{
			VisualElementAsset visualElementAsset = parent ?? this.visualTree;
			visualElementAsset.Add(vea);
			bool flag = vea.id == 0;
			if (flag)
			{
				vea.id = this.GenerateNewId(vea);
			}
			return vea;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal VisualElementAsset ReparentElementInDocument(VisualElementAsset vea, VisualElementAsset newParent, int index = -1)
		{
			VisualElementAsset visualElementAsset = newParent ?? this.visualTree;
			int num = ((index == -1) ? visualElementAsset.childCount : index);
			visualElementAsset.Insert(num, vea);
			bool flag = vea.id == 0;
			if (flag)
			{
				vea.id = this.GenerateNewId(vea);
			}
			bool isRoot = vea.isRoot;
			if (isRoot)
			{
				vea.stylesheetPaths.Clear();
				vea.stylesheets.Clear();
			}
			return vea;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void Swallow(VisualElementAsset parent, VisualTreeAsset other)
		{
			List<UxmlAsset> list;
			using (CollectionPool<List<UxmlAsset>, UxmlAsset>.Get(out list))
			{
				list.AddRange(other.DepthFirstTraversal());
				VisualElementAsset visualElementAsset = parent ?? this.visualTree;
				list.Clear();
				for (int i = 0; i < other.visualTree.childCount; i++)
				{
					list.Add(other.visualTree[i]);
				}
				for (int j = 0; j < list.Count; j++)
				{
					UxmlAsset uxmlAsset = list[j];
					VisualElementAsset visualElementAsset2 = uxmlAsset as VisualElementAsset;
					bool flag = visualElementAsset2 != null;
					if (flag)
					{
						visualElementAsset2.id = this.GenerateNewId(visualElementAsset2);
						VisualTreeAsset.UpdateUxmlObjectAssetsParentId(visualElementAsset2);
					}
					visualElementAsset.Add(uxmlAsset);
				}
			}
		}

		private static void UpdateUxmlObjectAssetsParentId(VisualElementAsset visualElementAsset)
		{
			List<UxmlObjectAsset> list;
			using (CollectionPool<List<UxmlObjectAsset>, UxmlObjectAsset>.Get(out list))
			{
				visualElementAsset.GetChildrenUxmlObjectAssets(list);
				foreach (UxmlObjectAsset uxmlObjectAsset in list)
				{
					uxmlObjectAsset.parentId = visualElementAsset.id;
				}
			}
		}

		internal static void SwallowStyleRule(VisualTreeAsset previous, VisualTreeAsset next, VisualElementAsset vea)
		{
			bool flag = vea.ruleIndex < 0;
			if (!flag)
			{
				StyleSheet orCreateInlineStyleSheet = next.GetOrCreateInlineStyleSheet();
				StyleSheet styleSheet = previous.inlineSheet;
				StyleRule styleRule = styleSheet.rules[vea.ruleIndex];
				int num = orCreateInlineStyleSheet.rules.Length;
				StyleRule styleRule2 = orCreateInlineStyleSheet.AddRule();
				styleRule2.customPropertiesCount = styleRule.customPropertiesCount;
				for (int i = 0; i < styleRule.properties.Length; i++)
				{
					StyleProperty styleProperty = styleRule.properties[i];
					StyleProperty styleProperty2 = styleRule2.AddProperty(styleProperty.name);
					styleProperty2.requireVariableResolve = styleProperty.requireVariableResolve;
					StyleSheetUtility.TransferStylePropertyHandles(styleSheet, styleProperty, orCreateInlineStyleSheet, styleProperty2);
				}
				vea.ruleIndex = num;
				orCreateInlineStyleSheet.RequestRebuild(StyleSheet.RebuildOptions.None);
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal VisualElementAsset AddElementOfType(VisualElementAsset parent, string fullTypeName)
		{
			UxmlNamespaceDefinition uxmlNamespaceDefinition = this.FindUxmlNamespaceDefinitionForTypeName(parent, fullTypeName);
			VisualElementAsset visualElementAsset = new VisualElementAsset(fullTypeName, uxmlNamespaceDefinition);
			return this.AddElementToDocument(visualElementAsset, parent);
		}

		[CompilerGenerated]
		internal static VisualElement <Create>g__CreateError|80_0(ref VisualTreeAsset.<>c__DisplayClass80_0 A_0)
		{
			Debug.LogErrorFormat(VisualTreeAsset.NoRegisteredFactoryErrorMessage, new object[] { A_0.asset.fullTypeName });
			return new Label(string.Format("Unknown type: '{0}'", A_0.asset.fullTypeName));
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static string NoRegisteredFactoryErrorMessage = "Element '{0}' is missing a UxmlElementAttribute and has no registered factory method. Please ensure that you have the correct namespace imported.";

		internal const string TemplateAliasExistsError = "VisualTreeAsset: could not register a template alias for asset `{0}`, alias is already defined for asset '{1}'";

		[SerializeField]
		private bool m_ImportedWithErrors;

		[SerializeField]
		private bool m_HasEditorElements;

		[SerializeField]
		private bool m_HasUpdatedUrls;

		[SerializeField]
		private bool m_ImportedWithWarnings;

		private static readonly Dictionary<string, VisualElement> s_TemporarySlotInsertionPoints = new Dictionary<string, VisualElement>();

		private static readonly List<int> s_VeaIdsPath = new List<int>();

		[SerializeField]
		private List<VisualTreeAsset.UsingEntry> m_Usings = new List<VisualTreeAsset.UsingEntry>();

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		[SerializeField]
		internal StyleSheet inlineSheet;

		[SerializeReference]
		private VisualElementAsset m_VisualTree;

		[SerializeField]
		private List<VisualTreeAsset.AssetEntry> m_AssetEntries = new List<VisualTreeAsset.AssetEntry>();

		[SerializeField]
		private List<VisualTreeAsset.SlotDefinition> m_Slots = new List<VisualTreeAsset.SlotDefinition>();

		[SerializeField]
		private int m_ContentContainerId;

		[SerializeField]
		private int m_ContentHash;

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
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

			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
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

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
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

		[Serializable]
		private struct AssetEntry
		{
			public Type type
			{
				get
				{
					Type type;
					if ((type = this.m_CachedType) == null)
					{
						type = (this.m_CachedType = Type.GetType(this.m_TypeFullName));
					}
					return type;
				}
			}

			public string path
			{
				get
				{
					return this.m_Path;
				}
			}

			public Object asset
			{
				get
				{
					bool isSet = this.m_AssetReference.isSet;
					Object @object;
					if (isSet)
					{
						@object = this.m_AssetReference.asset;
					}
					else
					{
						@object = null;
					}
					return @object;
				}
			}

			public AssetEntry(string path, Type type, Object asset)
			{
				this.m_Path = path;
				this.m_TypeFullName = type.AssemblyQualifiedName;
				this.m_CachedType = type;
				this.m_AssetReference = asset;
				this.m_InstanceID = ((asset != null) ? asset.GetInstanceID() : 0);
			}

			[SerializeField]
			private string m_Path;

			[SerializeField]
			private string m_TypeFullName;

			[SerializeField]
			private LazyLoadReference<Object> m_AssetReference;

			[SerializeField]
			private int m_InstanceID;

			private Type m_CachedType;
		}
	}
}
