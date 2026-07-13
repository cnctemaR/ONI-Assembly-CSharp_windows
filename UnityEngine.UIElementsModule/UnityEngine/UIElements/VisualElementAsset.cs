using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule", "UnityEditor.UIToolkitAuthoringModule" })]
	[Serializable]
	internal class VisualElementAsset : UxmlAsset
	{
		public int ruleIndex
		{
			get
			{
				return this.m_RuleIndex;
			}
			set
			{
				this.m_RuleIndex = value;
			}
		}

		public string[] classes
		{
			get
			{
				return this.m_Classes;
			}
			internal set
			{
				this.m_Classes = value;
			}
		}

		public List<string> stylesheetPaths
		{
			get
			{
				List<string> list;
				if ((list = this.m_StylesheetPaths) == null)
				{
					list = (this.m_StylesheetPaths = new List<string>());
				}
				return list;
			}
			set
			{
				this.m_StylesheetPaths = value;
			}
		}

		public bool hasStylesheetPaths
		{
			get
			{
				return this.m_StylesheetPaths != null;
			}
		}

		public List<StyleSheet> stylesheets
		{
			get
			{
				List<StyleSheet> list;
				if ((list = this.m_Stylesheets) == null)
				{
					list = (this.m_Stylesheets = new List<StyleSheet>());
				}
				return list;
			}
			set
			{
				this.m_Stylesheets = value;
			}
		}

		public bool hasStylesheets
		{
			get
			{
				return this.m_Stylesheets != null;
			}
		}

		public UxmlSerializedData serializedData
		{
			get
			{
				return this.m_SerializedData;
			}
			set
			{
				this.m_SerializedData = value;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal bool skipClone
		{
			get
			{
				return this.m_SkipClone;
			}
			set
			{
				this.m_SkipClone = value;
			}
		}

		public VisualElementAsset(string fullTypeName, UxmlNamespaceDefinition xmlNamespace = default(UxmlNamespaceDefinition))
			: base(fullTypeName, xmlNamespace)
		{
		}

		private static bool IdsPathMatchesAttributeOverrideIdsPath(List<int> idsPath, List<int> attributeOverrideIdsPath, int templateId)
		{
			bool flag = idsPath == null || attributeOverrideIdsPath == null || idsPath.Count == 0 || attributeOverrideIdsPath.Count == 0;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				int num = idsPath.IndexOf(templateId);
				bool flag3 = idsPath.Count != attributeOverrideIdsPath.Count + num + 1;
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					for (int i = idsPath.Count - 1; i > num; i--)
					{
						bool flag4 = idsPath[i] != attributeOverrideIdsPath[i - num - 1];
						if (flag4)
						{
							return false;
						}
					}
					flag2 = true;
				}
			}
			return flag2;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal virtual VisualElement Instantiate(CreationContext cc)
		{
			VisualElement visualElement = (VisualElement)this.serializedData.CreateInstance();
			this.serializedData.Deserialize(visualElement);
			bool hasOverrides = cc.hasOverrides;
			if (hasOverrides)
			{
				cc.veaIdsPath.Add(base.id);
				for (int i = cc.serializedDataOverrides.Count - 1; i >= 0; i--)
				{
					foreach (TemplateAsset.UxmlSerializedDataOverride uxmlSerializedDataOverride in cc.serializedDataOverrides[i].attributeOverrides)
					{
						bool flag = uxmlSerializedDataOverride.m_ElementId == base.id && VisualElementAsset.IdsPathMatchesAttributeOverrideIdsPath(cc.veaIdsPath, uxmlSerializedDataOverride.m_ElementIdsPath, cc.serializedDataOverrides[i].templateId);
						if (flag)
						{
							uxmlSerializedDataOverride.m_SerializedData.Deserialize(visualElement);
						}
					}
				}
				cc.veaIdsPath.Remove(base.id);
			}
			bool hasStylesheetPaths = this.hasStylesheetPaths;
			if (hasStylesheetPaths)
			{
				for (int j = 0; j < this.stylesheetPaths.Count; j++)
				{
					visualElement.AddStyleSheetPath(this.stylesheetPaths[j]);
				}
			}
			bool hasStylesheets = this.hasStylesheets;
			if (hasStylesheets)
			{
				for (int k = 0; k < this.stylesheets.Count; k++)
				{
					bool flag2 = this.stylesheets[k] != null;
					if (flag2)
					{
						visualElement.styleSheets.Add(this.stylesheets[k]);
					}
				}
			}
			bool flag3 = this.classes != null;
			if (flag3)
			{
				for (int l = 0; l < this.classes.Length; l++)
				{
					visualElement.AddToClassList(this.classes[l]);
				}
			}
			return visualElement;
		}

		internal override bool Accepts(UxmlAsset asset, out string errorMessage)
		{
			bool flag = !asset.isRoot;
			errorMessage = ((!flag) ? "[UI Toolkit] Cannot add a root UXML asset as a children of a UXML asset." : null);
			return flag;
		}

		public override string ToString()
		{
			string text;
			return base.TryGetAttributeValue("name", out text) ? string.Format("{0}({1})({2})", text, base.fullTypeName, base.id) : string.Format("({0})({1})", base.fullTypeName, base.id);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void AddStyleSheet(StyleSheet styleSheet)
		{
			bool flag = styleSheet == null || this.stylesheets.Contains(styleSheet);
			if (!flag)
			{
				this.stylesheets.Add(styleSheet);
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		public void AddStyleSheets(IEnumerable<StyleSheet> styleSheets)
		{
			foreach (StyleSheet styleSheet in styleSheets)
			{
				this.AddStyleSheet(styleSheet);
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void RemoveStyleSheet(StyleSheet styleSheet)
		{
			this.stylesheets.RemoveAll((StyleSheet s) => s == styleSheet);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void AddStyleClass(string className)
		{
			if (this.m_Classes == null)
			{
				this.m_Classes = Array.Empty<string>();
			}
			bool flag = Array.IndexOf<string>(this.m_Classes, className) == -1;
			if (flag)
			{
				CollectionExtensions.AddToArray<string>(ref this.m_Classes, className);
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void RemoveStyleClass(string className)
		{
			bool flag = this.m_Classes == null;
			if (!flag)
			{
				CollectionExtensions.RemoveFromArray<string>(ref this.m_Classes, className);
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		public void ClearStyleSheets()
		{
			this.stylesheets.Clear();
		}

		private protected override void OnVisualTreeAssetChanged(VisualTreeAsset previousVta, VisualTreeAsset newVta)
		{
			base.OnVisualTreeAssetChanged(previousVta, newVta);
			bool flag = this.ruleIndex < 0;
			if (!flag)
			{
				bool flag2 = !previousVta;
				if (flag2)
				{
					this.ruleIndex = -1;
					Debug.LogWarning("VisualElementAsset previously had inline styles that were lost.");
				}
				else
				{
					bool flag3 = newVta;
					if (flag3)
					{
						VisualTreeAsset.SwallowStyleRule(previousVta, newVta, this);
					}
				}
			}
		}

		internal const string k_LostInlineStyles = "VisualElementAsset previously had inline styles that were lost.";

		[SerializeField]
		private int m_RuleIndex = -1;

		[SerializeField]
		private string[] m_Classes = Array.Empty<string>();

		[SerializeField]
		private List<string> m_StylesheetPaths;

		[SerializeField]
		private List<StyleSheet> m_Stylesheets;

		[SerializeReference]
		private UxmlSerializedData m_SerializedData;

		[SerializeField]
		private bool m_SkipClone;
	}
}
