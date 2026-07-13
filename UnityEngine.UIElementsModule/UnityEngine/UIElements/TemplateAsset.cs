using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule", "UnityEditor.UIToolkitAuthoringModule" })]
	[Serializable]
	internal class TemplateAsset : VisualElementAsset
	{
		public string templateAlias
		{
			get
			{
				return this.m_TemplateAlias;
			}
			set
			{
				this.m_TemplateAlias = value;
			}
		}

		public List<TemplateAsset.AttributeOverride> attributeOverrides
		{
			get
			{
				return this.m_AttributeOverrides;
			}
			set
			{
				this.m_AttributeOverrides = value;
			}
		}

		public bool hasAttributeOverride
		{
			get
			{
				List<TemplateAsset.AttributeOverride> attributeOverrides = this.m_AttributeOverrides;
				return attributeOverrides != null && attributeOverrides.Count > 0;
			}
		}

		public List<TemplateAsset.UxmlSerializedDataOverride> serializedDataOverrides
		{
			get
			{
				return this.m_SerializedDataOverride;
			}
			set
			{
				this.m_SerializedDataOverride = value;
			}
		}

		internal override VisualElement Instantiate(CreationContext cc)
		{
			TemplateContainer templateContainer = (TemplateContainer)base.Instantiate(cc);
			bool flag = templateContainer.templateSource == null;
			if (flag)
			{
				TemplateContainer templateContainer2 = templateContainer;
				VisualTreeAsset visualTreeAsset = cc.visualTreeAsset;
				templateContainer2.templateSource = ((visualTreeAsset != null) ? visualTreeAsset.ResolveTemplate(templateContainer.templateId) : null);
				bool flag2 = templateContainer.templateSource == null;
				if (flag2)
				{
					templateContainer.Add(new Label("Unknown Template: '" + templateContainer.templateId + "'"));
					return templateContainer;
				}
			}
			List<CreationContext.AttributeOverrideRange> list;
			VisualElement visualElement;
			using (CollectionPool<List<CreationContext.AttributeOverrideRange>, CreationContext.AttributeOverrideRange>.Get(out list))
			{
				List<CreationContext.SerializedDataOverrideRange> list2;
				using (CollectionPool<List<CreationContext.SerializedDataOverrideRange>, CreationContext.SerializedDataOverrideRange>.Get(out list2))
				{
					bool flag3 = cc.attributeOverrides != null;
					if (flag3)
					{
						list.AddRange(cc.attributeOverrides);
					}
					bool flag4 = this.attributeOverrides.Count > 0;
					if (flag4)
					{
						list.Add(new CreationContext.AttributeOverrideRange(cc.visualTreeAsset, this.attributeOverrides));
					}
					bool flag5 = cc.serializedDataOverrides != null;
					if (flag5)
					{
						list2.AddRange(cc.serializedDataOverrides);
					}
					bool flag6 = this.serializedDataOverrides.Count > 0;
					if (flag6)
					{
						list2.Add(new CreationContext.SerializedDataOverrideRange(cc.visualTreeAsset, this.serializedDataOverrides, base.id));
					}
					List<int> list3 = ((cc.veaIdsPath != null) ? new List<int>(cc.veaIdsPath) : new List<int>());
					CreationContext creationContext = new CreationContext(cc.slotInsertionPoints, list, list2, null, null, list3, null, this);
					templateContainer.templateSource.CloneTree(templateContainer, creationContext);
					visualElement = templateContainer;
				}
			}
			return visualElement;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal List<VisualTreeAsset.SlotUsageEntry> slotUsages
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				return this.m_SlotUsages;
			}
			set
			{
				this.m_SlotUsages = value;
			}
		}

		public TemplateAsset(string templateAlias, UxmlNamespaceDefinition xmlNamespace = default(UxmlNamespaceDefinition))
			: base(TemplateAsset.UxmlInstanceTypeName, xmlNamespace)
		{
			Assert.IsFalse(string.IsNullOrEmpty(templateAlias), "Template alias must not be null or empty");
			this.m_TemplateAlias = templateAlias;
		}

		public void AddSlotUsage(string slotName, int resId)
		{
			bool flag = this.m_SlotUsages == null;
			if (flag)
			{
				this.m_SlotUsages = new List<VisualTreeAsset.SlotUsageEntry>();
			}
			this.m_SlotUsages.Add(new VisualTreeAsset.SlotUsageEntry(slotName, resId));
		}

		public void SetAttributeOverride(string attributeName, string value, string[] pathToTemplateAsset)
		{
			bool flag = pathToTemplateAsset == null;
			if (flag)
			{
				Debug.LogError("Cannot set attribute override without a path to the template asset.");
			}
			else
			{
				string text = string.Join(" ", pathToTemplateAsset);
				for (int i = 0; i < this.attributeOverrides.Count; i++)
				{
					TemplateAsset.AttributeOverride attributeOverride = this.attributeOverrides[i];
					bool flag2 = attributeOverride.NamesPathMatchesElementNamesPath(pathToTemplateAsset) && attributeOverride.m_AttributeName == attributeName;
					if (flag2)
					{
						bool flag3 = attributeOverride.m_ElementName != text;
						if (!flag3)
						{
							attributeOverride.m_ElementName = text;
							attributeOverride.m_AttributeName = attributeName;
							attributeOverride.m_Value = value;
							this.attributeOverrides[i] = attributeOverride;
							return;
						}
					}
				}
				TemplateAsset.AttributeOverride attributeOverride2 = new TemplateAsset.AttributeOverride
				{
					m_ElementName = text,
					m_NamesPath = pathToTemplateAsset,
					m_AttributeName = attributeName,
					m_Value = value
				};
				this.attributeOverrides.Add(attributeOverride2);
			}
		}

		public void RemoveAttributeOverride(string attributeName, string[] pathToTemplateAsset)
		{
			for (int i = 0; i < this.attributeOverrides.Count; i++)
			{
				TemplateAsset.AttributeOverride attributeOverride = this.attributeOverrides[i];
				bool flag = attributeOverride.NamesPathMatchesElementNamesPath(pathToTemplateAsset) && attributeOverride.m_AttributeName == attributeName;
				if (flag)
				{
					this.attributeOverrides.RemoveAt(i);
					break;
				}
			}
		}

		private protected override void OnVisualTreeAssetChanged(VisualTreeAsset previousVta, VisualTreeAsset newVta)
		{
			base.OnVisualTreeAssetChanged(previousVta, newVta);
			VisualTreeAsset visualTreeAsset = ((previousVta != null) ? previousVta.ResolveTemplate(this.templateAlias) : null);
			if (previousVta != null)
			{
				previousVta.TryUnregisterTemplate(this.templateAlias);
			}
			bool flag = !newVta || newVta == null;
			if (!flag)
			{
				bool flag2 = newVta.TemplateExists(this.templateAlias);
				bool flag3 = flag2 && newVta.ResolveTemplate(this.templateAlias) != visualTreeAsset;
				if (flag3)
				{
					bool flag4 = previousVta;
					if (flag4)
					{
						Debug.LogWarning("TemplateAsset previously linked to a different VisualTreeAsset.");
					}
				}
				else
				{
					bool flag5 = !visualTreeAsset || visualTreeAsset == null;
					if (flag5)
					{
						bool flag6 = !flag2;
						if (flag6)
						{
							Debug.LogError("TemplateAsset previously had a template registration that was lost.");
						}
					}
					else
					{
						newVta.TryRegisterTemplate(this.templateAlias, visualTreeAsset);
					}
				}
			}
		}

		public static readonly string UxmlInstanceTypeName = "UnityEngine.UIElements.Instance";

		internal const string k_AttributeOverrideElementNameAttributeName = "element-name";

		internal const string k_DifferentTemplateWarning = "TemplateAsset previously linked to a different VisualTreeAsset.";

		internal const string k_LostTemplateError = "TemplateAsset previously had a template registration that was lost.";

		[SerializeField]
		private string m_TemplateAlias;

		[SerializeField]
		private List<TemplateAsset.AttributeOverride> m_AttributeOverrides = new List<TemplateAsset.AttributeOverride>();

		[SerializeField]
		private List<TemplateAsset.UxmlSerializedDataOverride> m_SerializedDataOverride = new List<TemplateAsset.UxmlSerializedDataOverride>();

		[SerializeField]
		private List<VisualTreeAsset.SlotUsageEntry> m_SlotUsages;

		[Serializable]
		public struct AttributeOverride
		{
			public bool NamesPathMatchesElementNamesPath(IList<string> elementNamesPath)
			{
				bool flag = elementNamesPath == null || this.m_NamesPath == null || elementNamesPath.Count == 0 || this.m_NamesPath.Length == 0;
				bool flag2;
				if (flag)
				{
					flag2 = false;
				}
				else
				{
					bool flag3 = this.m_NamesPath.Length == 1;
					if (flag3)
					{
						flag2 = this.m_NamesPath[0] == elementNamesPath[elementNamesPath.Count - 1];
					}
					else
					{
						bool flag4 = this.m_NamesPath.Length != elementNamesPath.Count;
						if (flag4)
						{
							flag2 = false;
						}
						else
						{
							for (int i = elementNamesPath.Count - 1; i >= 0; i--)
							{
								bool flag5 = elementNamesPath[i] != this.m_NamesPath[i];
								if (flag5)
								{
									return false;
								}
							}
							flag2 = true;
						}
					}
				}
				return flag2;
			}

			public string m_ElementName;

			public string[] m_NamesPath;

			public string m_AttributeName;

			public string m_Value;
		}

		[Serializable]
		public struct UxmlSerializedDataOverride
		{
			public int m_ElementId;

			public List<int> m_ElementIdsPath;

			[SerializeReference]
			public UxmlSerializedData m_SerializedData;
		}
	}
}
