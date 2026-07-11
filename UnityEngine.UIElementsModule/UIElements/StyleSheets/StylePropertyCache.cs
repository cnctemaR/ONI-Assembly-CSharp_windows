using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements.StyleSheets
{
	internal static class StylePropertyCache
	{
		public static bool TryGetSyntax(string name, out string syntax)
		{
			return StylePropertyCache.s_PropertySyntaxCache.TryGetValue(name, out syntax);
		}

		public static string FindClosestPropertyName(string name)
		{
			float num = float.MaxValue;
			string text = null;
			foreach (string text2 in StylePropertyCache.s_PropertySyntaxCache.Keys)
			{
				float num2 = 1f;
				bool flag = text2.Contains(name);
				if (flag)
				{
					num2 = 0.1f;
				}
				float num3 = (float)StringUtils.LevenshteinDistance(name, text2) * num2;
				bool flag2 = num3 < num;
				if (flag2)
				{
					num = num3;
					text = text2;
				}
			}
			return text;
		}

		private static Dictionary<string, string> s_PropertySyntaxCache = new Dictionary<string, string>
		{
			{ "width", "<length> | <percentage> | auto" },
			{ "height", "<length> | <percentage> | auto" },
			{ "max-width", "<length> | <percentage> | none" },
			{ "max-height", "<length> | <percentage> | none" },
			{ "min-width", "<length> | <percentage> | auto" },
			{ "min-height", "<length> | <percentage> | auto" },
			{ "flex-wrap", "nowrap | wrap | wrap-reverse" },
			{ "flex-basis", "<'width'>" },
			{ "flex-grow", "<number>" },
			{ "flex-shrink", "<number>" },
			{ "overflow", "hidden | visible | scroll" },
			{ "-unity-overflow-clip-box", "padding-box | content-box" },
			{ "left", "<length> | <percentage> | auto" },
			{ "top", "<length> | <percentage> | auto" },
			{ "right", "<length> | <percentage> | auto" },
			{ "bottom", "<length> | <percentage> | auto" },
			{ "margin-left", "<length> | <percentage> | auto" },
			{ "margin-top", "<length> | <percentage> | auto" },
			{ "margin-right", "<length> | <percentage> | auto" },
			{ "margin-bottom", "<length> | <percentage> | auto" },
			{ "padding-left", "<length> | <percentage>" },
			{ "padding-top", "<length> | <percentage>" },
			{ "padding-right", "<length> | <percentage>" },
			{ "padding-bottom", "<length> | <percentage>" },
			{ "position", "relative | absolute" },
			{ "-unity-text-align", "upper-left | middle-left | lower-left | upper-center | middle-center | lower-center | upper-right | middle-right | lower-right" },
			{ "-unity-font-style", "normal | italic | bold | bold-and-italic" },
			{ "-unity-font", "<resource> | <url>" },
			{ "font-size", "<length> | <percentage>" },
			{ "white-space", "normal | nowrap" },
			{ "color", "<color>" },
			{ "flex-direction", "row | row-reverse | column | column-reverse" },
			{ "background-color", "<color>" },
			{ "background-image", "<resource> | <url> | none" },
			{ "-unity-background-scale-mode", "stretch-to-fill | scale-and-crop | scale-to-fit" },
			{ "-unity-background-image-tint-color", "<color>" },
			{ "align-content", "flex-start | flex-end | center | stretch | auto" },
			{ "align-items", "flex-start | flex-end | center | stretch | auto" },
			{ "align-self", "flex-start | flex-end | center | stretch | auto" },
			{ "justify-content", "flex-start | flex-end | center | space-between | space-around" },
			{ "border-left-color", "<color>" },
			{ "border-top-color", "<color>" },
			{ "border-right-color", "<color>" },
			{ "border-bottom-color", "<color>" },
			{ "border-left-width", "<length>" },
			{ "border-top-width", "<length>" },
			{ "border-right-width", "<length>" },
			{ "border-bottom-width", "<length>" },
			{ "border-top-left-radius", "<length> | <percentage>" },
			{ "border-top-right-radius", "<length> | <percentage>" },
			{ "border-bottom-right-radius", "<length> | <percentage>" },
			{ "border-bottom-left-radius", "<length> | <percentage>" },
			{ "-unity-slice-left", "<integer>" },
			{ "-unity-slice-top", "<integer>" },
			{ "-unity-slice-right", "<integer>" },
			{ "-unity-slice-bottom", "<integer>" },
			{ "opacity", "<number>" },
			{ "cursor", "[ [ <resource> | <url> ] [ <integer> <integer> ]? ] | [ arrow | text | resize-vertical | resize-horizontal | link | slide-arrow | resize-up-right | resize-up-left | move-arrow | rotate-arrow | scale-arrow | arrow-plus | arrow-minus | pan | orbit | zoom | fps | split-resize-up-down | split-resize-left-right ]" },
			{ "visibility", "visible | hidden" },
			{ "display", "flex | none" },
			{ "border-color", "<color>{1,4}" },
			{ "border-radius", "[ <length> | <percentage> ]{1,4}" },
			{ "border-width", "<length>{1,4}" },
			{ "flex", "none | [ <'flex-grow'> <'flex-shrink'>? || <'flex-basis'> ]" },
			{ "margin", "[ <length> | <percentage> | auto ]{1,4}" },
			{ "padding", "[ <length> | <percentage> ]{1,4}" }
		};
	}
}
