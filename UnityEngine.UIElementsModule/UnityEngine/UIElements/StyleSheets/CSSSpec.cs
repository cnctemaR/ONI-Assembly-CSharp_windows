using System;
using System.Text.RegularExpressions;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements.StyleSheets
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static class CSSSpec
	{
		public static int GetSelectorSpecificity(string selector)
		{
			int num = 0;
			StyleSelectorPart[] array;
			bool flag = CSSSpec.ParseSelector(selector, out array);
			if (flag)
			{
				num = CSSSpec.GetSelectorSpecificity(array);
			}
			return num;
		}

		public static int GetSelectorSpecificity(StyleSelectorPart[] parts)
		{
			int num = 1;
			for (int i = 0; i < parts.Length; i++)
			{
				switch (parts[i].type)
				{
				case StyleSelectorType.Type:
					num++;
					break;
				case StyleSelectorType.Class:
				case StyleSelectorType.PseudoClass:
					num += 10;
					break;
				case StyleSelectorType.RecursivePseudoClass:
					throw new ArgumentException("Recursive pseudo classes are not supported");
				case StyleSelectorType.ID:
					num += 100;
					break;
				}
			}
			return num;
		}

		public static bool ValidateSelector(string selector)
		{
			return CSSSpec.rgx.Matches(selector).Count > 0;
		}

		public static bool ParseSelector(string selector, out StyleSelectorPart[] parts)
		{
			MatchCollection matchCollection = CSSSpec.rgx.Matches(selector);
			int count = matchCollection.Count;
			bool flag = count < 1;
			bool flag2;
			if (flag)
			{
				parts = null;
				flag2 = false;
			}
			else
			{
				parts = new StyleSelectorPart[count];
				for (int i = 0; i < count; i++)
				{
					Match match = matchCollection[i];
					StyleSelectorType styleSelectorType = StyleSelectorType.Unknown;
					string text = string.Empty;
					bool flag3 = !string.IsNullOrEmpty(match.Groups["wildcard"].Value);
					if (flag3)
					{
						text = "*";
						styleSelectorType = StyleSelectorType.Wildcard;
					}
					else
					{
						bool flag4 = !string.IsNullOrEmpty(match.Groups["id"].Value);
						if (flag4)
						{
							text = match.Groups["id"].Value.Substring(1);
							styleSelectorType = StyleSelectorType.ID;
						}
						else
						{
							bool flag5 = !string.IsNullOrEmpty(match.Groups["class"].Value);
							if (flag5)
							{
								text = match.Groups["class"].Value.Substring(1);
								styleSelectorType = StyleSelectorType.Class;
							}
							else
							{
								bool flag6 = !string.IsNullOrEmpty(match.Groups["pseudoclass"].Value);
								if (flag6)
								{
									string value = match.Groups["param"].Value;
									bool flag7 = !string.IsNullOrEmpty(value);
									if (flag7)
									{
										text = value;
										styleSelectorType = StyleSelectorType.RecursivePseudoClass;
									}
									else
									{
										text = match.Groups["pseudoclass"].Value.Substring(1);
										styleSelectorType = StyleSelectorType.PseudoClass;
									}
								}
								else
								{
									bool flag8 = !string.IsNullOrEmpty(match.Groups["type"].Value);
									if (flag8)
									{
										text = match.Groups["type"].Value;
										styleSelectorType = StyleSelectorType.Type;
									}
								}
							}
						}
					}
					parts[i] = new StyleSelectorPart
					{
						type = styleSelectorType,
						value = text
					};
				}
				flag2 = true;
			}
			return flag2;
		}

		private static readonly Regex rgx = new Regex("(?<id>#[-]?\\w[\\w-]*)|(?<class>\\.[\\w-]+)|(?<pseudoclass>:[\\w-]+(\\((?<param>.+)\\))?)|(?<type>([^\\-]\\w+|\\w+))|(?<wildcard>\\*)|\\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private const int typeSelectorWeight = 1;

		private const int classSelectorWeight = 10;

		private const int idSelectorWeight = 100;
	}
}
