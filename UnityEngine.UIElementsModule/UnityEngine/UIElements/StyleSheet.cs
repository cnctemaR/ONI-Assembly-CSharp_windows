using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.Bindings;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	[HelpURL("UIE-USS")]
	[Serializable]
	public class StyleSheet : ScriptableObject
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

		internal StyleRule[] rules
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				return this.m_Rules;
			}
		}

		internal List<StyleSheet> flattenedRecursiveImports
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				return this.m_FlattenedImportedStyleSheets;
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

		internal Dictionary<string, StyleComplexSelector>[] tables
		{
			get
			{
				Dictionary<string, StyleComplexSelector>[] array;
				if ((array = this.m_Tables) == null)
				{
					array = (this.m_Tables = new Dictionary<string, StyleComplexSelector>[]
					{
						new Dictionary<string, StyleComplexSelector>(StringComparer.Ordinal),
						new Dictionary<string, StyleComplexSelector>(StringComparer.Ordinal),
						new Dictionary<string, StyleComplexSelector>(StringComparer.Ordinal)
					});
				}
				return array;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal bool isDefaultStyleSheet
		{
			get
			{
				return this.m_IsDefaultStyleSheet;
			}
			set
			{
				this.m_IsDefaultStyleSheet = value;
				bool flag = this.flattenedRecursiveImports != null;
				if (flag)
				{
					foreach (StyleSheet styleSheet in this.flattenedRecursiveImports)
					{
						styleSheet.isDefaultStyleSheet = value;
					}
				}
			}
		}

		private bool TryCheckAccess<T>(T[] list, StyleValueType type, StyleValueHandle handle, out T value)
		{
			bool flag = handle.valueType != type || handle.valueIndex < 0 || handle.valueIndex >= list.Length;
			bool flag2;
			if (flag)
			{
				value = default(T);
				flag2 = false;
			}
			else
			{
				value = list[handle.valueIndex];
				flag2 = true;
			}
			return flag2;
		}

		private T CheckAccess<T>(T[] list, StyleValueType type, StyleValueHandle handle)
		{
			T t = default(T);
			bool flag = handle.valueType != type;
			if (flag)
			{
				Debug.LogErrorFormat(this, "Trying to read value of type {0} while reading a value of type {1}", new object[] { type, handle.valueType });
			}
			else
			{
				bool flag2 = list == null || handle.valueIndex < 0 || handle.valueIndex >= list.Length;
				if (flag2)
				{
					Debug.LogError("Accessing invalid property", this);
				}
				else
				{
					t = list[handle.valueIndex];
				}
			}
			return t;
		}

		internal virtual void OnEnable()
		{
			this.SetupReferences();
		}

		internal void FlattenImportedStyleSheetsRecursive()
		{
			this.m_FlattenedImportedStyleSheets = new List<StyleSheet>();
			this.FlattenImportedStyleSheetsRecursive(this);
		}

		private void FlattenImportedStyleSheetsRecursive(StyleSheet sheet)
		{
			bool flag = sheet.imports == null;
			if (!flag)
			{
				for (int i = 0; i < sheet.imports.Length; i++)
				{
					StyleSheet styleSheet = sheet.imports[i].styleSheet;
					bool flag2 = styleSheet == null;
					if (!flag2)
					{
						styleSheet.isDefaultStyleSheet = this.isDefaultStyleSheet;
						this.FlattenImportedStyleSheetsRecursive(styleSheet);
						this.m_FlattenedImportedStyleSheets.Add(styleSheet);
					}
				}
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal StyleRule AddRule()
		{
			return this.AddRuleAtIndex(-1);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal StyleRule AddRuleAtIndex(int index)
		{
			return this.AddRuleAtIndex(index, null);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal StyleRule AddRule(string selector)
		{
			return this.AddRuleAtIndex(-1, selector);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal StyleRule AddRuleAtIndex(int index, string selector)
		{
			bool flag = index == -1;
			if (flag)
			{
				index = this.rules.Length;
			}
			StyleRule styleRule = new StyleRule(this);
			bool flag2 = !string.IsNullOrEmpty(selector);
			if (flag2)
			{
				styleRule.AddSelector(selector);
			}
			this.InsertValueInArray<StyleRule>(ref this.m_Rules, index, styleRule);
			this.RequestRebuild(StyleSheet.RebuildOptions.None);
			return styleRule;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal bool RemoveRule(StyleRule rule)
		{
			bool flag = rule.styleSheet != this;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				int num = Array.IndexOf<StyleRule>(this.m_Rules, rule);
				bool flag3 = num < 0;
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					this.RemoveRule(num);
					flag2 = false;
				}
			}
			return flag2;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void RemoveRule(int ruleIndex)
		{
			bool flag = ruleIndex < 0 || ruleIndex >= this.m_Rules.Length;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("ruleIndex");
			}
			StyleRule styleRule = this.rules[ruleIndex];
			CollectionExtensions.RemoveFromArray<StyleRule>(ref this.m_Rules, ruleIndex);
			styleRule.styleSheet = null;
			this.RequestRebuild(StyleSheet.RebuildOptions.None);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void SetRules(StyleRule[] newRules)
		{
			this.m_Rules = newRules;
			this.SetupReferences();
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void RequestRebuild(StyleSheet.RebuildOptions options = StyleSheet.RebuildOptions.None)
		{
			this.m_RequiresRebuild = true;
			this.MarkAsChanged();
			bool flag = (options & StyleSheet.RebuildOptions.Synchronous) == StyleSheet.RebuildOptions.Synchronous;
			if (flag)
			{
				this.RebuildIfNecessary();
			}
		}

		internal void RebuildIfNecessary()
		{
			bool requiresRebuild = this.m_RequiresRebuild;
			if (requiresRebuild)
			{
				this.SetupReferences();
			}
		}

		internal void SetupReferences()
		{
			bool flag = this.tables != null;
			if (flag)
			{
				this.tables[0].Clear();
				this.tables[1].Clear();
				this.tables[2].Clear();
			}
			this.nonEmptyTablesMask = 0;
			this.firstRootSelector = null;
			this.firstWildCardSelector = null;
			bool flag2 = this.rules == null || this.rules.Length == 0;
			if (flag2)
			{
				this.m_RequiresRebuild = false;
			}
			else
			{
				int num = 0;
				for (int i = 0; i < this.rules.Length; i++)
				{
					StyleRule styleRule = this.rules[i];
					styleRule.styleSheet = this;
					bool flag3 = styleRule.complexSelectors == null;
					if (!flag3)
					{
						StyleComplexSelector[] complexSelectors = styleRule.complexSelectors;
						int j = 0;
						while (j < complexSelectors.Length)
						{
							StyleComplexSelector styleComplexSelector = complexSelectors[j];
							styleComplexSelector.rule = styleRule;
							styleComplexSelector.ruleIndex = i;
							styleComplexSelector.nextInTable = null;
							styleComplexSelector.CachePseudoStateMasks(this);
							styleComplexSelector.CalculateHashes();
							styleComplexSelector.orderInStyleSheet = num++;
							StyleSelector[] selectors = styleComplexSelector.selectors;
							StyleSelector styleSelector = selectors[selectors.Length - 1];
							StyleSelectorPart styleSelectorPart = styleSelector.parts[0];
							string text = styleSelectorPart.value;
							StyleSheet.OrderedSelectorType orderedSelectorType = StyleSheet.OrderedSelectorType.None;
							switch (styleSelectorPart.type)
							{
							case StyleSelectorType.Wildcard:
							{
								bool flag4 = this.firstWildCardSelector != null;
								if (flag4)
								{
									styleComplexSelector.nextInTable = this.firstWildCardSelector;
								}
								this.firstWildCardSelector = styleComplexSelector;
								break;
							}
							case StyleSelectorType.Type:
								text = styleSelectorPart.value;
								orderedSelectorType = StyleSheet.OrderedSelectorType.Type;
								break;
							case StyleSelectorType.Class:
								orderedSelectorType = StyleSheet.OrderedSelectorType.Class;
								break;
							case StyleSelectorType.PseudoClass:
							{
								bool flag5 = (styleSelector.pseudoStateMask & 128) != 0;
								if (flag5)
								{
									bool flag6 = this.firstRootSelector != null;
									if (flag6)
									{
										styleComplexSelector.nextInTable = this.firstRootSelector;
									}
									this.firstRootSelector = styleComplexSelector;
								}
								else
								{
									bool flag7 = this.firstWildCardSelector != null;
									if (flag7)
									{
										styleComplexSelector.nextInTable = this.firstWildCardSelector;
									}
									this.firstWildCardSelector = styleComplexSelector;
								}
								break;
							}
							case StyleSelectorType.RecursivePseudoClass:
								goto IL_0205;
							case StyleSelectorType.ID:
								orderedSelectorType = StyleSheet.OrderedSelectorType.Name;
								break;
							default:
								goto IL_0205;
							}
							IL_0224:
							bool flag8 = orderedSelectorType != StyleSheet.OrderedSelectorType.None;
							if (flag8)
							{
								Dictionary<string, StyleComplexSelector> dictionary = this.tables[(int)orderedSelectorType];
								StyleComplexSelector styleComplexSelector2;
								bool flag9 = dictionary.TryGetValue(text, out styleComplexSelector2);
								if (flag9)
								{
									styleComplexSelector.nextInTable = styleComplexSelector2;
								}
								this.nonEmptyTablesMask |= 1 << (int)orderedSelectorType;
								dictionary[text] = styleComplexSelector;
							}
							j++;
							continue;
							IL_0205:
							Debug.LogError(string.Format("Invalid first part type {0}", styleSelectorPart.type), this);
							goto IL_0224;
						}
						styleRule.customPropertiesCount = 0;
						foreach (StyleProperty styleProperty in styleRule.properties)
						{
							bool isCustomProperty = styleProperty.isCustomProperty;
							if (isCustomProperty)
							{
								styleRule.customPropertiesCount++;
							}
							foreach (StyleValueHandle styleValueHandle in styleProperty.values)
							{
								bool flag10 = styleValueHandle.IsVarFunction();
								if (flag10)
								{
									styleProperty.requireVariableResolve = true;
									break;
								}
							}
						}
					}
				}
				this.m_RequiresRebuild = false;
			}
		}

		private int AddValueToArray<T>(ref T[] array, T value)
		{
			CollectionExtensions.AddToArray<T>(ref array, value);
			this.MarkAsChanged();
			return array.Length - 1;
		}

		private int InsertValueInArray<T>(ref T[] array, int index, T value)
		{
			CollectionExtensions.InsertIntoArray<T>(ref array, index, value);
			this.MarkAsChanged();
			return index;
		}

		internal int AddValue(StyleValueKeyword keyword)
		{
			this.MarkAsChanged();
			return (int)keyword;
		}

		internal int AddValue(StyleValueFunction function)
		{
			this.MarkAsChanged();
			return (int)function;
		}

		internal int AddValue(float value)
		{
			return this.AddValueToArray<float>(ref this.floats, value);
		}

		internal int AddValue(Dimension value)
		{
			return this.AddValueToArray<Dimension>(ref this.dimensions, value);
		}

		internal int AddValue(Color value)
		{
			return this.AddValueToArray<Color>(ref this.colors, value);
		}

		internal int AddValue(ScalableImage value)
		{
			return this.AddValueToArray<ScalableImage>(ref this.scalableImages, value);
		}

		internal int AddValue(string value)
		{
			return this.AddValueToArray<string>(ref this.strings, value);
		}

		internal int AddValue(Object value)
		{
			return this.AddValueToArray<Object>(ref this.assets, value);
		}

		internal int AddValue(Enum value)
		{
			string enumExportString = StyleSheetUtility.GetEnumExportString(value);
			return this.AddValueToArray<string>(ref this.strings, enumExportString);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal StyleValueKeyword ReadKeyword(StyleValueHandle handle)
		{
			return (StyleValueKeyword)handle.valueIndex;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal bool TryReadKeyword(StyleValueHandle handle, out StyleValueKeyword value)
		{
			value = (StyleValueKeyword)handle.valueIndex;
			return handle.valueType == StyleValueType.Keyword;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal float ReadFloat(StyleValueHandle handle)
		{
			bool flag = handle.valueType == StyleValueType.Dimension;
			float num;
			if (flag)
			{
				Dimension dimension = this.CheckAccess<Dimension>(this.dimensions, StyleValueType.Dimension, handle);
				num = dimension.value;
			}
			else
			{
				num = this.CheckAccess<float>(this.floats, StyleValueType.Float, handle);
			}
			return num;
		}

		internal bool TryReadFloat(StyleValueHandle handle, out float value)
		{
			bool flag = this.TryCheckAccess<float>(this.floats, StyleValueType.Float, handle, out value);
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				Dimension dimension;
				bool flag3 = this.TryCheckAccess<Dimension>(this.dimensions, StyleValueType.Float, handle, out dimension);
				value = dimension.value;
				flag2 = flag3;
			}
			return flag2;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal Dimension ReadDimension(StyleValueHandle handle)
		{
			bool flag = handle.valueType == StyleValueType.Float;
			Dimension dimension;
			if (flag)
			{
				float num = this.CheckAccess<float>(this.floats, StyleValueType.Float, handle);
				dimension = new Dimension(num, Dimension.Unit.Unitless);
			}
			else
			{
				dimension = this.CheckAccess<Dimension>(this.dimensions, StyleValueType.Dimension, handle);
			}
			return dimension;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal bool TryReadDimension(StyleValueHandle handle, out Dimension value)
		{
			bool flag = this.TryCheckAccess<Dimension>(this.dimensions, StyleValueType.Dimension, handle, out value);
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				float num;
				bool flag3 = this.TryCheckAccess<float>(this.floats, StyleValueType.Float, handle, out num);
				value = new Dimension(num, Dimension.Unit.Unitless);
				flag2 = flag3;
			}
			return flag2;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal Color ReadColor(StyleValueHandle handle)
		{
			bool flag = handle.valueType == StyleValueType.Enum;
			Color color2;
			if (flag)
			{
				string text = this.ReadEnum(handle);
				Color color;
				StyleSheetColor.TryGetColor(text.ToLowerInvariant(), out color);
				color2 = color;
			}
			else
			{
				color2 = this.CheckAccess<Color>(this.colors, StyleValueType.Color, handle);
			}
			return color2;
		}

		internal bool TryReadColor(StyleValueHandle handle, out Color value)
		{
			bool flag = this.TryCheckAccess<Color>(this.colors, StyleValueType.Color, handle, out value);
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				string text;
				bool flag3 = this.TryCheckAccess<string>(this.strings, StyleValueType.Enum, handle, out text);
				if (flag3)
				{
					flag2 = StyleSheetColor.TryGetColor(text.ToLowerInvariant(), out value);
				}
				else
				{
					value = default(Color);
					flag2 = false;
				}
			}
			return flag2;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal string ReadString(StyleValueHandle handle)
		{
			return this.CheckAccess<string>(this.strings, StyleValueType.String, handle);
		}

		internal bool TryReadString(StyleValueHandle handle, out string value)
		{
			return this.TryCheckAccess<string>(this.strings, StyleValueType.String, handle, out value);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal string ReadEnum(StyleValueHandle handle)
		{
			return this.CheckAccess<string>(this.strings, StyleValueType.Enum, handle);
		}

		internal bool TryReadEnum(StyleValueHandle handle, out string value)
		{
			return this.TryCheckAccess<string>(this.strings, StyleValueType.Enum, handle, out value);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal TEnum ReadEnum<TEnum>(StyleValueHandle handle) where TEnum : struct, Enum
		{
			string text = this.ReadEnum(handle);
			TEnum tenum;
			return Enum.TryParse<TEnum>(StyleSheetUtility.ConvertDashToHungarian(text), out tenum) ? tenum : default(TEnum);
		}

		internal bool TryReadEnum<TEnum>(StyleValueHandle handle, out TEnum value) where TEnum : struct, Enum
		{
			string text;
			bool flag = this.TryReadEnum(handle, out text) && Enum.TryParse<TEnum>(StyleSheetUtility.ConvertDashToHungarian(text), out value);
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				value = default(TEnum);
				flag2 = false;
			}
			return flag2;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal string ReadVariable(StyleValueHandle handle)
		{
			return this.CheckAccess<string>(this.strings, StyleValueType.Variable, handle);
		}

		internal bool TryReadVariable(StyleValueHandle handle, out string value)
		{
			return this.TryCheckAccess<string>(this.strings, StyleValueType.Variable, handle, out value);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal string ReadResourcePath(StyleValueHandle handle)
		{
			return this.CheckAccess<string>(this.strings, StyleValueType.ResourcePath, handle);
		}

		internal bool TryReadResourcePath(StyleValueHandle handle, out string value)
		{
			return this.TryCheckAccess<string>(this.strings, StyleValueType.ResourcePath, handle, out value);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal Object ReadAssetReference(StyleValueHandle handle)
		{
			return this.CheckAccess<Object>(this.assets, StyleValueType.AssetReference, handle);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal string ReadMissingAssetReferenceUrl(StyleValueHandle handle)
		{
			return this.CheckAccess<string>(this.strings, StyleValueType.MissingAssetReference, handle);
		}

		internal bool TryReadMissingAssetReferenceUrl(StyleValueHandle handle, out string value)
		{
			return this.TryCheckAccess<string>(this.strings, StyleValueType.MissingAssetReference, handle, out value);
		}

		internal bool TryReadAssetReference(StyleValueHandle handle, out Object value)
		{
			return this.TryCheckAccess<Object>(this.assets, StyleValueType.AssetReference, handle, out value);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal StyleValueFunction ReadFunction(StyleValueHandle handle)
		{
			return (StyleValueFunction)handle.valueIndex;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal bool TryReadFunction(StyleValueHandle handle, out StyleValueFunction value)
		{
			value = (StyleValueFunction)handle.valueIndex;
			return handle.valueType == StyleValueType.Function;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal string ReadFunctionName(StyleValueHandle handle)
		{
			bool flag = handle.valueType != StyleValueType.Function;
			string text;
			if (flag)
			{
				Debug.LogErrorFormat(this, string.Format("Trying to read value of type {0} while reading a value of type {1}", StyleValueType.Function, handle.valueType), Array.Empty<object>());
				text = string.Empty;
			}
			else
			{
				StyleValueFunction valueIndex = (StyleValueFunction)handle.valueIndex;
				text = valueIndex.ToUssString();
			}
			return text;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal ScalableImage ReadScalableImage(StyleValueHandle handle)
		{
			return this.CheckAccess<ScalableImage>(this.scalableImages, StyleValueType.ScalableImage, handle);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal bool TryReadScalableImage(StyleValueHandle handle, out ScalableImage value)
		{
			return this.TryCheckAccess<ScalableImage>(this.scalableImages, StyleValueType.ScalableImage, handle, out value);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal StylePropertyName ReadStylePropertyName(StyleValueHandle handle)
		{
			return new StylePropertyName(this.CheckAccess<string>(this.strings, StyleValueType.Enum, handle));
		}

		internal bool TryReadStylePropertyName(StyleValueHandle handle, out StylePropertyName value)
		{
			string text;
			bool flag = this.TryCheckAccess<string>(this.strings, StyleValueType.Enum, handle, out text);
			bool flag2;
			if (flag)
			{
				value = new StylePropertyName(text);
				flag2 = true;
			}
			else
			{
				value = default(StylePropertyName);
				flag2 = false;
			}
			return flag2;
		}

		internal Length ReadLength(StyleValueHandle handle)
		{
			bool flag = handle.valueType == StyleValueType.Keyword;
			Length length2;
			if (flag)
			{
				StyleValueKeyword styleValueKeyword = this.ReadKeyword(handle);
				if (!true)
				{
				}
				Length length;
				if (styleValueKeyword != StyleValueKeyword.Auto)
				{
					if (styleValueKeyword != StyleValueKeyword.None)
					{
						length = default(Length);
					}
					else
					{
						length = Length.None();
					}
				}
				else
				{
					length = Length.Auto();
				}
				if (!true)
				{
				}
				length2 = length;
			}
			else
			{
				Dimension dimension = this.ReadDimension(handle);
				length2 = (dimension.IsLength() ? dimension.ToLength() : default(Length));
			}
			return length2;
		}

		internal bool TryReadLength(StyleValueHandle handle, out Length value)
		{
			StyleValueKeyword styleValueKeyword;
			bool flag = this.TryReadKeyword(handle, out styleValueKeyword);
			bool flag2;
			if (flag)
			{
				StyleValueKeyword styleValueKeyword2 = styleValueKeyword;
				StyleValueKeyword styleValueKeyword3 = styleValueKeyword2;
				if (styleValueKeyword3 != StyleValueKeyword.Auto)
				{
					if (styleValueKeyword3 != StyleValueKeyword.None)
					{
						value = default(Length);
						flag2 = false;
					}
					else
					{
						value = Length.None();
						flag2 = true;
					}
				}
				else
				{
					value = Length.Auto();
					flag2 = true;
				}
			}
			else
			{
				Dimension dimension;
				bool flag3 = this.TryReadDimension(handle, out dimension) && dimension.IsLength();
				if (flag3)
				{
					value = dimension.ToLength();
					flag2 = true;
				}
				else
				{
					value = default(Length);
					flag2 = false;
				}
			}
			return flag2;
		}

		internal Angle ReadAngle(StyleValueHandle handle)
		{
			bool flag = handle.valueType == StyleValueType.Keyword;
			Angle angle2;
			if (flag)
			{
				StyleValueKeyword styleValueKeyword = this.ReadKeyword(handle);
				if (!true)
				{
				}
				Angle angle;
				if (styleValueKeyword != StyleValueKeyword.None)
				{
					angle = default(Angle);
				}
				else
				{
					angle = Angle.None();
				}
				if (!true)
				{
				}
				angle2 = angle;
			}
			else
			{
				Dimension dimension = this.ReadDimension(handle);
				angle2 = (dimension.IsAngle() ? dimension.ToAngle() : default(Angle));
			}
			return angle2;
		}

		internal bool TryReadAngle(StyleValueHandle handle, out Angle value)
		{
			StyleValueKeyword styleValueKeyword;
			bool flag = this.TryReadKeyword(handle, out styleValueKeyword);
			bool flag2;
			if (flag)
			{
				StyleValueKeyword styleValueKeyword2 = styleValueKeyword;
				StyleValueKeyword styleValueKeyword3 = styleValueKeyword2;
				if (styleValueKeyword3 != StyleValueKeyword.None)
				{
					value = default(Angle);
					flag2 = false;
				}
				else
				{
					value = Angle.None();
					flag2 = true;
				}
			}
			else
			{
				Dimension dimension;
				bool flag3 = this.TryReadDimension(handle, out dimension) && dimension.IsAngle();
				if (flag3)
				{
					value = dimension.ToAngle();
					flag2 = true;
				}
				else
				{
					value = default(Angle);
					flag2 = false;
				}
			}
			return flag2;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal TimeValue ReadTimeValue(StyleValueHandle handle)
		{
			Dimension dimension = this.ReadDimension(handle);
			return dimension.IsTimeValue() ? dimension.ToTime() : default(TimeValue);
		}

		internal bool TryReadTimeValue(StyleValueHandle handle, out TimeValue value)
		{
			Dimension dimension;
			bool flag = this.TryReadDimension(handle, out dimension) && dimension.IsTimeValue();
			bool flag2;
			if (flag)
			{
				value = dimension.ToTime();
				flag2 = true;
			}
			else
			{
				value = default(TimeValue);
				flag2 = false;
			}
			return flag2;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void WriteKeyword(ref StyleValueHandle handle, StyleValueKeyword value)
		{
			handle.valueType = StyleValueType.Keyword;
			handle.valueIndex = (int)value;
			this.MarkAsChanged();
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void WriteFloat(ref StyleValueHandle handle, float value)
		{
			bool flag = handle.valueType == StyleValueType.Float;
			if (flag)
			{
				this.floats[handle.valueIndex] = value;
			}
			else
			{
				int num = this.AddValue(value);
				handle.valueType = StyleValueType.Float;
				handle.valueIndex = num;
			}
			this.MarkAsChanged();
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void WriteDimension(ref StyleValueHandle handle, Dimension dimension)
		{
			bool flag = handle.valueType == StyleValueType.Dimension;
			if (flag)
			{
				this.dimensions[handle.valueIndex] = dimension;
			}
			else
			{
				int num = this.AddValue(dimension);
				handle.valueType = StyleValueType.Dimension;
				handle.valueIndex = num;
			}
			this.MarkAsChanged();
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void WriteColor(ref StyleValueHandle handle, Color color)
		{
			bool flag = handle.valueType == StyleValueType.Color;
			if (flag)
			{
				this.colors[handle.valueIndex] = color;
			}
			else
			{
				int num = this.AddValue(color);
				handle.valueType = StyleValueType.Color;
				handle.valueIndex = num;
			}
			this.MarkAsChanged();
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void WriteString(ref StyleValueHandle handle, string value)
		{
			bool flag = handle.valueType == StyleValueType.String;
			if (flag)
			{
				this.strings[handle.valueIndex] = value;
			}
			else
			{
				int num = this.AddValue(value);
				handle.valueType = StyleValueType.String;
				handle.valueIndex = num;
			}
			this.MarkAsChanged();
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void WriteEnum<TEnum>(ref StyleValueHandle handle, TEnum value) where TEnum : Enum
		{
			string enumExportString = StyleSheetUtility.GetEnumExportString(value);
			this.WriteEnumAsString(ref handle, enumExportString);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void WriteEnumAsString(ref StyleValueHandle handle, string valueStr)
		{
			bool flag = handle.valueType == StyleValueType.Enum;
			if (flag)
			{
				this.strings[handle.valueIndex] = valueStr;
			}
			else
			{
				int num = this.AddValue(valueStr);
				handle.valueType = StyleValueType.Enum;
				handle.valueIndex = num;
			}
			this.MarkAsChanged();
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void WriteVariable(ref StyleValueHandle handle, string variableName)
		{
			bool flag = handle.valueType == StyleValueType.Variable;
			if (flag)
			{
				this.strings[handle.valueIndex] = variableName;
			}
			else
			{
				int num = this.AddValue(variableName);
				handle.valueType = StyleValueType.Variable;
				handle.valueIndex = num;
			}
			this.MarkAsChanged();
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void WriteResourcePath(ref StyleValueHandle handle, string resourcePath)
		{
			bool flag = handle.valueType == StyleValueType.ResourcePath;
			if (flag)
			{
				this.strings[handle.valueIndex] = resourcePath;
			}
			else
			{
				int num = this.AddValue(resourcePath);
				handle.valueType = StyleValueType.ResourcePath;
				handle.valueIndex = num;
			}
			this.MarkAsChanged();
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void WriteAssetReference(ref StyleValueHandle handle, Object value)
		{
			bool flag = handle.valueType == StyleValueType.AssetReference;
			if (flag)
			{
				this.assets[handle.valueIndex] = value;
			}
			else
			{
				int num = this.AddValue(value);
				handle.valueType = StyleValueType.AssetReference;
				handle.valueIndex = num;
			}
			this.MarkAsChanged();
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void WriteMissingAssetReferenceUrl(ref StyleValueHandle handle, string assetReference)
		{
			bool flag = handle.valueType == StyleValueType.MissingAssetReference;
			if (flag)
			{
				this.strings[handle.valueIndex] = assetReference;
			}
			else
			{
				int num = this.AddValue(assetReference);
				handle.valueType = StyleValueType.MissingAssetReference;
				handle.valueIndex = num;
			}
			this.MarkAsChanged();
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void WriteFunction(ref StyleValueHandle handle, StyleValueFunction function)
		{
			handle.valueType = StyleValueType.Function;
			handle.valueIndex = (int)function;
			this.MarkAsChanged();
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void WriteScalableImage(ref StyleValueHandle handle, ScalableImage scalableImage)
		{
			bool flag = handle.valueType == StyleValueType.ScalableImage;
			if (flag)
			{
				this.scalableImages[handle.valueIndex] = scalableImage;
			}
			else
			{
				int num = this.AddValue(scalableImage);
				handle.valueType = StyleValueType.ScalableImage;
				handle.valueIndex = num;
			}
			this.MarkAsChanged();
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void WriteStylePropertyName(ref StyleValueHandle handle, StylePropertyName propertyName)
		{
			string text = ((propertyName.id != StylePropertyId.Unknown) ? propertyName.ToString() : "ignored");
			bool flag = handle.valueType == StyleValueType.Enum;
			if (flag)
			{
				this.strings[handle.valueIndex] = text;
			}
			else
			{
				int num = this.AddValue(text);
				handle.valueType = StyleValueType.Enum;
				handle.valueIndex = num;
			}
			this.MarkAsChanged();
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void WriteCommaSeparator(ref StyleValueHandle handle)
		{
			handle.valueIndex = 0;
			handle.valueType = StyleValueType.CommaSeparator;
			this.MarkAsChanged();
		}

		internal void WriteLength(ref StyleValueHandle handle, Length value)
		{
			bool flag = value.IsAuto();
			if (flag)
			{
				this.WriteKeyword(ref handle, StyleValueKeyword.Auto);
			}
			else
			{
				bool flag2 = value.IsNone();
				if (flag2)
				{
					this.WriteKeyword(ref handle, StyleValueKeyword.None);
				}
				else
				{
					this.WriteDimension(ref handle, value.ToDimension());
				}
			}
		}

		internal void WriteAngle(ref StyleValueHandle handle, Angle value)
		{
			bool flag = value.IsNone();
			if (flag)
			{
				this.WriteKeyword(ref handle, StyleValueKeyword.None);
			}
			else
			{
				this.WriteDimension(ref handle, value.ToDimension());
			}
		}

		internal void WriteTimeValue(ref StyleValueHandle handle, TimeValue value)
		{
			this.WriteDimension(ref handle, value.ToDimension());
		}

		private void MarkAsChanged()
		{
			bool flag = this.rules == null || this.rules.Length == 0;
			if (flag)
			{
				this.contentHash = 0;
			}
			else
			{
				this.contentHash = Random.Range(1, int.MaxValue);
			}
			UIElementsUtility.MarkStyleSheetAsChanged(this);
		}

		[NonSerialized]
		private bool m_RequiresRebuild = true;

		[SerializeField]
		private bool m_ImportedWithErrors;

		[SerializeField]
		private bool m_ImportedWithWarnings;

		[SerializeField]
		private StyleRule[] m_Rules = Array.Empty<StyleRule>();

		[SerializeField]
		internal float[] floats = Array.Empty<float>();

		[SerializeField]
		internal Dimension[] dimensions = Array.Empty<Dimension>();

		[SerializeField]
		internal Color[] colors = Array.Empty<Color>();

		[SerializeField]
		internal string[] strings = Array.Empty<string>();

		[SerializeField]
		internal Object[] assets = Array.Empty<Object>();

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		[SerializeField]
		internal StyleSheet.ImportStruct[] imports = Array.Empty<StyleSheet.ImportStruct>();

		[SerializeField]
		private List<StyleSheet> m_FlattenedImportedStyleSheets = new List<StyleSheet>();

		[SerializeField]
		private int m_ContentHash;

		[SerializeField]
		internal ScalableImage[] scalableImages = Array.Empty<ScalableImage>();

		[NonSerialized]
		internal Dictionary<string, StyleComplexSelector>[] m_Tables;

		[NonSerialized]
		internal int nonEmptyTablesMask;

		[NonSerialized]
		internal StyleComplexSelector firstRootSelector;

		[NonSerialized]
		internal StyleComplexSelector firstWildCardSelector;

		[NonSerialized]
		private bool m_IsDefaultStyleSheet;

		[Flags]
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal enum RebuildOptions
		{
			None = 0,
			Synchronous = 1
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		[Serializable]
		internal struct ImportStruct
		{
			public StyleSheet styleSheet;

			public string[] mediaQueries;
		}

		internal enum OrderedSelectorType
		{
			None = -1,
			Name,
			Type,
			Class,
			Length
		}
	}
}
