using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	[Serializable]
	internal class StyleComplexSelector
	{
		public int specificity
		{
			get
			{
				return this.m_Specificity;
			}
			internal set
			{
				this.m_Specificity = value;
			}
		}

		public StyleRule rule
		{
			get; [VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			internal set;
		}

		public bool isSimple
		{
			get
			{
				StyleSelector[] selectors = this.selectors;
				return selectors != null && selectors.Length == 1;
			}
		}

		public StyleSelector[] selectors
		{
			get
			{
				return this.m_Selectors;
			}
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			internal set
			{
				this.m_Selectors = value;
			}
		}

		internal StyleComplexSelector()
		{
		}

		public bool TrySetSelectorsFromString(string complexSelectorStr, out string error)
		{
			StyleSelector[] array;
			int num;
			bool flag = !SelectorUtility.ExtractSelectorsAndSpecificityFromString(complexSelectorStr, out array, out num, out error);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				this.selectors = array;
				this.specificity = num;
				error = null;
				flag2 = true;
			}
			return flag2;
		}

		internal void CachePseudoStateMasks(StyleSheet styleSheet)
		{
			bool flag = StyleComplexSelector.s_PseudoStates == null;
			if (flag)
			{
				StyleComplexSelector.s_PseudoStates = new Dictionary<string, StyleComplexSelector.PseudoStateData>();
				StyleComplexSelector.s_PseudoStates["active"] = new StyleComplexSelector.PseudoStateData(PseudoStates.Active, false);
				StyleComplexSelector.s_PseudoStates["hover"] = new StyleComplexSelector.PseudoStateData(PseudoStates.Hover, false);
				StyleComplexSelector.s_PseudoStates["checked"] = new StyleComplexSelector.PseudoStateData(PseudoStates.Checked, false);
				StyleComplexSelector.s_PseudoStates["selected"] = new StyleComplexSelector.PseudoStateData(PseudoStates.Checked, false);
				StyleComplexSelector.s_PseudoStates["disabled"] = new StyleComplexSelector.PseudoStateData(PseudoStates.Disabled, false);
				StyleComplexSelector.s_PseudoStates["focus"] = new StyleComplexSelector.PseudoStateData(PseudoStates.Focus, false);
				StyleComplexSelector.s_PseudoStates["root"] = new StyleComplexSelector.PseudoStateData(PseudoStates.Root, false);
				StyleComplexSelector.s_PseudoStates["inactive"] = new StyleComplexSelector.PseudoStateData(PseudoStates.Active, true);
				StyleComplexSelector.s_PseudoStates["enabled"] = new StyleComplexSelector.PseudoStateData(PseudoStates.Disabled, true);
			}
			int i = 0;
			int num = this.selectors.Length;
			while (i < num)
			{
				StyleSelector styleSelector = this.selectors[i];
				StyleSelectorPart[] parts = styleSelector.parts;
				PseudoStates pseudoStates = PseudoStates.None;
				PseudoStates pseudoStates2 = PseudoStates.None;
				bool flag2 = true;
				int num2 = 0;
				while (num2 < styleSelector.parts.Length && flag2)
				{
					bool flag3 = styleSelector.parts[num2].type == StyleSelectorType.PseudoClass;
					if (flag3)
					{
						StyleComplexSelector.PseudoStateData pseudoStateData;
						bool flag4 = StyleComplexSelector.s_PseudoStates.TryGetValue(parts[num2].value, out pseudoStateData);
						if (flag4)
						{
							bool flag5 = !pseudoStateData.negate;
							if (flag5)
							{
								pseudoStates |= pseudoStateData.state;
							}
							else
							{
								pseudoStates2 |= pseudoStateData.state;
							}
						}
						else
						{
							Debug.LogWarningFormat(styleSheet, "Unknown pseudo class \"{0}\" in StyleSheet {1}", new object[]
							{
								parts[num2].value,
								styleSheet.name
							});
							flag2 = false;
						}
					}
					num2++;
				}
				bool flag6 = flag2;
				if (flag6)
				{
					styleSelector.pseudoStateMask = (int)pseudoStates;
					styleSelector.negatedPseudoStateMask = (int)pseudoStates2;
				}
				else
				{
					styleSelector.pseudoStateMask = -1;
					styleSelector.negatedPseudoStateMask = -1;
				}
				i++;
			}
		}

		public override string ToString()
		{
			return string.Format("[{0}]", string.Join(", ", this.m_Selectors.Select<StyleSelector, string>((StyleSelector x) => x.ToString()).ToArray<string>()));
		}

		private static int StyleSelectorPartCompare(StyleSelectorPart x, StyleSelectorPart y)
		{
			bool flag = y.type < x.type;
			int num;
			if (flag)
			{
				num = -1;
			}
			else
			{
				bool flag2 = y.type > x.type;
				if (flag2)
				{
					num = 1;
				}
				else
				{
					num = y.value.CompareTo(x.value);
				}
			}
			return num;
		}

		internal unsafe void CalculateHashes()
		{
			bool isSimple = this.isSimple;
			if (!isSimple)
			{
				for (int i = this.selectors.Length - 2; i > -1; i--)
				{
					StyleComplexSelector.s_HashList.AddRange(this.selectors[i].parts);
				}
				StyleComplexSelector.s_HashList.RemoveAll((StyleSelectorPart p) => p.type != StyleSelectorType.Class && p.type != StyleSelectorType.ID && p.type != StyleSelectorType.Type);
				StyleComplexSelector.s_HashList.Sort(new Comparison<StyleSelectorPart>(StyleComplexSelector.StyleSelectorPartCompare));
				bool flag = true;
				StyleSelectorType styleSelectorType = StyleSelectorType.Unknown;
				string text = "";
				int num = 0;
				int num2 = Math.Min(4, StyleComplexSelector.s_HashList.Count);
				for (int j = 0; j < num2; j++)
				{
					bool flag2 = flag;
					if (flag2)
					{
						flag = false;
					}
					else
					{
						while (num < StyleComplexSelector.s_HashList.Count && StyleComplexSelector.s_HashList[num].type == styleSelectorType && StyleComplexSelector.s_HashList[num].value == text)
						{
							num++;
						}
						bool flag3 = num == StyleComplexSelector.s_HashList.Count;
						if (flag3)
						{
							break;
						}
					}
					styleSelectorType = StyleComplexSelector.s_HashList[num].type;
					text = StyleComplexSelector.s_HashList[num].value;
					bool flag4 = styleSelectorType == StyleSelectorType.ID;
					Salt salt;
					if (flag4)
					{
						salt = Salt.IdSalt;
					}
					else
					{
						bool flag5 = styleSelectorType == StyleSelectorType.Class;
						if (flag5)
						{
							salt = Salt.ClassSalt;
						}
						else
						{
							salt = Salt.TagNameSalt;
						}
					}
					*((ref this.ancestorHashes.hashes.FixedElementField) + (IntPtr)j * 4) = text.GetHashCode() * (int)salt;
				}
				StyleComplexSelector.s_HashList.Clear();
			}
		}

		[NonSerialized]
		public Hashes ancestorHashes;

		[SerializeField]
		private int m_Specificity;

		[SerializeField]
		private StyleSelector[] m_Selectors = Array.Empty<StyleSelector>();

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		[SerializeField]
		internal int ruleIndex;

		[NonSerialized]
		internal StyleComplexSelector nextInTable;

		[NonSerialized]
		internal int orderInStyleSheet;

		private static Dictionary<string, StyleComplexSelector.PseudoStateData> s_PseudoStates;

		private static readonly List<StyleSelectorPart> s_HashList = new List<StyleSelectorPart>();

		private struct PseudoStateData
		{
			public PseudoStateData(PseudoStates state, bool negate)
			{
				this.state = state;
				this.negate = negate;
			}

			public readonly PseudoStates state;

			public readonly bool negate;
		}
	}
}
