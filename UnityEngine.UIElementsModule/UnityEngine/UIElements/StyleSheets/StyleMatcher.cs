using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine.UIElements.StyleSheets.Syntax;

namespace UnityEngine.UIElements.StyleSheets
{
	internal class StyleMatcher : BaseStyleMatcher
	{
		private string current
		{
			get
			{
				return base.hasCurrent ? this.m_PropertyParts[base.currentIndex] : null;
			}
		}

		public override int valueCount
		{
			get
			{
				return this.m_PropertyParts.Length;
			}
		}

		public override bool isCurrentVariable
		{
			get
			{
				return base.hasCurrent && this.current.StartsWith("var(", StringComparison.Ordinal);
			}
		}

		public override bool isCurrentComma
		{
			get
			{
				return base.hasCurrent && this.current == ",";
			}
		}

		private void Initialize(string propertyValue)
		{
			base.Initialize();
			this.m_PropertyParts = this.m_Parser.Parse(propertyValue);
		}

		public MatchResult Match(Expression exp, string propertyValue)
		{
			MatchResult matchResult = new MatchResult
			{
				errorCode = MatchResultErrorCode.None
			};
			bool flag = string.IsNullOrEmpty(propertyValue);
			MatchResult matchResult2;
			if (flag)
			{
				matchResult.errorCode = MatchResultErrorCode.EmptyValue;
				matchResult2 = matchResult;
			}
			else
			{
				this.Initialize(propertyValue);
				string current = this.current;
				bool flag2 = current == "initial" || current.StartsWith("env(", StringComparison.Ordinal);
				bool flag3;
				if (flag2)
				{
					base.MoveNext();
					flag3 = true;
				}
				else
				{
					flag3 = base.Match(exp);
				}
				bool flag4 = !flag3;
				if (flag4)
				{
					matchResult.errorCode = MatchResultErrorCode.Syntax;
					matchResult.errorValue = this.current;
				}
				else
				{
					bool hasCurrent = base.hasCurrent;
					if (hasCurrent)
					{
						matchResult.errorCode = MatchResultErrorCode.ExpectedEndOfValue;
						matchResult.errorValue = this.current;
					}
				}
				matchResult2 = matchResult;
			}
			return matchResult2;
		}

		protected override bool MatchKeyword(string keyword)
		{
			return string.Compare(this.current, keyword, StringComparison.OrdinalIgnoreCase) == 0;
		}

		protected override bool MatchNumber(Expression exp)
		{
			string current = this.current;
			Match match = StyleMatcher.s_NumberRegex.Match(current);
			bool success = match.Success;
			if (success)
			{
				float num;
				bool flag = float.TryParse(current, NumberStyles.Float, CultureInfo.InvariantCulture, out num);
				if (flag)
				{
					return exp.min <= num && num <= exp.max;
				}
			}
			return false;
		}

		protected override bool MatchInteger()
		{
			string current = this.current;
			Match match = StyleMatcher.s_IntegerRegex.Match(current);
			return match.Success;
		}

		protected override bool MatchLength()
		{
			string current = this.current;
			Match match = StyleMatcher.s_LengthRegex.Match(current);
			bool success = match.Success;
			bool flag;
			if (success)
			{
				flag = true;
			}
			else
			{
				match = StyleMatcher.s_ZeroRegex.Match(current);
				flag = match.Success;
			}
			return flag;
		}

		protected override bool MatchPercentage()
		{
			string current = this.current;
			Match match = StyleMatcher.s_PercentRegex.Match(current);
			bool success = match.Success;
			bool flag;
			if (success)
			{
				flag = true;
			}
			else
			{
				match = StyleMatcher.s_ZeroRegex.Match(current);
				flag = match.Success;
			}
			return flag;
		}

		protected override bool MatchColor()
		{
			string current = this.current;
			Match match = StyleMatcher.s_HexColorRegex.Match(current);
			bool success = match.Success;
			bool flag;
			if (success)
			{
				flag = true;
			}
			else
			{
				match = StyleMatcher.s_RgbRegex.Match(current);
				bool success2 = match.Success;
				if (success2)
				{
					flag = true;
				}
				else
				{
					match = StyleMatcher.s_RgbaRegex.Match(current);
					bool success3 = match.Success;
					if (success3)
					{
						flag = true;
					}
					else
					{
						Color clear = Color.clear;
						bool flag2 = StyleSheetColor.TryGetColor(current, out clear);
						flag = flag2;
					}
				}
			}
			return flag;
		}

