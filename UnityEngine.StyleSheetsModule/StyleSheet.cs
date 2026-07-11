using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.StyleSheets
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	[Serializable]
	internal class StyleSheet : ScriptableObject
	{
		public StyleRule[] rules
		{
			get
			{
				return this.m_Rules;
			}
			internal set
			{
				this.m_Rules = value;
				this.SetupReferences();
			}
		}

		public StyleComplexSelector[] complexSelectors
		{
			get
			{
				return this.m_ComplexSelectors;
			}
			internal set
			{
				this.m_ComplexSelectors = value;
				this.SetupReferences();
			}
		}

		private static bool TryCheckAccess<T>(T[] list, StyleValueType type, StyleValueHandle[] handles, int index, out T value)
		{
			bool flag = false;
			value = default(T);
			if (index < handles.Length)
			{
				StyleValueHandle styleValueHandle = handles[index];
				if (styleValueHandle.valueType == type && styleValueHandle.valueIndex >= 0 && styleValueHandle.valueIndex < list.Length)
				{
					value = list[styleValueHandle.valueIndex];
					flag = true;
				}
				else
				{
					Debug.LogErrorFormat("Trying to read value of type {0} while reading a value of type {1}", new object[] { type, styleValueHandle.valueType });
				}
			}
			return flag;
		}

		private static T CheckAccess<T>(T[] list, StyleValueType type, StyleValueHandle handle)
		{
			T t = default(T);
			if (handle.valueType != type)
			{
				Debug.LogErrorFormat("Trying to read value of type {0} while reading a value of type {1}", new object[] { type, handle.valueType });
			}
			else if (list == null || handle.valueIndex < 0 || handle.valueIndex >= list.Length)
			{
				Debug.LogError("Accessing invalid property");
			}
			else
			{
				t = list[handle.valueIndex];
			}
			return t;
		}

		private void OnEnable()
		{
			this.hasSelectorsCached = false;
			this.SetupReferences();
		}

		private void SetupReferences()
		{
			if (this.complexSelectors != null && this.rules != null)
			{
				this.orderedClassSelectors = new Dictionary<string, StyleComplexSelector>(StringComparer.Ordinal);
				this.orderedNameSelectors = new Dictionary<string, StyleComplexSelector>(StringComparer.Ordinal);
				this.orderedTypeSelectors = new Dictionary<string, StyleComplexSelector>(StringComparer.Ordinal);
				int i = 0;
				while (i < this.complexSelectors.Length)
				{
					StyleComplexSelector styleComplexSelector = this.complexSelectors[i];
					if (styleComplexSelector.ruleIndex < this.rules.Length)
					{
						styleComplexSelector.rule = this.rules[styleComplexSelector.ruleIndex];
					}
					styleComplexSelector.orderInStyleSheet = i;
					StyleSelector styleSelector = styleComplexSelector.selectors[styleComplexSelector.selectors.Length - 1];
					StyleSelectorPart styleSelectorPart = styleSelector.parts[0];
					string text = styleSelectorPart.value;
					Dictionary<string, StyleComplexSelector> dictionary = null;
					switch (styleSelectorPart.type)
					{
					case StyleSelectorType.Wildcard:
					case StyleSelectorType.Type:
						text = styleSelectorPart.value ?? "*";
						dictionary = this.orderedTypeSelectors;
						break;
					case StyleSelectorType.Class:
						dictionary = this.orderedClassSelectors;
						break;
					case StyleSelectorType.PseudoClass:
						text = "*";
						dictionary = this.orderedTypeSelectors;
						break;
					case StyleSelectorType.RecursivePseudoClass:
						goto IL_013B;
					case StyleSelectorType.ID:
						dictionary = this.orderedNameSelectors;
						break;
					default:
						goto IL_013B;
					}
					IL_015B:
					if (dictionary != null)
					{
						StyleComplexSelector styleComplexSelector2;
						if (dictionary.TryGetValue(text, out styleComplexSelector2))
						{
							styleComplexSelector.nextInTable = styleComplexSelector2;
						}
						dictionary[text] = styleComplexSelector;
					}
					i++;
					continue;
					IL_013B:
					Debug.LogError(string.Format("Invalid first part type {0}", styleSelectorPart.type));
					goto IL_015B;
				}
			}
		}

		public StyleValueKeyword ReadKeyword(StyleValueHandle handle)
		{
			return (StyleValueKeyword)handle.valueIndex;
		}

		public float ReadFloat(StyleValueHandle handle)
		{
			return StyleSheet.CheckAccess<float>(this.floats, StyleValueType.Float, handle);
		}

		public bool TryReadFloat(StyleValueHandle[] handles, int index, out float value)
		{
			return StyleSheet.TryCheckAccess<float>(this.floats, StyleValueType.Float, handles, index, out value);
		}

		public Color ReadColor(StyleValueHandle handle)
		{
			return StyleSheet.CheckAccess<Color>(this.colors, StyleValueType.Color, handle);
		}

		public bool TryReadColor(StyleValueHandle[] handles, int index, out Color value)
		{
			return StyleSheet.TryCheckAccess<Color>(this.colors, StyleValueType.Color, handles, index, out value);
		}

		public string ReadString(StyleValueHandle handle)
		{
			return StyleSheet.CheckAccess<string>(this.strings, StyleValueType.String, handle);
		}

		public bool TryReadString(StyleValueHandle[] handles, int index, out string value)
		{
			return StyleSheet.TryCheckAccess<string>(this.strings, StyleValueType.String, handles, index, out value);
		}

		public string ReadEnum(StyleValueHandle handle)
		{
			return StyleSheet.CheckAccess<string>(this.strings, StyleValueType.Enum, handle);
		}

		public bool TryReadEnum(StyleValueHandle[] handles, int index, out string value)
		{
			return StyleSheet.TryCheckAccess<string>(this.strings, StyleValueType.Enum, handles, index, out value);
		}

		public string ReadResourcePath(StyleValueHandle handle)
		{
			return StyleSheet.CheckAccess<string>(this.strings, StyleValueType.ResourcePath, handle);
		}

		public bool TryReadResourcePath(StyleValueHandle[] handles, int index, out string value)
		{
			return StyleSheet.TryCheckAccess<string>(this.strings, StyleValueType.ResourcePath, handles, index, out value);
		}

		public Object ReadAssetReference(StyleValueHandle handle)
		{
			return StyleSheet.CheckAccess<Object>(this.assets, StyleValueType.AssetReference, handle);
		}

		public bool TryReadAssetReference(StyleValueHandle[] handles, int index, out Object value)
		{
			return StyleSheet.TryCheckAccess<Object>(this.assets, StyleValueType.AssetReference, handles, index, out value);
		}

		[SerializeField]
		private StyleRule[] m_Rules;

		[SerializeField]
		private StyleComplexSelector[] m_ComplexSelectors;

		[SerializeField]
		internal float[] floats;

		[SerializeField]
		internal Color[] colors;

		[SerializeField]
		internal string[] strings;

		[SerializeField]
		internal Object[] assets;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[NonSerialized]
		internal Dictionary<string, StyleComplexSelector> orderedNameSelectors;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[NonSerialized]
		internal Dictionary<string, StyleComplexSelector> orderedTypeSelectors;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[NonSerialized]
		internal Dictionary<string, StyleComplexSelector> orderedClassSelectors;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[NonSerialized]
		internal bool hasSelectorsCached;
	}
}
