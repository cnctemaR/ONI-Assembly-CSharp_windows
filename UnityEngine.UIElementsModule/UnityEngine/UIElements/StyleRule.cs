using System;
using Unity.Collections;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	[Serializable]
	internal class StyleRule
	{
		internal StyleSheet styleSheet { get; set; }

		internal StyleRule(StyleSheet styleSheet)
		{
			this.styleSheet = styleSheet;
		}

		public StyleComplexSelector[] complexSelectors
		{
			get
			{
				return this.m_ComplexSelectors;
			}
		}

		internal void SetSelectors(StyleComplexSelector[] selectors)
		{
			this.m_ComplexSelectors = selectors;
		}

		public StyleProperty[] properties
		{
			get
			{
				return this.m_Properties;
			}
		}

		internal void SetProperties(StyleProperty[] props)
		{
			this.m_Properties = props;
		}

		public bool TryAddSelector(string selectorStr, out StyleComplexSelector selector)
		{
			string text;
			return this.TryAddSelector(selectorStr, out selector, out text);
		}

		public bool TryAddSelector(string selectorStr, out StyleComplexSelector selector, out string error)
		{
			StyleSelector[] array;
			int num;
			bool flag = !SelectorUtility.ExtractSelectorsAndSpecificityFromString(selectorStr, out array, out num, out error);
			bool flag2;
			if (flag)
			{
				selector = null;
				flag2 = false;
			}
			else
			{
				selector = new StyleComplexSelector
				{
					selectors = array,
					specificity = num
				};
				CollectionExtensions.AddToArray<StyleComplexSelector>(ref this.m_ComplexSelectors, selector);
				selector.rule = this;
				this.styleSheet.RequestRebuild(StyleSheet.RebuildOptions.None);
				flag2 = true;
			}
			return flag2;
		}

		public StyleComplexSelector AddSelector(string selectorStr)
		{
			StyleComplexSelector styleComplexSelector;
			string text;
			bool flag = !this.TryAddSelector(selectorStr, out styleComplexSelector, out text);
			if (flag)
			{
				throw new InvalidOperationException(text);
			}
			return styleComplexSelector;
		}

		public bool RemoveSelector(StyleComplexSelector selector)
		{
			int num = Array.IndexOf<StyleComplexSelector>(this.m_ComplexSelectors, selector);
			bool flag = num < 0;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				this.RemoveSelector(num);
				flag2 = true;
			}
			return flag2;
		}

		public bool RemoveSelector(int index)
		{
			bool flag = index < 0 || index >= this.m_ComplexSelectors.Length;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			StyleComplexSelector styleComplexSelector = this.m_ComplexSelectors[index];
			CollectionExtensions.RemoveFromArray<StyleComplexSelector>(ref this.m_ComplexSelectors, index);
			styleComplexSelector.ruleIndex = -1;
			styleComplexSelector.rule = null;
			styleComplexSelector.nextInTable = null;
			this.styleSheet.RequestRebuild(StyleSheet.RebuildOptions.None);
			return true;
		}

		public StyleProperty AddProperty(string propertyName)
		{
			StyleProperty styleProperty = new StyleProperty
			{
				name = propertyName
			};
			CollectionExtensions.AddToArray<StyleProperty>(ref this.m_Properties, styleProperty);
			bool isCustomProperty = styleProperty.isCustomProperty;
			if (isCustomProperty)
			{
				this.customPropertiesCount++;
			}
			this.styleSheet.RequestRebuild(StyleSheet.RebuildOptions.None);
			return styleProperty;
		}

		public bool RemoveProperty(StyleProperty property)
		{
			int num = Array.IndexOf<StyleProperty>(this.m_Properties, property);
			bool flag = num < 0;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				this.RemoveProperty(num);
				flag2 = true;
			}
			return flag2;
		}

		public bool RemoveProperty(int index)
		{
			bool flag = index < 0 || index >= this.m_Properties.Length;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			StyleProperty styleProperty = this.m_Properties[index];
			CollectionExtensions.RemoveFromArray<StyleProperty>(ref this.m_Properties, index);
			bool isCustomProperty = styleProperty.isCustomProperty;
			if (isCustomProperty)
			{
				this.customPropertiesCount--;
			}
			this.styleSheet.RequestRebuild(StyleSheet.RebuildOptions.None);
			return true;
		}

		public StyleProperty FindLastProperty(string propertyName)
		{
			for (int i = this.properties.Length - 1; i >= 0; i--)
			{
				bool flag = this.properties[i].name == propertyName;
				if (flag)
				{
					return this.properties[i];
				}
			}
			return null;
		}

		[SerializeField]
		private StyleComplexSelector[] m_ComplexSelectors = Array.Empty<StyleComplexSelector>();

		[SerializeField]
		private StyleProperty[] m_Properties = Array.Empty<StyleProperty>();

		[SerializeField]
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal int line;

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		[NonSerialized]
		internal int customPropertiesCount;
	}
}