		protected override bool MatchResource()
		{
			string current = this.current;
			Match match = StyleMatcher.s_ResourceRegex.Match(current);
			bool flag = !match.Success;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				string text = match.Groups[1].Value.Trim();
				match = StyleMatcher.s_VarFunctionRegex.Match(text);
				flag2 = !match.Success;
			}
			return flag2;
		}

		protected override bool MatchUrl()
		{
			string current = this.current;
			Match match = StyleMatcher.s_UrlRegex.Match(current);
			bool flag = !match.Success;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				string text = match.Groups[1].Value.Trim();
				match = StyleMatcher.s_VarFunctionRegex.Match(text);
				flag2 = !match.Success;
			}
			return flag2;
		}

		protected override bool MatchTime()
		{
			string current = this.current;
			Match match = StyleMatcher.s_TimeRegex.Match(current);
			return match.Success;
		}

		protected override bool MatchFilterFunction()
		{
			string current = this.current;
			Match match = StyleMatcher.s_FilterFunctionRegex.Match(current);
			return match.Success;
		}

		protected override bool MatchMaterialPropertyValue()
		{
			string current = this.current;
			Match match = StyleMatcher.s_PropFunctionRegex.Match(current);
			return match.Success;
		}

		protected override bool MatchAngle()
		{
			string current = this.current;
			Match match = StyleMatcher.s_AngleRegex.Match(current);
			bool success = match.Success;
			bool flag;
			if (success)
			{
				flag = true;
			}
			else
			{
				match = StyleMatcher.s_ZeroRegex.Match(current);
				flag = match.Success;
			}
			return flag;
		}

		protected override bool MatchCustomIdent()
		{
			string current = this.current;
			Match match = BaseStyleMatcher.s_CustomIdentRegex.Match(current);
			return match.Success && match.Length == current.Length;
		}

		private StylePropertyValueParser m_Parser = new StylePropertyValueParser();

		private string[] m_PropertyParts;

		private static readonly Regex s_NumberRegex = new Regex("^[+-]?\\d+(?:\\.\\d+)?$", RegexOptions.Compiled);

		private static readonly Regex s_IntegerRegex = new Regex("^[+-]?\\d+$", RegexOptions.Compiled);

		private static readonly Regex s_ZeroRegex = new Regex("^0(?:\\.0+)?$", RegexOptions.Compiled);

		private static readonly Regex s_LengthRegex = new Regex("^[+-]?\\d+(?:\\.\\d+)?(?:px)$", RegexOptions.Compiled);

		private static readonly Regex s_PercentRegex = new Regex("^[+-]?\\d+(?:\\.\\d+)?(?:%)$", RegexOptions.Compiled);

		private static readonly Regex s_HexColorRegex = new Regex("^#[a-fA-F0-9]{3}(?:[a-fA-F0-9]{3})?$", RegexOptions.Compiled);

		private static readonly Regex s_RgbRegex = new Regex("^rgb\\(\\s*(\\d+\\.?\\d*)\\s*,\\s*(\\d+\\.?\\d*)\\s*,\\s*(\\d+\\.?\\d*)\\s*\\)$", RegexOptions.Compiled);

		private static readonly Regex s_RgbaRegex = new Regex("rgba\\(\\s*(\\d+\\.?\\d*)\\s*,\\s*(\\d+\\.?\\d*)\\s*,\\s*(\\d+\\.?\\d*)\\s*,\\s*(\\d+\\.?\\d*)\\s*\\)$", RegexOptions.Compiled);

		private static readonly Regex s_VarFunctionRegex = new Regex("^var\\(.+\\)$", RegexOptions.Compiled);

		private static readonly Regex s_ResourceRegex = new Regex("^resource\\((.+)\\)$", RegexOptions.Compiled);

		private static readonly Regex s_UrlRegex = new Regex("^url\\((.+)\\)$", RegexOptions.Compiled);

		private static readonly Regex s_TimeRegex = new Regex("^[+-]?\\.?\\d+(?:\\.\\d+)?(?:s|ms)$", RegexOptions.Compiled);

		private static readonly Regex s_FilterFunctionRegex = new Regex("^([a-zA-Z0-9\\-]+)\\(.*\\)$", RegexOptions.Compiled);

		private static readonly Regex s_PropFunctionRegex = new Regex("^prop\\(\"[a-zA-Z0-9_]+\"\\s+.+\\)$", RegexOptions.Compiled);

		private static readonly Regex s_AngleRegex = new Regex("^[+-]?\\d+(?:\\.\\d+)?(?:deg|grad|rad|turn)$", RegexOptions.Compiled);
	}
}
